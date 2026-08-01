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
            Name = "colCarrera",
            HeaderText = "Carrera",
            DataPropertyName = nameof(Estudiante.Carrera),
            FillWeight = 16,
            MinimumWidth = 110
        });

        grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "colSemestre",
            HeaderText = "Semestre",
            DataPropertyName = nameof(Estudiante.Semestre),
            FillWeight = 8,
            MinimumWidth = 90,
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
        List<Estudiante> todos;

        try
        {
            todos = RepositorioSqlServer.Listar(txtBuscar.Text);
        }
        catch (Exception ex)
        {
            MostrarErrorDeBaseDatos("No se pudo consultar la lista de estudiantes.", ex);
            todos = new List<Estudiante>();
        }


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
        var posicion = RepositorioSqlServer.Listar()
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
        txtCarrera.Text = seleccionado.Carrera;
        nudSemestre.Value = Math.Clamp(seleccionado.Semestre, (int)nudSemestre.Minimum, (int)nudSemestre.Maximum);

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
            Carrera = txtCarrera.Text,
            Semestre = (int)nudSemestre.Value
        };

        var esNuevo = _idEnEdicion == 0;

        try
        {
            if (esNuevo)
            {
                RepositorioSqlServer.Insertar(estudiante);
            }
            else
            {
                RepositorioSqlServer.Actualizar(estudiante);
            }
        }
        catch (InvalidOperationException ex)
        {
            // Documento duplicado detectado por la restriccion de la tabla.
            Advertir(ex.Message, txtDocumento);
            return;
        }
        catch (Exception ex)
        {
            MostrarErrorDeBaseDatos("No se pudo guardar el estudiante.", ex);
            return;
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

        try
        {
            RepositorioSqlServer.Eliminar(_idEnEdicion);
        }
        catch (Exception ex)
        {
            MostrarErrorDeBaseDatos("No se pudo eliminar el estudiante.", ex);
            return;
        }

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

        if (!Validaciones.EsNombreValido(txtNombres.Text))
        {
            Advertir("Los nombres no admiten numeros ni signos.", txtNombres);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtApellidos.Text))
        {
            Advertir("Los apellidos son obligatorios.", txtApellidos);
            return false;
        }

        if (!Validaciones.EsNombreValido(txtApellidos.Text))
        {
            Advertir("Los apellidos no admiten numeros ni signos.", txtApellidos);
            return false;
        }

        if (dtpFechaNacimiento.Value.Date > DateTime.Today)
        {
            Advertir("La fecha de nacimiento no puede ser futura.", dtpFechaNacimiento);
            return false;
        }

        if (dtpFechaNacimiento.Value.Date < DateTime.Today.AddYears(-120))
        {
            Advertir("Revisa la fecha de nacimiento: la edad no es valida.", dtpFechaNacimiento);
            return false;
        }

        if (!Validaciones.EsDocumentoValido(txtDocumento.Text))
        {
            Advertir(
                "El documento solo admite letras y numeros, entre 5 y 20 caracteres.",
                txtDocumento);
            return false;
        }

        if (!Validaciones.EsTelefonoValido(txtTelefono.Text))
        {
            Advertir(
                "El telefono solo admite numeros, espacios y los signos + ( ) -, " +
                "con al menos 7 digitos.",
                txtTelefono);
            return false;
        }

        if (!Validaciones.EsEmailValido(txtEmail.Text))
        {
            Advertir(
                "El email no tiene un formato valido. Ejemplo: nombre@dominio.com",
                txtEmail);
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtCarrera.Text))
        {
            Advertir("La carrera es obligatoria.", txtCarrera);
            return false;
        }

        if (!Validaciones.EsCarreraValida(txtCarrera.Text))
        {
            Advertir(
                "La carrera solo admite letras, espacios, puntos y guiones, " +
                "entre 3 y 80 caracteres.",
                txtCarrera);
            return false;
        }

        if (!Validaciones.EsSemestreValido((int)nudSemestre.Value))
        {
            Advertir("El semestre debe estar entre 1 y 12.", nudSemestre);
            return false;
        }

        try
        {
            if (RepositorioSqlServer.ExisteDocumento(txtDocumento.Text, _idEnEdicion))
            {
                Advertir("Ya existe un estudiante con ese documento.", txtDocumento);
                return false;
            }
        }
        catch (Exception ex)
        {
            MostrarErrorDeBaseDatos("No se pudo verificar el documento.", ex);
            return false;
        }

        return true;
    }

    private static void Advertir(string mensaje, Control foco)
    {
        MessageBox.Show(mensaje, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        foco.Focus();
    }

    /// <summary>
    /// Informa de un fallo al hablar con SQL Server, indicando donde se
    /// configura el servidor para que el usuario pueda corregirlo.
    /// </summary>
    private void MostrarErrorDeBaseDatos(string queFallo, Exception ex)
    {
        MessageBox.Show(
            $"{queFallo}\n\n" +
            $"Detalle: {ex.Message}\n\n" +
            $"Revisa que SQL Server este encendido y que la cadena de conexion " +
            $"sea correcta en:\n{Configuracion.RutaArchivo}",
            "Error de base de datos",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);

        lblEstado.Text = "Sin conexion con la base de datos";
    }

    private void LimpiarFormulario()
    {
        _idEnEdicion = 0;

        txtDocumento.Clear();
        txtNombres.Clear();
        txtApellidos.Clear();
        dtpFechaNacimiento.Value = new DateTime(2000, 1, 1);
        txtTelefono.Clear();
        txtEmail.Clear();
        txtDireccion.Clear();
        txtCarrera.Clear();
        nudSemestre.Value = 1;

        btnGuardar.Text = "Guardar";
        grid.ClearSelection();
        txtDocumento.Focus();
    }
}
