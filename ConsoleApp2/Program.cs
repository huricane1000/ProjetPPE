using System;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.InteropServices;

namespace gsb_cloture
{
    class Program
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr h, int cmd);

        static void Main(string[] args)
        {
            IntPtr h = GetConsoleWindow();

            ShowWindow(h, 0);


            int secBeforeRefresh = 20;
            Connexion co = new Connexion();
            co.Connect();

            //  While esapce key is not input run the code.
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                System.Threading.Thread.Sleep(secBeforeRefresh * 1000);


                //  If we are between the 1st and the 10th execute this.
                if (Entre(1, 10))
                {
                    co.SendRequest("UPDATE fichefrais " +
                        "SET idetat=CL " +
                        "WHERE mois=" + GetMonthBefore());

                    SqlDataReader get = co.GetValueFromRequest("SELECT * " +
                        "FROM fichefrais " +
                        "WHERE mois=" + GetMonthBefore());

                }

                //  Else if we are after the 20th execute this.
                else if (Entre(20, 31))
                {
                    co.SendRequest("UPDATE fichefrais " +
                        "SET idetat=RB " +
                        "WHERE mois=" + GetMonthBefore());
                }
                Console.WriteLine(DateTime.Now);
            }

            co.Close();
        }
        public static string GetMonthBefore()
        {
            DateTime date = DateTime.Now.AddMonths(-1);
            return date.Year.ToString() + date.Month.ToString();
        }

        public static string GetMoisPrecedent()
        {
            DateTime date = DateTime.Now.AddMonths(-1);
            return date.Month.ToString();
        }
        public static string GetMoisPrecedent(DateTime date)
        {
            date = date.AddMonths(-1);
            return date.Month.ToString();
        }
        public static string GetMoisSuivant()
        {
            DateTime date = DateTime.Now.AddMonths(1);
            return date.Month.ToString();
        }
        public static string GetMoisSuivant(DateTime date)
        {
            date = date.AddMonths(1);
            return date.Month.ToString();
        }
        public static bool Entre(int debut, int fin)
        {
            DateTime date = DateTime.Now;

            if (date.Day >= debut && date.Day <= fin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool Entre(DateTime date, int debut, int fin)
        {
            if (date.Day >= debut && date.Day <= fin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// The main Connection Class.
    /// Permit the connection with an SQL base and send requests.
    /// </summary>
    public class Connexion
    {
        SqlConnection connection = null;

        private static readonly string serveur = "localhost";
        private static readonly string bdd = "gsb_frais";
        private static readonly string user = "root";
        private static readonly string pass = "";

        /// <summary>
        /// Instanciate the connection to database
        /// </summary>
        public void Connect()
        {
            try
            {
                string connectLine = "Data Source=" + serveur + ";Initial Catalog=" + bdd + ";User ID=" + user + ";Password=" + pass + ";Integrated Security=False";
                connection = new SqlConnection(connectLine);
                connection.Open();
                Console.Write(connection.State);
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
        /// <summary>
        /// Get a value from the request if you want to show values
        /// </summary>
        public SqlDataReader GetValueFromRequest(string request)
        {
            SqlCommand command = new SqlCommand(request, connection);
            Console.Write(connection.State);
            Console.Read();
            SqlDataReader toReturn = command.ExecuteReader();
            return toReturn;
        }

        /// <summary>
        /// Just send a request to the database.
        /// </summary>
        public void SendRequest(string request)
        {
            SqlCommand command = new SqlCommand(request, connection);
            command.ExecuteReader();
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        public void Close()
        {
            connection.Close();
        }


    }
}