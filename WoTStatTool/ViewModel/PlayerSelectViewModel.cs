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
            _FetchAll = new Lazy<RelayCommand>(() => new RelayCommand(o => FetchAllPlayers(), o => !IsFetchingAll));
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IsFetchingAll))
                    FetchAll.RaiseCanExecuteChange();
            };
        }

        private bool _IsFetchingAll;
        public bool IsFetchingAll
        {
            get => _IsFetchingAll;
            set
            {
                if (_IsFetchingAll == value) return;
                _IsFetchingAll = value;
                OnPropertyChanged(nameof(IsFetchingAll));
            }
        }

        private Lazy<RelayCommand> _FetchAll;
        public RelayCommand FetchAll => _FetchAll.Value;

        private async Task FetchAllPlayers()
        {
            IsFetchingAll = true;
            List<Task> tasks = new List<Task>();
            foreach (var id in DaySnapshot.GetExistingPlayerIDs())
                tasks.Add(new DaySnapshot(id).CreateNewSnapshotAsync(Client));
            await Task.WhenAll(tasks);
            IsFetchingAll = false;
        }

        private void DisplayExistingRecords()
        {
            var ids = DaySnapshot.GetExistingPlayerIDs();
            if (!ids.Any())
                return;
            var task = Client.GetPlayerStatsAsync(ids);
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
