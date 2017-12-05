using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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

namespace WotStatsTool
{
    /// <summary>
    /// Interaction logic for DateTimeRangePicker.xaml
    /// </summary>
    public partial class DateTimeRangePicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /*
         * setter and getter for dependency properties are completely ignored
         * 2 way binding with "new FrameworkPropertyMetadata(<default>, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)" only when control will change property
         */

        public static readonly DependencyProperty StartDate2Property = DependencyProperty.Register(
            nameof(StartDate2),
            typeof(DateTime),
            typeof(DateTimeRangePicker),
            new FrameworkPropertyMetadata(default(DateTime), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DependencyPropertyChanged));

        public DateTime StartDate2
        {
            get => (DateTime)GetValue(StartDate2Property);
            set => SetValue(StartDate2Property, value);
        }

        public static readonly DependencyProperty StartDatesProperty = DependencyProperty.Register(
            nameof(StartDates),
            typeof(ObservableCollection<DateTime>),
            typeof(DateTimeRangePicker),
            new FrameworkPropertyMetadata(DependencyPropertyChanged));
        public DateTime StartDates
        {
            get => (DateTime)GetValue(StartDatesProperty);
            set => SetValue(StartDatesProperty, value);
        }

        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
            nameof(EndDate),
            typeof(DateTime),
            typeof(DateTimeRangePicker),
            new FrameworkPropertyMetadata(default(DateTime), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public DateTime EndDate
        {
            get => (DateTime)GetValue(EndDateProperty);
            set => SetValue(EndDateProperty, value);
        }

        public static readonly DependencyProperty EndDatesProperty = DependencyProperty.Register(nameof(EndDates), typeof(ObservableCollection<DateTime>), typeof(DateTimeRangePicker));
        public DateTime EndDates
        {
            get => (DateTime)GetValue(EndDatesProperty);
            set => SetValue(EndDatesProperty, value);
        }

        public static readonly DependencyProperty EndDatesVisibleProperty = DependencyProperty.Register(nameof(EndDatesVisible), typeof(bool), typeof(DateTimeRangePicker));
        public bool EndDatesVisible
        {
            get => (bool)GetValue(EndDatesVisibleProperty);
            set => SetValue(EndDatesVisibleProperty, value);
        }

        private static void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimeRangePicker instance)
            {
                switch (e.Property.Name)
                {
                    case nameof(StartDates): instance.SubscribeToCollectionChanges(e, instance.StartDates_CollectionChanged, instance.InitStartDatesTicks); break;
                    case nameof(EndDatesVisible): instance.SetStartDatesTicks(); break;
                }
            }
            else
                throw new ArgumentException($"expected type {nameof(DateTimeRangePicker)}, got {d.GetType()}");
        }

        public void SubscribeToCollectionChanges(DependencyPropertyChangedEventArgs e, NotifyCollectionChangedEventHandler changeListener, Action<ObservableCollection<DateTime>> init)
        {
            if (e.OldValue != null)
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= changeListener;
            if (e.NewValue != null)
            {
                var coll = e.NewValue as ObservableCollection<DateTime>;
                coll.CollectionChanged += changeListener;
                init(coll);
            }
        }

        private Dictionary<double, long> DatesTicksDict = new Dictionary<double, long>();

        #region StartDatesTicks
        public DoubleCollection StartDatesTicks { get; private set; } = new DoubleCollection();

        private ObservableCollection<DateTime> _StartDates;

        public void InitStartDatesTicks(ObservableCollection<DateTime> dates)
        {
            _StartDates = dates;
            foreach (DateTime date in dates)
                InsertStartDate(date);
        }

        private void InsertStartDate(DateTime date)
        {
            var ticks = (double)date.Ticks;
            if (!DatesTicksDict.TryGetValue(ticks, out _))
                DatesTicksDict.Add(ticks, date.Ticks);
        }

        public void StartDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetStartDatesTicks();
        }

        public void SetStartDatesTicks()
        {
            StartDatesTicks = new DoubleCollection(_StartDates.Select(d => (double)d.Ticks));
            OnPropertyChanged(nameof(StartDatesTicks));
        }
        #endregion StartDatesTicks

        #region EndDatesTicks



        #endregion EndDatesTicks

        public DateTimeRangePicker()
        {
            InitializeComponent();
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
