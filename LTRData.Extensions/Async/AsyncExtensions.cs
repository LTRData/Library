#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Threading.Tasks;

namespace LTRData.Extensions.Async;

/// <summary>
/// </summary>
public static class AsyncExtensions
{
#if NET462_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Waits for a ValueTask to complete, or throws AggregateException if the ValueTask fails. If the ValueTask
    /// has already completed successfully when this method is called, it returns immediately without any further
    /// allocations. Otherwise as Task object is created for waiting and for re-throwing any exceptions etc.
    /// </summary>
    /// <param name="task">ValueTask</param>
    public static void Wait(this in ValueTask task)
    {
        if (!task.IsCompletedSuccessfully)
        {
            task.AsTask().Wait();
        }
    }

    /// <summary>
    /// Waits for a ValueTask to complete, or throws AggregateException if the ValueTask fails. If the ValueTask
    /// has already completed successfully when this method is called, the result is returned immediately without
    /// any further allocations. Otherwise as Task object is created for waiting for results, exceptions etc.
    /// </summary>
    /// <param name="task">ValueTask</param>
    public static void Wait<T>(this in ValueTask<T> task)
    {
        if (!task.IsCompletedSuccessfully)
        {
            task.AsTask().Wait();
        }
    }

    /// <summary>
    /// Waits for a ValueTask to complete, or throws AggregateException if the ValueTask fails. If the ValueTask
    /// has already completed successfully when this method is called, the result is returned immediately without
    /// any further allocations. Otherwise as Task object is created for waiting for results, exceptions etc.
    /// </summary>
    /// <param name="task">ValueTask</param>
    public static T WaitForResult<T>(this in ValueTask<T> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            return task.Result;
        }
        else
        {
            return task.AsTask().Result;
        }
    }

#endif

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// </summary>
    public static IAsyncResult AsAsyncResult<T>(this Task<T> task, AsyncCallback? callback, object? state)
    {
        var returntask = task.ContinueWith((t, _) => t.Result, state, TaskScheduler.Default);

        if (callback is not null)
        {
            returntask.ContinueWith(callback.Invoke, TaskScheduler.Default);
        }

        return returntask;
    }

    /// <summary>
    /// </summary>
    public static IAsyncResult AsAsyncResult(this Task task, AsyncCallback? callback, object? state)
    {
        var returntask = task.ContinueWith((t, _) => { }, state, TaskScheduler.Default);

        if (callback is not null)
        {
            returntask.ContinueWith(callback.Invoke, TaskScheduler.Default);
        }

        return returntask;
    }

#endif

}

#endif
