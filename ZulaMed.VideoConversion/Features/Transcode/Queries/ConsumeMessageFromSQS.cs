using Amazon.SQS;
using Amazon.SQS.Model;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.VideoConversion.Features.Transcode.Queries;

public class ConsumeMessageFromSqs : IQuery<ReceiveMessageResponse>
{
}


public class ConsumeMessageFromSqsHandler : IQueryHandler<ConsumeMessageFromSqs, ReceiveMessageResponse>
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<SqsQueueOptions> _options;

    public ConsumeMessageFromSqsHandler(IAmazonSQS sqs, IOptions<SqsQueueOptions> options)
    {
        _sqs = sqs;
        _options = options;
    }
    
    
    public async ValueTask<ReceiveMessageResponse> Handle(ConsumeMessageFromSqs query, CancellationToken cancellationToken)
    {
        var message = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = await GetSqsUrl(),
        }, cancellationToken);
        return message;
    }
    
    private async Task<string> GetSqsUrl()
    {
        var request = new GetQueueUrlRequest
        {
            QueueName = _options.Value.QueueName
        };
        var response = await _sqs.GetQueueUrlAsync(request);
        return response.QueueUrl;
    }
}