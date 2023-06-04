namespace ZulaMed.VideoConversion.Infrastructure;

public interface ICommand<T>
{
}

public interface ICommandHandler<in TCommand, TReturn>
   where TCommand : ICommand<TReturn>
{
   public Task<TReturn> HandleAsync(TCommand command, CancellationToken token); 
}