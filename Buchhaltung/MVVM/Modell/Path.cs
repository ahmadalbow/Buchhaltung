using Buchhaltung.Core;
using Buchhaltung.MVVM.ViewModell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buchhaltung.MVVM.Modell
{
    class Path : ObservableObject
    {
        public  string path { get; set; }
		private bool _isBehSt;

		public bool isBehSt
        {
			get { return _isBehSt; }
			set { _isBehSt = value;
				OnPropertyChanged();
			}
		}

		private int number;
		public RelayCommand RemovePathCommand { get; set; }

        public int Number
		{
			get { return MainViewModell.paths.IndexOf(this) +1; }
			set 
			{ 
				number = value;
				OnPropertyChanged();
			}
		}

		
        public string pathName
        {
			get { return path.Split("\\")[path.Split("\\").Length-1]; }
			
		}
		public string Website { get; set; }
        public Path()
        {
			RemovePathCommand = new RelayCommand(o =>
			{
				MainViewModell.paths.Remove(this);
				foreach (var path in MainViewModell.paths)
				{
					path.Number = MainViewModell.paths.IndexOf(path);
				}
            });
        }

    }
}
