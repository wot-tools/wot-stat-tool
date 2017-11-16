using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotStatsTool
{
    static class Helpers
    {
        public static void PreventAsyncDeadlockHack<T>(Task<T> task, Action<Task<T>> afterCallAction)
        {
            //can't block on UI thread without causing a deadlock
            task.ContinueWith(afterCallAction, TaskContinuationOptions.ExecuteSynchronously);



            //var notifier = new NotifyTaskCompletion<T>(task);
            //notifier.PropertyChanged += (o, e) =>
            //{
            //    if (e.PropertyName == nameof(notifier.IsSuccessfullyCompleted))
            //        afterCallAction?.Invoke(notifier.Result);
            //};
        }
    }
}
