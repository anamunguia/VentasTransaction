using AccesoDatos;
using AccesoDatos.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VentasTransaction
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();

            try
            {
                LoadProducts();
                LoadClients();
                LoadStock();
                InitConceptos();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void LoadProducts()
        {
            Producto accesoProductos = new Producto();
            SqlDataAdapter adapter = accesoProductos.ObtenerProductos();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ProductoGrid.DataSource = dt;
        }

        private void LoadClients()
        {
            Ciente accesoClientes = new Cliente();
            SqlDataAdapter adapter = accesoClientes.ObtenerClientes();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ClienteGrid.DataSource = dt;
        }

        private void LoadStock()
        {
            ExistenciaProd existencias = new ExistenciaProd();
            SqlDataAdapter adapter = existencias.ObtenerExistencias();
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            ExistenciaProdGrid.DataSource = dt;
            ExistenciaProdGrid.DataSource = dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void AgregarCliente(string nombreCliente)
        {
            Cliente cliente = new Cliente);
            cliente.Nombre = nombreCliente;
            Cliente accesoClientes = new Cliente();
            accesoClientes.CrearCliente(cliente);
        }

        private void BorrarCLiente()
        {
            try
            {
                int clienteId;
                if (int.TryParse(ClienteGrid.SelectedRows[0].Cells[0].Value.ToString(), out clienteId))
                {
                    Cliente accesoClientes = new Cliente();
                    accesoClientes.EliminarCliente(clienteId);
                    LoadClients();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void EditarCliente()
        {
            if (ClienteGrid.SelectedRows.Count > 0)
            {
                int clienteId;
                if (int.TryParse(ClientesGrid.SelectedRows[0].Cells[0].Value.ToString(), out clienteId))
                {
                    Cliente accesoClientes = new Cliente();
                    string nombre = InputBox.ShowDialog("Nuevo valor::", "Editar cliente");
                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        accesoClientes.ActualizarCliente(clienteId, nombre);
                        LoadClients();
                    }

                }
            }
        }

        private void GuardarProducto()
        {
            try
            {
                Producto producto = new Producto();
                producto.Descripcion = descriptionText.Text;
                decimal number;
                if (decimal.TryParse(priceText.Text, out number))
                {
                    producto.PrecioUnitario = number;
                }
                Producto accesoProductos = new Producto();
                accesoProductos.CrearProducto(producto);
                descriptionText.Text = "";
                priceText.Text = "";
            }
            catch (Exception ex)
            {

            }

        }

        private void GuardarVenta()
        {

        }


        private void agregarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                GuardarProducto();
                LoadProducts();
                LoadStock();
            }
            catch (Exception ex)
            {

            }

        }

        private void borrarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                int productoId;
                if (int.TryParse(ProductoGrid.SelectedRows[0].Cells[0].Value.ToString(), out productoId))
                {
                    Producto accesoProductos = new Producto();
                    accesoProductos.EliminarProducto(productoId);
                    CargarProductos();
                    CargarExistencias();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        private void borrarCliente_Click(object sender, EventArgs e)
        {
            BorrarCLiente();
        }

        private void actualizarCliente_Click(object sender, EventArgs e)
        {
            EditarCliente();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string client = textBox2.Text;
            if (!string.IsNullOrEmpty(client))
            {
                AgregarCliente(client);
                LoadClients();
            }
        }

        private void EditarExistencia_Click(object sender, EventArgs e)
        {
            int ExistenciaId;
            if (int.TryParse(ExistenciasGrid.SelectedRows[0].Cells[0].Value.ToString(), out ExistenciaId))
            {
                ExistenciaProd productoExistencia = new ExistenciaProd();
                decimal current;
                if (decimal.TryParse(InputBox.ShowDialog("Existencia actual:", "Editar Existencia"), out current))
                {
                    productoExistencia.ActualizarExistencia(ExistenciaId, current);
                    CargarExistencias();
                }

            }
        }

        private void generarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                int folioActual = 0;
                Venta venta = new Venta();
                venta.CLienteId = 1;
                venta.Folio = folioActual + 1;
                venta.Fecha = DateTime.Now;

                for (int i = 0; i < ExistenciaProdGrid.RowCount; i++)
                {
                    VentaDetalle concepto = new VentaDetalle();
                    concepto.ProductoId = int.Parse(conceptosGrid.Rows[i].Cells[0].Value.ToString());
                    concepto.Descripcion = conceptosGrid.Rows[i].Cells[1].Value.ToString();
                    concepto.Cantidad = decimal.Parse(conceptosGrid.Rows[i].Cells[2].Value.ToString());
                    concepto.PrecioUnitario = decimal.Parse(conceptosGrid.Rows[i].Cells[3].Value.ToString());
                    concepto.Importe = decimal.Parse(conceptosGrid.Rows[i].Cells[4].Value.ToString());
                    venta.Conceptos.Add(concepto);
                }
                Venta accesoVentas = new Venta();
                accesoVentas.crearVenta(venta);
            }
            catch (Exception ex)
            {

            }
        }

        private void agregarConcepto_Click(object sender, EventArgs e)
        {
            try
            {
                int rowIndex = conceptosGrid.Rows.Add();
                DataGridViewRow row = conceptosGrid.Rows[rowIndex];
                row.Cells["Id"].Value = ExistenciaProdGrid.SelectedRows[0].Cells[0].Value;//
                row.Cells["Descripcion"].Value = ExistenciaProdGrid.SelectedRows[0].Cells[1].Value;
                decimal existencia = decimal.Parse(ExistenciaProdGrid.SelectedRows[0].Cells[0].Value.ToString());
                decimal cantidad;
                if (decimal.TryParse(cantidadText.Text, out cantidad))
                {
                    if (cantidad > existencia) { cantidad = existencia; }
                    if (cantidad <= 0) { cantidad = 1; }

                    row.Cells["Cantidad"].Value = cantidad;
                }
                else
                {
                    row.Cells["Cantidad"].Value = 1;
                }
                decimal precio;
                if (decimal.TryParse(ExistenciaProdGrid.SelectedRows[0].Cells[2].Value.ToString(), out precio))
                {
                    row.Cells["Precio Unitario"].Value = precio;
                }

                row.Cells["Importe"].Value = cantidad * precio;

                MessageBox.Show("Agregado!", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "");
                throw new Exception(ex.Message);
            }

        }

        private void InitConceptos()
        {
            conceptosGrid.Columns.Add("Id", "Id");
            conceptosGrid.Columns.Add("Descripcion", "Descripcion");
            conceptosGrid.Columns.Add("Cantidad", "Cantidad");
            conceptosGrid.Columns.Add("Precio Unitario", "Precio Unitario");
            conceptosGrid.Columns.Add("Importe", "Importe");
        }
    }
}