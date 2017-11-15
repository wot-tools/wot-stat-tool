using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using WotStatsTool.Model;
using WotStatsTool.ViewModel;

namespace WotStatsTool
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly WGApiClient Client = new WGApiClient("https://api.worldoftanks", Region.eu, ApiKey.Key, new Logger());
        public ObservableCollection<TankStatisticsViewModel> Collection { get; } = new ObservableCollection<TankStatisticsViewModel>();

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

        private Lazy<ExpectedValuesSelectorViewModel> _ExpectedValuesSelector2;
        public ExpectedValuesSelectorViewModel ExpectedValueSelector2 => _ExpectedValuesSelector2.Value;

        // when userID changed
        //put into datepicker / snapshotpicker of some kind
        //DaySnapshot = new DaySnapshot(_ActivePlayer.ID);

        //when stats are to be fetched
        //DaySnapshot.CreateNewSnapshot(Client);
        //UpdateDates();

        public NotifyTaskCompletion ValueListLoading { get; set; }

        private bool _ShowExpectedValues = false;
        public bool ShowExpectedValues
        {
            get => _ShowExpectedValues;
            set
            {
                if (_ShowExpectedValues == value) return;
                _ShowExpectedValues = value;
                OnPropertyChanged(nameof(ShowExpectedValues));
                OnPropertyChanged(nameof(ExpectedValuesVisibility));
            }
        }

        public Visibility ExpectedValuesVisibility => ShowExpectedValues ? Visibility.Visible : Visibility.Collapsed;

        private bool _CompareExpectedValues = false;
        public bool CompareExpectedValues
        {
            get => _CompareExpectedValues;
            set
            {
                if (_CompareExpectedValues == value) return;
                _CompareExpectedValues = value;
                OnPropertyChanged(nameof(CompareExpectedValues));
                OnPropertyChanged(nameof(CompareValuesVisibility));
            }
        }

        public Visibility CompareValuesVisibility => CompareExpectedValues ? Visibility.Visible : Visibility.Collapsed;

        public MainWindow()
        {
            InitializeComponent();
            //DaySnapshot.ConvertOldSnapshots();

            //temp set context to itself
            DataContext = this;

            _StatTotals = new Lazy<StatTotalsViewModel>(() => new StatTotalsViewModel());
            _TankFilter = new Lazy<TankFilterViewModel>(() => new TankFilterViewModel());
            _PlayerSelect = new Lazy<PlayerSelectViewModel>(() => new PlayerSelectViewModel(Client, DisplayRangeSelector));
            _DisplayRangeSelector = new Lazy<DisplayRangeSelectorViewModel>(() => new DisplayRangeSelectorViewModel(Client));
            _ExpectedValuesSelector = new Lazy<ExpectedValuesSelectorViewModel>(
                () => new ExpectedValuesSelectorViewModel(new VbaddictExpectedValueList(), new XvmExpectedValueList()));
            _ExpectedValuesSelector2 = new Lazy<ExpectedValuesSelectorViewModel>(
                () => new ExpectedValuesSelectorViewModel(new VbaddictExpectedValueList(), new XvmExpectedValueList()));

            //set regular sort direction to descending instead of ascending
            dgOverview.Sorting += (o, e) => e.Column.SortDirection = e.Column.SortDirection ?? System.ComponentModel.ListSortDirection.Ascending;

            Helpers.PreventAsyncDeadlockHack(Client.GetVehiclesAsync(), t => TankStatisticsViewModel.Tanks = t.Result);
            
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
            ExpectedValueSelector2.PropertyChanged += CreateUpdateDataGridListener(nameof(ExpectedValueSelector2.SelectedExpectedValues));

            Title = $"WoT Stat Tool {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private PropertyChangedEventHandler CreateUpdateDataGridListener(string propertyName) =>
            (sender, e) => { if (e.PropertyName == propertyName) UpdateDataGrid(); };

        private void UpdateDataGrid()
        {
            SetDataGrid(DisplayRangeSelector.Data);
        }

        private TankStatisticsViewModel MakeRow(TankStatistics stats)
        {
            ExpectedValueSelector.SelectedExpectedValues.TryGetValue(stats.TankID, out ExpectedValues values);
            ExpectedValueSelector2.SelectedExpectedValues.TryGetValue(stats.TankID, out ExpectedValues values2);
            return new TankStatisticsViewModel(stats, values, values2);
        }

        private void SetDataGrid(IEnumerable<TankStatistics> stats)
        {
            Collection.Clear();
            if (stats == null)
            {
                StatTotals.Reset();
                return;
            }
            var rows = stats.Select(MakeRow).Where(r => r.MeetsFilterCriteria(TankFilter.TankFilter)).ToArray();
            foreach (TankStatisticsViewModel row in rows)
                Collection.Add(row);
            //lblWN8.Text = WN8.AccountWN8(ExpectedValueList, WN8Version, columns).ToString("N2");
            StatTotals.Update(ExpectedValueSelector.SelectedExpectedValues, rows.Select(r => r.ToTankStatistics()));
        }
    }

    public class IsLessThanConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new IsLessThanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (String.IsNullOrWhiteSpace(value as string))
                val = 0;
            else
                val = double.Parse(value as string, culture.NumberFormat);
            double compareTo = double.Parse(parameter as string, culture.NumberFormat);
            return val < compareTo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
