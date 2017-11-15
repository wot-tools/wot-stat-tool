using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

namespace WotStatsTool.ViewModel
{
    public class TankStatisticsViewModel : BaseStatisticsViewModel
    {
        private readonly TankStatistics TankStatistics;
        private readonly ExpectedValues ExpectedValues;
        private readonly ExpectedValues ComparisonExpectedValues;

        public TankStatisticsViewModel(TankStatistics stats, ExpectedValues expectedValues, ExpectedValues comparisonExpectedValues)
            :base(stats.Statistics)
        {
            TankStatistics = stats;
            ExpectedValues = expectedValues;
            ComparisonExpectedValues = comparisonExpectedValues;
        }

        public int MarksOnGun => TankStatistics.MarksOnGun;
        
        public double ExFrags           => ExpectedValues?.Frags ?? double.NaN;
        public double ExDamage          => ExpectedValues?.Damage ?? double.NaN;
        public double ExSpots           => ExpectedValues?.Spots ?? double.NaN;
        public double ExDefense         => ExpectedValues?.Defense ?? double.NaN;
        public double ExWinrate         => ExpectedValues?.Winrate ?? double.NaN;

        public double CoFrags           => ComparisonExpectedValues?.Frags ?? double.NaN;
        public double CoDamage          => ComparisonExpectedValues?.Damage ?? double.NaN;
        public double CoSpots           => ComparisonExpectedValues?.Spots ?? double.NaN;
        public double CoDefense         => ComparisonExpectedValues?.Defense ?? double.NaN;
        public double CoWinrate         => ComparisonExpectedValues?.Winrate ?? double.NaN;

        public double DeltaFrags        => ExFrags - CoFrags;
        public double DeltaDamage       => ExDamage - CoDamage;
        public double DeltaSpots        => ExSpots - CoSpots;
        public double DeltaDefense      => ExDefense - CoDefense;
        public double DeltaWinrate      => ExWinrate - CoWinrate;

        public override double WN8
        {
            get
            {
                if (ExpectedValues == null)
                    return double.NaN;
                return WGApi.WN8.Calculate(TankStatistics.Statistics, ExpectedValues);
            }
        }

        public double ComparisonWN8
        {
            get
            {
                if (ComparisonExpectedValues == null)
                    return double.NaN;
                return WGApi.WN8.Calculate(TankStatistics.Statistics, ComparisonExpectedValues);
            }
        }

        public double DeltaWN8 => WN8 - ComparisonWN8;

        public static Dictionary<int, Tank> Tanks;
        private Tank _Tank;
        private Tank Tank
        {
            get
            {
                if (_Tank != null || Tanks.TryGetValue(TankStatistics.TankID, out _Tank))
                    return _Tank;
                return null;
            }
        }

        public string Name => Tank?.Name ?? $"_{TankStatistics.TankID}";
        public string Nation => Tank?.Nation.ToString();
        public bool IsPremium => Tank?.IsPremium ?? false;
        public string Type => Tank?.VehicleType.ToString();
        public int Tier => Tank?.Tier ?? -1;

        public bool MeetsFilterCriteria(TankFilter filter)
        {
            if (Tank == null)
                return false;

            if (!filter.Tiers.HasFlag((Tiers)(Math.Pow(2, Tank.Tier - 1))))
                return false;

            if (!filter.Nations.HasFlag(Tank.Nation))
                return false;

            if (!filter.VehicleTypes.HasFlag(Tank.VehicleType))
                return false;

            //flag nonpremium    tank premium
            //    0                   0           1
            //    0                   1           0
            //    1                   0           0
            //    1                   1           0
            if (!filter.Premiums.HasFlag(Premiums.NonPremium) && !Tank.IsPremium)
                return false;

            //flag premium    tank premium
            //    0                   0           0
            //    0                   1           1
            //    1                   0           0
            //    1                   1           0
            if (!filter.Premiums.HasFlag(Premiums.Premium) && Tank.IsPremium)
                return false;

            if (!MatchTankName(filter.Text))
                return false;

            return true;
        }

        private bool MatchTankName(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
                return true;
            if (Name.ToLower().Contains(query.ToLower()))
                return true;
            return false;
        }

        public TankStatistics ToTankStatistics() => TankStatistics;
    }
}
