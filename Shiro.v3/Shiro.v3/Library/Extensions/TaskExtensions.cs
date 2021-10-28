using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Shiro.Library.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// from:http://stackoverflow.com/a/22864616/1538014
        /// </summary>
        /// <param name="task"></param>
        /// <param name="acceptableExceptions"></param>
        public static async void Forget(this Task task, params Type[] acceptableExceptions)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // TODO: consider whether derived types are also acceptable.
                if (!((IList) acceptableExceptions).Contains(ex.GetType()))
                    throw;
            }
        }

        public static void Forget(this Task task)
        {
        }

        public static Task RunWithInterval(TimeSpan pollInterval, Action action, CancellationToken token)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (;;)
                    {
                        if (token.WaitCancellationRequested(pollInterval))
                            break;
                        action();
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        public static Task RunWithInterval(TimeSpan pollInterval, Action<Ellipse> action, Ellipse ellipse,
            CancellationToken token)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (;;)
                    {
                        if (token.WaitCancellationRequested(pollInterval))
                            break;
                        action(ellipse);
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        }

        /// <summary>
        /// todo:soru:When this is needed?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Task<T> StartSTATask<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            var thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        } 
    }

    internal static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(this CancellationToken token, TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}