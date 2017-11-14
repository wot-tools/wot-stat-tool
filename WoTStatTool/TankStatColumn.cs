using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

namespace WotStatsTool
{
    public class TankStatColumn : Statistics
    {
        public static Dictionary<int, Tank> Tanks;
        public int ID { get; private set; }
        private ExpectedValues ExpectedValues;

        public TankStatColumn(KeyValuePair<int, Statistics> source, ExpectedValues expectedValues) : base(source.Value)
        {
            ID = source.Key;
            ExpectedValues = expectedValues;
        }

        public double ExpectedFrag => ExpectedValues.Frags;

        public double WN8
        {
            get
            {
                if (ExpectedValues == null)
                    return -1;
                return CalculateWN8(ExpectedValues);
            }
        }

        private Tank _Tank;
        private Tank Tank
        {
            get
            {
                if (_Tank != null || Tanks.TryGetValue(ID, out _Tank))
                    return _Tank;
                return null;
            }
        }

        public string Name { get { return Tank?.Name ?? $"_{ID}"; } }
        public string Nation { get { return Tank?.Nation.ToString(); } }
        public bool IsPremium { get { return Tank?.IsPremium ?? false; } }
        public string Type { get { return Tank?.VehicleType.ToString(); } }
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
    }
}
