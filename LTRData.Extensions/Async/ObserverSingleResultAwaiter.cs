#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Runtime.CompilerServices;

namespace LTRData.Extensions.Async;

/// <summary>
/// Async/await extension for <see cref="IObservable{T}"/> objects.
/// </summary>
/// <typeparam name="T">Type of result of asynchronous operation</typeparam>
public class ObserverSingleResultAwaiter<T> : IObserver<T>, ICriticalNotifyCompletion
{
    private Action? continuation = null;

    private Exception? exception = null;

    private T? result = default;

    private readonly IDisposable asyncSubscription;

    /// <summary>
    /// Creates a new instance by subscribing to an <see cref="IObservable{T}"/>
    /// </summary>
    /// <param name="observable">Asynchronous operation that will be awaited by this instance</param>
    protected internal ObserverSingleResultAwaiter(IObservable<T> observable)
    {
        asyncSubscription = observable.Subscribe(this);
    }

    /// <summary>
    /// Gets current state of asynchronous operation.
    /// </summary>
    /// <returns>State of asynchronous operation</returns>
    public bool IsCompleted => result is not null || exception is not null;

    /// <summary>
    /// Gets current state of asynchronous operation.
    /// </summary>
    /// <returns>State of asynchronous operation</returns>
    public bool IsCompletedSuccessfully => result is not null;

    /// <summary>
    /// Gets result of asynchronous operation.
    /// </summary>
    /// <returns>Result of asynchronous operation</returns>
    public T GetResult()
    {
        if (exception is not null)
        {
            throw new AggregateException(exception);
        }

        return result ?? throw new InvalidOperationException("Task not finished");
    }

    void IObserver<T>.OnCompleted()
    {
        asyncSubscription.Dispose();
        continuation?.Invoke();
    }

    void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => this.continuation = continuation;

    void IObserver<T>.OnError(Exception error)
    {
        exception = error;
        asyncSubscription.Dispose();
        continuation?.Invoke();
    }

    void IObserver<T>.OnNext(T value) => result = value;

    void INotifyCompletion.OnCompleted(Action continuation) => throw new NotSupportedException();
}

#endif
