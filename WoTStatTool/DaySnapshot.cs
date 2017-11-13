using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WGApi;

namespace WotStatsTool
{
    class DaySnapshot
    {
        private int ID;
        public DateTime[] AvailableDates
        {
            get
            {
                return System.IO.Directory.GetFiles(Directory)
                    .Select(p => new DateTime(long.Parse(Path.GetFileNameWithoutExtension(p))))
                    .ToArray();
            }
        }

        private static string BaseDirectory
        {
            get
            {
                Uri baseUri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                return WebUtility.UrlDecode(Path.GetDirectoryName(baseUri.AbsolutePath));
            }
        }

        private string Directory => Path.Combine(BaseDirectory, ID.ToString());

        public DaySnapshot(int id)
        {
            ID = id;
            System.IO.Directory.CreateDirectory(Directory);
        }

        public Dictionary<int, Statistics> this[DateTime date]
        {
            get
            {
                return Load(date);
            }
        }

        public async Task<DateTime> CreateNewSnapshotAsync(WGApiClient client)
        {
            var stats = await client.GetPlayerTankStatsAsync(ID);
            var newSnapshot = stats.ToDictionary(s => s.TankID, s => s.Stats);

            DateTime now = DateTime.Now;
            if (false == AvailableDates.Any() || CreateIntermediateSnapshot(Load(AvailableDates.Max()), newSnapshot).Any())
            {
                await SaveAsync(newSnapshot, now);
                return now;
            }
            return DateTime.MinValue;
        }

        private async Task SaveAsync(Dictionary<int, Statistics> data, DateTime date)
        {
            using (Stream stream = File.Create(Path.Combine(Directory, date.Ticks.ToString())))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteAsync(JsonConvert.SerializeObject(data));
        }

        private Dictionary<int, Statistics> Load(DateTime date)
        {
            using (Stream stream = File.OpenRead(Path.Combine(Directory, date.Ticks.ToString())))
            using (StreamReader reader = new StreamReader(stream))
                return JsonConvert.DeserializeObject<Dictionary<int, Statistics>>(reader.ReadToEnd());
        }

        public Dictionary<int, Statistics> CreateIntermediateSnapshot(DateTime begin, DateTime end)
        {
            return CreateIntermediateSnapshot(this[begin], this[end]);
        }

        public static Dictionary<int, Statistics> CreateIntermediateSnapshot(Dictionary<int, Statistics> older, Dictionary<int, Statistics> newer)
        {
            Dictionary<int, Statistics> result = new Dictionary<int, Statistics>();
            foreach (var statPair in newer)
            {
                if (older.TryGetValue(statPair.Key, out Statistics olderStats) && statPair.Value.Battles == olderStats.Battles)
                    continue;
                result.Add(statPair.Key, statPair.Value - olderStats);
            }
            return result;
        }

        public static IEnumerable<int> GetExistingPlayerIDs()
        {
            return System.IO.Directory.GetDirectories(BaseDirectory).Select(ParsePlayerDirectory).Where(id => id > 0);
        }

        private static int ParsePlayerDirectory(string path)
        {
            string id = Path.GetFileName(path);
            if (int.TryParse(id, out int result))
                return result;
            return -1;
        }
    }
}
