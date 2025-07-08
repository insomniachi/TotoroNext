namespace TotoroNext.Module.Abstractions;

public interface IDialogService
{
    Task<DialogResult> Ask(string tilte, string question);
    Task<DialogResult> AskSkip();
}

public enum DialogResult
{
    Ok,
    Yes,
    No,
    Cancel
}
