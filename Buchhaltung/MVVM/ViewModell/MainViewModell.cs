using Buchhaltung.Core; using Buchhaltung.MVVM.Modell; using System; using System.Collections.Generic; using System.Collections.ObjectModel; using System.Linq;
using System.IO;
using System.Text.Json;
using Path = Buchhaltung.MVVM.Modell.Path;
using System.Runtime.InteropServices;
using Ookii.Dialogs.Wpf;
using System.Windows.Shapes;

namespace Buchhaltung.MVVM.ViewModell {     class MainViewModell     {         public RelayCommand addFromAirbnbCommand { get; set; }         public RelayCommand addFromBookingCommand { get; set; }         public RelayCommand addAusgaben { get; set; }
        public RelayCommand ImportFileCommand { get; set; }
        public RelayCommand ExportFileCommand { get; set; }
        public RelayCommand CreateBuchHaltungCommand { get; set; }         public static ObservableCollection<Path> paths { get; set; }         public static ObservableCollection<Ausgabe> Ausgaben { get; set; }         public List<Reservation> reservations { get; set; }         public double TotalUStFromResertvertions { get; set; }         public double TotalVorSteuerFromResertvertions { get; set; }
        public double TotalZahlastFromResertvertions { get; set; }         public double TotalNettoEinahmeFromResertvertions { get; set; }         public double TotalBruttoinahmeFromResertvertions { get; set; }         public double TotalBehSt { get; set; }         public double TotalVorSteuerFromAusgaben { get; set; }         public double TotalBetragFromAusgaben { get; set; }         public double TotalVorsteuer { get; set; }         public double Gewinn { get; set; }         public double TotalZahllast { get; set; }          public List<string> pathsText { get; set; }         public MainViewModell()         {              TotalZahlastFromResertvertions = 0;             TotalVorSteuerFromResertvertions = 0;             TotalUStFromResertvertions = 0;             TotalNettoEinahmeFromResertvertions = 0;             TotalBruttoinahmeFromResertvertions = 0;             TotalVorSteuerFromAusgaben = 0;             TotalBetragFromAusgaben = 0;             TotalBehSt = 0;             pathsText = new List<string>();             reservations = new List<Reservation>();             paths = new ObservableCollection<Path>();             Ausgaben = new ObservableCollection<Ausgabe>();             addFromAirbnbCommand = new RelayCommand(o =>             {                                                    Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();                 Nullable<bool> result = openFileDlg.ShowDialog();                 if (result == true)                 {                     string text = "";                     using(var streamreader = new StreamReader(openFileDlg.FileName))
                    {
                        string ln;
                        while ((ln = streamreader.ReadLine()) != null)
                        {
                            text += ln;
                        }
                        foreach(string t in pathsText)
                        {
                            if (t ==text) { return; }
                        }
                        Path path = new Path()
                        {
                            path = openFileDlg.FileName,
                            Website = "Airbnb"
                        };
                        paths.Add(path);
                        pathsText.Add(text);
                        text = string.Empty;
                    }                                      }                              });             addFromBookingCommand = new RelayCommand(o =>             {                 Path path;                 Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();                 Nullable<bool> result = openFileDlg.ShowDialog();                 if (result == true)                 {                     path = new Path()                     {                         path = openFileDlg.FileName,                         Website = "Booking"                     };                     paths.Add(path);                 }              });             addAusgaben = new RelayCommand(o =>              {                  AusgabenWindow ausgabenWindow = new AusgabenWindow();                 ausgabenWindow.Top = GetMousePosition().Y-250;                 ausgabenWindow.Left = GetMousePosition().X - 400;                 ausgabenWindow.ShowDialog();             });             CreateBuchHaltungCommand = new RelayCommand(o =>             {                                 foreach(Path path in paths)                 {                     if(path.Website == "Airbnb")                     {                         reservations.AddRange(Reservation.CreateAirbnbReservation(path.path));                     }                     if (path.Website == "Booking")                     {
                        List<Reservation> _reservations = Reservation.CreateBookingReservation(path.path);                         if(path.isBehSt) foreach (Reservation reservation in _reservations) reservation.City = "Dresden";                         reservations.AddRange(_reservations);                     }                 }                 reservations= reservations.OrderBy(o => o.StartDate).ToList();                 Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();                 saveFileDlg.DefaultExt = "xlsx";                 saveFileDlg.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";                 Nullable<bool> result = saveFileDlg.ShowDialog();                                  string[] Sheets = new string[3] { "Buchungen", "Ausgaben", "Gewinn" };                 if (result == true)                 {                     string path = saveFileDlg.FileName;                     Excel excel = new Excel(path, new string[] { "Buchungen", "Ausgaben", "Gewinn" });                                       string[] header = new string[] { "Buchung ID" , "Date","Gesamtbetrag","Host Gebühren","Brutto Einahme","MwSt","Vorsteuer","Zahllast","BehrSt","Netto Einahme"};                                                         excel.writeArray(1, "A1:J1", header);                     excel.changeColumnWidth(1, "A1:J1", 16);                     excel.changeColumnWidth(1, "C1:J1", 13);                     for (int i = 0; i < reservations.Count; i++)                     {                         Reservation reservation = reservations[i];                                                 string[] Werte = new string[] {reservation.ID, reservation.StartDate.ToString("dd/MM/yyyy"), String.Format("{0:0.00}", reservation.TotalAmount) , String.Format("{0:0.00}", reservation.HostFee), String.Format("{0:0.00}", reservation.Payout), String.Format("{0:0.00}", reservation.USt), String.Format("{0:0.00}", reservation.VorSteuer), String.Format("{0:0.00}", reservation.Zahllast), String.Format("{0:0.00}", reservation.BeherbungsSteuer),String.Format("{0:0.00}", reservation.Payout - reservation.PaymentFee - reservation.Zahllast) };                                               excel.writeArray(1, $"A{i + 2}:J{i + 2}", Werte);                         TotalZahlastFromResertvertions = TotalZahlastFromResertvertions + reservation.Zahllast;                         TotalVorSteuerFromResertvertions = TotalVorSteuerFromResertvertions + reservation.VorSteuer;                         TotalUStFromResertvertions = TotalUStFromResertvertions + reservation.USt;                         TotalNettoEinahmeFromResertvertions = TotalNettoEinahmeFromResertvertions + reservation.Payout - reservation.PaymentFee - reservation.Zahllast ;                         TotalBruttoinahmeFromResertvertions = TotalBruttoinahmeFromResertvertions + reservation.Payout;                         TotalBehSt += reservation.BeherbungsSteuer;                         if (i ==reservations.Count - 1)                         {                                                         string[] totalWerte = new string[] {   "Total  " + String.Format("{0:0.00}", TotalBruttoinahmeFromResertvertions), "Total  " + String.Format("{0:0.00}", TotalUStFromResertvertions), "Total  " + String.Format("{0:0.00}", TotalVorSteuerFromResertvertions), "Total  " +  String.Format("{0:0.00}", TotalZahlastFromResertvertions), "Total  " + String.Format("{0:0.00}", TotalBehSt), "Total  " + String.Format("{0:0.00}", TotalNettoEinahmeFromResertvertions) };                             excel.writeArray(1, $"E{i + 3}:J{i + 3}", totalWerte);                         }                     }                                          header = new string[] { "Typ", "Date", "Betrag", "Vorsteuer","Details" };                     excel.writeArray(2, "A1:E1", header);                                        excel.changeColumnWidth(2, "A1:E1", 13);                     for(int i = 0; i < Ausgaben.Count; i++)
                    {
                        Ausgabe ausgabe = Ausgaben[i];                        
                        string[] werte = new string[] { ausgabe.art, ausgabe.Date.ToString("dd/MM/yyyy"), String.Format("{0:0.00}", ausgabe.Betrag), String.Format("{0:0.00}", ausgabe.VorSteuer), ausgabe.details };
                        excel.writeArray(2, $"A{i + 2}:E{i + 2}", werte);                      
                        TotalBetragFromAusgaben += ausgabe.Betrag;
                        TotalVorSteuerFromAusgaben += ausgabe.VorSteuer;
                        if(i == Ausgaben.Count - 1) 
                        {                                                     
                            string[] totalWerte = new string[] { "Total  " + String.Format("{0:0.00}", TotalBetragFromAusgaben), "Total  " + String.Format("{0:0.00}", TotalVorSteuerFromAusgaben),  };                           
                            excel.writeArray(2, $"C{i + 3}:D{i + 3}", totalWerte);
                        }
                    }                                          header = new string[] { "Total Brutto Einahmen", "Total Ausgaben" ,"Total USt", "Total Vorsteuer", "Total Zahllast" ,"BehSt", "Gewinn"};                     excel.changeColumnWidth(3, "A1:G1", 13);                   
                    excel.writeArray(3, "A1:G1", header);
                    TotalVorsteuer = TotalVorSteuerFromAusgaben + TotalVorSteuerFromResertvertions;
                    TotalZahllast = TotalUStFromResertvertions - TotalVorsteuer;
                    Gewinn = TotalBruttoinahmeFromResertvertions - TotalZahllast - TotalBetragFromAusgaben;
                    string[] TotalWerte = new string[] { String.Format("{0:0.00}", TotalBruttoinahmeFromResertvertions) ,String.Format("{0:0.00}", TotalBetragFromAusgaben), String.Format("{0:0.00}", TotalUStFromResertvertions), String.Format("{0:0.00}", TotalVorsteuer) , String.Format("{0:0.00}", TotalZahllast), String.Format("{0:0.00}", TotalBehSt), String.Format("{0:0.00}", Gewinn) };                                         excel.writeArray(3, "A2:G2", TotalWerte);                      TotalZahlastFromResertvertions = 0;
                    TotalVorSteuerFromResertvertions = 0;
                    TotalUStFromResertvertions = 0;
                    TotalNettoEinahmeFromResertvertions = 0;
                    TotalBruttoinahmeFromResertvertions = 0;
                    TotalVorSteuerFromAusgaben = 0;
                    TotalBetragFromAusgaben = 0;
                    TotalBehSt = 0;
                    pathsText.Clear();
                    reservations.Clear();                     excel.save();                                 }             });             ExportFileCommand = new RelayCommand(o =>
            {
                foreach (Path path in paths)                 {                     if (path.Website == "Airbnb")                     {                         reservations.AddRange(Reservation.CreateAirbnbReservation(path.path));                     }                     if (path.Website == "Booking")                     {
                        List<Reservation> _reservations = Reservation.CreateBookingReservation(path.path);                         if (path.isBehSt) foreach (Reservation reservation in _reservations) reservation.City = "Dresden";                         reservations.AddRange(_reservations);                     }                 }
                Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();                 saveFileDlg.DefaultExt = "bow";                 saveFileDlg.Filter = "bow files (*.bow)|*.bow|All files (*.*)|*.*";                 Nullable<bool> result = saveFileDlg.ShowDialog();
                if (result == true)
                {
                    string path = saveFileDlg.FileName;                    
                    using (StreamWriter outputFile = new StreamWriter(path))
                    {
                        outputFile.WriteLine("Reservations:");
                        foreach(var reservation in reservations) {
                            string strJson = JsonSerializer.Serialize<Reservation>(reservation);
                            outputFile.WriteLine(strJson);
                        }
                        outputFile.WriteLine("Ausgaben:");
                        foreach (var ausgabe in Ausgaben)
                        {
                            string strJson = JsonSerializer.Serialize<Ausgabe>(ausgabe);
                            outputFile.WriteLine(strJson);
                        }
                    }
                    reservations.Clear();
                }
            });             ImportFileCommand = new RelayCommand(o =>
            {
                Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();                 openFileDlg.Filter = "bow files (*.bow)|*.bow|All files (*.*)|*.*";                 Nullable<bool> result = openFileDlg.ShowDialog();                 if (result == true)                 {                     string text = "";                     using (var streamreader = new StreamReader(openFileDlg.FileName))
                    {
                        string ln;
                        bool inReservations = false;
                        bool inAusgaben = false;
                        while ((ln = streamreader.ReadLine()) != null)
                        {
                            if (ln == "Reservations:") { inReservations = true; inAusgaben = false; continue; }
                            if (ln == "Ausgaben:") { inReservations = false; inAusgaben = true; continue; }
                            if (inReservations)
                            {
                                var reservation = JsonSerializer.Deserialize<Reservation>(ln);
                               reservations.Add(reservation);
                            }
                            if(inAusgaben)
                            {
                                var ausgabe = JsonSerializer.Deserialize<Ausgabe>(ln);
                                Ausgaben.Add(ausgabe);  
                            }

                        }
                        
                    }

                }
            });


        }         [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static System.Drawing.Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new System.Drawing.Point(w32Mouse.X, w32Mouse.Y);
        }     } } 