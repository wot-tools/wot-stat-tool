using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;

namespace WotStatsTool.ViewModel
{
    public class ExpectedValuesSelectorViewModel : BaseViewModel
    {
        private bool _VbAddictSelected;
        public bool VbAddictSelected
        {
            get => _VbAddictSelected;
            set
            {
                if (_VbAddictSelected == value) return;
                _VbAddictSelected = value;
                OnPropertyChanged(nameof(VbAddictSelected));
                OnPropertyChanged(nameof(CurrentlySelectedList));
            }
        }

        private bool _XvmSelected;
        public bool XvmSelected
        {
            get => _XvmSelected;
            set
            {
                if (_XvmSelected == value) return;
                _XvmSelected = value;
                OnPropertyChanged(nameof(XvmSelected));
                OnPropertyChanged(nameof(CurrentlySelectedList));
            }
        }

        private readonly VbaddictExpectedValueList VbAddict;
        private readonly XvmExpectedValueList Xvm;

        public IExpectedValueList CurrentlySelectedList
        {
            get
            {
                if (VbAddictSelected) return VbAddict;
                if (XvmSelected) return Xvm;
                throw new NotImplementedException("undefined state in ExpectedValuesSelectorViewModel: no source selected");
            }
        }

        public readonly ObservableCollection<string> Versions = new ObservableCollection<string>();

        public ExpectedValuesSelectorViewModel(VbaddictExpectedValueList vbAddict, XvmExpectedValueList xvm)
        {
            VbAddict = vbAddict;
            Xvm = xvm;
            XvmSelected = true;
        }
    }
}
