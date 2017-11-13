using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WotStatsTool
{
    public class NotifyTaskCompletion : INotifyPropertyChanged
    {
        public NotifyTaskCompletion() { }

        public NotifyTaskCompletion(Task task)
        {
            StartNewTask(task);
        }

        public void StartNewTask(Task task)
        {
            Task = task;
            OnPropertyChanged(String.Empty);
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch { }

            OnPropertyChanged("Status");
            OnPropertyChanged("IsCompleted");
            OnPropertyChanged("IsNotCompleted");

            if (task.IsCanceled)
            {
                OnPropertyChanged("IsCanceled");
            }
            else if (task.IsFaulted)
            {
                OnPropertyChanged("IsFaulted");
                OnPropertyChanged("Exception");
                OnPropertyChanged("InnerException");
                OnPropertyChanged("ErrorMessage");
            }
            else
            {
                OnPropertyChanged("IsSuccessfullyCompleted");
            }
        }

        public Task Task { get; private set; }
        
        public TaskStatus Status => Task?.Status ?? TaskStatus.RanToCompletion;
        public bool IsCompleted => Task?.IsCompleted ?? true;
        public bool IsNotCompleted => !Task?.IsCompleted ?? false;
        public bool IsSuccessfullyCompleted => Task == null ? true : Task.Status == TaskStatus.RanToCompletion;
        public bool IsCanceled => Task?.IsCanceled ?? false;
        public bool IsFaulted => Task?.IsFaulted ?? false;

        public AggregateException Exception => Task.Exception;
        public Exception InnerException => Exception?.InnerException;
        public string ErrorMessage => InnerException?.Message;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }

    public class NotifyTaskCompletion<TResult> : NotifyTaskCompletion
    {
        public NotifyTaskCompletion() { }

        public NotifyTaskCompletion(Task<TResult> task)
            :base(task){ }
        
        public TResult Result => (Task == null || Task.Status != TaskStatus.RanToCompletion) ? default(TResult) : ((Task<TResult>)Task).Result;
    }
}
