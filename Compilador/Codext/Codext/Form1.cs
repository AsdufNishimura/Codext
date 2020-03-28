using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Codext
{
    public partial class frmCodext : Form
    {
        SqlConnection Conexion;
        DataTable tabla;
        SqlDataAdapter Adapter;
        Instrucción miInstruccion;

        public frmCodext()
        {
            InitializeComponent();
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            try
            {
                miInstruccion = new Instrucción();
                miInstruccion.Cadena = "DECIE";
                AnalizadorLexico(miInstruccion, 0, "0");
                MessageBox.Show(miInstruccion.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmCodext_Load(object sender, EventArgs e)
        {

        }

        public string CargarQuery(string Query)
        {
            string Consulta = "";
            try
            {
                tabla = new DataTable();
                Conexion = new SqlConnection("Server=CheZep;Database=Codext;Trusted_Connection=True;");
                Adapter = new SqlDataAdapter(Query, Conexion);
                Adapter.Fill(tabla);

                foreach(DataRow row in tabla.Rows)
                {
                    foreach(DataColumn column in tabla.Columns)
                    {
                        Consulta = row[column].ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Consulta;
        }

        public bool EstadoFinal(string Query)
        {
            bool EsFinal = false;
            try
            {
                tabla = new DataTable();
                Conexion = new SqlConnection("Server=CheZep;Database=Codext;Trusted_Connection=True;");
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

        public string AnalizadorLexico(Instrucción otraInstruccion, int Pos, string Estado)
        {
            string strConsulta = "";
            try
            {
                if(Pos <= otraInstruccion.Cadena.Length - 1)
                {
                    strConsulta = CargarQuery("select A_" + otraInstruccion.Cadena[Pos] + " from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return AnalizadorLexico(otraInstruccion, Pos + 1, strConsulta);
                    else
                        throw new Exception("Instrucción no válida");
                }
                else
                {
                    if (EstadoFinal("select * from Lexico where Estado = " + Estado))
                    {
                        otraInstruccion.Token = CargarQuery("select A_Token from Lexico where Estado = " + Estado);
                    }
                    else
                    {
                        strConsulta = CargarQuery("select A_del from Lexico where Estado = " + Estado);
                        if (strConsulta != "")
                            return AnalizadorLexico(otraInstruccion, Pos + 1, strConsulta);
                        else
                            throw new Exception("Instrucción no válida");

                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return otraInstruccion.Token;
        }
    }

}
