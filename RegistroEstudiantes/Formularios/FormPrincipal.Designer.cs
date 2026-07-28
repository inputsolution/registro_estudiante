namespace RegistroEstudiantes.Formularios;

partial class FormPrincipal
{
    private System.ComponentModel.IContainer components = null;

    private GroupBox grupoDatos;
    private Label lblDocumento;
    private TextBox txtDocumento;
    private Label lblNombres;
    private TextBox txtNombres;
    private Label lblApellidos;
    private TextBox txtApellidos;
    private Label lblFechaNacimiento;
    private DateTimePicker dtpFechaNacimiento;
    private Label lblTelefono;
    private TextBox txtTelefono;
    private Label lblEmail;
    private TextBox txtEmail;
    private Label lblDireccion;
    private TextBox txtDireccion;
    private Label lblGrado;
    private TextBox txtGrado;

    private Button btnGuardar;
    private Button btnNuevo;
    private Button btnEliminar;

    private Label lblBuscar;
    private TextBox txtBuscar;
    private DataGridView grid;

    private Panel panelPaginacion;
    private Button btnPrimera;
    private Button btnAnterior;
    private Label lblPagina;
    private Button btnSiguiente;
    private Button btnUltima;
    private Label lblPorPagina;
    private ComboBox cboPorPagina;

    private StatusStrip barraEstado;
    private ToolStripStatusLabel lblEstado;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        grupoDatos = new GroupBox();
        lblDocumento = new Label();
        txtDocumento = new TextBox();
        lblNombres = new Label();
        txtNombres = new TextBox();
        lblApellidos = new Label();
        txtApellidos = new TextBox();
        lblFechaNacimiento = new Label();
        dtpFechaNacimiento = new DateTimePicker();
        lblTelefono = new Label();
        txtTelefono = new TextBox();
        lblEmail = new Label();
        txtEmail = new TextBox();
        lblDireccion = new Label();
        txtDireccion = new TextBox();
        lblGrado = new Label();
        txtGrado = new TextBox();
        btnGuardar = new Button();
        btnNuevo = new Button();
        btnEliminar = new Button();
        lblBuscar = new Label();
        txtBuscar = new TextBox();
        grid = new DataGridView();
        panelPaginacion = new Panel();
        btnPrimera = new Button();
        btnAnterior = new Button();
        lblPagina = new Label();
        btnSiguiente = new Button();
        btnUltima = new Button();
        lblPorPagina = new Label();
        cboPorPagina = new ComboBox();
        barraEstado = new StatusStrip();
        lblEstado = new ToolStripStatusLabel();

        grupoDatos.SuspendLayout();
        panelPaginacion.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        barraEstado.SuspendLayout();
        SuspendLayout();

        // ---------- Grupo de datos ----------
        grupoDatos.Text = "Datos del estudiante";
        grupoDatos.Location = new Point(12, 12);
        grupoDatos.Size = new Size(860, 165);
        grupoDatos.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // Fila 1
        lblDocumento.Text = "Documento:";
        lblDocumento.Location = new Point(18, 32);
        lblDocumento.Size = new Size(90, 20);
        lblDocumento.TextAlign = ContentAlignment.MiddleLeft;

        txtDocumento.Location = new Point(110, 29);
        txtDocumento.Size = new Size(160, 25);
        txtDocumento.MaxLength = 20;

        lblNombres.Text = "Nombres:";
        lblNombres.Location = new Point(295, 32);
        lblNombres.Size = new Size(70, 20);
        lblNombres.TextAlign = ContentAlignment.MiddleLeft;

        txtNombres.Location = new Point(370, 29);
        txtNombres.Size = new Size(190, 25);
        txtNombres.MaxLength = 60;

        lblApellidos.Text = "Apellidos:";
        lblApellidos.Location = new Point(580, 32);
        lblApellidos.Size = new Size(70, 20);
        lblApellidos.TextAlign = ContentAlignment.MiddleLeft;

        txtApellidos.Location = new Point(655, 29);
        txtApellidos.Size = new Size(190, 25);
        txtApellidos.MaxLength = 60;

        // Fila 2
        lblFechaNacimiento.Text = "F. nacim.:";
        lblFechaNacimiento.Location = new Point(18, 68);
        lblFechaNacimiento.Size = new Size(90, 20);
        lblFechaNacimiento.TextAlign = ContentAlignment.MiddleLeft;

        dtpFechaNacimiento.Location = new Point(110, 65);
        dtpFechaNacimiento.Size = new Size(160, 25);
        dtpFechaNacimiento.Format = DateTimePickerFormat.Short;
        dtpFechaNacimiento.Value = new DateTime(2010, 1, 1);

        lblTelefono.Text = "Telefono:";
        lblTelefono.Location = new Point(295, 68);
        lblTelefono.Size = new Size(70, 20);
        lblTelefono.TextAlign = ContentAlignment.MiddleLeft;

        txtTelefono.Location = new Point(370, 65);
        txtTelefono.Size = new Size(190, 25);
        txtTelefono.MaxLength = 20;

        lblEmail.Text = "Email:";
        lblEmail.Location = new Point(580, 68);
        lblEmail.Size = new Size(70, 20);
        lblEmail.TextAlign = ContentAlignment.MiddleLeft;

        txtEmail.Location = new Point(655, 65);
        txtEmail.Size = new Size(190, 25);
        txtEmail.MaxLength = 80;

        // Fila 3
        lblDireccion.Text = "Direccion:";
        lblDireccion.Location = new Point(18, 104);
        lblDireccion.Size = new Size(90, 20);
        lblDireccion.TextAlign = ContentAlignment.MiddleLeft;

        txtDireccion.Location = new Point(110, 101);
        txtDireccion.Size = new Size(450, 25);
        txtDireccion.MaxLength = 120;

        lblGrado.Text = "Grado:";
        lblGrado.Location = new Point(580, 104);
        lblGrado.Size = new Size(70, 20);
        lblGrado.TextAlign = ContentAlignment.MiddleLeft;

        txtGrado.Location = new Point(655, 101);
        txtGrado.Size = new Size(190, 25);
        txtGrado.MaxLength = 30;

        // Botones
        btnGuardar.Text = "Guardar";
        btnGuardar.Location = new Point(110, 133);
        btnGuardar.Size = new Size(100, 28);
        btnGuardar.Click += BtnGuardar_Click;

        btnNuevo.Text = "Nuevo";
        btnNuevo.Location = new Point(220, 133);
        btnNuevo.Size = new Size(100, 28);
        btnNuevo.Click += BtnNuevo_Click;

        btnEliminar.Text = "Eliminar";
        btnEliminar.Location = new Point(330, 133);
        btnEliminar.Size = new Size(100, 28);
        btnEliminar.Click += BtnEliminar_Click;

        grupoDatos.Controls.AddRange(new Control[]
        {
            lblDocumento, txtDocumento,
            lblNombres, txtNombres,
            lblApellidos, txtApellidos,
            lblFechaNacimiento, dtpFechaNacimiento,
            lblTelefono, txtTelefono,
            lblEmail, txtEmail,
            lblDireccion, txtDireccion,
            lblGrado, txtGrado,
            btnGuardar, btnNuevo, btnEliminar
        });

        // ---------- Buscador ----------
        lblBuscar.Text = "Buscar:";
        lblBuscar.Location = new Point(12, 190);
        lblBuscar.Size = new Size(55, 20);
        lblBuscar.TextAlign = ContentAlignment.MiddleLeft;

        txtBuscar.Location = new Point(70, 187);
        txtBuscar.Size = new Size(280, 25);
        txtBuscar.PlaceholderText = "Documento, nombres o apellidos...";
        txtBuscar.TextChanged += TxtBuscar_TextChanged;

        // ---------- Grilla ----------
        // Se reserva el espacio inferior para la barra de paginacion.
        grid.Location = new Point(12, 220);
        grid.Size = new Size(860, 264);
        grid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoGenerateColumns = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowHeadersVisible = false;
        grid.BackgroundColor = SystemColors.Window;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.RowTemplate.Height = 26;
        grid.ScrollBars = ScrollBars.Both;
        grid.AllowUserToResizeRows = false;

        // Espacio interno amplio para que los titulos no queden pegados a las
        // lineas divisorias, que es lo que hacia dificil leerlos.
        grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
        grid.EnableHeadersVisualStyles = false;

        grid.AllowUserToResizeColumns = true;
        grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

        grid.SelectionChanged += Grid_SelectionChanged;

        // ---------- Barra de paginacion ----------
        panelPaginacion.Location = new Point(12, 490);
        panelPaginacion.Size = new Size(860, 32);
        panelPaginacion.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

        btnPrimera.Text = "|<";
        btnPrimera.Location = new Point(0, 2);
        btnPrimera.Size = new Size(34, 26);
        btnPrimera.Click += BtnPrimera_Click;

        btnAnterior.Text = "<";
        btnAnterior.Location = new Point(38, 2);
        btnAnterior.Size = new Size(34, 26);
        btnAnterior.Click += BtnAnterior_Click;

        lblPagina.Text = "Pagina 1 de 1";
        lblPagina.Location = new Point(78, 2);
        lblPagina.Size = new Size(170, 26);
        lblPagina.TextAlign = ContentAlignment.MiddleCenter;

        btnSiguiente.Text = ">";
        btnSiguiente.Location = new Point(254, 2);
        btnSiguiente.Size = new Size(34, 26);
        btnSiguiente.Click += BtnSiguiente_Click;

        btnUltima.Text = ">|";
        btnUltima.Location = new Point(292, 2);
        btnUltima.Size = new Size(34, 26);
        btnUltima.Click += BtnUltima_Click;

        // Anclado a la derecha para que acompañe el borde al redimensionar.
        lblPorPagina.Text = "Filas por pagina:";
        lblPorPagina.Location = new Point(690, 2);
        lblPorPagina.Size = new Size(105, 26);
        lblPorPagina.TextAlign = ContentAlignment.MiddleRight;
        lblPorPagina.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        cboPorPagina.Location = new Point(800, 4);
        cboPorPagina.Size = new Size(60, 25);
        cboPorPagina.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPorPagina.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cboPorPagina.Items.AddRange(new object[] { "10", "25", "50", "100" });
        cboPorPagina.SelectedIndex = 0;
        cboPorPagina.SelectedIndexChanged += CboPorPagina_SelectedIndexChanged;

        panelPaginacion.Controls.AddRange(new Control[]
        {
            btnPrimera, btnAnterior, lblPagina, btnSiguiente, btnUltima,
            lblPorPagina, cboPorPagina
        });

        // ---------- Barra de estado ----------
        lblEstado.Text = "Listo";
        barraEstado.Items.Add(lblEstado);
        barraEstado.Location = new Point(0, 530);

        // ---------- Formulario ----------
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(884, 552);
        MinimumSize = new Size(900, 590);
        Controls.AddRange(new Control[]
        {
            grupoDatos, lblBuscar, txtBuscar, grid, panelPaginacion, barraEstado
        });
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Registro de Estudiantes";
        Load += FormPrincipal_Load;

        grupoDatos.ResumeLayout(false);
        grupoDatos.PerformLayout();
        panelPaginacion.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        barraEstado.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}
