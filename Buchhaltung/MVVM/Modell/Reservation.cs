using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Buchhaltung.MVVM.Modell
{
    class Reservation
    {
        public string Website { get; set; }
        public string Type { get; set; }
        public string ID { get; set; }
        public string City { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Listing { get; set; }
        public string Guest { get; set; }
        public double TotalAmount { get; set; }
        public double USt
        {
            get
            {
                if (HostFee == 0) return 0;
                return TotalAmount * 7 / 107;
            }
        }
        public double VorSteuer
        {
            get
            {
                return HostFee * 19 / 119;
            }
        }
        public double Zahllast
        {
            get
            {
                return USt - VorSteuer;
            }
        }
        public double Payout
        {
            get
            {

                return TotalAmount - HostFee;
            }
        }
        public double HostFee { get; set; }
        public string Status { get; set; }

        public double BeherbungsSteuer
        {
            get
            {
                if (Website == "Booking" && City == "Dresden") return TotalAmount * 6 / 100;
                return 0; ;
            }

        }
        public double PaymentFee { get; set; }


        public static List<Reservation> CreateBookingReservation(string path)
        {
            var reservations = new List<Reservation>();
            using (var streamReader = new StreamReader(path))
            {
                List<string> lines = new List<string>();
                string ln;
                int count = 0;
                while ((ln = streamReader.ReadLine()) != null)
                {
                    try
                    {
                        if (ln[0] == '\"')
                        {
                            ln = ln.Replace("\"", "");
                        }
                        float.Parse(ln[0].ToString());
                        lines.Add(ln);
                        count++;
                    }
                    catch
                    {
                        if (count == 0) continue;
                        lines[count - 1] = lines[count - 1].Replace("\n", "") + ln;
                    }

                }
                for (int n = 0; n < lines.Count; n++)
                {
                    string curLine = lines[n];
                   
                    string[] linecontent = curLine.Replace("\"", "").Split(',');
                    Reservation reservation = new Reservation()
                    {

                        Website = "Booking",
                        ID = linecontent[0],
                        StartDate = DateTime.Parse(linecontent[3]),
                        EndDate = DateTime.Parse(linecontent[4]),
                        Guest = linecontent[6],
                        TotalAmount = Double.Parse(linecontent[12]),
                        HostFee = Double.Parse(linecontent[13]),
                        PaymentFee = Double.Parse(linecontent[14]),
                    };
                    if (linecontent[15] == "Stayed") reservation.Status = "confirmed";
                    if (linecontent[15] == "Cancelled") reservation.Status = "cancelled";
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }
        public static List<Reservation> CreateAirbnbReservation(string path)
        {
            var reservations = new List<Reservation>();
            using (var streamReader = new StreamReader(path))
            {
                List<string> lines = new List<string>();
                string ln;
                while ((ln = streamReader.ReadLine()) != null)
                {
                    lines.Add(ln);
                }
                for (int n = 1; n < lines.Count; n++)
                {
                    string curLine = lines[n];
                    string[] linecontent = curLine.Split(',');

                    if (linecontent[2] == "") continue;
                    Reservation reservation = new Reservation()
                    {
                        Website = "Airbnb",
                        Type = linecontent[1],
                        ID = linecontent[2],
                        StartDate = DateTime.Parse(linecontent[3]),
                        Guest = linecontent[5],
                        PaymentFee = 0,
                    };
                    reservation.Status = "confirmed";
                    if (linecontent[12] == "")
                    {
                        reservation.HostFee = 0;
                        reservation.TotalAmount = Double.Parse(linecontent[10]);
                    }
                    else
                    {
                        reservation.HostFee = Double.Parse(linecontent[12]);
                        reservation.TotalAmount = Double.Parse(linecontent[10]) + Double.Parse(linecontent[12]);
                    }
                    reservations.Add(reservation);
                }
            }
            return reservations;
        }

    }
}
