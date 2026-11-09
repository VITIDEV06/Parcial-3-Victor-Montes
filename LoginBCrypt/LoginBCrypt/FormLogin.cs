using LoginBCrypt.Servicios;

namespace LoginBCrypt
{
    /// <summary>
    /// Pantalla 1: inicio de sesión. Los controles se crean por código para mantenerlo simple.
    /// </summary>
    public class FormLogin : Form
    {
        private readonly UsuarioService _servicio;
        private readonly TextBox txtUsuario = new();
        private readonly TextBox txtPassword = new();
        private readonly Label lblMensaje = new();

        public FormLogin(UsuarioService servicio)
        {
            _servicio = servicio;

            Text = "Iniciar sesión";
            ClientSize = new Size(360, 340);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            var titulo = new Label
            {
                Text = "Iniciar sesión",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(40, 25)
            };

            var lblUsuario = new Label { Text = "Usuario", AutoSize = true, Location = new Point(40, 90) };
            txtUsuario.SetBounds(40, 115, 280, 30);

            var lblPassword = new Label { Text = "Contraseña", AutoSize = true, Location = new Point(40, 160) };
            txtPassword.SetBounds(40, 185, 280, 30);
            txtPassword.UseSystemPasswordChar = true; // oculta los caracteres

            var btnIngresar = new Button
            {
                Text = "Iniciar sesión",
                Location = new Point(40, 235),
                Size = new Size(280, 40),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;

            lblMensaje.SetBounds(40, 290, 280, 30);
            lblMensaje.TextAlign = ContentAlignment.MiddleCenter;

            Controls.AddRange(new Control[]
            {
                titulo, lblUsuario, txtUsuario, lblPassword, txtPassword, btnIngresar, lblMensaje
            });

            AcceptButton = btnIngresar; // Enter = Iniciar sesión
        }

        private void BtnIngresar_Click(object? sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (usuario.Length == 0 || password.Length == 0)
            {
                Mostrar("Escribe tu usuario y contraseña.", Color.DarkOrange);
                return;
            }

            try
            {
                string? nombre = _servicio.IniciarSesion(usuario, password);

                if (nombre == null)
                {
                    Mostrar("Usuario o contraseña incorrectos.", Color.Firebrick);
                    txtPassword.Clear();
                    return;
                }

                Mostrar("Acceso exitoso.", Color.SeaGreen);

                // Abre la pantalla principal; al cerrar sesión se vuelve aquí
                Hide();
                using (var principal = new FormPrincipal(_servicio, nombre))
                {
                    principal.ShowDialog();
                }

                txtUsuario.Clear();
                txtPassword.Clear();
                lblMensaje.Text = "";
                Show();
                txtUsuario.Focus();
            }
            catch (Exception ex)
            {
                Mostrar("Error de base de datos: " + ex.Message, Color.Firebrick);
            }
        }

        private void Mostrar(string texto, Color color)
        {
            lblMensaje.ForeColor = color;
            lblMensaje.Text = texto;
        }
    }
}
