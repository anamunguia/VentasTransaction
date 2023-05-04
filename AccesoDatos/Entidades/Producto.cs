using System;
using System.Data;
using System.Data.SqlClient;

namespace AccesoDatos.Entidades
{
    public class Producto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }


        public void CrearProducto(Producto producto)
        {
            try
            {
                string query = "INSERT INTO Existencias" +
                    "(Descripcion,PrecioUnitario) " +
                    "VALUES" +
                    "(@Descripcion,@PrecioUnitario)";

                using (SqlConnection con = new SqlConnection(Conexion.ConectionString))
                {
                    SqlTransaction transaction;
                    con.Open();
                    transaction = con.BeginTransaction();

                    try
                    {

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Transaction = transaction;

                            cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                            cmd.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);

                            if (!int.TryParse(cmd.ExecuteScalar().ToString(), out int idProducto))
                            {
                                throw new Exception("Ocurrio un error al obtener el id del Producto");
                            }
                            ExistenciaProd existencia = new ExistenciaProd();
                            existencia.AgregarExistenciaEnCero(con, transaction, idProducto);
                        }
            
                    transaction.Commit();
                }
                catch(Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
            }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
    }
}

public void ActualizarProducto(Producto producto)
{
            try
            {
                string query = "UPDATE Existencias SET Descripcion = @Descripcion, PrecioUnitario = @PrecioUnitario WHERE Id = @Id";

                using (SqlConnection con = new SqlConnection(query))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Transaction = transaction;

                            cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                            cmd.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);
                            cmd.Parameters.AddWithValue("@Id", producto.Id);

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
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
                throw new Exception(ex.Message);
            }
}
public void EliminarProducto(int id)
{
    try
    {
        string query = "DELETE FROM Existencias where Id = @Id";

        using (SqlConnection con = new SqlConnection(query))
        {
            con.Open();
            SqlTransaction transaction = con.BeginTransaction();


            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                try
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = transaction;

                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}



        
    }

}