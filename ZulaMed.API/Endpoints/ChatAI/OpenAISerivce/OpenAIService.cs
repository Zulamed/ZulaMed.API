using Newtonsoft.Json;
using ZulaMed.API.Domain.QuestionAI;

namespace ZulaMed.API.Endpoints.ChatAI.OpenAISerivce;

public class OpenAIService : IOpenAIService
{ 
    private const string key = "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";
    private const string url = "https://api.openai.com/v1/chat/completions";

    public List<dynamic> messages { get; init; } = null!;

    private HttpClient _httpClient;
    
    public OpenAIService()
    {
        _httpClient = new HttpClient();
        messages = new List<dynamic>();
        
        messages.Add(new {role = "system",
        content = "You are a professional doctor, specialized in [Your Specialization]. " +
                  "You will answer questions from novice doctors as clearly and correctly " +
                  "as possible, related strictly to medical inquiries. Please ignore or " +
                  "redirect questions not related to medicine by stating 'I can only provide" +
                  " advice on medical topics.' The dialogue is in Russian. Stick strictly " +
                  "to medical topics. And answer only in russian "});
        
        
    }

    public  async Task<string> SendAnswerToAi()
    { 
        var request = new
        { 
            messages,
            model = "gpt-3.5-turbo",
            max_tokens = 500
            
        };
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
          var requestJson = JsonConvert.SerializeObject(request);
          var requestContent = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json");
          var httpResponseMessage = await _httpClient.PostAsync(url, requestContent);
          var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
           
           var responseObject = JsonConvert.DeserializeAnonymousType(jsonString, new {
               choices = new [] {new {message=new {role=string.Empty,content=string.Empty}}},
               error=new {message=string.Empty}
               
           });
           
           
              if (!string.IsNullOrEmpty(responseObject?.error?.message))  // Check for errors
              {
                  return "Error in response";
              }

                 
        var messageObject = responseObject?.choices[0].message;
        if (messageObject != null)
        {
            messages.Add(messageObject);

            return messageObject.content;
        }

        return "Error";
    }
}



public interface IOpenAIService
{
    public Task<string> SendAnswerToAi();


}