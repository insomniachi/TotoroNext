namespace TotoroNext.Module;

public class LoadableAction : ILoadable
{
    private readonly Func<Task> _action;
    
    private LoadableAction(Func<Task> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public bool IsExecuting { get; private set; }

    public event EventHandler? IsExecutingChanged;

    public async Task Execute()
    {
        if (IsExecuting)
        {
            return;
        }

        IsExecuting = true;
        IsExecutingChanged?.Invoke(this, EventArgs.Empty);

        try
        {
            await _action();
        }
        finally
        {
            IsExecuting = false;
            IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public static LoadableAction Create(Func<Task> action) => new(action);
}
