using AccesoDatos;
using System;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GuardarVenta();
        }

        //Debemos reubicar este metodo 
        private void GuardarVenta()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.ConnectionString))
                {
                    SqlTransaction transaction;
                    con.Open();
                    transaction = con.BeginTransaction();

                    try
                    {
                        string query = "select top(1) Folio from Folios";
                        int folioActual = 0;
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Transaction = transaction;

                            if (!int.TryParse(cmd.ExecuteScalar().ToString(), out folioActual))
                            {
                                throw new Exception("Ocurrio un error al obtener el folio");
                            }
                        }

                        Venta venta = new Venta();
                        venta.CLienteId = 1;
                        venta.Folio = folioActual + 1;
                        venta.Fecha = DateTime.Now;

                        VentaDetalle producto1 = new VentaDetalle();
                        producto1.ProductoId = 1;
                        producto1.Cantidad = 1;
                        producto1.Descripcion = "Azucar kg";
                        producto1.PrecioUnitario = 27.00m;
                        producto1.Importe = producto1.Cantidad * producto1.PrecioUnitario;

                        VentaDetalle producto2 = new VentaDetalle();
                        producto2.ProductoId = 2;
                        producto2.Cantidad = 1;
                        producto2.Descripcion = "Jugo Mango";
                        producto2.PrecioUnitario = 10.00m;
                        producto2.Importe = producto2.Cantidad * producto2.PrecioUnitario;

                        venta.Conceptos.Add(producto1);
                        venta.Conceptos.Add(producto2);

                        query = "INSERT INTO Ventas " +
                            "(Folio,Fecha,ClienteId,Total) " +
                            "VALUES " +
                            "(@Folio,@Fecha,@ClienteId,@Total);select scope_identity()";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@Folio", venta.Folio);
                            cmd.Parameters.AddWithValue("@Fecha", venta.Fecha);
                            cmd.Parameters.AddWithValue("@ClienteId", venta.CLienteId);
                            cmd.Parameters.AddWithValue("@Total", venta.Total);


                            if (!int.TryParse(cmd.ExecuteScalar().ToString(), out int idVenta))
                            {
                                throw new Exception("Ocurrio un error al obtener el id de la venta");
                            }
                            venta.Id = idVenta;
                        }

                        foreach (VentaDetalle concepto in venta.Conceptos)
                        {

                            query = "INSERT INTO VentasDetalle" +
                                    "(VentaId,ProductoId,Cantidad,Descripcion,PrecioUnitario,Importe) " +
                                    "VALUES" +
                                    "(@VentaId,@ProductoId,@Cantidad,@Descripcion,@PrecioUnitario,@Importe)";

                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Transaction = transaction;

                                cmd.Parameters.AddWithValue("@VentaId", venta.Id);
                                cmd.Parameters.AddWithValue("@ProductoId", concepto.ProductoId);
                                cmd.Parameters.AddWithValue("@Cantidad", concepto.Cantidad);
                                cmd.Parameters.AddWithValue("@Descripcion", concepto.Descripcion);
                                cmd.Parameters.AddWithValue("@PrecioUnitario", concepto.PrecioUnitario);
                                cmd.Parameters.AddWithValue("@Importe", concepto.Importe);
                                cmd.ExecuteNonQuery();
                            }


                            query = "Update Existencias " +
                                    "set Existencia = Existencia-@Cantidad " +
                                    "where ProductoId = @ProductoId";

                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Transaction = transaction;

                                cmd.Parameters.AddWithValue("@ProductoId", concepto.ProductoId);
                                cmd.Parameters.AddWithValue("@Cantidad", concepto.Cantidad);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        query = "Update Folios set Folio = Folio + 1 ";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Transaction = transaction;

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        MessageBox.Show($"Venta guardada correctamente con folio {venta.Folio}");

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ocurrio un error al guardar la venta {ex.Message}");
            }
        }
    }
}