using System.Net;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserS3.PostBanner;


public class CreateBannerEndpoint : Endpoint<Request, Response>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;
    private readonly ZulaMedDbContext _dbContext;
    private readonly IAmazonCloudFront _cloudFront;
    private readonly IOptions<CloudFrontOptions> _cloudfrontOptions;

    public CreateBannerEndpoint(IAmazonS3 s3Client, IOptions<S3BucketOptions> s3Options, ZulaMedDbContext dbContext,
        IAmazonCloudFront cloudFront,
        IOptions<CloudFrontOptions> cloudfrontOptions)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
        _dbContext = dbContext;
        _cloudFront = cloudFront;
        _cloudfrontOptions = cloudfrontOptions;
    }

    public override void Configure()
    {
        Post("/user/{userId}/banner");
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var fileExtension = Path.GetExtension(req.Banner.FileName);
        var guid = req.UserId;
        var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => req.UserId == (Guid)x.Id, ct);
        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var response = await _s3Client.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = _s3Options.Value.BucketName,
            Key = $"users/banners/{guid}",
            ContentType = req.Banner.ContentType,
            InputStream = req.Banner.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = req.Banner.FileName,
                ["x-amz-meta-extension"] = fileExtension
            }
        }, ct);
        await _cloudFront.CreateInvalidationAsync(new CreateInvalidationRequest()
        {
            DistributionId = _cloudfrontOptions.Value.DistributionId,
            InvalidationBatch = new InvalidationBatch()
            {
                CallerReference = DateTime.Now.Ticks.ToString(),
                Paths = new Paths
                {
                    Quantity = 1,
                    Items = new List<string>()
                    {
                        $"/users/banners/{guid}"
                    }
                }
            }
        }, ct);
        var ticks = DateTime.Now.Ticks;
        var url = _s3Options.Value.BaseUrl + $"/users/banners/{guid}?{ticks}";
        user.BannerUrl = (BannerUrl)url;
        switch (response.HttpStatusCode)
        {
            case  HttpStatusCode.OK:
                await _dbContext.SaveChangesAsync(ct);
                await SendAsync(new Response()
                {
                    BannerUrl= url
                }, cancellation: ct);
                break;
            default:
                ThrowError("Encountered an error while uploading the photo");
                break;
        }

    }
}