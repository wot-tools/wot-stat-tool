using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;

namespace WotStatsTool.Model
{
    public class TankStatistics
    {
        public int TankID { get; set; }
        public Statistics Statistics { get; set; }
        public int MarksOnGun { get; set; }

        public TankStatistics(int tankID, Statistics stats, int marksOnGun)
        {
            TankID = tankID;
            Statistics = stats;
            MarksOnGun = marksOnGun;
        }
    }
}
