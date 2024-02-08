namespace GoldOneAPI.Manager
{
    public class DBConn
    {
        private static string GetConnectionString()
        {
            // Return My.Settings.ConnString.ToString
            //  return "Data Source=192.168.0.84,36832;Initial Catalog=EQMS;User ID=randy;Password=otik"; //test
            //return "Data Source=192.168.0.222,36832;Initial Catalog=EQMS;User ID=randy;Password=otik"; //live
            // return "Data Source=LERJUN-PC;Initial Catalog=GoldOne;User ID=test;Password=1234"; //live
            return "Data Source=LAPTOP-AIQ21DVO\\ODECCISERVER;Initial Catalog=GoldOne;User ID=test;Password=1234"; //live
                                                                                                                   //  return "Data Source=EC2AMAZ-AN808JE\\MSSQLSERVER01;Initial Catalog=AOPCDB;User ID=test;Password=1234"; //server
        }

        public static string ConnectionString
        {
            get
            {
                return GetConnectionString();
            }
        }
        private static string GetPath()
        {
            return "C:\\Files\\";
            //return "C:\\inetpub\\AOPCAPP\\public\\assets\\img\\";
        }
        public static string Path
        {
            get
            {
                return GetPath();
            }
        }
    }
}
