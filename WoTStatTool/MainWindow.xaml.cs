using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly WGApiClient Client = new WGApiClient("https://api.worldoftanks", Region.eu, ApiKey.Key, new Logger());
        public ObservableCollection<TankStatColumn> Collection { get; } = new ObservableCollection<TankStatColumn>();

        //viewmodels are initialized this way to ensure the binding works
        //binding does not work here when a viewmodel is directly assigned anywhere except here
        //if new assignments to viewmodels are needed, implement INotifyPropertyChanged on mainwindow
        private readonly Lazy<StatTotalsViewModel> _StatTotals;
        public StatTotalsViewModel StatTotals => _StatTotals.Value;

        private Lazy<TankFilterViewModel> _TankFilter;
        public TankFilterViewModel TankFilter => _TankFilter.Value;

        private Lazy<PlayerSelectViewModel> _PlayerSelect;
        public PlayerSelectViewModel PlayerSelect => _PlayerSelect.Value;

        private Lazy<DisplayRangeSelectorViewModel> _DisplayRangeSelector;
        public DisplayRangeSelectorViewModel DisplayRangeSelector => _DisplayRangeSelector.Value;

        private Lazy<ExpectedValuesSelectorViewModel> _ExpectedValuesSelector;
        public ExpectedValuesSelectorViewModel ExpectedValueSelector => _ExpectedValuesSelector.Value;
        
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

            _StatTotals = new Lazy<StatTotalsViewModel>(() => new StatTotalsViewModel());
            _TankFilter = new Lazy<TankFilterViewModel>(() => new TankFilterViewModel());
            _PlayerSelect = new Lazy<PlayerSelectViewModel>(() => new PlayerSelectViewModel(Client, DisplayRangeSelector));
            _DisplayRangeSelector = new Lazy<DisplayRangeSelectorViewModel>(() => new DisplayRangeSelectorViewModel(Client));
            _ExpectedValuesSelector = new Lazy<ExpectedValuesSelectorViewModel>(
                () => new ExpectedValuesSelectorViewModel(new VbaddictExpectedValueList(), new XvmExpectedValueList()));

            //set regular sort direction to descending instead of ascending
            dgOverview.Sorting += (o, e) => e.Column.SortDirection = e.Column.SortDirection ?? System.ComponentModel.ListSortDirection.Ascending;

            Helpers.PreventAsyncDeadlockHack(Client.GetVehiclesAsync(), t => TankStatColumn.Tanks = t.Result);
            
            //temp remove later
            PlayerSelect.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "UserName")
                    tbUsername.Text = PlayerSelect.UserName;
                if (e.PropertyName == "UserID")
                    DisplayRangeSelector.LoadUser(PlayerSelect.UserID);
            };

            TankFilter.PropertyChanged += CreateUpdateDataGridListener(nameof(TankFilter.TankFilter));
            DisplayRangeSelector.PropertyChanged += CreateUpdateDataGridListener(nameof(DisplayRangeSelector.Data));
            ExpectedValueSelector.PropertyChanged += CreateUpdateDataGridListener(nameof(ExpectedValueSelector.SelectedExpectedValues));

            Title = $"WoT Stat Tool {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private PropertyChangedEventHandler CreateUpdateDataGridListener(string propertyName) =>
            (sender, e) => { if (e.PropertyName == propertyName) UpdateDataGrid(); };

        private void UpdateDataGrid()
        {
            SetDataGrid(DisplayRangeSelector.Data);
        }

        private TankStatColumn MakeColumn(KeyValuePair<int, Statistics> kvp)
        {
            ExpectedValueSelector.SelectedExpectedValues.TryGetValue(kvp.Key, out ExpectedValues values);
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
            StatTotals.Update(ExpectedValueSelector.SelectedExpectedValues, columns);
        }
    }
}
