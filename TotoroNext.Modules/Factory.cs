using Microsoft.Extensions.DependencyInjection;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.Module;


public interface IFactory<TService, TId>
    where TService : notnull
{
    TService Create(TId id);

    TService? CreateDefault();
}

public class Factory<TService, TId>(IServiceScopeFactory serviceScopeFactory,
                                    ILocalSettingsService localSettingsService,
                                    string defaultKey) : IFactory<TService, TId>
    where TService : notnull
{
    public TService Create(TId? id)
    {
        if(id is null)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
        }

        using var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredKeyedService<TService>(id);
    }

    public TService? CreateDefault()
    {
        using var scope = serviceScopeFactory.CreateScope();

        var key = localSettingsService.ReadSetting<TId>(defaultKey, default);

        if(EqualityComparer<TId>.Default.Equals(key, default))
        {
            return scope.ServiceProvider.GetKeyedServices<TService>(KeyedService.AnyKey).First();
        }
        else
        {
            return scope.ServiceProvider.GetRequiredKeyedService<TService>(key);
        }
    }
}
