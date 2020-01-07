using MySql.Data.MySqlClient;

namespace wsLapInformatica.Model
{
    public class ConectDB
    {
        MySqlConnection myConn;
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database {get; set;}

        public void Conectar()
        {
            string strConnection = $"server={Host}; User Id={User}; password={Password}; database={Database}";

            myConn = new MySqlConnection(strConnection);
            myConn.Open();
        }

        public void Fechar()
        {
            myConn.Close();
        }

        public MySqlConnection Conexao()
        {
            return myConn;
        }
    }
}