using RegistroEstudiantes.Datos;
using RegistroEstudiantes.Modelos;

namespace RegistroEstudiantes.Formularios;

public partial class FormPrincipal : Form
{
    /// <summary>
    /// Id del estudiante que se esta editando. Cero significa registro nuevo.
    /// </summary>
    private int _idEnEdicion;

    /// <summary>
    /// Evita que al recargar la grilla se dispare la carga del formulario.
    /// </summary>
    private bool _cargandoGrilla;

    public FormPrincipal()
    {
        InitializeComponent();
    }

    private void FormPrincipal_Load(object? sender, EventArgs e)
    {
        ConfigurarColumnas();
        RepositorioMemoria.CargarEjemplos();
        RefrescarGrilla();
        LimpiarFormulario();
    }

    private void ConfigurarColumnas()
    {
        grid.Columns.Clear();

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colDocumento",
            HeaderText = "Documento",
            DataPropertyName = nameof(Estudiante.Documento),
            FillWeight = 13
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colNombres",
            HeaderText = "Nombres",
            DataPropertyName = nameof(Estudiante.Nombres),
            FillWeight = 15
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colApellidos",
            HeaderText = "Apellidos",
            DataPropertyName = nameof(Estudiante.Apellidos),
            FillWeight = 16
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colFechaNacimiento",
            HeaderText = "F. nacimiento",
            DataPropertyName = nameof(Estudiante.FechaNacimiento),
            FillWeight = 11,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colEdad",
            HeaderText = "Edad",
            DataPropertyName = nameof(Estudiante.Edad),
            FillWeight = 5,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colGrado",
            HeaderText = "Grado",
            DataPropertyName = nameof(Estudiante.Grado),
            FillWeight = 7
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTelefono",
            HeaderText = "Telefono",
            DataPropertyName = nameof(Estudiante.Telefono),
            FillWeight = 12
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colEmail",
            HeaderText = "Email",
            DataPropertyName = nameof(Estudiante.Email),
            FillWeight = 21
        });
    }

    private void RefrescarGrilla()
    {
        _cargandoGrilla = true;

        var lista = RepositorioMemoria.Listar(txtBuscar.Text);
        grid.DataSource = lista;
        grid.ClearSelection();

        _cargandoGrilla = false;

        lblEstado.Text = lista.Count == 1
            ? "1 estudiante"
            : $"{lista.Count} estudiantes";
    }

    private void Grid_SelectionChanged(object? sender, EventArgs e)
    {
        if (_cargandoGrilla || grid.CurrentRow?.DataBoundItem is not Estudiante seleccionado)
        {
            return;
        }

        _idEnEdicion = seleccionado.Id;

        txtDocumento.Text = seleccionado.Documento;
        txtNombres.Text = seleccionado.Nombres;
        txtApellidos.Text = seleccionado.Apellidos;
        dtpFechaNacimiento.Value = seleccionado.FechaNacimiento;
        txtTelefono.Text = seleccionado.Telefono;
        txtEmail.Text = seleccionado.Email;
        txtDireccion.Text = seleccionado.Direccion;
        txtGrado.Text = seleccionado.Grado;

        btnGuardar.Text = "Actualizar";
        lblEstado.Text = $"Editando: {seleccionado.NombreCompleto}";
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (!Validar())
        {
            return;
        }

        var estudiante = new Estudiante
        {
            Id = _idEnEdicion,
            Documento = txtDocumento.Text,
            Nombres = txtNombres.Text,
            Apellidos = txtApellidos.Text,
            FechaNacimiento = dtpFechaNacimiento.Value.Date,
            Telefono = txtTelefono.Text,
            Email = txtEmail.Text,
            Direccion = txtDireccion.Text,
            Grado = txtGrado.Text
        };

        if (_idEnEdicion == 0)
        {
            RepositorioMemoria.Insertar(estudiante);
            lblEstado.Text = "Estudiante guardado";
        }
        else
        {
            RepositorioMemoria.Actualizar(estudiante);
            lblEstado.Text = "Estudiante actualizado";
        }

        RefrescarGrilla();
        LimpiarFormulario();
    }

    private void BtnNuevo_Click(object? sender, EventArgs e)
    {
        LimpiarFormulario();
        lblEstado.Text = "Nuevo registro";
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idEnEdicion == 0)
        {
            MessageBox.Show(
                "Selecciona un estudiante de la lista para eliminarlo.",
                "Eliminar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var nombre = $"{txtNombres.Text} {txtApellidos.Text}".Trim();
        var respuesta = MessageBox.Show(
            $"Eliminar a {nombre}?",
            "Confirmar",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (respuesta != DialogResult.Yes)
        {
            return;
        }

        RepositorioMemoria.Eliminar(_idEnEdicion);
        RefrescarGrilla();
        LimpiarFormulario();
        lblEstado.Text = "Estudiante eliminado";
    }

    private void TxtBuscar_TextChanged(object? sender, EventArgs e)
    {
        RefrescarGrilla();
    }

    private bool Validar()
    {
        if (string.IsNullOrWhiteSpace(txtDocumento.Text))
        {
            Advertir("El documento es obligatorio.", txtDocumento);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtNombres.Text))
        {
            Advertir("Los nombres son obligatorios.", txtNombres);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtApellidos.Text))
        {
            Advertir("Los apellidos son obligatorios.", txtApellidos);
            return false;
        }

        if (dtpFechaNacimiento.Value.Date > DateTime.Today)
        {
            Advertir("La fecha de nacimiento no puede ser futura.", dtpFechaNacimiento);
            return false;
        }

        var email = txtEmail.Text.Trim();
        if (email.Length > 0 && (!email.Contains('@') || !email.Contains('.')))
        {
            Advertir("El email no tiene un formato valido.", txtEmail);
            return false;
        }

        if (RepositorioMemoria.ExisteDocumento(txtDocumento.Text, _idEnEdicion))
        {
            Advertir("Ya existe un estudiante con ese documento.", txtDocumento);
            return false;
        }

        return true;
    }

    private static void Advertir(string mensaje, Control foco)
    {
        MessageBox.Show(mensaje, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        foco.Focus();
    }

    private void LimpiarFormulario()
    {
        _idEnEdicion = 0;

        txtDocumento.Clear();
        txtNombres.Clear();
        txtApellidos.Clear();
        dtpFechaNacimiento.Value = new DateTime(2010, 1, 1);
        txtTelefono.Clear();
        txtEmail.Clear();
        txtDireccion.Clear();
        txtGrado.Clear();

        btnGuardar.Text = "Guardar";
        grid.ClearSelection();
        txtDocumento.Focus();
    }
}
