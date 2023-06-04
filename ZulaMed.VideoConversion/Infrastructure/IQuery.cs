namespace ZulaMed.VideoConversion.Infrastructure;

public interface IQuery<T>
{
}

public interface IQueryHandler<in TQuery, TReturn>
   where TQuery : IQuery<TReturn>
{
   public Task<TReturn> HandleAsync(TQuery query, CancellationToken token); 
} 