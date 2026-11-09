using LoginBCrypt.Datos;
using Microsoft.Data.Sqlite;

namespace LoginBCrypt.Servicios
{
    /// <summary>Fila que se muestra en la tabla de usuarios (sin contraseña ni hash).</summary>
    public class UsuarioFila
    {
        public long ID { get; set; }
        public string Nombre { get; set; } = "";
        public string Usuario { get; set; } = "";
        public string FechaCreacion { get; set; } = "";
    }

    /// <summary>
    /// Lógica de usuarios: usuario de prueba, verificación de contraseña con BCrypt
    /// y registro de los intentos de inicio de sesión.
    /// </summary>
    public class UsuarioService
    {
        private const string FormatoFecha = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Crea el usuario de prueba solo si la tabla Usuarios está vacía.
        /// </summary>
        public void CrearUsuarioDePrueba()
        {
            using var conexion = new SqliteConnection(ConexionBD.CadenaConexion);
            conexion.Open();

            using var contar = new SqliteCommand("SELECT COUNT(*) FROM Usuarios;", conexion);
            long cantidad = (long)contar.ExecuteScalar()!;

            if (cantidad > 0)
                return;

            // BCrypt genera un hash con "sal" aleatoria; la contraseña original NO se guarda.
            string hash = BCrypt.Net.BCrypt.HashPassword("Victor123*");

            const string sql = @"
                INSERT INTO Usuarios (Nombre, Usuario, PasswordHash, FechaCreacion)
                VALUES (@nombre, @usuario, @hash, @fecha);";

            using var insertar = new SqliteCommand(sql, conexion);
            insertar.Parameters.AddWithValue("@nombre", "Victor Montes");
            insertar.Parameters.AddWithValue("@usuario", "victor");
            insertar.Parameters.AddWithValue("@hash", hash);
            insertar.Parameters.AddWithValue("@fecha", DateTime.Now.ToString(FormatoFecha));
            insertar.ExecuteNonQuery();
        }

        /// <summary>
        /// Valida las credenciales. Devuelve el nombre del usuario si son correctas,
        /// o null si no lo son. Siempre registra el intento (Exitoso o Fallido).
        /// </summary>
        public string? IniciarSesion(string usuario, string password)
        {
            using var conexion = new SqliteConnection(ConexionBD.CadenaConexion);
            conexion.Open();

            string? nombre = null;

            // Consulta parametrizada: evita inyección SQL
            using (var buscar = new SqliteCommand(
                "SELECT Nombre, PasswordHash FROM Usuarios WHERE Usuario = @usuario;", conexion))
            {
                buscar.Parameters.AddWithValue("@usuario", usuario);

                using var lector = buscar.ExecuteReader();
                if (lector.Read())
                {
                    string hashGuardado = lector.GetString(1);

                    // Verify compara la contraseña escrita contra el hash guardado
                    if (BCrypt.Net.BCrypt.Verify(password, hashGuardado))
                        nombre = lector.GetString(0);
                }
            }

            RegistrarIntento(conexion, usuario, nombre != null ? "Exitoso" : "Fallido");
            return nombre;
        }

        /// <summary>
        /// Crea un usuario nuevo guardando solo el hash BCrypt de la contraseña.
        /// Devuelve false si el nombre de usuario ya existe.
        /// </summary>
        public bool CrearUsuario(string nombre, string usuario, string password)
        {
            using var conexion = new SqliteConnection(ConexionBD.CadenaConexion);
            conexion.Open();

            // Evita duplicados (además, la columna Usuario es UNIQUE en la tabla)
            using (var existe = new SqliteCommand(
                "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @usuario;", conexion))
            {
                existe.Parameters.AddWithValue("@usuario", usuario);
                if ((long)existe.ExecuteScalar()! > 0)
                    return false;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            const string sql = @"
                INSERT INTO Usuarios (Nombre, Usuario, PasswordHash, FechaCreacion)
                VALUES (@nombre, @usuario, @hash, @fecha);";

            using var insertar = new SqliteCommand(sql, conexion);
            insertar.Parameters.AddWithValue("@nombre", nombre);
            insertar.Parameters.AddWithValue("@usuario", usuario);
            insertar.Parameters.AddWithValue("@hash", hash);
            insertar.Parameters.AddWithValue("@fecha", DateTime.Now.ToString(FormatoFecha));
            insertar.ExecuteNonQuery();
            return true;
        }

        /// <summary>
        /// Devuelve los usuarios para la tabla. No incluye PasswordHash.
        /// </summary>
        public List<UsuarioFila> ObtenerUsuarios()
        {
            var lista = new List<UsuarioFila>();

            using var conexion = new SqliteConnection(ConexionBD.CadenaConexion);
            conexion.Open();

            using var comando = new SqliteCommand(
                "SELECT Id, Nombre, Usuario, FechaCreacion FROM Usuarios ORDER BY Id;", conexion);
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                lista.Add(new UsuarioFila
                {
                    ID = lector.GetInt64(0),
                    Nombre = lector.GetString(1),
                    Usuario = lector.GetString(2),
                    FechaCreacion = lector.GetString(3)
                });
            }

            return lista;
        }

        /// <summary>
        /// Guarda un intento de inicio de sesión en la tabla IntentosLogin.
        /// </summary>
        private static void RegistrarIntento(SqliteConnection conexion, string usuario, string resultado)
        {
            const string sql = @"
                INSERT INTO IntentosLogin (Usuario, Fecha, Resultado)
                VALUES (@usuario, @fecha, @resultado);";

            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@usuario", usuario);
            comando.Parameters.AddWithValue("@fecha", DateTime.Now.ToString(FormatoFecha));
            comando.Parameters.AddWithValue("@resultado", resultado);
            comando.ExecuteNonQuery();
        }
    }
}
