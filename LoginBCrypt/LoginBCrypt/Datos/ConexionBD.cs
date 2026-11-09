using Microsoft.Data.Sqlite;

namespace LoginBCrypt.Datos
{
    /// <summary>
    /// Se encarga de la conexión con SQLite y de crear la base de datos y sus tablas.
    /// </summary>
    public static class ConexionBD
    {
        // El archivo .db se guarda junto al ejecutable (carpeta bin\...)
        private static readonly string RutaBD =
            Path.Combine(AppContext.BaseDirectory, "LoginBCrypt.db");

        public static string CadenaConexion => $"Data Source={RutaBD}";

        /// <summary>
        /// Crea el archivo LoginBCrypt.db y las tablas si todavía no existen.
        /// </summary>
        public static void Inicializar()
        {
            const string sql = @"
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Usuario TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FechaCreacion TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS IntentosLogin (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Usuario TEXT NOT NULL,
                    Fecha TEXT NOT NULL,
                    Resultado TEXT NOT NULL
                );";

            using var conexion = new SqliteConnection(CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(sql, conexion);
            comando.ExecuteNonQuery();
        }
    }
}
