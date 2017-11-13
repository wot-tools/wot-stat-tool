using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WGApi;
using WotStatsTool.ViewModel;

namespace WotStatsTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WGApiClient Client = new WGApiClient("https://api.worldoftanks", Region.eu, ApiKey.Key, new Logger());
        private IExpectedValueList ExpectedValueList = new XvmExpectedValueList();
        public ObservableCollection<TankStatColumn> Collection { get; set; } = new ObservableCollection<TankStatColumn>();

        public ObservableCollection<string> Versions { get; set; } = new ObservableCollection<string>();

        private void SetVersions(IEnumerable<string> versions)
        {
            Versions.Clear();
            foreach (string version in versions)
                Versions.Add(version);
        }

        public string _SelectedExpectedVersion;
        public string SelectedExpectedVersion
        {
            get => _SelectedExpectedVersion;
            set
            {
                if (_SelectedExpectedVersion == value) return;
                _SelectedExpectedVersion = value;
                UpdateDataGrid();
            }
        }

        //viewmodels are initialized this way to ensure the binding works
        //binding does not work here when a viewmodel is directly assigned anywhere except here
        //if new assignments to viewmodels are needed, implement INotifyPropertyChanged on mainwindow
        private StatTotalsViewModel _StatTotals;
        public StatTotalsViewModel StatTotals
        {
            get
            {
                if (_StatTotals == null)
                    _StatTotals = new StatTotalsViewModel();
                return _StatTotals;
            }
        }

        private TankFilterViewModel _TankFilter;
        public TankFilterViewModel TankFilter
        {
            get
            {
                if (_TankFilter == null)
                    _TankFilter = new TankFilterViewModel();
                return _TankFilter;
            }
        }

        private PlayerSelectViewModel _PlayerSelect;
        public PlayerSelectViewModel PlayerSelect
        {
            get
            {
                if (_PlayerSelect == null)
                    //maybe wrap client in a lambda to account for a changing client?
                    _PlayerSelect = new PlayerSelectViewModel(Client, DisplayRangeSelector);
                return _PlayerSelect;
            }
        }

        private DisplayRangeSelectorViewModel _DisplayRangeSelector;
        public DisplayRangeSelectorViewModel DisplayRangeSelector
        {
            get
            {
                if (_DisplayRangeSelector == null)
                    _DisplayRangeSelector = new DisplayRangeSelectorViewModel(Client);
                return _DisplayRangeSelector;
            }
        }

        // when userID changed
        //put into datepicker / snapshotpicker of some kind
        //DaySnapshot = new DaySnapshot(_ActivePlayer.ID);

        //when stats are to be fetched
        //DaySnapshot.CreateNewSnapshot(Client);
        //UpdateDates();

        public NotifyTaskCompletion ValueListLoading { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            //temp set context to itself
            DataContext = this;

            //set regular sort direction to descending instead of ascending
            dgOverview.Sorting += (o, e) => e.Column.SortDirection = e.Column.SortDirection ?? System.ComponentModel.ListSortDirection.Ascending;

            Helpers.PreventAsyncDeadlockHack(Client.GetVehiclesAsync(), t => TankStatColumn.Tanks = t.Result);
            
            ValueListLoading = new NotifyTaskCompletion(ExpectedValueList.Initialize());
            
            ValueListLoading.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(ValueListLoading.IsSuccessfullyCompleted))
                {
                    SetVersions(ExpectedValueList.Versions);
                    Console.WriteLine(Versions.Max());
                    SelectedExpectedVersion = Versions.Max();
                }
            };

            //temp remove later
            PlayerSelect.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "UserName")
                    tbUsername.Text = PlayerSelect.UserName;
                if (e.PropertyName == "UserID")
                    DisplayRangeSelector.LoadUser(PlayerSelect.UserID);
            };



            TankFilter.PropertyChanged += (sender, e) => { if (e.PropertyName == "TankFilter") UpdateDataGrid(); };
            DisplayRangeSelector.PropertyChanged += (sender, e) => { if (e.PropertyName == "Data") UpdateDataGrid(); };

            Title = $"WoT Stat Tool {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void UpdateDataGrid()
        {
            SetDataGrid(DisplayRangeSelector.Data);
        }

        private TankStatColumn MakeColumn(KeyValuePair<int, Statistics> kvp)
        {
            ExpectedValueList[SelectedExpectedVersion].TryGetValue(kvp.Key, out ExpectedValues values);
            return new TankStatColumn(kvp, values);
        }

        private void SetDataGrid(Dictionary<int, Statistics> stats)
        {
            Collection.Clear();
            if (stats == null)
            {
                StatTotals.Reset();
                return;
            }
            var columns = stats.Select(MakeColumn).Where(c => c.MeetsFilterCriteria(TankFilter.TankFilter)).ToArray();
            foreach (TankStatColumn column in columns)
                Collection.Add(column);
            //lblWN8.Text = WN8.AccountWN8(ExpectedValueList, WN8Version, columns).ToString("N2");
            StatTotals.Update(ExpectedValueList[SelectedExpectedVersion], columns);
        }
    }
}
