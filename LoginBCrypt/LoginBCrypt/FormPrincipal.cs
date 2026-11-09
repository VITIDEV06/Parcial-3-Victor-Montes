using LoginBCrypt.Servicios;

namespace LoginBCrypt
{
    /// <summary>
    /// Pantalla principal (después del login): crear usuarios y ver la lista de usuarios.
    /// </summary>
    public class FormPrincipal : Form
    {
        private readonly UsuarioService _servicio;
        private readonly TextBox txtNombre = new();
        private readonly TextBox txtUsuario = new();
        private readonly TextBox txtPassword = new();
        private readonly Label lblMensaje = new();
        private readonly DataGridView tabla = new();

        public FormPrincipal(UsuarioService servicio, string nombreSesion)
        {
            _servicio = servicio;

            Text = "LoginBCrypt";
            ClientSize = new Size(640, 560);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            var lblBienvenida = new Label
            {
                Text = $"¡Bienvenido, {nombreSesion}!",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                AutoSize = true,
                Location = new Point(20, 18)
            };

            var btnCerrar = new Button
            {
                Text = "Cerrar sesión",
                Location = new Point(500, 14),
                Size = new Size(120, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(235, 235, 235)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => Close(); // vuelve al login

            // --- Sección: crear usuario ---
            var lblCrear = new Label
            {
                Text = "Crear usuario",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 70)
            };

            var lblNombre = new Label { Text = "Nombre", AutoSize = true, Location = new Point(20, 105) };
            txtNombre.SetBounds(20, 128, 190, 28);

            var lblUsuario = new Label { Text = "Usuario", AutoSize = true, Location = new Point(225, 105) };
            txtUsuario.SetBounds(225, 128, 190, 28);

            var lblPassword = new Label { Text = "Contraseña", AutoSize = true, Location = new Point(430, 105) };
            txtPassword.SetBounds(430, 128, 190, 28);
            txtPassword.UseSystemPasswordChar = true;

            var btnCrear = new Button
            {
                Text = "Crear usuario",
                Location = new Point(20, 172),
                Size = new Size(160, 36),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCrear.FlatAppearance.BorderSize = 0;
            btnCrear.Click += BtnCrear_Click;

            lblMensaje.SetBounds(195, 172, 425, 36);
            lblMensaje.TextAlign = ContentAlignment.MiddleLeft;

            // --- Sección: tabla de usuarios ---
            var lblLista = new Label
            {
                Text = "Usuarios registrados",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 230)
            };

            tabla.SetBounds(20, 260, 600, 280);
            tabla.ReadOnly = true;
            tabla.AllowUserToAddRows = false;
            tabla.AllowUserToDeleteRows = false;
            tabla.RowHeadersVisible = false;
            tabla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            tabla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabla.BackgroundColor = Color.White;
            tabla.BorderStyle = BorderStyle.FixedSingle;

            Controls.AddRange(new Control[]
            {
                lblBienvenida, btnCerrar, lblCrear,
                lblNombre, txtNombre, lblUsuario, txtUsuario, lblPassword, txtPassword,
                btnCrear, lblMensaje, lblLista, tabla
            });

            CargarUsuarios();
        }

        private void BtnCrear_Click(object? sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (nombre.Length == 0 || usuario.Length == 0 || password.Length == 0)
            {
                Mostrar("Completa nombre, usuario y contraseña.", Color.DarkOrange);
                return;
            }

            try
            {
                if (!_servicio.CrearUsuario(nombre, usuario, password))
                {
                    Mostrar("Ese usuario ya existe.", Color.Firebrick);
                    return;
                }

                Mostrar("Usuario creado correctamente.", Color.SeaGreen);
                txtNombre.Clear();
                txtUsuario.Clear();
                txtPassword.Clear();
                CargarUsuarios(); // actualiza la tabla
            }
            catch (Exception ex)
            {
                Mostrar("Error al crear el usuario: " + ex.Message, Color.Firebrick);
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                tabla.DataSource = _servicio.ObtenerUsuarios();
                tabla.Columns["FechaCreacion"]!.HeaderText = "Fecha de creación";
                tabla.Columns["ID"]!.FillWeight = 30;
            }
            catch (Exception ex)
            {
                Mostrar("Error al cargar usuarios: " + ex.Message, Color.Firebrick);
            }
        }

        private void Mostrar(string texto, Color color)
        {
            lblMensaje.ForeColor = color;
            lblMensaje.Text = texto;
        }
    }
}
