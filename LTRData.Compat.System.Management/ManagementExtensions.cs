using Microsoft.Management.Infrastructure.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0057 // Use range operator
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace LTRData.Compat.System.Management;

internal static class ManagementExtensions
{
    internal sealed class ObservableSingleResultAwaiter<T> : IObserver<T>, ICriticalNotifyCompletion, IDisposable
    {
        private IDisposable observer;
        private Action? continuation;
        private Exception? error;
        private T? value;

        public ObservableSingleResultAwaiter(IObservable<T> observable)
        {
            observer = observable.Subscribe(this);
        }

        public bool IsCompleted => observer is null;

        public void OnCompleted() => Dispose();

        public void OnCompleted(Action continuation) => throw new NotImplementedException();

        public void OnError(Exception error)
        {
            Dispose();
            this.error = error;
            continuation?.Invoke();
        }

        public void OnNext(T value) 
        {
            Dispose();
            this.value = value;
            continuation?.Invoke();
        }

        public void UnsafeOnCompleted(Action continuation) => this.continuation = continuation;

        public T GetResult()
        {
            if (error is not null)
            {
                throw new AggregateException(error);
            }

            if (value is null)
            {
                throw new InvalidOperationException("Result not yet available");
            }

            return value;
        }

        public void Dispose()
        {
            observer?.Dispose();
            observer = null!;
        }
    }

    internal sealed class ObservableMultipleResultsAwaiter<T> : IObserver<T>, ICriticalNotifyCompletion, IDisposable
    {
        private IDisposable observer;
        private Action? continuation;
        private Exception? error;
        private List<T>? values;

        public ObservableMultipleResultsAwaiter(IObservable<T> observable)
        {
            observer = observable.Subscribe(this);
        }

        public bool IsCompleted => observer is null;

        public void OnCompleted()
        {
            Dispose();
            continuation?.Invoke();
        }

        public void OnCompleted(Action continuation) => throw new NotImplementedException();

        public void OnError(Exception error)
        {
            Dispose();
            this.error = error;
            continuation?.Invoke();
        }

        public void OnNext(T value)
        {
            values ??= new(1);
            values.Add(value);
        }

        public void UnsafeOnCompleted(Action continuation) => this.continuation = continuation;

        public List<T> GetResult()
        {
            if (error is not null)
            {
                throw new AggregateException(error);
            }

            if (observer is not null)
            {
                throw new InvalidOperationException("Result not yet available");
            }

            return values ?? [];
        }

        public void Dispose()
        {
            observer?.Dispose();
            observer = null!;
        }
    }

    internal static ObservableSingleResultAwaiter<object> GetAwaiter(this CimAsyncStatus observable)
        => new(observable);

    internal static ObservableSingleResultAwaiter<T> GetAwaiter<T>(this CimAsyncResult<T> observable)
        => new(observable);

    internal static ObservableMultipleResultsAwaiter<T> GetAwaiter<T>(this CimAsyncMultipleResults<T> observable)
        => new(observable);
}
