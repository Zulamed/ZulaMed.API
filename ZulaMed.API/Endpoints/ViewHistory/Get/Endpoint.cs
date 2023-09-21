using FastEndpoints;
using Mediator;
using ZulaMed.API.Endpoints.ViewHistory.Get.GetByTitle;
using ZulaMed.API.Endpoints.ViewHistory.Get.GetByUser;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.Get;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/viewHistory/{ownerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        Response response;
        if (req.Title is not null)
        {
            response = await _mediator.Send(new GetByTitleQuery
            {
                OwnerId = req.OwnerId,
                Title = req.Title,
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        }
        else
        {
            response = await _mediator.Send(new GetByUserQuery
            {
                OwnerId = req.OwnerId,
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        }
        await SendAsync(response, cancellation: ct);
    }
}