using System;
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

        Color colorResaltado = Color.FromArgb(226, 130, 27);
        TaskCompletionSource<object> teclaEnter = new TaskCompletionSource<object>();

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


        #region AnalisisLéxico
        // Comienza el proceso del análisis lexico, utilizando el texto que se halle en la caja.
        private async void btnComenzar_Click(object sender, EventArgs e)
        {
            boolean resultado_léxico, resultado_sintaxis;            
            resultado_léxico = AnalisisLexicoPausado();
            if(resultado_léxico) { AnalisisSintactico(); } else { return; }            

        }

        private async boolean AnalisisLexicoPausado()
        {
            txtCodigo.ReadOnly = true;
            btnSiguientePaso.Enabled = true;

            try
            {
                dgvTSIdentificadores.Rows.Clear();
                dgvTSConstantesNumericas.Rows.Clear();
                IDXX = 0;
                CNXX = 0;
                CRXX = 0;

                txtConsola.Text = "Comenzando proceso.";
                txtTokens.Text = "";
                txtEvaluacion.Text = "";
                string strTokenAux;                
                ObtenerSubcadenas();
                int renglonActual = 0;
                int intPosRenglon = 0;
                int intPosRenglonTokens = 0;
                txtRenglones.Text = txtCodigo.Lines.Count().ToString();
                txtRenglonActual.Text = renglonActual.ToString();

                foreach (List<string> Subcadenas in lstRenglones)
                {
                    txtCodigo.BackColor = colorResaltado;
                    txtRenglonActual.Text = (renglonActual + 1).ToString();
                    txtConsola.Text = "Leyendo el renglón " + (renglonActual + 1);                    
                    txtCodigo.Focus();
                    txtCodigo.Select(intPosRenglon, txtCodigo.Lines[renglonActual].Length);
                    //await Task.Delay(2000);
                    this.Focus();
                    await teclaEnter.Task;                    
                    teclaEnter = new TaskCompletionSource<object>();
                    //await Task.Run(() =>
                    //                    {
                    //                        void frmCodext_KeyDowna(object senderinterno, KeyEventArgs einterno)
                    //                        {
                    //                            if (einterno.KeyCode == Keys.Enter)
                    //                                return;
                    //                        }
                    //                    });
                    txtCodigo.BackColor = Color.FromArgb(255, 240, 240, 240);
                    foreach (string Subcadena in Subcadenas)
                    {
                        //Aquí debería pausar xd creo
                        txtSubcadena.Text = Subcadena;
                        txtSubcadena.BackColor = colorResaltado;
                        txtConsola.Text = "Leyendo la subcadena " + Subcadena;
                        //await Task.Delay(2000);
                        this.Focus();
                        await teclaEnter.Task;
                        teclaEnter = new TaskCompletionSource<object>();
                        txtSubcadena.BackColor = Color.FromArgb(255, 240, 240, 240);

                        miInstruccion = new Instrucción();
                        miInstruccion.Cadena = Subcadena;


                        string tsres;

                        if (Subcadena.Contains("_"))
                        {
                            tsres = VerificarTablasDeSimbolos("ID");

                            if (tsres != "")
                            {
                                strTokenAux = tsres;
                                dgvTSIdentificadores.Select();
                                txtEvaluacion.Text += strTokenAux + " ";
                                txtEvaluacion.BackColor = colorResaltado;
                                txtConsola.Text = "Se encontró el identificador en la tabla de símbolos, su token es " + strTokenAux;
                                //await Task.Delay(2000);
                                this.Focus();
                                await teclaEnter.Task;
                                teclaEnter = new TaskCompletionSource<object>();
                                txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                                break;
                            }
                        }

                        if (Subcadena.Contains("#"))
                        {
                            if (Subcadena.Contains(".") || Subcadena.Contains("E"))
                            {
                                tsres = VerificarTablasDeSimbolos("CR");
                                if (tsres != "")
                                {
                                    strTokenAux = tsres;
                                    dgvTSConstantesNumericas.Select();
                                    txtEvaluacion.Text += strTokenAux + " ";
                                    txtEvaluacion.BackColor = colorResaltado;
                                    txtConsola.Text = "Se encontró la constante numérica real en la tabla de símbolos, su token es " + strTokenAux;
                                    //await Task.Delay(2000);
                                    this.Focus();
                                    await teclaEnter.Task;
                                    teclaEnter = new TaskCompletionSource<object>();
                                    txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                                    break;
                                }
                            }
                            else
                            {
                                tsres = VerificarTablasDeSimbolos("CN");
                                if (tsres != "")
                                {
                                    strTokenAux = tsres;
                                    dgvTSConstantesNumericas.Select();
                                    txtEvaluacion.Text += strTokenAux + " ";
                                    txtEvaluacion.BackColor = colorResaltado;
                                    txtConsola.Text = "Se encontró la constante numérica entera en la tabla de símbolos, su token es " + strTokenAux;
                                    //await Task.Delay(2000);
                                    this.Focus();
                                    await teclaEnter.Task;
                                    teclaEnter = new TaskCompletionSource<object>();
                                    txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                                    break;
                                }
                            }
                            
                        }

                        strTokenAux = EncontrarToken(miInstruccion, 0, "0");

                        if (strTokenAux == "ERRL")
                        {
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = colorResaltado;
                            txtConsola.Text = "Error detectado: no se reconoció el elemento. Se asignará un token de error léxico (ERRL)";
                            //await Task.Delay(2000);
                            this.Focus();
                            await teclaEnter.Task;
                            teclaEnter = new TaskCompletionSource<object>();
                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                        }
                        else
                        {
                            //Aquí también  
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = colorResaltado;
                            txtConsola.Text = "Se identifico la subcadena " + Subcadena + " con el token " + strTokenAux;
                            //await Task.Delay(2000);
                            this.Focus();
                            await teclaEnter.Task;
                            teclaEnter = new TaskCompletionSource<object>();
                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);

                            string tipoToken = strTokenAux.Substring(0, 2);
                            string numToken = strTokenAux.Substring(2, 2);                            

                            if (tipoToken == "ID")
                            {                                
                                    dgvTSIdentificadores.Rows.Add(numToken, Subcadena, "-", "-");
                                    dgvTSIdentificadores.Select();
                                    txtConsola.Text = "Se agregó el identificador con el token " + strTokenAux + " a la tabla de símbolos.";
                                    //await Task.Delay(2000);
                                    this.Focus();
                                    await teclaEnter.Task;
                                    teclaEnter = new TaskCompletionSource<object>();
                            }
                            else if (tipoToken == "CN")
                            {                                
                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
                                dgvTSConstantesNumericas.Select();
                                txtConsola.Text = "Se agregó la constante numérica entera con el token " + strTokenAux + " a la tabla de símbolos.";
                                //await Task.Delay(2000);
                                this.Focus();
                                await teclaEnter.Task;
                                teclaEnter = new TaskCompletionSource<object>();
                            }
                            else if (tipoToken == "CR")
                            {
                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
                                dgvTSConstantesNumericas.Select();
                                txtConsola.Text = "Se agregó la constante numérica real con el token " + strTokenAux + " a la tabla de símbolos.";
                                //await Task.Delay(2000);
                                this.Focus();
                                await teclaEnter.Task;
                                teclaEnter = new TaskCompletionSource<object>();
                            }


                            txtSubcadena.Text = "";
                        }
                    }
                    //Aquí pausa igual                    
                    txtTokens.Text += txtEvaluacion.Text + "\n";
                    txtTokens.BackColor = colorResaltado;
                    txtConsola.Text = "Se ha llegado al final del renglón " + (renglonActual + 1);
                    txtTokens.Focus();
                    txtTokens.Select(intPosRenglonTokens, txtTokens.Lines[renglonActual].Length);
                    //await Task.Delay(2000);
                    this.Focus();
                    await teclaEnter.Task;
                    teclaEnter = new TaskCompletionSource<object>();
                    txtTokens.BackColor = Color.FromArgb(255, 240, 240, 240);

                    txtEvaluacion.Text = "";

                    intPosRenglon += (txtCodigo.Lines[renglonActual].Length + 1);
                    intPosRenglonTokens += (txtTokens.Lines[renglonActual].Length + 1);
                    renglonActual++;
                    
                }
                txtConsola.Text = "Ha finalizado el análisis léxico.";                
                this.Focus();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConsola.Text = "Ocurrió un error inesperado.";
                return false;                
            }
            txtCodigo.ReadOnly = false;
            btnSiguientePaso.Enabled = false;
        }

        private boolean AnalisisLexico()
        {
            txtCodigo.ReadOnly = true;

            try
            {
                dgvTSIdentificadores.Rows.Clear();
                dgvTSConstantesNumericas.Rows.Clear();
                IDXX = 0;
                CNXX = 0;
                CRXX = 0;

                txtConsola.Text = "Comenzando proceso.";
                txtTokens.Text = "";
                txtEvaluacion.Text = "";
                string strTokenAux;                
                ObtenerSubcadenas();
                int renglonActual = 0;
                int intPosRenglon = 0;
                int intPosRenglonTokens = 0;
                txtRenglones.Text = txtCodigo.Lines.Count().ToString();
                txtRenglonActual.Text = renglonActual.ToString();

                foreach (List<string> Subcadenas in lstRenglones)
                {
                    txtCodigo.BackColor = colorResaltado;
                    txtRenglonActual.Text = (renglonActual + 1).ToString();
                    txtConsola.Text = "Leyendo el renglón " + (renglonActual + 1);                    
                    txtCodigo.Focus();
                    txtCodigo.Select(intPosRenglon, txtCodigo.Lines[renglonActual].Length);
                                        
                    txtCodigo.BackColor = Color.FromArgb(255, 240, 240, 240);
                    foreach (string Subcadena in Subcadenas)
                    {
                        txtSubcadena.Text = Subcadena;
                        txtSubcadena.BackColor = colorResaltado;
                        txtConsola.Text = "Leyendo la subcadena " + Subcadena;
                        txtSubcadena.BackColor = Color.FromArgb(255, 240, 240, 240);

                        miInstruccion = new Instrucción();
                        miInstruccion.Cadena = Subcadena;


                        string tsres;

                        if (Subcadena.Contains("_"))
                        {
                            tsres = VerificarTablasDeSimbolos("ID");

                            if (tsres != "")
                            {
                                strTokenAux = tsres;
                                dgvTSIdentificadores.Select();
                                txtEvaluacion.Text += strTokenAux + " ";
                                txtEvaluacion.BackColor = colorResaltado;
                                txtConsola.Text = "Se encontró el identificador en la tabla de símbolos, su token es " + strTokenAux;
                                txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                                break;
                            }
                        }

                        if (Subcadena.Contains("#"))
                        {
                            if (Subcadena.Contains(".") || Subcadena.Contains("E"))
                            {
                                tsres = VerificarTablasDeSimbolos("CR");
                                if (tsres != "")
                                {
                                    strTokenAux = tsres;
                                    dgvTSConstantesNumericas.Select();
                                    txtEvaluacion.Text += strTokenAux + " ";
                                    txtEvaluacion.BackColor = colorResaltado;
                                    txtConsola.Text = "Se encontró la constante numérica real en la tabla de símbolos, su token es " + strTokenAux;         
                                    txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                                    break;
                                }
                            }
                            else
                            {
                                tsres = VerificarTablasDeSimbolos("CN");
                                if (tsres != "")
                                {
                                    strTokenAux = tsres;
                                    dgvTSConstantesNumericas.Select();
                                    txtEvaluacion.Text += strTokenAux + " ";
                                    txtEvaluacion.BackColor = colorResaltado;
                                    txtConsola.Text = "Se encontró la constante numérica entera en la tabla de símbolos, su token es " + strTokenAux;  
                                    txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                                    break;
                                }
                            }
                            
                        }

                        strTokenAux = EncontrarToken(miInstruccion, 0, "0");

                        if (strTokenAux == "ERRL")
                        {
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = colorResaltado;
                            txtConsola.Text = "Error detectado: no se reconoció el elemento. Se asignará un token de error léxico (ERRL)"; 
                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
                        }
                        else
                        {                            
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = colorResaltado;
                            txtConsola.Text = "Se identifico la subcadena " + Subcadena + " con el token " + strTokenAux;                           
                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);

                            string tipoToken = strTokenAux.Substring(0, 2);
                            string numToken = strTokenAux.Substring(2, 2);                            

                            if (tipoToken == "ID")
                            {                                
                                    dgvTSIdentificadores.Rows.Add(numToken, Subcadena, "-", "-");
                                    dgvTSIdentificadores.Select();
                                    txtConsola.Text = "Se agregó el identificador con el token " + strTokenAux + " a la tabla de símbolos.";
                            }
                            else if (tipoToken == "CN")
                            {                                
                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
                                dgvTSConstantesNumericas.Select();
                                txtConsola.Text = "Se agregó la constante numérica entera con el token " + strTokenAux + " a la tabla de símbolos.";
                            }
                            else if (tipoToken == "CR")
                            {
                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
                                dgvTSConstantesNumericas.Select();
                                txtConsola.Text = "Se agregó la constante numérica real con el token " + strTokenAux + " a la tabla de símbolos.";
                            }


                            txtSubcadena.Text = "";
                        }
                    }              
                    txtTokens.Text += txtEvaluacion.Text + "\n";
                    txtTokens.BackColor = colorResaltado;
                    txtConsola.Text = "Se ha llegado al final del renglón " + (renglonActual + 1);
                    txtTokens.Focus();
                    txtTokens.Select(intPosRenglonTokens, txtTokens.Lines[renglonActual].Length);
                    txtTokens.BackColor = Color.FromArgb(255, 240, 240, 240);

                    txtEvaluacion.Text = "";

                    intPosRenglon += (txtCodigo.Lines[renglonActual].Length + 1);
                    intPosRenglonTokens += (txtTokens.Lines[renglonActual].Length + 1);
                    renglonActual++;
                    
                }
                txtConsola.Text = "Ha finalizado el análisis léxico.";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConsola.Text = "Ocurrió un error inesperado.";  
                return false;              
            }
            txtCodigo.ReadOnly = false;
        }

        public string EncontrarToken(Instrucción otraInstruccion, int Pos, string Estado)
        {
            string strConsulta = "";

            if (Pos <= otraInstruccion.Cadena.Length - 1)
            {
                if (Convert.ToInt32(otraInstruccion.Cadena[Pos]) >= 97 && Convert.ToInt32(otraInstruccion.Cadena[Pos]) <= 122)
                {
                    strConsulta = CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "m] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 1, strConsulta);
                    else
                        return "ERRL";
                }
                if (Convert.ToInt32(otraInstruccion.Cadena[Pos]) == 92 && otraInstruccion.Cadena[Pos + 1] == 'n')
                {
                    strConsulta = CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "n] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 2, strConsulta);
                    else
                        return "ERRL";
                }
                else
                {
                    strConsulta = CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 1, strConsulta);
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
                            CNXX++;
                            otraInstruccion.Token = (CNXX < 10) ? "CN0" + CNXX : "CN" + CNXX;
                            break;
                    }

                    lstInstrucciones.Add(otraInstruccion);
                }
                else
                {
                    strConsulta = CargarQuery("select A_del from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 1, strConsulta);
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

        private void btnSiguientePaso_Click(object sender, EventArgs e)
        {
            teclaEnter.TrySetResult(null);
        }

        public string VerificarTablasDeSimbolos(string strToken)
        {            
            if (strToken == "ID" && dgvTSIdentificadores.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvTSIdentificadores.Rows)
                {
                    if (r.Cells[1].Value.ToString() == txtSubcadena.Text)
                    {
                        return "ID" + r.Cells[0].Value.ToString();
                    }
                }
                return "";
            }
            else if ((strToken == "CN") && dgvTSConstantesNumericas.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvTSConstantesNumericas.Rows)
                {
                    if ((r.Cells[1].Value.ToString() + "#") == txtSubcadena.Text)
                    {
                        return "CN" + r.Cells[0].Value.ToString();
                    }
                }
                return "";
            }
            else if ((strToken == "CR") && dgvTSConstantesNumericas.Rows.Count > 0)
            {
                foreach (DataGridViewRow r in dgvTSConstantesNumericas.Rows)
                {
                    if ((r.Cells[1].Value.ToString() + "#") == txtSubcadena.Text)
                    {
                        return "CR" + r.Cells[0].Value.ToString();
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region AnalisisSintáctico
        public async boolean AnalisisSintacticoPausado()
        {
            
        }

        public boolean AnalisisSintactico()
        {

        }

        #endregion

        void frmCodext_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            teclaEnter.TrySetResult(null);
        }
    }

}
