using Amazon;
using Amazon.CloudFront;
using Amazon.S3;

namespace ZulaMed.API;

public class CloudFrontOptions
{
    public required string DistributionId { get; init; }
}

public static class AwsExtensions
{
   public static void AddAmazon(this IServiceCollection services)
   {
      services.AddOptions<S3BucketOptions>()
          .BindConfiguration("S3BucketOptions")
          .ValidateDataAnnotations();

      services.AddOptions<CloudFrontOptions>()
          .BindConfiguration("CloudFrontOptions")
          .ValidateDataAnnotations();
      
      AWSConfigsS3.EnableUnicodeEncodingForObjectMetadata = true;
      services.AddSingleton<IAmazonS3, AmazonS3Client>();
      services.AddSingleton<IAmazonCloudFront, AmazonCloudFrontClient>();
   }  
}