using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

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

        public IEnumerable<TankStatistics> this[DateTime date] => Load(date);

        public async Task<DateTime> CreateNewSnapshotAsync(WGApiClient client)
        {
            var stats = client.GetPlayerTankStatsAsync(ID);
            var marks = client.GetPlayerMarksAsync(ID);
            await Task.WhenAll(stats, marks);

            var marksDict = marks.Result.ToDictionary(m => m.TankID, m => m.Mark);
            Func<int, int> tryGetMark = id =>
            {
                if (marksDict.TryGetValue(id, out int mark))
                    return mark;
                return 0;
            };

            var newSnapshot = stats.Result.Select(s => new TankStatistics(s.TankID, s.Stats, tryGetMark(s.TankID)));
            
            DateTime now = DateTime.Now;
            if (false == AvailableDates.Any() || CreateIntermediateSnapshot(Load(AvailableDates.Max()), newSnapshot).Any())
            {
                await SaveAsync(newSnapshot, now);
                return now;
            }
            return DateTime.MinValue;
        }

        private async Task SaveAsync(IEnumerable<TankStatistics> data, DateTime date)
        {
            using (Stream stream = File.Create(Path.Combine(Directory, date.Ticks.ToString())))
            using (StreamWriter writer = new StreamWriter(stream))
                await writer.WriteAsync(JsonConvert.SerializeObject(data));
        }

        private IEnumerable<TankStatistics> Load(DateTime date)
        {
            using (Stream stream = File.OpenRead(Path.Combine(Directory, date.Ticks.ToString())))
            using (StreamReader reader = new StreamReader(stream))
                return JsonConvert.DeserializeObject<TankStatistics[]>(reader.ReadToEnd());
        }

        public static void ConvertOldSnapshots()
        {
            foreach(string dir in System.IO.Directory.GetDirectories(BaseDirectory))
                foreach(string path in System.IO.Directory.GetFiles(dir))
                {
                    Dictionary<int, Statistics> shot;
                    using (Stream stream = File.OpenRead(path))
                    using (StreamReader reader = new StreamReader(stream))
                        shot = JsonConvert.DeserializeObject<Dictionary<int, Statistics>>(reader.ReadToEnd());

                    using (Stream stream = File.Create(path))
                    using (StreamWriter writer = new StreamWriter(stream))
                        writer.Write(JsonConvert.SerializeObject(shot.Select(s => new TankStatistics(s.Key, s.Value, 0))));
                }
        }

        public IEnumerable<TankStatistics> CreateIntermediateSnapshot(DateTime begin, DateTime end)
        {
            return CreateIntermediateSnapshot(this[begin], this[end]);
        }

        public static IEnumerable<TankStatistics> CreateIntermediateSnapshot(IEnumerable<TankStatistics> older, IEnumerable<TankStatistics> newer)
        {
            //marks on gun are currently ignored when comparing snapshots
            var olderDict = older.ToDictionary(s => s.TankID, s => s.Statistics);

            foreach (var new_ in newer)
            {
                if (olderDict.TryGetValue(new_.TankID, out Statistics oldStats) && new_.Statistics.Battles == oldStats.Battles)
                    continue;
                yield return new TankStatistics(new_.TankID, new_.Statistics - oldStats, 0);
            }
        }

        public static IEnumerable<int> GetExistingPlayerIDs()
        {
            try
            {
                return System.IO.Directory.GetDirectories(BaseDirectory).Select(ParsePlayerDirectory).Where(id => id > 0);
            }
            catch (DirectoryNotFoundException)
            {
                return Enumerable.Empty<int>();
            }
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
