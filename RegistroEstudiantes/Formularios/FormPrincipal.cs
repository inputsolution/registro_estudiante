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

    /// <summary>
    /// Pagina que se esta mostrando, empezando en 1.
    /// </summary>
    private int _paginaActual = 1;

    /// <summary>
    /// Cantidad de paginas del filtro vigente. Siempre al menos 1.
    /// </summary>
    private int _totalPaginas = 1;

    /// <summary>
    /// Registros por pagina. Coincide con el primer valor del desplegable.
    /// </summary>
    private int _filasPorPagina = 10;

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

    /// <summary>
    /// Al mover la ventana a un monitor con otro escalado, las fuentes cambian
    /// de tamaño y los anchos minimos deben recalcularse.
    /// </summary>
    protected override void OnDpiChanged(DpiChangedEventArgs e)
    {
        base.OnDpiChanged(e);
        AjustarAnchosMinimos();
    }

    /// <summary>
    /// Ajusta un ancho en pixeles al escalado de pantalla de Windows. Con el
    /// sistema al 125% o 150% las fuentes crecen, y un ancho fijo recortaria
    /// el titulo.
    /// </summary>
    private int Escalar(int pixeles) =>
        (int)Math.Ceiling(pixeles * (DeviceDpi / 96.0));

    /// <summary>
    /// Ancho que necesita un titulo de columna para verse completo, medido con
    /// la fuente real del encabezado mas el espacio interno y el margen que
    /// deja el propio DataGridView.
    /// </summary>
    private int AnchoDelTitulo(string titulo)
    {
        var fuente = grid.ColumnHeadersDefaultCellStyle.Font ?? Font;
        var medido = TextRenderer.MeasureText(titulo, fuente);
        var relleno = grid.ColumnHeadersDefaultCellStyle.Padding;

        // El margen extra cubre el borde de la celda y el glifo de ordenamiento.
        return medido.Width + relleno.Horizontal + Escalar(12);
    }

    private void ConfigurarColumnas()
    {
        grid.Columns.Clear();

        // MinimumWidth evita que el modo Fill recorte el encabezado cuando la
        // ventana es angosta: la columna deja de encogerse en ese ancho y, si
        // hace falta, aparece la barra de desplazamiento horizontal.
        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colDocumento",
            HeaderText = "Documento",
            DataPropertyName = nameof(Estudiante.Documento),
            FillWeight = 13,
            MinimumWidth = 105
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colNombres",
            HeaderText = "Nombres",
            DataPropertyName = nameof(Estudiante.Nombres),
            FillWeight = 15,
            MinimumWidth = 100
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colApellidos",
            HeaderText = "Apellidos",
            DataPropertyName = nameof(Estudiante.Apellidos),
            FillWeight = 16,
            MinimumWidth = 100
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colFechaNacimiento",
            HeaderText = "Nacimiento",
            DataPropertyName = nameof(Estudiante.FechaNacimiento),
            FillWeight = 12,
            MinimumWidth = 130,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "dd/MM/yyyy",
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colEdad",
            HeaderText = "Edad",
            DataPropertyName = nameof(Estudiante.Edad),
            FillWeight = 6,
            MinimumWidth = 64,
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
            FillWeight = 7,
            MinimumWidth = 70,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colTelefono",
            HeaderText = "Telefono",
            DataPropertyName = nameof(Estudiante.Telefono),
            FillWeight = 12,
            MinimumWidth = 105
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colEmail",
            HeaderText = "Email",
            DataPropertyName = nameof(Estudiante.Email),
            FillWeight = 19,
            MinimumWidth = 110
        });

        AjustarAnchosMinimos();
    }

    /// <summary>
    /// Garantiza que ninguna columna pueda encogerse por debajo de lo que mide
    /// su propio titulo. Los valores escritos a mano son solo un punto de
    /// partida; aqui se corrigen con la medida real de la fuente en uso.
    /// </summary>
    private void AjustarAnchosMinimos()
    {
        foreach (DataGridViewColumn columna in grid.Columns)
        {
            var necesario = AnchoDelTitulo(columna.HeaderText);

            if (columna.MinimumWidth < necesario)
            {
                columna.MinimumWidth = necesario;
            }
        }
    }

    /// <summary>
    /// Recarga la grilla con la pagina actual del filtro vigente.
    /// </summary>
    private void RefrescarGrilla()
    {
        var todos = RepositorioMemoria.Listar(txtBuscar.Text);

        _totalPaginas = Math.Max(1, (int)Math.Ceiling(todos.Count / (double)_filasPorPagina));
        _paginaActual = Math.Clamp(_paginaActual, 1, _totalPaginas);

        var pagina = todos
            .Skip((_paginaActual - 1) * _filasPorPagina)
            .Take(_filasPorPagina)
            .ToList();

        _cargandoGrilla = true;
        grid.DataSource = pagina;
        grid.ClearSelection();
        _cargandoGrilla = false;

        ActualizarPaginacion(todos.Count, pagina.Count);
    }

    /// <summary>
    /// Ajusta los controles de paginacion y el texto de la barra de estado.
    /// </summary>
    private void ActualizarPaginacion(int totalRegistros, int enEstaPagina)
    {
        lblPagina.Text = $"Pagina {_paginaActual} de {_totalPaginas}";

        btnPrimera.Enabled = _paginaActual > 1;
        btnAnterior.Enabled = _paginaActual > 1;
        btnSiguiente.Enabled = _paginaActual < _totalPaginas;
        btnUltima.Enabled = _paginaActual < _totalPaginas;

        if (totalRegistros == 0)
        {
            lblEstado.Text = string.IsNullOrWhiteSpace(txtBuscar.Text)
                ? "Sin estudiantes registrados"
                : "La busqueda no arrojo resultados";
            return;
        }

        var desde = ((_paginaActual - 1) * _filasPorPagina) + 1;
        var hasta = desde + enEstaPagina - 1;

        lblEstado.Text = totalRegistros == 1
            ? "1 estudiante"
            : $"Mostrando {desde}-{hasta} de {totalRegistros} estudiantes";
    }

    private void IrAPagina(int pagina)
    {
        _paginaActual = pagina;
        RefrescarGrilla();
    }

    /// <summary>
    /// Calcula en que pagina quedo un registro despues de ordenar la lista
    /// completa, para poder mostrarselo al usuario tras guardarlo.
    /// </summary>
    private int PaginaDe(string documento)
    {
        var posicion = RepositorioMemoria.Listar()
            .FindIndex(e => e.Documento.Equals(documento.Trim(), StringComparison.OrdinalIgnoreCase));

        return posicion < 0
            ? _paginaActual
            : (posicion / _filasPorPagina) + 1;
    }

    private void BtnPrimera_Click(object? sender, EventArgs e) => IrAPagina(1);

    private void BtnAnterior_Click(object? sender, EventArgs e) => IrAPagina(_paginaActual - 1);

    private void BtnSiguiente_Click(object? sender, EventArgs e) => IrAPagina(_paginaActual + 1);

    private void BtnUltima_Click(object? sender, EventArgs e) => IrAPagina(_totalPaginas);

    private void CboPorPagina_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (int.TryParse(cboPorPagina.SelectedItem?.ToString(), out var filas))
        {
            _filasPorPagina = filas;
            IrAPagina(1);
        }
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

        var esNuevo = _idEnEdicion == 0;

        if (esNuevo)
        {
            RepositorioMemoria.Insertar(estudiante);
        }
        else
        {
            RepositorioMemoria.Actualizar(estudiante);
        }

        // Se limpia el buscador para que el registro guardado no quede oculto
        // tras un filtro que no lo incluya.
        if (txtBuscar.Text.Length > 0)
        {
            _cargandoGrilla = true;
            txtBuscar.Clear();
            _cargandoGrilla = false;
        }

        _paginaActual = PaginaDe(estudiante.Documento);
        RefrescarGrilla();
        LimpiarFormulario();

        lblEstado.Text = esNuevo ? "Estudiante guardado" : "Estudiante actualizado";
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

        // Si se elimino el unico registro de la ultima pagina, RefrescarGrilla
        // ajusta la pagina actual para no dejarla vacia.
        RefrescarGrilla();
        LimpiarFormulario();
        lblEstado.Text = "Estudiante eliminado";
    }

    private void TxtBuscar_TextChanged(object? sender, EventArgs e)
    {
        // Un filtro nuevo puede dejar menos paginas que la actual, asi que se
        // vuelve al principio.
        IrAPagina(1);
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
