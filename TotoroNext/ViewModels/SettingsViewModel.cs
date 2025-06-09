using System.Runtime.CompilerServices;
using System.Text.Json;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TotoroNext.Module;
using TotoroNext.Module.Abstractions;

namespace TotoroNext.ViewModels;


public partial class SettingsModel : ReactiveObject
{
    private ILocalSettingsService? _localSettingsService;

    public void SetBackingStore(ILocalSettingsService localSettingsService) => _localSettingsService = localSettingsService;

    protected void SetAndSaveProperty<TProperty>(ref TProperty field, TProperty value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<TProperty>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        _localSettingsService?.SaveSetting(propertyName, value);
        this.RaisePropertyChanged(propertyName);
    }

    public Guid? SelectedMediaEngine { get; set => SetAndSaveProperty(ref field, value); }

    public Guid? SelectedAnimeProvider { get; set => SetAndSaveProperty(ref field, value); }

    public Guid? SelectedTrackingService { get; set => SetAndSaveProperty(ref field, value); }

    public Guid? SelectedSegmentsProvider { get; set => SetAndSaveProperty(ref field, value); }
}


public partial class SettingsViewModel(IEnumerable<Descriptor> modules) : ReactiveObject, IInitializable
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
    };
    private readonly string _filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TotoroNext", "settings.json");

    public List<Descriptor> MediaEngines { get; } = [.. modules.Where(x => x.Components.Contains(ComponentTypes.MediaEngine))];
    public List<Descriptor> AnimeProviders { get; } = [.. modules.Where(x => x.Components.Contains(ComponentTypes.AnimeProvider))];
    public List<Descriptor> TrackingServices { get; } = [.. modules.Where(x => x.Components.Contains(ComponentTypes.Tracking))];
    public List<Descriptor> SegmentProviders { get; } = [.. modules.Where(x => x.Components.Contains(ComponentTypes.MediaSegments))];


    [Reactive]
    public SettingsModel Settings { get; set; } = null!;

    public void Initialize()
    {
        if(Settings is not null)
        {
            return;
        }

        if(File.Exists(_filePath))
        {
            Settings = JsonSerializer.Deserialize<SettingsModel>(File.ReadAllText(_filePath), _options) ?? new();
        }
        else
        {
            Settings = new();
        }

        Settings.Changed.Subscribe(_ =>
        {
            File.WriteAllText(_filePath, JsonSerializer.Serialize(Settings, _options));
        });

        this.RaisePropertyChanged(nameof(Settings));
    }
}
