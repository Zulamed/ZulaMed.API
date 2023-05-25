using FastEndpoints;
using Mediator;

namespace ZulaMed.API.Endpoints.VideoRestApi.Put;

public class Endpoint : Endpoint<Request,VideoDTO>
{
   private readonly IMediator _mediator;

   public Endpoint(IMediator mediator)
   {
      _mediator = mediator;
   }
   
   public override void Configure()
   {
      Put("/video/{id}");
      AllowAnonymous();
   }

   public override async Task HandleAsync(Request req, CancellationToken ct)
   {
      var result = await _mediator.Send(req.MapToCommand(), ct);
      if (result.TryPickT0(out var video, out var error))
      {
         await SendOkAsync(ct);
         return;
      }
      AddError(error.Value.Message);
      await SendErrorsAsync(cancellation: ct);
   }
}