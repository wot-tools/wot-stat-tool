using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;

namespace WotStatsTool.Model
{
    public class TankFilter
    {
        public Tiers Tiers { get; set; } = Tiers.All;
        public Nations Nations { get; set; } = Nations.All;
        public Premiums Premiums { get; set; } = Premiums.All;
        public VehicleTypes VehicleTypes { get; set; } = VehicleTypes.All;
        public MarksOfExcellence MarksOfExcellence { get; set; } = MarksOfExcellence.All;
        public string Text { get; set; }
    }

    [Flags]
    public enum Premiums
    {
        None = 0,
        NonPremium = 1,
        Premium = 2,
        All = 3,
    }
}
