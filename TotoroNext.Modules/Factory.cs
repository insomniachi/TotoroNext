using Microsoft.Extensions.DependencyInjection;

namespace TotoroNext.Module;


public interface IFactory<TService,TId>
    where TService : notnull
{
    TService Create(TId id);
}

public class Factory<TService, TId>(IServiceScopeFactory serviceScopeFactory) : IFactory<TService, TId>
    where TService : notnull
{
    public TService Create(TId id)
    {
        using var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredKeyedService<TService>(id);
    }
}
