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

                using (SqlConnection con = new SqlConnection(conexion.ConnectionString))
                {
                    SqlTransaction transaction;
                    con.Open();
                    transaction = con.BeginTransaction();

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
                        Console.WriteLine("ID: " + idProducto);
                        ProductoExistencia productoExistencia = new ProductoExistencia();
                        productoExistencia.AgregarExistenciaEnCero(con, transaction, idProducto);
                    }
                    transaction.Commit();
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


            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaction;

                cmd.Parameters.AddWithValue("@Descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@PrecioUnitario", producto.PrecioUnitario);
                cmd.Parameters.AddWithValue("@Id", producto.Id);

                cmd.ExecuteNonQuery();
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

public Producto ObtenerProducto(int id)
{
    try
    {
        string query = "SELECT Id, Descripcion, PrecioUnitario FROM Existencias WHERE Id = @Id";

        using (SqlConnection con = new SqlConnection(query))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Producto producto = new Producto();

                            producto.Id = reader.GetInt32(0);
                            producto.Descripcion = reader.GetString(1);
                            producto.PrecioUnitario = reader.GetDecimal(2);

                            return producto;
                        }
                    }
                }
            }
        }

        return null;
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

        
    }

}