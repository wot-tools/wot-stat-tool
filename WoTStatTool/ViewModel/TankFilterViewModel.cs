using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;
using WotStatsTool.Model;

namespace WotStatsTool.ViewModel
{
    public class TankFilterViewModel : BaseViewModel
    {
        #region Tiers
        public bool Tier1
        {
            get => Tiers.HasFlag(Tiers.One);
            set
            {
                if (Tier1 == value) return;
                Tiers ^= Tiers.One;
                OnPropertyChanged(nameof(Tier1));
            }
        }

        public bool Tier2
        {
            get => Tiers.HasFlag(Tiers.Two);
            set
            {
                if (Tier2 == value) return;
                Tiers ^= Tiers.Two;
                OnPropertyChanged(nameof(Tier2));
            }
        }

        public bool Tier3
        {
            get => Tiers.HasFlag(Tiers.Three);
            set
            {
                if (Tier3 == value) return;
                Tiers ^= Tiers.Three;
                OnPropertyChanged(nameof(Tier3));
            }
        }

        public bool Tier4
        {
            get => Tiers.HasFlag(Tiers.Four);
            set
            {
                if (Tier4 == value) return;
                Tiers ^= Tiers.Four;
                OnPropertyChanged(nameof(Tier4));
            }
        }

        public bool Tier5
        {
            get => Tiers.HasFlag(Tiers.Five);
            set
            {
                if (Tier5 == value) return;
                Tiers ^= Tiers.Five;
                OnPropertyChanged(nameof(Tier5));
            }
        }

        public bool Tier6
        {
            get => Tiers.HasFlag(Tiers.Six);
            set
            {
                if (Tier6 == value) return;
                Tiers ^= Tiers.Six;
                OnPropertyChanged(nameof(Tier6));
            }
        }

        public bool Tier7
        {
            get => Tiers.HasFlag(Tiers.Seven);
            set
            {
                if (Tier7 == value) return;
                Tiers ^= Tiers.Seven;
                OnPropertyChanged(nameof(Tier7));
            }
        }

        public bool Tier8
        {
            get => Tiers.HasFlag(Tiers.Eight);
            set
            {
                if (Tier8 == value) return;
                Tiers ^= Tiers.Eight;
                OnPropertyChanged(nameof(Tier8));
            }
        }

        public bool Tier9
        {
            get => Tiers.HasFlag(Tiers.Nine);
            set
            {
                if (Tier9 == value) return;
                Tiers ^= Tiers.Nine;
                OnPropertyChanged(nameof(Tier9));
            }
        }

        public bool Tier10
        {
            get => Tiers.HasFlag(Tiers.Ten);
            set
            {
                if (Tier10 == value) return;
                Tiers ^= Tiers.Ten;
                OnPropertyChanged(nameof(Tier10));
            }
        }
        #endregion Tiers

        #region Nations
        public bool USSR
        {
            get => Nations.HasFlag(Nations.USSR);
            set
            {
                if (USSR == value) return;
                Nations ^= Nations.USSR;
                OnPropertyChanged(nameof(USSR));
            }
        }

        public bool France
        {
            get => Nations.HasFlag(Nations.France);
            set
            {
                if (France == value) return;
                Nations ^= Nations.France;
                OnPropertyChanged(nameof(France));
            }
        }

        public bool Germany
        {
            get => Nations.HasFlag(Nations.Germany);
            set
            {
                if (Germany == value) return;
                Nations ^= Nations.Germany;
                OnPropertyChanged(nameof(Germany));
            }
        }

        public bool USA
        {
            get => Nations.HasFlag(Nations.USA);
            set
            {
                if (USA == value) return;
                Nations ^= Nations.USA;
                OnPropertyChanged(nameof(USA));
            }
        }

        public bool UK
        {
            get => Nations.HasFlag(Nations.UK);
            set
            {
                if (UK == value) return;
                Nations ^= Nations.UK;
                OnPropertyChanged(nameof(UK));
            }
        }

        public bool China
        {
            get => Nations.HasFlag(Nations.China);
            set
            {
                if (China == value) return;
                Nations ^= Nations.China;
                OnPropertyChanged(nameof(China));
            }
        }

        public bool Japan
        {
            get => Nations.HasFlag(Nations.Japan);
            set
            {
                if (Japan == value) return;
                Nations ^= Nations.Japan;
                OnPropertyChanged(nameof(Japan));
            }
        }

        public bool Czech
        {
            get => Nations.HasFlag(Nations.Czech);
            set
            {
                if (Czech == value) return;
                Nations ^= Nations.Czech;
                OnPropertyChanged(nameof(Czech));
            }
        }

        public bool Sweden
        {
            get => Nations.HasFlag(Nations.Sweden);
            set
            {
                if (Sweden == value) return;
                Nations ^= Nations.Sweden;
                OnPropertyChanged(nameof(Sweden));
            }
        }

        public bool Poland
        {
            get => Nations.HasFlag(Nations.Poland);
            set
            {
                if (Poland == value) return;
                Nations ^= Nations.Poland;
                OnPropertyChanged(nameof(Poland));
            }
        }

        public bool Italy
        {
            get => Nations.HasFlag(Nations.Italy);
            set
            {
                if (Italy == value) return;
                Nations ^= Nations.Italy;
                OnPropertyChanged(nameof(Italy));
            }
        }
        #endregion Nations

        #region VehicleTypes
        public bool Heavy
        {
            get => VehicleTypes.HasFlag(VehicleTypes.Heavy);
            set
            {
                if (Heavy == value) return;
                VehicleTypes ^= VehicleTypes.Heavy;
                OnPropertyChanged(nameof(Heavy));
            }
        }

        public bool Medium
        {
            get => VehicleTypes.HasFlag(VehicleTypes.Medium);
            set
            {
                if (Medium == value) return;
                VehicleTypes ^= VehicleTypes.Medium;
                OnPropertyChanged(nameof(Medium));
            }
        }

        public bool Light
        {
            get => VehicleTypes.HasFlag(VehicleTypes.Light);
            set
            {
                if (Light == value) return;
                VehicleTypes ^= VehicleTypes.Light;
                OnPropertyChanged(nameof(Light));
            }
        }

        public bool TD
        {
            get => VehicleTypes.HasFlag(VehicleTypes.TD);
            set
            {
                if (TD == value) return;
                VehicleTypes ^= VehicleTypes.TD;
                OnPropertyChanged(nameof(TD));
            }
        }
        public bool SPG
        {
            get => VehicleTypes.HasFlag(VehicleTypes.SPG);
            set
            {
                if (SPG == value) return;
                VehicleTypes ^= VehicleTypes.SPG;
                OnPropertyChanged(nameof(SPG));
            }
        }
        #endregion

        #region Marks of Excellence
        public bool MoE0
        {
            get => MarksOfExcellence.HasFlag(MarksOfExcellence.Zero);
            set
            {
                if (MoE0 == value) return;
                MarksOfExcellence ^= MarksOfExcellence.Zero;
                OnPropertyChanged(nameof(MoE0));
            }
        }

        public bool MoE1
        {
            get => MarksOfExcellence.HasFlag(MarksOfExcellence.One);
            set
            {
                if (MoE1 == value) return;
                MarksOfExcellence ^= MarksOfExcellence.One;
                OnPropertyChanged(nameof(MoE1));
            }
        }

        public bool MoE2
        {
            get => MarksOfExcellence.HasFlag(MarksOfExcellence.Two);
            set
            {
                if (MoE2 == value) return;
                MarksOfExcellence ^= MarksOfExcellence.Two;
                OnPropertyChanged(nameof(MoE2));
            }
        }

        public bool MoE3
        {
            get => MarksOfExcellence.HasFlag(MarksOfExcellence.Three);
            set
            {
                if (MoE3 == value) return;
                MarksOfExcellence ^= MarksOfExcellence.Three;
                OnPropertyChanged(nameof(MoE3));
            }
        }
        #endregion Marks

        public bool NonPremium
        {
            get => Premiums.HasFlag(Premiums.NonPremium);
            set
            {
                if (NonPremium == value) return;
                Premiums ^= Premiums.NonPremium;
                OnPropertyChanged(nameof(NonPremium));
            }
        }

        public bool Premium
        {
            get => Premiums.HasFlag(Premiums.Premium);
            set
            {
                if (Premium == value) return;
                Premiums ^= Premiums.Premium;
                OnPropertyChanged(nameof(Premium));
            }
        }

        #region Commands
        private RelayCommand _SetTiersChecked;
        public RelayCommand SetTiersChecked
        {
            get
            {
                if (_SetTiersChecked == null)
                    _SetTiersChecked = new RelayCommand(o => Tiers = Tiers.All, o => Tiers != Tiers.All);
                return _SetTiersChecked;
            }
        }

        private RelayCommand _SetTiersUnchecked;
        public RelayCommand SetTiersUnchecked
        {
            get
            {
                if (_SetTiersUnchecked == null)
                    _SetTiersUnchecked = new RelayCommand(o => Tiers = Tiers.None, o => Tiers != Tiers.None);
                return _SetTiersUnchecked;
            }
        }

        private RelayCommand _SetNationsChecked;
        public RelayCommand SetNationsChecked
        {
            get
            {
                if (_SetNationsChecked == null)
                    _SetNationsChecked = new RelayCommand(o => Nations = Nations.All, o => Nations != Nations.All);
                return _SetNationsChecked;
            }
        }

        private RelayCommand _SetNationsUnchecked;
        public RelayCommand SetNationsUnchecked
        {
            get
            {
                if (_SetNationsUnchecked == null)
                    _SetNationsUnchecked = new RelayCommand(o => Nations = Nations.None, o => Nations != Nations.None);
                return _SetNationsUnchecked;
            }
        }

        private RelayCommand _SetVehicleTypesChecked;
        public RelayCommand SetVehicleTypesChecked
        {
            get
            {
                if (_SetVehicleTypesChecked == null)
                    _SetVehicleTypesChecked = new RelayCommand(o => VehicleTypes = VehicleTypes.All, o => VehicleTypes != VehicleTypes.All);
                return _SetVehicleTypesChecked;
            }
        }

        private RelayCommand _SetVehicleTypesUnchecked;
        public RelayCommand SetVehicleTypesUnchecked
        {
            get
            {
                if (_SetVehicleTypesUnchecked == null)
                    _SetVehicleTypesUnchecked = new RelayCommand(o => VehicleTypes = VehicleTypes.None, o => VehicleTypes != VehicleTypes.None);
                return _SetVehicleTypesUnchecked;
            }
        }

        private RelayCommand _SetMoEChecked;
        public RelayCommand SetMoEChecked
        {
            get
            {
                if (_SetMoEChecked == null)
                    _SetMoEChecked = new RelayCommand(o => MarksOfExcellence = MarksOfExcellence.All, o => MarksOfExcellence != MarksOfExcellence.All);
                return _SetMoEChecked;
            }
        }

        private RelayCommand _SetMoEUnchecked;
        public RelayCommand SetMoEUnchecked
        {
            get
            {
                if (_SetMoEUnchecked == null)
                    _SetMoEUnchecked = new RelayCommand(o => MarksOfExcellence = MarksOfExcellence.None, o => MarksOfExcellence != MarksOfExcellence.None);
                return _SetMoEUnchecked;
            }
        }

        private RelayCommand _ClearText;
        public RelayCommand ClearText
        {
            get
            {
                if (_ClearText == null)
                    _ClearText = new RelayCommand(o => Text = String.Empty, o => !String.IsNullOrEmpty(Text));
                return _ClearText;
            }
        }
        #endregion Commands

        public string Text
        {
            get => TankFilter.Text;
            set
            {
                if (TankFilter.Text == value) return;
                TankFilter.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private Nations Nations
        {
            get => TankFilter.Nations;
            set
            {
                if (TankFilter.Nations == value) return;
                TankFilter.Nations = value;
                OnPropertyChanged(String.Empty);
            }
        }

        private Tiers Tiers
        {
            get => TankFilter.Tiers;
            set
            {
                if (TankFilter.Tiers == value) return;
                TankFilter.Tiers = value;
                //String.Empty is interpreted by WPF as a change in all properties
                OnPropertyChanged(String.Empty);
            }
        }

        private MarksOfExcellence MarksOfExcellence
        {
            get => TankFilter.MarksOfExcellence;
            set
            {
                if (TankFilter.MarksOfExcellence == value) return;
                TankFilter.MarksOfExcellence = value;
                //String.Empty is interpreted by WPF as a change in all properties
                OnPropertyChanged(String.Empty);
            }
        }

        private Premiums Premiums
        {
            get => TankFilter.Premiums;
            set
            {
                if (TankFilter.Premiums == value) return;
                TankFilter.Premiums = value;
                OnPropertyChanged(String.Empty);
            }
        }

        private VehicleTypes VehicleTypes
        {
            get => TankFilter.VehicleTypes;
            set
            {
                if (TankFilter.VehicleTypes == value) return;
                TankFilter.VehicleTypes = value;
                OnPropertyChanged(String.Empty);
            }
        }

        public TankFilter TankFilter { get; }

        public TankFilterViewModel()
        {
            TankFilter = new TankFilter();
            PropertyChanged += NotifyTankFilterChange;
        }

        private void NotifyTankFilterChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if the changed property is not TankFilter, then indicate a TankFilter change
            //this is working, because every other property in this ViewModel directly comes from TankFilter
            //and therefore changing anything also affects the TankFilter
            if (e.PropertyName != nameof(TankFilter))
                OnPropertyChanged(nameof(TankFilter));
        }
    }
}
