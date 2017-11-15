using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

namespace WotStatsTool.ViewModel
{
    public class StatTotalsViewModel : BaseStatisticsViewModel
    {
        public StatTotalsViewModel() : base(null) { }
        
        public void Update(Dictionary<int, ExpectedValues> expectedValues, IEnumerable<TankStatistics> stats)
        {
            //Statistics only counts vehicles with existing expected values
            WN8 = WGApi.WN8.AccountWN8(expectedValues, stats.ToDictionary(s => s.TankID, s => s.Statistics), out Statistics);
            TriggerEveryPropertyChanged();
        }

        private void TriggerEveryPropertyChanged()
        {
            //too lazy to type it all
            foreach (var property in typeof(StatTotalsViewModel).GetProperties())
                OnPropertyChanged(property.Name);
        }

        public void Reset()
        {
            Statistics = new Statistics();
            TriggerEveryPropertyChanged();
        }
    }
}
