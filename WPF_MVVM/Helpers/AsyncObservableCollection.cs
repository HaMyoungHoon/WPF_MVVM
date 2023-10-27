using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WPF_MVVM.Helpers
{
    internal class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private AsyncOperation? asyncOp = null;

        public AsyncObservableCollection()
        {
            CreateAsyncOp();
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
            CreateAsyncOp();
        }

        private void CreateAsyncOp()
        {
            // Create the AsyncOperation to post events on the creator thread
            asyncOp = AsyncOperationManager.CreateOperation(null);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Post the CollectionChanged event on the creator thread
            asyncOp?.Post(RaiseCollectionChanged, e);
        }

        private void RaiseCollectionChanged(object? param)
        {
            // We are in the creator thread, call the base implementation directly
            if (param is NotifyCollectionChangedEventArgs args)
            {
                base.OnCollectionChanged(args);
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            // Post the PropertyChanged event on the creator thread
            asyncOp?.Post(RaisePropertyChanged, e);
        }

        private void RaisePropertyChanged(object? param)
        {
            if (param is PropertyChangedEventArgs args)
            {
                base.OnPropertyChanged(args);
            }
        }
    }
}
