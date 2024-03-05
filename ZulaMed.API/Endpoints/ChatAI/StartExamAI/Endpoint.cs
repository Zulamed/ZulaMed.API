using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.UserExamAI;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Endpoints.ChatAI.OpenAISerivce;


namespace ZulaMed.API.Endpoints.ChatAI.StartExamAI;


public class StartExamAICommandHandler : Mediator.ICommandHandler<StartExamAICommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public StartExamAICommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(StartExamAICommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(x => (Guid)x.Id == command.UserId, cancellationToken);
            
 
            if (user is null )
            {
                return new NotFound();
            }

            _dbContext.Set<UserExamAI>().Add(new UserExamAI
            {
                Id = (UserExamAIID)Guid.NewGuid(),
                User = user,
                Questions = null,
                ExamTopic = command.SelectedTopic,
                ExamStartTime = (UserExamStartTime)DateTime.UtcNow,
                ExamEndTime = null,
                ExamTime = null
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            //FormattableString sql =
            //    $"""
            //     UPDATE "User"
            //     SET "SubscriberCount" = "SubscriberCount" + 1
            //     WHERE "Id" = {command.SubToUserId}
            //     """;
            //await _dbContext.Database.ExecuteSqlAsync(sql, cancellationToken: cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            return new Success();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            return new Error<string>(e.Message);
        }
    }
}





public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;
    private readonly IOpenAIService _service;


    public Endpoint(IMediator mediator, IOpenAIService service)
    {
        _mediator = mediator;
        _service = service;
    }

    
    public override void Configure()
    {
        Post("user/startExam");
        Description(c => c.Produces(200), clearDefaults: true);
        
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
       // var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);

       var user = req.UserId;

       var result = await _mediator.Send(new StartExamAICommand
        {
            SelectedTopic = (UserExamTopic)"Что нужно сделать если апендецит",
            UserId = req.UserId
        }, ct);
       
       
        await result.Match(
            a => SendOkAsync(ct),
            e => SendAsync(new
            {
                StatusCode = 500,
                Message = e.Value
            }, 500, ct),
            n => SendNotFoundAsync(ct)
        );
    }
}


