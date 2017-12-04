using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WGApi;
using WotStatsTool.Model;
using WotStatsTool.Services;

namespace WotStatsTool.ViewModel
{
    public class DisplayRangeSelectorViewModel : BaseViewModel
    {
        private readonly WGApiClient Client;
        private readonly INotificationService NotificationService;

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
                else
                {
                    SetEndDates();
                    EndDate = EndDates.FirstOrDefault();
                }
            }
        }

        public DoubleCollection StartDatesTicks => new DoubleCollection(StartDates.Select(d => (double)d.Ticks));
        public double StartDatesMinimum => StartDatesTicks.First();
        public double StartDatesMaximum => StartDatesTicks.Last();


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
                    SetEndDates();
                    if (EndDates.Count == 1)
                        EndDate = EndDates.Single();
                }
                TryCreateNewData();
            }
        }

        private Dictionary<double, DateTime> Dates => DaySnapshot.AvailableDates.ToDictionary(d => (double)d.Ticks);

        public long StartDateTicks
        {
            get => StartDate.Ticks;
            set => StartDate = Dates[value];
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

        public double EndDateTicks
        {
            get => EndDate.Ticks;
            set => EndDate = new DateTime((long)value);
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
                OnPropertyChanged(nameof(StartDatesTicks));
                OnPropertyChanged(nameof(StartDatesMinimum));
                OnPropertyChanged(nameof(StartDatesMaximum));
            }
        }

        private Lazy<RelayCommand> _NewestSnapshot;
        public RelayCommand NewestSnapshot => _NewestSnapshot.Value;

        private Lazy<RelayCommand> _NewestDifference;
        public RelayCommand NewestDifference => _NewestDifference.Value;

        private Lazy<RelayCommand> _PreviousDifference;
        public RelayCommand PreviousDifference => _PreviousDifference.Value;

        private Lazy<RelayCommand> _NextDifference;
        public RelayCommand NextDifference => _NextDifference.Value;

        private Lazy<RelayCommand> _DeleteSelected;
        public RelayCommand DeleteSelected => _DeleteSelected.Value;

        public NotifyTaskCompletion SnapshotTakingTask { get; private set; } = new NotifyTaskCompletion();

        public DisplayRangeSelectorViewModel(WGApiClient client, INotificationService notificationService)
        {
            Client = client;
            NotificationService = notificationService;

            _NewestSnapshot = new Lazy<RelayCommand>(() => new RelayCommand(o => LoadNewestSnapshot(), o => DaySnapshot?.AvailableDates?.Length > 0));
            _NewestDifference = new Lazy<RelayCommand>(() => new RelayCommand(o => LoadNewestDifference(), o => DaySnapshot?.AvailableDates?.Length > 1));
            _PreviousDifference = new Lazy<RelayCommand>(() => new RelayCommand(o => ShiftDifference(-1), o => IsDiffMode && StartDates.IndexOf(StartDate) > 1));
            _NextDifference = new Lazy<RelayCommand>(() => new RelayCommand(o => ShiftDifference(+1), o => IsDiffMode && StartDates.IndexOf(EndDate) <= StartDates.Count - 2));
            _DeleteSelected = new Lazy<RelayCommand>(() => new RelayCommand(o => DeleteSelectedSnapshot(), o => !IsDiffMode && StartDate != DateTime.MinValue));
            StartDates.CollectionChanged += (sender, e) => NewestDifference.RaiseCanExecuteChange();
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

        private void SetEndDates()
        {
            SetDates(EndDates, DaySnapshot?.AvailableDates?.Where(t => t > _StartDate));
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
            if (StartDates.Count < 2)
            {
                LoadNewestSnapshot();
                return;
            }
            IsDiffMode = true;
            SetEndDates();
            StartDate = StartDates[StartDates.Count - 2];
            EndDate = EndDates.Last();
        }

        private void ShiftDifference(int increment)
        {
            int startIndex = StartDates.IndexOf(StartDate) + increment;
            int endIndex = StartDates.IndexOf(EndDate) + increment;

            if (startIndex < 0 || endIndex >= StartDates.Count)
                return;

            StartDate = StartDates[startIndex];
            EndDate = StartDates[endIndex];
        }

        public void LoadUser(int userID)
        {
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MinValue;
            StartDates.Clear();
            EndDates.Clear();
            DaySnapshot = new DaySnapshot(userID);
            if (IsDiffMode)
                LoadNewestDifference();
            else
                LoadNewestSnapshot();
        }

        private void DeleteSelectedSnapshot()
        {
            var selected = StartDate;
            if (NotificationService.Notify("TODO: message_delete", "TODO: caption_delete", System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                if (DaySnapshot.DeleteSnapshot(selected))
                {
                    StartDates.Remove(selected);
                    StartDate = StartDates.LastOrDefault();
                }
        }

        private void SelectFittingSnapshot(TimeSpan time)
        {
            DateTime now = DateTime.Now;
            SelectFittingSnapshot(time.Days, d => (d, (int)(now - d).TotalDays));
        }

        private void SelectFittingSnapshot(int battles)
        {
            int totalBattles = DaySnapshot[DaySnapshot.AvailableDates.Last()].Sum(s => s.Statistics.Battles);
            SelectFittingSnapshot(battles, d => (d, totalBattles - DaySnapshot[d].Sum(s => s.Statistics.Battles)));
        }

        private void SelectFittingSnapshot(int difference, Func<DateTime, (DateTime date, int difference)> getDifference)
        {
            var closestMatch = DaySnapshot.AvailableDates
                .Select(getDifference)
                .OrderBy(o => Math.Abs(o.difference - difference))
                .FirstOrDefault().date;

            if (closestMatch != DateTime.MinValue)
            {
                IsDiffMode = true;
                StartDate = closestMatch;
                EndDate = EndDates.Last();
            }
        }
    }
}
