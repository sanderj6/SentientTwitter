using Azure;

using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using SentientTwitter.Data;
using Azure.AI.TextAnalytics;
using System.Threading.Channels;

namespace SentientTwitter.Services;

public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
    private readonly ILogger<BackgroundTaskQueue> _logger;
    public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger, int capacity)
    {
        _logger = logger;
        // Capacity should be set based on the expected application load and
        // number of concurrent threads accessing the queue.            
        // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task,
        // which completes only when space became available. This leads to backpressure,
        // in case too many publishers/calls start accumulating.
        // The solution? Change BoundedChannelFullMode.Wait to use one of Drop* options
        // or you can simply check if the queue is full and skip the write if the task can be skipped
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = true
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        if (workItem == null)
            return;
        if (_queue.Reader.Count < 250)
            await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        if (!await _queue.Reader.WaitToReadAsync(cancellationToken))
            return null;
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}