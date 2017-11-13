using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WGApi;

namespace WotStatsTool.ViewModel
{
    public class PlayerSelectViewModel : BaseViewModel
    {
        private WGApiClient Client;

        public string UserName => ActivePlayer?.Nickname;
        public int UserID => ActivePlayer?.ID ?? -1;

        public PlayerIDRecord _ActivePlayer;
        public PlayerIDRecord ActivePlayer
        {
            get => _ActivePlayer;
            set
            {
                if ((_ActivePlayer?.ID ?? -1) == (value?.ID ?? -1))
                    return;
                _ActivePlayer = value;
                _NameSearchQuery = _ActivePlayer?.Nickname;
                OnPropertyChanged(nameof(NameSearchQuery));
                OnPropertyChanged(nameof(UserID));
                OnPropertyChanged(nameof(UserName));
            }
        }

        private RelayCommand _FetchStatistics;
        public RelayCommand FetchStatistics
        {
            get
            {
                if (_FetchStatistics == null)
                    _FetchStatistics = new RelayCommand(o => DisplayRangeSelector.TakeNewSnapshot(), o => ActivePlayer != null || DisplayRangeSelector.SnapshotTakingTask.IsCompleted);
                return _FetchStatistics;
            }
        }

        private RelayCommand _SearchPlayer;
        public RelayCommand SearchPlayer
        {
            get
            {
                if (_SearchPlayer == null)
                    _SearchPlayer = new RelayCommand(o => SearchPlayerExact(NameSearchQuery), o => !String.IsNullOrWhiteSpace(NameSearchQuery) || DisplayRangeSelector.SnapshotTakingTask.IsCompleted);
                return _SearchPlayer;
            }
        }

        private string _NameSearchQuery;
        public string NameSearchQuery
        {
            get => _NameSearchQuery;
            set
            {
                if (_NameSearchQuery == value) return;
                _NameSearchQuery = value;
                SearchPlayers(NameSearchQuery);
                OnPropertyChanged(nameof(NameSearchQuery));
            }
        }

        public ObservableCollection<PlayerIDRecord> MatchingPlayers { get; } = new ObservableCollection<PlayerIDRecord>();

        private readonly DisplayRangeSelectorViewModel DisplayRangeSelector;

        public PlayerSelectViewModel(WGApiClient client, DisplayRangeSelectorViewModel displayRangeSelector)
        {
            Client = client;
            DisplayRangeSelector = displayRangeSelector;
            DisplayExistingRecords();
        }

        private void DisplayExistingRecords()
        {
            var task = Client.GetPlayerStatsAsync(DaySnapshot.GetExistingPlayerIDs());
            Action<Task<Dictionary<int, PlayerInfo>>> action = t => SetMatchingPlayers(t.Result.Select(p => new PlayerIDRecord { ID = p.Key, Nickname = p.Value.Nick }));

            task.ContinueWith(action, TaskContinuationOptions.ExecuteSynchronously);
            //Helpers.PreventAsyncDeadlockHack(task, action);
        }

        private void SetMatchingPlayers(IEnumerable<PlayerIDRecord> players)
        {
            MatchingPlayers.Clear();
            if (players == null) return;
            foreach (var player in players)
                MatchingPlayers.Add(player);
        }

        private void SearchPlayers(string query)
        {
            if (String.IsNullOrWhiteSpace(query))
            {
                DisplayExistingRecords();
                return;
            }
            var task = Client.SearchPlayerStartsWithAsync(query);
            Action<Task<Dictionary<string, int>>> action = t =>
            {
                var players = t.Result?.Select(r => new PlayerIDRecord { Nickname = r.Key, ID = r.Value }).ToArray();
                SetMatchingPlayers(players);
            };
            Helpers.PreventAsyncDeadlockHack(task, action);
        }

        private void SearchPlayerExact(string query)
        {
            var task = Client.SearchPlayerExactAsync(query);
            Action<Task<int>> action = t =>
            {
                int id = t.Result;
                if (id > 0)
                    ActivePlayer = new PlayerIDRecord { Nickname = query, ID = id };
            };
            Helpers.PreventAsyncDeadlockHack(task, action);
        }
    }
}
