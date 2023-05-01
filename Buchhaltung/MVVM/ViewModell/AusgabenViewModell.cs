using Buchhaltung.Core;
using Buchhaltung.MVVM.Modell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Buchhaltung.MVVM.ViewModell
{
    class AusgabenViewModell : ObservableObject
    {
        private string _selectedItem;

        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }
        private string _date;

        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }
        private string _betrag;

        public string Betrag
        {
            get { return _betrag; }
            set
            {
                _betrag = value;
                OnPropertyChanged();
            }
        }
        private string _details;

        public string Details
        {
            get { return _details; }
            set
            {
                _details = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddAusgabe { get; set; }
        public AusgabenViewModell()
        {
            AddAusgabe = new RelayCommand(o =>
            {
                try
                {
                    Ausgabe ausgabe = new Ausgabe()
                    {
                        art = SelectedItem.Split(": ")[1],
                        Betrag = double.Parse(Betrag),
                        Date = DateTime.ParseExact(Date, "dd.MM.yyyy", null),
                        details = Details
                    };
                    MainViewModell.Ausgaben.Add(ausgabe);
                    Betrag = "";
                }
                catch (Exception)
                {

                    MessageBox.Show("Wrong Date Syntax");
                }



            });
        }
    }
}
