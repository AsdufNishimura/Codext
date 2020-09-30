using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Codext
{
    public static class Database
    {
        static SqlConnection Conexion;        
        static SqlDataAdapter Adapter;
        static DataTable tabla;

        //public static DataTable Tabla
        //{
        //    get { return tabla; }
        //}

        const string nombreServidor = @"ASDUF\SQLEXPRESS";
        const string nombreBD = "Codext";


        public static string CargarQuery(string Query)
        {
            string Consulta = "";
            try
            {
                tabla = new DataTable();
                Conexion = new SqlConnection("Server=" + nombreServidor + ";Database=" + nombreBD + ";Trusted_Connection=True;");
                Adapter = new SqlDataAdapter(Query, Conexion);
                Adapter.Fill(tabla);

                if (tabla.Rows.Count > 0)
                {
                    foreach (DataRow row in tabla.Rows)
                    {
                        foreach (DataColumn column in tabla.Columns)
                        {
                            Consulta = row[column].ToString();
                        }
                    }
                }
                else
                    Consulta = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Consulta;
        }

        public static bool EstadoFinal(string Query)
        {
            bool EsFinal = false;
            try
            {
                tabla = new DataTable();
                Conexion = new SqlConnection("Server=" + nombreServidor + ";Database=" + nombreBD + ";Trusted_Connection=True;");
                Adapter = new SqlDataAdapter(Query, Conexion);
                Adapter.Fill(tabla);

                foreach (DataRow row in tabla.Rows)
                {
                    foreach (DataColumn column in tabla.Columns)
                    {
                        EsFinal = (column.ColumnName == "A_Token" && row[column].ToString() != "");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return EsFinal;
        }
    }
}
