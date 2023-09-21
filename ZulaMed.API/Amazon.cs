using Amazon;
using Amazon.S3;

namespace ZulaMed.API;

public static class AwsExtensions
{
   public static void AddAmazon(this IServiceCollection services)
   {
      services.AddOptions<S3BucketOptions>()
          .BindConfiguration("S3BucketOptions")
          .ValidateDataAnnotations();
      
      AWSConfigsS3.EnableUnicodeEncodingForObjectMetadata = true;
      services.AddSingleton<IAmazonS3, AmazonS3Client>();
   }  
}