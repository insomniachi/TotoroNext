using System.Text.Json;
using TotoroNext.Anime.Abstractions;
using TotoroNext.Anime.Abstractions.Models;

namespace TotoroNext;

internal class AnimeOverridesRepository : IAnimeOverridesRepository
{
    private readonly string _file = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", $"overrides.json");
    private readonly Dictionary<long, AnimeOverrides> _overrides = [];

    public AnimeOverridesRepository()
    {
        if (File.Exists(_file))
        {
            _overrides = JsonSerializer.Deserialize<Dictionary<long, AnimeOverrides>>(File.ReadAllText(_file)) ?? [];
        }
    }

    public void Revert(long id)
    {
        if (!_overrides.TryGetValue(id, out var @override))
        {
            return;
        }

        @override.Revert();
    }

    public AnimeOverrides? GetOverrides(long id)
    {
        if (!_overrides.TryGetValue(id, out var @override))
        {
            return null;
        }

        return @override;
    }

    public void CreateOrUpdate(long id, AnimeOverrides overrides)
    {
        _overrides[id] = overrides;
        File.WriteAllText(_file, JsonSerializer.Serialize(_overrides));
    }
}
