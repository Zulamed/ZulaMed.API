using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;
using VoException = Vogen.ValueObjectValidationException;

namespace ZulaMed.API.Endpoints.VideoRestApi.Post;

public class CreateVideoCommandHandler
    : Mediator.ICommandHandler<CreateVideoCommand, Result<Video, VoException>>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;
    private readonly IOptions<SqsQueueOptions> _sqsOptions;
    private readonly ZulaMedDbContext _dbContext;

    public CreateVideoCommandHandler(ZulaMedDbContext dbContext, IAmazonS3 s3Client,
        IOptions<S3BucketOptions> s3Options, IOptions<SqsQueueOptions> sqsOptions)
    {
        _dbContext = dbContext;
        _s3Client = s3Client;
        _s3Options = s3Options;
        _sqsOptions = sqsOptions;
    }

    public async ValueTask<Result<Video, ValueObjectValidationException>> Handle(CreateVideoCommand command,
        CancellationToken cancellationToken)
    {
        var dbSet = _dbContext.Set<Video>();
        var guid = Guid.NewGuid();
        try
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => (Guid)x.Id == command.VideoPublisherId,
                cancellationToken: cancellationToken);
            if (user is null)
            {
                throw new VoException("User not Found");
            }

            var fileExtension = Path.GetExtension(command.Video.FileName);
            var entity = await dbSet.AddAsync(new Video
            {
                Id = (VideoId)guid,
                VideoDescription = (VideoDescription)command.VideoDescription,
                VideoPublishedDate = (VideoPublishedDate)DateTime.UtcNow,
                VideoThumbnail = (VideoThumbnail)command.VideoThumbnail,
                VideoTitle = (VideoTitle)command.VideoTitle,
                VideoUrl = (VideoUrl)(_s3Options.Value.BaseUrl + $"/videos/{guid}"),
                Publisher = user
            }, cancellationToken);

            await _s3Client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _s3Options.Value.BucketName,
                Key = $"videos/{guid}",
                ContentType = command.Video.ContentType,
                InputStream = command.Video.OpenReadStream(),
                Metadata =
                {
                    ["x-amz-meta-originalname"] = command.Video.FileName,
                    ["x-amz-meta-extension"] = fileExtension
                },
            }, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);
            
            return entity.Entity;
        }
        catch (DbUpdateException)
        {
            await _s3Client.DeleteObjectAsync(_s3Options.Value.BucketName, $"videos/{guid}", cancellationToken);
            return new Error<VoException>(new VoException("Error while saving to database"));
        }
        catch (VoException e)
        {
            return new Error<VoException>(e);
        }
    }
}

public class Endpoint : Endpoint<Request, VideoDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/video");
        AllowAnonymous();
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var result = await _mediator.Send(request.MapToCommand(), ct);
        if (result.TryPickT0(out var value, out var error))
        {
            await SendOkAsync(value.MapToResponse(), ct);
            return;
        }

        AddError(error.Value.Message);
        await SendErrorsAsync(cancellation: ct);
    }
}