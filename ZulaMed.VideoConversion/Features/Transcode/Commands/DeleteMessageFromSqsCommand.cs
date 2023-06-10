using System.Net;
using Amazon.SQS;
using Amazon.SQS.Model;
using Mediator;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace ZulaMed.VideoConversion.Features.Transcode.Commands;

public class DeleteMessageFromSqsCommand : ICommand<OneOf<Success, Error>>
{
    public required string ReceiptHandle { get; init; }
}

public class DeleteMessageFromSqsCommandHandler : ICommandHandler<DeleteMessageFromSqsCommand, OneOf<Success, Error>>
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IOptions<SqsQueueOptions> _options;

    public DeleteMessageFromSqsCommandHandler(IAmazonSQS sqsClient, IOptions<SqsQueueOptions> options)
    {
        _sqsClient = sqsClient;
        _options = options;
    }


    public async ValueTask<OneOf<Success, Error>> Handle(DeleteMessageFromSqsCommand command, CancellationToken cancellationToken)
    {
        var response = await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest()
        {
            QueueUrl = await GetSqsUrl(),
            ReceiptHandle = command.ReceiptHandle
        }, cancellationToken);
        if (response.HttpStatusCode != HttpStatusCode.OK)
            return new Error();
        return new Success();
    }

    private async Task<string> GetSqsUrl()
    {
        var request = new GetQueueUrlRequest
        {
            QueueName = _options.Value.QueueName
        };
        var response = await _sqsClient.GetQueueUrlAsync(request);
        return response.QueueUrl;
    }
}