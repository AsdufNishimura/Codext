﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Codext
{
    public partial class frmCodext : Form
    {
        SqlConnection Conexion;
        DataTable tabla;
        SqlDataAdapter Adapter;
        Instrucción miInstruccion;
        List<Instrucción> lstInstrucciones = new List<Instrucción>();
        List<string> lstSubcadenas;
        List<List<string>> lstRenglones;
        int IDXX = 0, CNXX = 0, CRXX = 0;

        const string nombreServidor = @"ASDUF\SQLEXPRESS";
        const string nombreBD = "Codext";



        public frmCodext()
        {
            InitializeComponent();
        }

        private void frmCodext_Load(object sender, EventArgs e)
        {
            dgvTSIdentificadores.Columns.Add("columnNumero", "# Identificador");
            dgvTSIdentificadores.Columns.Add("columnNombre", "Nombre");
            dgvTSIdentificadores.Columns.Add("columnTipo", "Tipo de dato");
            dgvTSIdentificadores.Columns.Add("columnValor", "Valor");

            dgvTSConstantesNumericas.Columns.Add("columnNumero", "# Constante");
            dgvTSConstantesNumericas.Columns.Add("columnValor", "Valor");

            dgvTSOperadores.Columns.Add("columnNumero", "# Operador");
            dgvTSOperadores.Columns.Add("columnJerarquia", "Jerarquía");

            dgvTSOperadores.Rows.Add("**", 1);
            dgvTSOperadores.Rows.Add("*", 2);
            dgvTSOperadores.Rows.Add("/", 2);
            dgvTSOperadores.Rows.Add("+", 3);
            dgvTSOperadores.Rows.Add("-", 3);
            dgvTSOperadores.Rows.Add("!", 4);
            dgvTSOperadores.Rows.Add("&", 5);
            dgvTSOperadores.Rows.Add("|", 6);
            dgvTSOperadores.Rows.Add(">", 7);
            dgvTSOperadores.Rows.Add("<", 7);
            dgvTSOperadores.Rows.Add("<>", 7);
            dgvTSOperadores.Rows.Add("=", 7);
            dgvTSOperadores.Rows.Add("<=", 7);
            dgvTSOperadores.Rows.Add(">=", 7);
        }


        // Carga un archivo externo para un programa.
        private void btnCargar_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialogEntrada = new OpenFileDialog();
                dialogEntrada.Filter = ".txt files (*.txt)|*.txt";
                dialogEntrada.ShowDialog();

                if(dialogEntrada.CheckFileExists)
                {
                    System.IO.StreamReader srLector = new System.IO.StreamReader(dialogEntrada.FileName);
                    txtCodigo.Text = srLector.ReadToEnd();
                    srLector.Close();
                }
                else
                {
                    MessageBox.Show("Algo salió mal.\nIntentelo de nuevo.", "Error", MessageBoxButtons.OK);
                }
                txtConsola.Text = "Archivo cargado.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region AnalizadorLexico
        // Comienza el proceso del análisis lexico, utilizando el texto que se halle en la caja.
        private async void btnComenzar_Click(object sender, EventArgs e)
        {
            try
            {
                txtConsola.Text = "Comenzando proceso.";
                txtTokens.Text = "";
                txtEvaluacion.Text = "";
                string strTokenAux;
                txtRenglones.Text = txtCodigo.Lines.Count().ToString();
                ObtenerSubcadenas();
                int renglonActual = 0;
                int intPosRenglon = 0;
                int intPosRenglonTokens = 0;

                foreach (List<string> Subcadenas in lstRenglones)
                {
                    txtCodigo.BackColor = Color.Goldenrod;
                    txtConsola.Text = "Leyendo el renglón " + renglonActual;
                    txtCodigo.Focus();
                    txtCodigo.Select(intPosRenglon, txtCodigo.Lines[renglonActual].Length);
                    await Task.Delay(2000);
                    txtCodigo.BackColor = Color.FromArgb(255, 240, 240, 240);
                    foreach (string Subcadena in Subcadenas)
                    {
                        //Aquí debería pausar xd creo
                        txtSubcadena.Text = Subcadena;
                        txtSubcadena.BackColor = Color.Goldenrod;
                        txtConsola.Text = "Leyendo la subcadena " + Subcadena;                        
                        await Task.Delay(2000);
                        txtSubcadena.BackColor = Color.FromArgb(255, 240, 240, 240);

                        miInstruccion = new Instrucción();
                        miInstruccion.Cadena = Subcadena;

                        strTokenAux = AnalizadorLexico(miInstruccion, 0, "0");

                        if (strTokenAux == "ERRL")
                        {
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = Color.Goldenrod;
                            txtConsola.Text = "Error detectado: no se reconoció el elemento. Se asignará un token de error léxico (ERRL)";
                            await Task.Delay(2000);
                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                        }
                        else
                        {
                            //Aquí también                        
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = Color.Goldenrod;
                            txtConsola.Text = "Se identifico la subcadena " + Subcadena + " con el token " + strTokenAux;
                            await Task.Delay(2000);
                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);

                            string tipoToken = strTokenAux.Substring(0, 2);
                            string numToken = strTokenAux.Substring(2, 2);

                            if (tipoToken == "ID")
                            {
                                dgvTSIdentificadores.Rows.Add(numToken, Subcadena, "-", "-");
                                dgvTSIdentificadores.Select();
                                txtConsola.Text = "Se agregó el identificador con el token " + strTokenAux + " a la tabla de símbolos.";
                                await Task.Delay(2000);

                            }
                            else if (tipoToken == "CN" || tipoToken == "CR")
                            {
                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
                                dgvTSConstantesNumericas.Select();
                                txtConsola.Text = "Se agregó la constante numérica con el token " + strTokenAux + " a la tabla de símbolos.";
                                await Task.Delay(2000);
                            }


                            txtSubcadena.Text = "";
                        }
                    }
                    //Aquí pausa igual                    
                    txtTokens.Text += txtEvaluacion.Text + "\n";
                    txtTokens.BackColor = Color.Goldenrod;
                    txtConsola.Text = "Se ha llegado al final del renglón " + renglonActual;
                    txtTokens.Focus();
                    txtTokens.Select(intPosRenglonTokens, txtTokens.Lines[renglonActual].Length);
                    await Task.Delay(2000);
                    txtTokens.BackColor = Color.FromArgb(255, 240, 240, 240);

                    txtEvaluacion.Text = "";

                    intPosRenglon += (txtCodigo.Lines[renglonActual].Length + 1);
                    intPosRenglonTokens += (txtTokens.Lines[renglonActual].Length + 1);
                    renglonActual++;
                    
                }
                txtConsola.Text = "Ha finalizado el análisis léxico.";
                this.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConsola.Text = "Ocurrió un error inesperado.";                
            }
        }

        public string AnalizadorLexico(Instrucción otraInstruccion, int Pos, string Estado)
        {
            string strConsulta = "";

            if (Pos <= otraInstruccion.Cadena.Length - 1)
            {
                if (Convert.ToInt32(otraInstruccion.Cadena[Pos]) >= 97 && Convert.ToInt32(otraInstruccion.Cadena[Pos]) <= 122)
                {
                    strConsulta = CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "m] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return AnalizadorLexico(otraInstruccion, Pos + 1, strConsulta);
                    else
                        return "ERRL";
                }
                if (Convert.ToInt32(otraInstruccion.Cadena[Pos]) == 92 && otraInstruccion.Cadena[Pos + 1] == 'n')
                {
                    strConsulta = CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "n] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return AnalizadorLexico(otraInstruccion, Pos + 2, strConsulta);
                    else
                        return "ERRL";
                }
                else
                {
                    strConsulta = CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return AnalizadorLexico(otraInstruccion, Pos + 1, strConsulta);
                    else
                        return "ERRL";
                }

            }
            else
            {
                if (EstadoFinal("select * from Lexico where Estado = " + Estado))
                {
                    otraInstruccion.Token = CargarQuery("select A_Token from Lexico where Estado = " + Estado);

                    //Esto es para hacer el conteo de Identificadores y Constantes Numericas (enteras y reales)
                    switch (otraInstruccion.Token)
                    {
                        case "IDXX":
                            IDXX++;
                            otraInstruccion.Token = (IDXX < 10) ? "ID0" + IDXX : "ID" + IDXX;
                            break;
                        case "CRXX":
                            CRXX++;
                            otraInstruccion.Token = (CRXX < 10) ? "CR0" + CRXX : "CR" + CRXX;
                            break;
                        case "CNXX":
                            otraInstruccion.Token = (CNXX < 10) ? "CN0" + CNXX : "CN" + CNXX;
                            break;
                    }

                    lstInstrucciones.Add(otraInstruccion);
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
            return otraInstruccion.Token;
        }

        public string CargarQuery(string Query)
        {
            string Consulta = "";
            try
            {
                tabla = new DataTable();
                Conexion = new SqlConnection("Server=" + nombreServidor + ";Database=" + nombreBD + ";Trusted_Connection=True;");
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

        public void ObtenerSubcadenas()
        {
            string strAux;
            string strSubcadena = "";
            lstRenglones = new List<List<string>>();
            foreach (string Cad in txtCodigo.Lines)
            {
                strAux = Cad;
                lstSubcadenas = new List<string>();
                
                for (int i = 0; i <= strAux.Length - 1; i++)
                {

                    //Esto es para que pueda cadenas o comentarios
                    if (strAux[i] == '<' && (strAux[i + 1] == '"' || strAux[i + 1] == '/'))
                    {

                        do
                        {
                            strSubcadena += strAux[i];
                            i++;
                        } while ((strAux[i] != '"' || strAux[i] != '/') && strAux[i + 1] != '>');
                        strSubcadena += strAux[i] + ">";
                        lstSubcadenas.Add(strSubcadena);
                        strSubcadena = "";
                        i++;
                    }
                    else if (strAux[i] == ' ')
                    {
                        continue;
                    }
                    else
                    {
                        if (strAux[i] != ' ')
                        {
                            strSubcadena += strAux[i];
                            if (i == strAux.Length - 1 || strAux[i + 1] == ' ')
                            {
                                lstSubcadenas.Add(strSubcadena);
                                strSubcadena = "";
                            }
                        }
                            
                        else
                        {
                            lstSubcadenas.Add(strSubcadena);
                            strSubcadena = "";
                        }
                    }

                }
                lstRenglones.Add(lstSubcadenas);
            }
        }
        #endregion
    }

}
