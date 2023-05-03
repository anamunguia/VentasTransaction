using System;
using System.Data;
using System.Data.SqlClient;

namespace AccesoDatos.Entidades
{
    public class Folios
    {
        public int Id { get; set; }
        public int Folio { get; set; }

        public int ObtenerFolio(SqlConnection con, SqlTransaction transaction)
        {
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
                return folioActual;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void ActualizarFolio(SqlConnection con, SqlTransaction transaction)
        {
            try
            {
                string query = "Update Folios set Folio = Folio + 1 ";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }

}