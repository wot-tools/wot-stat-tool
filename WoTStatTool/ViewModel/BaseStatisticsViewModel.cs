using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

namespace WotStatsTool.ViewModel
{
    public abstract class BaseStatisticsViewModel : BaseViewModel
    {
        protected Statistics Statistics;

        public int Spotted              => Statistics?.Spotted ?? 0;
        public int Victories            => Statistics?.Victories ?? 0;
        public int Damage               => Statistics?.Damage ?? 0;
        public int Experience           => Statistics?.Experience ?? 0;
        public int Frags                => Statistics?.Frags ?? 0;
        public int SurvivedBattles      => Statistics?.SurvivedBattles ?? 0;
        public int Cap                  => Statistics?.Cap ?? 0;
        public int Decap                => Statistics?.Decap ?? 0;
        public int DamageReceived       => Statistics?.DamageReceived ?? 0;
        public int Hits                 => Statistics?.Hits ?? 0;
        public int Shots                => Statistics?.Shots ?? 0;
        public int Battles              => Statistics?.Battles ?? 0;
        public int Draws                => Statistics?.Draws ?? 0;

        public double AvgSpotted        => Spotted / (double)Battles;
        public double Winrate           => Victories / (double)Battles;
        public double Drawrate          => Draws / (double)Battles;
        public double Lossrate          => (Battles - Draws - Victories) / (double)Battles;
        public double AvgDamage         => Damage / (double)Battles;
        public double AvgExperience     => Experience / (double)Battles;
        public double AvgFrags          => Frags / (double)Battles;
        public double AvgSurvivedBattles => SurvivedBattles / (double)Battles;
        public double AvgCap            => Cap / (double)Battles;
        public double AvgDecap          => Decap / (double)Battles;
        public double AvgDamageReceived => DamageReceived / (double)Battles;
        public double Hitrate           => Hits / (double)Shots;

        public virtual double WN8 { get; protected set; }

        protected BaseStatisticsViewModel(Statistics stats)
        {
            Statistics = stats;
        }

        public Statistics ToStatistics() => Statistics;
    }
}
