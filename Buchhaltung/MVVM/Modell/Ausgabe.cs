using Buchhaltung.Core;
using Buchhaltung.MVVM.ViewModell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Buchhaltung.MVVM.Modell
{
    public class Ausgabe: ObservableObject
    {

        private string _Number;

        public string Number
        {
            get
            {
                return (1 + MainViewModell.Ausgaben.IndexOf(this)) + "    " + art;
            }
            set
            {
                _Number = value;
                OnPropertyChanged();
            }
        }


        public double VorSteuer
        {
            get
            {
                if (art == "Rechnung") return Betrag * 19 / 119;
                return 0;
            }
        }
        public string art { get; set; }
       
        public DateTime Date { get; set; }
        public double Betrag { get; set; }
        [JsonIgnore]
        public RelayCommand RemoveAusgabe { get; set; }
        public Ausgabe()
        {
            RemoveAusgabe = new RelayCommand(o =>
            {
                MainViewModell.Ausgaben.Remove(this);
                foreach (var ausgabe in MainViewModell.Ausgaben)
                {
                    ausgabe.Number = (1 + MainViewModell.Ausgaben.IndexOf(ausgabe)) + "    " + art;
                }
            });
        }

    }
}
