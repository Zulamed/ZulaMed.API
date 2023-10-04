using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace ZulaMed.API;

public static class FirebaseExtensions 
{
   public static void AddFirebase(this IServiceCollection services, GoogleCredential credential)
   {
      services.AddSingleton(FirebaseApp.Create(new AppOptions
      {
         Credential = credential 
      }));
      
      services.AddSingleton<FirebaseAuth>(provider =>
      {
         var app = provider.GetRequiredService<FirebaseApp>();
         return FirebaseAuth.GetAuth(app);
      });
   } 
}