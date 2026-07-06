using System.Collections.Concurrent;

using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Framework;

/// <summary>
/// Run methods parallel or as fire and forget
/// todo: release after post v1?
/// </summary>
internal static class Methods
{
    /// <summary>
    /// Runs the given functions in parallel and returns all methods started within the timeout.
    /// <para>Blocks until all functions finish or the timeout cancels execution.</para>
    /// </summary>
    /// <remarks>
    /// <para>Uses Parallel.ForEach with a fixed max degree of parallelism.</para>
    /// <para>Exceptions inside functions are caught and default(T) is added.</para>
    /// <para>Null return values are skipped.</para>
    /// <para>Execution stops early if cancellation is triggered by timeout.</para>
    /// <para>Useful for CPU-bound work or short local IO operations.</para>
    /// </remarks>
    /// <example>
    /// <code class="language-csharp hljs">
    /// class Car {
    ///     public string Name { get; set; }
    /// }
    /// 
    /// class CarApi {
    ///     List&lt;Car&gt; GetByName(string name) {
    ///         return Client.Get&lt;List&lt;Car&gt;&gt;("https://systemlibrary.com/cars/q=?" + name);   
    ///     }
    /// }
    /// 
    /// var carApi = new CarApi();
    /// var cars = Methods.Parallel&lt;Car&gt;(
    ///     () => carApi.GetByName("blue"),
    ///     () => carApi.GetByName("red"),
    ///     () => carApi.GetByName("orange")
    /// );
    /// 
    /// // Variable 'cars' is filled after all three api requests have completed.
    /// // Assume we got 1 blue, 0 red and 1 orange
    /// // 'cars' now contain a total of 2 objects of type 'Car'
    /// </code>
    /// </example>
    internal static List<T> Parallel<T>(params Func<T>[] functions)
    {
        return Parallel(30000, functions);
    }

    /// <summary>
    /// Runs the given functions in parallel and returns all methods started within the timeout.
    /// <para>Blocks until all functions finish or the timeout cancels execution.</para>
    /// </summary>
    /// <remarks>
    /// <para>Uses Parallel.ForEach with a fixed max degree of parallelism.</para>
    /// <para>Exceptions inside functions are caught and default(T) is added.</para>
    /// <para>Null return values are skipped.</para>
    /// <para>Execution stops early if cancellation is triggered by timeout.</para>
    /// <para>Useful for CPU-bound work or short local IO operations.</para>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="functions"></param>
    /// <returns></returns>
    /// <example>
    /// <code>
    /// var responses = Methods.Parallel&lt;string&gt;(
    ///     () => GetRedCars(url),
    ///     () => GetBlueCars(url),
    ///     () => GetGreenCars(url)
    /// );
    /// </code>
    /// </example>
    internal static List<T> Parallel<T>(int cutoffMs, params Func<T>[] functions)
    {
        if (functions == null || functions.Length == 0) return new List<T>();

        var results = new ConcurrentBag<T>();

        using var cts = new CancellationTokenSource();

        var token = cts.Token;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 4),
            CancellationToken = token
        };

        var timeoutTask = Task.Delay(cutoffMs).ContinueWith(_ => cts.Cancel());

        try
        {
            System.Threading.Tasks.Parallel.ForEach(functions.Where(f => f != null), options, (f, state) =>
            {
                if (token.IsCancellationRequested)
                    state.Stop();

                if (state.IsStopped) return;

                try
                {
                    var result = f();
                    if (result != null)
                        results.Add(result);
                }
                catch (Exception ex)
                {
                    global::Log.Error(ex);

                    results.Add(default(T));
                }
            });
        }
        catch (OperationCanceledException)
        {
            global::Log.Error($"[Methods] Run timed out after {cutoffMs}ms. Collected {results.Count}/{functions.Length} results.");
        }

        return results.ToList();
    }

    /// <summary>
    /// Runs the given functions in parallel.
    /// <para>Blocks until all functions finish or the timeout cancels execution.</para>
    /// </summary>
    /// <remarks>
    /// <para>Uses Parallel.ForEach with a fixed max degree of parallelism.</para>
    /// <para>Exceptions inside functions are caught and default(T) is added.</para>
    /// <para>Null return values are skipped.</para>
    /// <para>Execution stops early if cancellation is triggered by timeout.</para>
    /// <para>Useful for CPU-bound work or short local IO operations.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var responses = Methods.Parallel&lt;string&gt;(
    ///     () => GetRedCars(url),
    ///     () => GetBlueCars(url),
    ///     () => GetGreenCars(url)
    /// );
    /// </code>
    /// </example>
    internal static void Parallel(int cutoffMs, params Action[] functions)
    {
        using var cts = new CancellationTokenSource();

        var token = cts.Token;

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 4),
            CancellationToken = token
        };

        var timeoutTask = Task.Delay(cutoffMs).ContinueWith(_ => cts.Cancel());

        int executed = 0;
        try
        {
            System.Threading.Tasks.Parallel.ForEach(functions.Where(f => f != null), options, (f, state) =>
            {
                if (token.IsCancellationRequested)
                    state.Stop();

                try
                {
                    f();
                    executed++;
                }
                catch (Exception ex)
                {
                    global::Log.Error(ex);
                }
            });
        }
        catch (OperationCanceledException)
        {
            global::Log.Error($"[Methods] Run timed out after {cutoffMs}ms. Ran " + executed + "/" + functions.Length);
        }
    }

    /// <summary>
    /// Run all actions separately in a non-blocking way
    /// <para>Each action passed is ran in a try catch without notifying callee</para>
    /// See the overloaded method if you want to ignore exceptions
    /// </summary>
    /// <remarks>
    /// All functions passed to this is ran in an unordered and non-blocking way
    /// <para>All functions passed will run till completion, erroring or till main thread is shut down</para>
    /// </remarks>
    /// <param name="onError">Callback invoked if an exception occured</param>
    /// <param name="actions">Array of methods to invoke in a non-blocking way</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// Async.FireAndForget((ex) => Logger.Error(ex), () => System.IO.File.AppendAllText("C:\\temp\\text.log", "hello world"));
    /// </code>
    /// </example>
    internal static void FireAndForget(Action<Exception> onError, params Action[] actions)
    {
        if (actions.IsNot()) return;

        foreach (var action in actions)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (onError != null)
                        onError(ex);
                }
            }
            );
        }
    }

    /// <summary>
    /// Run all actions separately in a non-blocking way
    /// <para>Each action passed is ran in a try catch without notifying callee</para>
    /// See the overloaded method to add a callback for logging exceptions
    /// </summary>
    /// <remarks>
    /// All functions passed to this is ran in an unordered and non-blocking way
    /// <para>All functions passed will run till completion, erroring or till main thread is shut down</para>
    /// </remarks>
    /// <param name="actions">Array of methods to invoke in a non-blocking way</param>
    /// <example>
    /// <code class="language-csharp hljs">
    /// Async.FireAndForget(() => System.IO.File.AppendAllText("C:\\temp\\text.log", "hello world"));
    /// </code>
    /// </example>
    internal static void FireAndForget(params Action[] actions)
    {
        FireAndForget(null, actions);
    }
}