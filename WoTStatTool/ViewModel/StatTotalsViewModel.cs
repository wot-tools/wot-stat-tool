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
    public class StatTotalsViewModel : BaseViewModel
    {
        private Statistics Statistics;

        public double WN8 { get; private set; }

        public int Damage => Statistics?.Damage ?? 0;
        public double AverageDamage => Statistics?.AvgDamage ?? 0;

        public int DamageReceived => Statistics?.DamageReceived ?? 0;
        public double AverageDamageReceived => Statistics?.AvgDamageReceived ?? 0;

        public int Cap => Statistics?.Cap ?? 0;
        public double AverageCap => Statistics?.AvgCap ?? 0;

        public int Battles => Statistics?.Battles ?? 0;
        public int Victories => Statistics?.Victories ?? 0;
        public double Winrate => Statistics?.Winrate ?? 0;

        public int Decap => Statistics?.Decap ?? 0;
        public double AverageDecap => Statistics?.AvgDecap ?? 0;

        public int Experience => Statistics?.Experience ?? 0;
        public double AverageExperience => Statistics?.AvgExperience ?? 0;

        public int Frags => Statistics?.Frags ?? 0;
        public double AverageFrags => Statistics?.AvgFrags ?? 0;

        public int Spotted => Statistics?.Spotted ?? 0;
        public double AverageSpotted => Statistics?.AvgSpotted ?? 0;

        public int SurvivedBattles => Statistics?.SurvivedBattles ?? 0;
        public double AverageSurvivedBattles => Statistics?.AvgSurvivedBattles ?? 0;
        

        public void Update(Dictionary<int, ExpectedValues> expectedValues, IEnumerable<TankStatColumn> stats)
        {
            //Statistics only counts vehicles with existing expected values
            WN8 = WGApi.WN8.AccountWN8(expectedValues, stats.ToDictionary(c => c.ID, c => (Statistics)c), out Statistics);
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
