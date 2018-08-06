using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SkladMe.Infrastructure
{
    public static class AsyncHelper
    {
        public static ThreadPoolRedirector RedirectToThreadPool() => new ThreadPoolRedirector();

        public static async Task ProcessCollectionAsync<T>(IEnumerable<T> coll, Func<T, Task> method)
        {
            var tasks = coll.Select(method).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public static async Task ExecuteAtUI(Action method)
        {
            await Application.Current.Dispatcher.BeginInvoke(method);
        }
    }

    public struct ThreadPoolRedirector : INotifyCompletion
    {
        // awaiter и awaitable в одном флаконе
        public ThreadPoolRedirector GetAwaiter() => this;

        // true означает выполнять продолжение немедленно 
        public bool IsCompleted => Thread.CurrentThread.IsThreadPoolThread;

        public void OnCompleted(Action continuation) =>
            ThreadPool.QueueUserWorkItem(o => continuation());

        public void GetResult() { }
    }
}
