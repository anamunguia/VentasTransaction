using System.Data;
using System.Data.SqlClient;

namespace AccesoDatos.Entidades
{
    public class ExistenciaProd
    {

        public int Id { get; set; }
        public decimal Existencia { get; set; }
        public int ProductoId { get; set; }

        public void ActualizarExistencia(SqlConnection con, SqlTransaction transaction, VentaDetalle concepto)
        {
            string query = "Update Existencias " +
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
        public void AgregarExistenciaEnCero(SqlConnection con, SqlTransaction transaction, int productoId)
        {
            string query = "Insert into Existencias (Existencia, ProductoId) +" +
                "VALUES (0, @ProductoId)";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text; cmd.Transaction = transaction;

                cmd.Parameters.AddWithValue("@ProductoId", productoId);
            }

        }

        public DataTable ObtenerExistenciaProd()
        {
            using (SqlConnection con = new SqlConnection())
            {
                string consulta = "select * from Existencias";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, Conexion.ConectionString);
                DataTable tablaProductos = new DataTable();
                adaptador.Fill(tablaProductos);
                return tablaProductos;
            }

        }



    }

}