using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

namespace WotStatsTool.ViewModel
{
    public class DisplayRangeSelectorViewModel : BaseViewModel
    {
        private WGApiClient Client;

        public ObservableCollection<DateTime> StartDates { get; } = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> EndDates { get; } = new ObservableCollection<DateTime>();

        private bool _IsDiffMode = false;
        public bool IsDiffMode
        {
            get => _IsDiffMode;
            set
            {
                if (_IsDiffMode == value) return;
                _IsDiffMode = value;
                OnPropertyChanged(nameof(IsDiffMode));
                if (!IsDiffMode)
                {
                    EndDate = DateTime.MinValue;
                    EndDates.Clear();
                }
            }
        }

        private DateTime _StartDate;
        public DateTime StartDate
        {
            get => _StartDate;
            set
            {
                if (_StartDate == value) return;
                _StartDate = value;
                if (_StartDate > EndDate) EndDate = DateTime.MinValue;
                OnPropertyChanged(nameof(StartDate));
                if (IsDiffMode)
                {
                    SetDates(EndDates, DaySnapshot?.AvailableDates?.Where(t => t > _StartDate));
                    if (EndDates.Count == 1)
                        EndDate = EndDates.Single();
                }
                TryCreateNewData();
            }
        }

        private DateTime _EndDate;
        public DateTime EndDate
        {
            get => _EndDate;
            set
            {
                if (_EndDate == value) return;
                _EndDate = value;
                OnPropertyChanged(nameof(EndDate));
                TryCreateNewData();
            }
        }

        private IEnumerable<TankStatistics> _Data;
        public IEnumerable<TankStatistics> Data
        {
            get => _Data;
            set
            {
                if (_Data == value) return;
                _Data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        private DaySnapshot _DaySnapshot;
        private DaySnapshot DaySnapshot
        {
            get
            {
                return _DaySnapshot;
            }
            set
            {
                _DaySnapshot = value;
                InitDates();
            }
        }

        private RelayCommand _NewestSnapshot;
        public RelayCommand NewestSnapshot
        {
            get
            {
                if (_NewestSnapshot == null)
                    _NewestSnapshot = new RelayCommand(o => LoadNewestSnapshot(), o => DaySnapshot?.AvailableDates?.Length > 0);
                return _NewestSnapshot;
            }
        }

        private RelayCommand _NewestDifference;
        public RelayCommand NewestDifference
        {
            get
            {
                if (_NewestDifference == null)
                    _NewestDifference = new RelayCommand(o => LoadNewestDifference(), o => DaySnapshot?.AvailableDates?.Length > 1);
                return _NewestDifference;
            }
        }

        public NotifyTaskCompletion SnapshotTakingTask { get; private set; } = new NotifyTaskCompletion();

        public DisplayRangeSelectorViewModel(WGApiClient client)
        {
            Client = client;
        }

        private void TryCreateNewData()
        {
            if (StartDate == DateTime.MinValue || IsDiffMode && EndDate == DateTime.MinValue)
            {
                Data = null;
                return;
            }
            Data = !IsDiffMode
                ? DaySnapshot[StartDate]
                : DaySnapshot.CreateIntermediateSnapshot(StartDate, EndDate);
        }

        public void TakeNewSnapshot()
        {
            SnapshotTakingTask.StartNewTask(TakeNewSnapshotAsync());
        }

        private async Task TakeNewSnapshotAsync()
        {
            DateTime newestTime = await DaySnapshot.CreateNewSnapshotAsync(Client);
            if (newestTime != DateTime.MinValue)
            {
                StartDates.Add(newestTime);
                EndDates.Add(newestTime);
            }
        }

        private void InitDates()
        {
            EndDates.Clear();
            //if (DaySnapshot == null) return;
            SetDates(StartDates, DaySnapshot?.AvailableDates);
        }

        private void SetDates(ObservableCollection<DateTime> collection, IEnumerable<DateTime> dates)
        {
            if (dates == null)
            {
                collection.Clear();
                return;
            }

            //non-destructive approach to keep as many items as possible to not break e.g. selected items and such
            //collectioncopy keeps track of all items that were in the collection and not in the new dates
            HashSet<DateTime> collectionCopy = new HashSet<DateTime>(collection);
            foreach (var date in dates)
                if (collectionCopy.Contains(date))
                    collectionCopy.Remove(date);
                else
                    InsertIntoSortedCollection(collection, date, (d1, d2) => d1.CompareTo(d2));
            //remove all dates from the collection that are not in the new dates
            foreach (var date in collectionCopy)
                collection.Remove(date);
            
        }

        private void InsertIntoSortedCollection<T>(Collection<T> collection, T item, Func<T, T, int> comparer)
        {
            bool inserted = false;

            //iterate through the collection and insert the item at the index of the first element bigger than the item
            for (int i = 0; i < collection.Count; i++)
                if (comparer(collection[i], item) > 0)
                {
                    collection.Insert(i, item);
                    inserted = true;
                    break;
                }

            //if item wasnt inserted, it was bigger than all items, so simply append it (also catches the case of an empty collection)
            if (!inserted)
                collection.Add(item);
        }

        private void LoadNewestSnapshot()
        {
            IsDiffMode = false;
            StartDate = StartDates.Last();
        }

        private void LoadNewestDifference()
        {
            IsDiffMode = true;
            StartDate = StartDates[StartDates.Count - 2];
            EndDate = EndDates.Last();
        }

        public void LoadUser(int userID)
        {
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
            StartDates.Clear();
            EndDates.Clear();
            DaySnapshot = new DaySnapshot(userID);
        }
    }
}
