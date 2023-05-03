using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AccesoDatos.Entidades
{
    public class Venta
    {
        public int Id { get; set; }
        public int Folio { get; private set; }
        public DateTime Fecha { get; set; }
        public int ClienteId { get; set; }
        public decimal Total { get; set; }
        public List<VentaDetalle> Conceptos { get; set; } = new List<VentaDetalle>();

        public void GuardarVenta(Venta venta)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.ConectionString))
                {
                    SqlTransaction transaction;
                    con.Open();
                    transaction = con.BeginTransaction();

                    try
                    {
                        Folios folio = new Folios();
                        int folioActual = folio.ObtenerFolio(con, transaction) + 1;

                        string query = "INSERT INTO Ventas " +
                            "(Folio,Fecha,ClienteId,Total) " +
                            "VALUES " +
                            "(@Folio,@Fecha,@ClienteId,@Total);select scope_identity()";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Transaction = transaction;
                            cmd.Parameters.AddWithValue("@Folio", folioActual);
                            cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);
                            cmd.Parameters.AddWithValue("@ClienteId", venta.ClienteId);
                            cmd.Parameters.AddWithValue("@Total", venta.Total);

                            string ejecutaQuery = cmd.ExecuteScalar().ToString(); ;
                            bool verdadero = int.TryParse(ejecutaQuery, out int idVenta);


                            if (!verdadero)
                            {
                                throw new Exception("Ocurrio un error al obtener el id de la venta");
                            }
                            venta.Id = idVenta;
                        }

                        foreach (VentaDetalle concepto in Conceptos)
                        {
                            concepto.VentaId = Id;
                            concepto.GuardarVentaDetalle(con, transaction, concepto);

                            ExistenciaProd existencia = new ExistenciaProd();
                            existencia.ActualizarExistencia(con, transaction, concepto);
                        }

                        folio.ActualizarFolio(con, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex) { transaction.Rollback(); throw ex; }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}