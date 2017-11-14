using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;

namespace WotStatsTool.ViewModel
{
    public class ExpectedValuesSelectorViewModel : BaseViewModel
    {
        public bool VbAddictSelected
        {
            get => RadioState_ == RadioState.VbAddict;
            set => RadioButtonPropertyChanges(RadioState.VbAddict, nameof(VbAddictSelected), value);
        }
        
        public bool XvmSelected
        {
            get => RadioState_ == RadioState.Xvm;
            set => RadioButtonPropertyChanges(RadioState.Xvm, nameof(XvmSelected), value);
        }

        private enum RadioState { VbAddict, Xvm }

        private RadioState RadioState_;

        private void RadioButtonPropertyChanges(RadioState state, string propertyName, bool value)
        {
            //ignore unselecting of the previous selected radio button
            if (value)
            {
                RadioState_ = state;
                OnPropertyChanged(propertyName);
                OnPropertyChanged(nameof(CurrentlySelectedList));
            }
        }

        private readonly VbaddictExpectedValueList VbAddict;
        private readonly XvmExpectedValueList Xvm;

        public IExpectedValueList CurrentlySelectedList
        {
            get
            {
                switch (RadioState_)
                {
                    case RadioState.VbAddict: return VbAddict;
                    case RadioState.Xvm: return Xvm;
                    default:  throw new NotImplementedException("undefined state in ExpectedValuesSelectorViewModel: no source selected");
                }
            }
        }

        private string _SelectedVersion;
        public string SelectedVersion
        {
            get => _SelectedVersion;
            set
            {
                //when clearing the Versions, this will be set to null. After setting the Versions, we update this anyways, so we can just ignore it.
                if (value == null) return;
                if (_SelectedVersion == value) return;
                _SelectedVersion = value;
                OnPropertyChanged(nameof(SelectedVersion));
                OnPropertyChanged(nameof(SelectedExpectedValues));
            }
        }

        public Dictionary<int, ExpectedValues> SelectedExpectedValues => CurrentlySelectedList[SelectedVersion];

        public ObservableCollection<string> Versions { get; } = new ObservableCollection<string>();

        private bool _IsInitialized = false;
        public bool IsInitialized
        {
            get => _IsInitialized;
            private set
            {
                if (_IsInitialized == value) return;
                _IsInitialized = value;
                OnPropertyChanged(nameof(IsInitialized));
            }
        }

        public ExpectedValuesSelectorViewModel(VbaddictExpectedValueList vbAddict, XvmExpectedValueList xvm)
        {
            VbAddict = vbAddict;
            Xvm = xvm;
            Initialize();
            PropertyChanged += ListenForSelectedListChanges;
        }

        private async Task Initialize()
        {
            await Task.WhenAll(VbAddict.Initialize(), Xvm.Initialize());
            XvmSelected = true;
            IsInitialized = true;
        }

        private void SetVersions(IEnumerable<string> versions)
        {
            Versions.Clear();
            foreach (string version in versions)
                Versions.Add(version);
        }

        private void ListenForSelectedListChanges(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentlySelectedList))
            {
                SetVersions(CurrentlySelectedList.Versions);
                SelectedVersion = Versions.Last();
            }
        }
    }
}
