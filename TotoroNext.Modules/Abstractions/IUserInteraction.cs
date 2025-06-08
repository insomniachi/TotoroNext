namespace TotoroNext.Module.Abstractions;

public interface IUserInteraction<TInput, TOutput>
{
    Task<TOutput?> GetValue(TInput input);
}
