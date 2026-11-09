using LoginBCrypt.Datos;
using LoginBCrypt.Servicios;

namespace LoginBCrypt
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var servicio = new UsuarioService();

            try
            {
                // Crea la base de datos y el usuario de prueba si hace falta
                ConexionBD.Inicializar();
                servicio.CrearUsuarioDePrueba();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo preparar la base de datos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new FormLogin(servicio));
        }
    }
}
