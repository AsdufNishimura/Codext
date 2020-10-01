using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Codext
{
    public partial class frmCodext : Form
    {   
        /**
         *  Propiedades y métodos propios del Form principal. 
         */
        Instrucción miInstruccion;
        List<Instrucción> lstInstrucciones = new List<Instrucción>();
        List<string> lstSubcadenas;
        List<List<string>> lstRenglones;
        int IDXX = 0, CNXX = 0, CRXX = 0;        

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



        /**
         *  Eventos de los controles visuales.
         */
        #region Controles
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
        
        private void btnComenzar_Click(object sender, EventArgs e)
        {
            txtGramatica.Text = "";
            txtSintaxis.Text = "";
            txtReglasSem.Text = "";
            txtSemantica.Text = "";
            bool resultado_léxico, resultado_sintaxis;
            AnalisisLexico();
            //btnSiguientePaso.Enabled = true;
            AnalisisSintactico();
            //if(resultado_léxico) { AnalisisSintacticoPausado(); } else { return; } 
            AnalisisSemantico();
            CodigoIntermedio();

            //  Compilador

        }

        private void btnSiguientePaso_Click(object sender, EventArgs e)
        {
            teclaEnter.TrySetResult(null);
        }
        #endregion



        /**  
         *  Estos métodos son utilizados por más de uno de los módulos.
         */
        #region MétodosComunes

        public string BottomUp(string LineaCodigo, int TokensTotales, int TokensActuales, string Tabla)
        {
            int VecesIterar = TokensTotales - TokensActuales;
            int CaracteresAEvaluar = LineaCodigo.Length - 5 * (TokensTotales - TokensActuales);
            string SubCadenaEvaluar, CadenaAux = "";

            if (LineaCodigo != "S" && LineaCodigo != "ERROR DE SINTAXIS" && LineaCodigo != "ERROR DE SEMANTICA")
            {

                for (int i = 0; i <= VecesIterar; i++)
                {
                    if (TokensActuales > 1)
                    {
                        SubCadenaEvaluar = ObtenerSubcadenaEvaluar(LineaCodigo, CaracteresAEvaluar, i * 5);
                        CadenaAux = Database.CargarQuery("select Produccion from " + Tabla + " where Definicion = '" + SubCadenaEvaluar + "' and CantidadTokens = " + TokensActuales);
                        if (CadenaAux != "")
                        {
                            if (CadenaAux != "S")
                            {
                                string NuevaLinea = CambiarLineaCodigo(LineaCodigo, i * 5, (i * 5) + SubCadenaEvaluar.Length - 1, CadenaAux);
                                return NuevaLinea;
                            }
                            else
                            {
                                return "S";
                            }
                        }
                        else
                        {
                            if (i < VecesIterar)
                                continue;
                            else
                                return BottomUp(LineaCodigo, TokensTotales, TokensActuales - 1, Tabla);
                        }
                    }
                    else
                    {

                        SubCadenaEvaluar = ObtenerSubcadenaEvaluar(LineaCodigo, CaracteresAEvaluar, i * 5);
                        if (SubCadenaEvaluar.Contains("CN") && SubCadenaEvaluar != "CNXX")
                            SubCadenaEvaluar = "CNXX";
                        if (SubCadenaEvaluar.Contains("CR") && SubCadenaEvaluar != "CRXX")
                            SubCadenaEvaluar = "CRXX";
                        if (SubCadenaEvaluar.Contains("ID") && SubCadenaEvaluar != "IDXX")
                        {
                            SubCadenaEvaluar = "IDXX";
                            string NuevaLinea = CambiarLineaCodigo(LineaCodigo, i * 5, (i * 5) + SubCadenaEvaluar.Length - 1, SubCadenaEvaluar);
                            return NuevaLinea;
                        }

                        if (SubCadenaEvaluar == "ERRL")
                            return (Tabla == "Sintaxis" ? "ERROR DE SINTAXIS" : "ERROR DE SEMANTICA");

                        CadenaAux = Database.CargarQuery("select Produccion from " + Tabla + " where Definicion LIKE '%" + SubCadenaEvaluar + "%' and CantidadTokens = 1");
                        if (CadenaAux != "")
                        {
                            if (CadenaAux != "S")
                            {
                                string NuevaLinea = CambiarLineaCodigo(LineaCodigo, i * 5, (i * 5) + SubCadenaEvaluar.Length - 1, CadenaAux);
                                return NuevaLinea;
                            }
                            else
                            {
                                return "S";
                            }
                        }
                        else
                        {
                            if (i < VecesIterar)
                                continue;
                            else
                                return (Tabla == "Sintaxis" ? "ERROR DE SINTAXIS" : "ERROR DE SEMANTICA");
                        }
                    }
                }
                return "S";
            }
            else
                return "";
        }

        public int ObtenerCantidadTokens(string LineaTokens)
        {
            int Cantidad = 1;

            foreach (char C in LineaTokens)
            {

                if (C == ' ')
                    Cantidad++;
            }
            return Cantidad;
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

        public string ObtenerSubcadenaEvaluar(string Cadena, int CarAEva, int Posicion)
        {
            string Subcadena = "";
            for (int i = 0; i <= CarAEva - 1; i++)
            {
                Subcadena = Subcadena + Cadena[Posicion];
                Posicion++;
            }
            return Subcadena;

        }

        public string CambiarLineaCodigo(string Cadena, int Desde, int Hasta, string Cambio)
        {
            string NuevaCadena = "";
            bool Bandera = true;

            for (int i = 0; i <= Cadena.Length - 1; i++)
            {
                if (i >= Desde && i <= Hasta)
                {
                    if (Bandera)
                    {
                        NuevaCadena = NuevaCadena + Cambio;
                        Bandera = false;
                    }
                }
                else
                {
                    NuevaCadena = NuevaCadena + Cadena[i];
                }
            }
            return NuevaCadena;
        }

        void frmCodext_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                teclaEnter.TrySetResult(null);
        }


        #endregion



        /**
         *  Fase de análisis léxio 
         */
        #region AnalisisLéxico


        private bool AnalisisLexico()
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
                                continue;
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
                                    continue;
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
                                    continue;
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
        }

        public string EncontrarToken(Instrucción otraInstruccion, int Pos, string Estado)
        {
            string strConsulta = "";

            if (Pos <= otraInstruccion.Cadena.Length - 1)
            {
                if (Convert.ToInt32(otraInstruccion.Cadena[Pos]) >= 97 && Convert.ToInt32(otraInstruccion.Cadena[Pos]) <= 122)
                {
                    strConsulta = Database.CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "m] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 1, strConsulta);
                    else
                        return "ERRL";
                }
                if (Convert.ToInt32(otraInstruccion.Cadena[Pos]) == 92 && otraInstruccion.Cadena[Pos + 1] == 'n')
                {
                    strConsulta = Database.CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "n] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 2, strConsulta);
                    else
                        return "ERRL";
                }
                else
                {
                    strConsulta = Database.CargarQuery("select [A_" + otraInstruccion.Cadena[Pos] + "] from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 1, strConsulta);
                    else
                        return "ERRL";
                }

            }
            else
            {
                if (Database.EstadoFinal("select * from Lexico where Estado = " + Estado))
                {
                    otraInstruccion.Token = Database.CargarQuery("select A_Token from Lexico where Estado = " + Estado);

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
                    strConsulta = Database.CargarQuery("select A_del from Lexico where Estado = " + Estado);
                    if (strConsulta != "")
                        return EncontrarToken(otraInstruccion, Pos + 1, strConsulta);
                    else
                        throw new Exception("Instrucción no válida");

                }
            }
            return otraInstruccion.Token;
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



        /**
         *  Fase de analisis sintactico 
         */
        #region AnalisisSintáctico
        
        public void AnalisisSintactico()
        {
            int CantidadTokens = 1;
            string Linea = "";

            foreach(string LineaTokens in txtTokens.Lines)
            {
                if (LineaTokens != "")
                {
                    if (LineaTokens[LineaTokens.Length - 1] == ' ')
                    {
                        for (int i = 0; i < LineaTokens.Length - 1; i++)
                            Linea += LineaTokens[i];
                    }

                }
                if(Linea != "")
                {
                    txtGramatica.Text += Linea + "\n";
                    do
                    {
                        Linea = BottomUp(Linea, ObtenerCantidadTokens(Linea), ObtenerCantidadTokens(Linea), "Sintaxis");
                        txtGramatica.Text += Linea + "\n";
                    } while (Linea != "S" && Linea != "ERROR DE SINTAXIS");
                    txtGramatica.Text += "\n";
                    txtSintaxis.Text += Linea + "\n";
                }


                CantidadTokens = 1;
                Linea = "";
            }
        }

        #endregion



        /**
         *  Fase de analisis semantico 
         */
        #region AnalisisSemántico
        public void AnalisisSemantico()
        {
            try
            {
                //  Primera pasada
                PrimeraPasada();

                //   Segunda pasada
                int CantidadTokens = 1;
                string Linea = "";

                foreach (string LineaTokens in txtTipos.Lines)
                {
                    if (LineaTokens != "")
                    {
                        if (LineaTokens[LineaTokens.Length - 1] == ' ')
                        {
                            for (int i = 0; i < LineaTokens.Length - 1; i++)
                                Linea += LineaTokens[i];
                        }

                    }
                    if (Linea != "")
                    {
                        txtReglasSem.Text += Linea + "\n";
                        do
                        {
                            Linea = BottomUp(Linea, ObtenerCantidadTokens(Linea), ObtenerCantidadTokens(Linea), "Semantica");
                            txtReglasSem.Text += Linea + "\n";
                        } while (Linea != "S" && Linea != "ERROR DE SEMANTICA");
                        txtReglasSem.Text += "\n";
                        txtSemantica.Text += Linea + "\n";
                    }


                    CantidadTokens = 1;
                    Linea = "";
                }

                //  Tercera pasada
                TerceraPasada();
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void PrimeraPasada()
        {
            txtTipos.Text = txtTokens.Text;
            Regex.Replace(txtTipos.Text, "COND", "BOOL");
            txtTipos.Text = Regex.Replace(txtTipos.Text, "PR16", "ENTE");
            txtTipos.Text = Regex.Replace(txtTipos.Text, @"CN[0-9]{2}", "ENTE");
            txtTipos.Text = Regex.Replace(txtTipos.Text, @"CR[0-9]{2}", "REAL");
            ComprobarTablaDeTipos();
        }

        public void ComprobarTablaDeTipos()
        {
            /*  dgvTSIdentificadores
             *  [0] ->  # Identificador
             *  [1] ->  Nombre
             *  [2] ->  Tipo de dato");
             *  [3] ->  Valor 
             */
            string strIden = "";
            string strValor = "";
            bool boolIden = false;
            bool boolValor = false;



            foreach (List<string> linea in lstRenglones)
            {
                if (linea.Contains("VALUE"))
                {
                    if (linea.ElementAt(linea.IndexOf("VALUE") + 3) == ",")
                    {
                        strIden = linea.ElementAt(linea.IndexOf("VALUE") + 2);
                        strValor = linea.ElementAt(linea.IndexOf("VALUE") + 4);
                        foreach (DataGridViewRow row in dgvTSIdentificadores.Rows)
                        {
                            if (row.Cells[1].Value.ToString() == strIden)
                            {
                                row.Cells[3].Value = strValor;
                                if (strValor.Contains("#"))
                                {
                                    if (strValor.Contains("."))
                                    {
                                        row.Cells[2].Value = "REAL";
                                    }
                                    else
                                    {
                                        row.Cells[2].Value = "ENTE";
                                    }
                                }
                                if (strValor.Contains("<"))
                                {
                                    row.Cells[2].Value = "CADE";
                                }
                                if (strValor.Contains("LOGIC") || strValor.Contains("true") || strValor.Contains("false"))
                                {
                                    row.Cells[2].Value = "BOOL";
                                }
                            }
                        }
                    }
                }
            }
            

            foreach (DataGridViewRow row in dgvTSIdentificadores.Rows)
            {
                string strTipoToken = "NULL";
                if(row.Cells[2].Value.ToString() != "-")
                {
                    strTipoToken = row.Cells[2].Value.ToString();
                }
                txtTipos.Text = Regex.Replace(txtTipos.Text, "ID" + row.Cells[0].Value.ToString(), strTipoToken);
            }
        }

        public void TerceraPasada()
        {
            //  Aquí uwu
            //  Revisar los contadores de las operaciones compuestas
            //  El for -no cerro = perfecto + hay dos instrucciones fulanas que se abrieron y no se cerraron
            //  txtConsola.Text
            int cntInicioPrograma = Regex.Matches(txtTipos.Text, "PR01").Count;
            int cntFinPrograma = Regex.Matches(txtTipos.Text, "PR02").Count;
            int cntApertura = Regex.Matches(txtTipos.Text, "PR03").Count;
            int cntCierre = Regex.Matches(txtTipos.Text, "PR04").Count;
            int cntParApertura = Regex.Matches(txtTipos.Text, "CE07").Count;
            int cntParCierre = Regex.Matches(txtTipos.Text, "CE08").Count;
            bool bandera = false;

            switch (cntApertura - cntCierre)
            {
                case 1:
                    txtConsola.Text = "ERROR: Hay un bloque de código abierto que no está cerrando.";
                    bandera = true;
                    break;
                case -1:
                    txtConsola.Text = "ERROR: Hay cierres de bloque de sobra. Se está cerrando un bloque antes de tiempo";
                    bandera = true;
                    break;
            }
            switch (cntParApertura - cntParCierre)
            {
                case 1:
                    txtConsola.Text = "ERROR: Hay un parentesis abierto que no está cerrando.";
                    bandera = true;
                    break;
                case -1:
                    txtConsola.Text = "ERROR: Hay parentesis de sobra. Se está cerrando un parentesis antes de tiempo";
                    bandera = true;
                    break;
            }
            switch (cntInicioPrograma)
            {
                case 0:
                    txtConsola.Text += "\nADVERTENCIA: No se encontró la instrucción de inicio.";
                    break;
                case 1:
                    break;
                default:
                    txtConsola.Text += "\nADVERTENCIA: La instrucción de inicio solo puede usarse una vez al inicio del programa.";
                    break;
            }
            switch (cntFinPrograma)
            {
                case 0:
                    txtConsola.Text += "\nADVERTENCIA: No se encontró la instrucción de fin.";
                    break;
                case 1:
                    break;
                default:
                    txtConsola.Text += "\nADVERTENCIA: La instrucción de fin solo puede usarse una vez al final del programa.";
                    break;
            }


            if (bandera) { throw new Exception("ERROR DE SEMÁNTICA"); }

        }

        #endregion



        /**
         *  Fase de código intermedio
         */
        #region CodigoIntermedio

        public void CodigoIntermedio()
        {
            //  Conversión a ordenación postfija de las expresiones aritmeticas, lógicas y relacionales          
            txtPostFijo.Clear();
            for (int i = 0; i < txtTokens.Lines.Length; i++)
            {
                string s;
                int intInicioSublista = 0, intFinSublista, j = 0, intContadorParentesisApertura = 0, intContadorParentesisCierre = 0;
                bool banderaSublista = false;
                int intCantidadTokens = ObtenerCantidadTokens(txtTokens.Lines[i].Trim());

                if (!string.IsNullOrEmpty(txtTokens.Lines[i]))
                {
                    while (j < intCantidadTokens)
                    {
                        s = txtTokens.Lines[i].Substring((j * 4) + j, 4);
                        if (banderaSublista == false)
                        {
                            if (s == "PR08" || s == "PR13")
                            {
                                banderaSublista = true;
                                intInicioSublista = ((j + 1) * 4) + (j + 1);
                            }


                        }
                        if (s == "CE07" && banderaSublista == true)
                        {
                            intContadorParentesisApertura++;
                        }
                        if (s == "CE08" && banderaSublista == true)
                        {
                            intContadorParentesisCierre++;
                            if (intContadorParentesisApertura == intContadorParentesisCierre)
                            {
                                banderaSublista = false;
                                intFinSublista = (j * 4) + j + 4;
                                string strSubcadena = txtTokens.Lines[i].Substring(intInicioSublista, intFinSublista - intInicioSublista);
                                
                                txtPostFijo.Text += txtTokens.Lines[i].Substring(0, intInicioSublista) + ConvertirAOrdenacionPostfijo(strSubcadena) + txtTokens.Lines[i].Substring(intFinSublista) + "\n";/* strCadena.Replace(strSubcadena,*/
                                    
                            }
                        }
                        j++;
                    }
                }




                    
                    //if(txtPostFijo.Lines[i].Contains("PR08"))
                    //{
                    //    txtPostFijo.Text = Regex.Replace(txtPostFijo.Text, txtPostFijo.Lines[i], "PR08 " + ConvertirAOrdenacionPostfijo(txtPostFijo.Lines[i].Substring(5, txtPostFijo.Lines[i].Length - 6)));                    
                    //}
                    //if(txtPostFijo.Lines[i].Contains("PR13"))
                    //{
                    //    txtPostFijo.Text = Regex.Replace(txtPostFijo.Text, txtPostFijo.Lines[i], "PR13 " + ConvertirAOrdenacionPostfijo(txtPostFijo.Lines[i].Substring(5, txtPostFijo.Lines[i].Length - 6)));
                    //}
                }



        }

        public string ConvertirAOrdenacionPostfijo(string strCadena)
        {
            string strOperandos = "", strOperadores = "", s;
            int intInicioSublista = 0, intFinSublista, i = 0, intContadorParentesisApertura = 0, intContadorParentesisCierre = 0;
            bool banderaSublista = false;
            int intCantidadTokens = ObtenerCantidadTokens(strCadena.Trim());

            while(i < intCantidadTokens)
            {
                s = strCadena.Substring((i * 4) + i, 4);
                if(banderaSublista == false)
                {
                    if(s == "PR08" || s == "PR13")
                    {
                        banderaSublista = true;
                        intInicioSublista = ((i + 1) * 4) + (i + 1);
                    }
                    if(s == "OPAS" ||   /*  +  */
                       s == "OPAR" ||   /*  -  */
                       s == "OPAM" ||   /*  *  */
                       s == "OPAD" ||   /*  /  */
                       s == "OPAP" ||   /*  ** */
                       s == "OPAC" ||   /*  %  */
                       s == "OPRM" ||   /*  >  */
                       s == "OPRm" ||   /*  <  */
                       s == "OPRI" ||   /*  =  */
                       s == "OPRD" ||   /*  <> */
                       s == "ORMI" ||   /*  >= */
                       s == "ORmI" ||   /*  <= */
                       s == "OPLA" ||   /*  &  */
                       s == "OPLO" ||   /*  |  */
                       s == "OPLN"      /*  !  */)
                    {
                        strOperadores = s;
                    }
                    if(s.StartsWith("ID") ||
                       s.StartsWith("CN") ||
                       s.StartsWith("CR"))
                    {
                        if (strOperandos != "")
                        {
                            strOperandos += " ";
                        }                        
                        strOperandos += s;
                        
                    }
                    
                }
                if (s == "CE07" && banderaSublista == true)
                {
                    intContadorParentesisApertura++;
                }
                if (s == "CE08" && banderaSublista == true)
                {
                    intContadorParentesisCierre++;
                    if (intContadorParentesisApertura == intContadorParentesisCierre)
                    {
                        banderaSublista = false;
                        intFinSublista = (i * 4) + i + 4;
                        string strSubcadena = strCadena.Substring(intInicioSublista, intFinSublista - intInicioSublista);
                        if(strOperandos != "")
                        {
                            strOperandos += " ";
                        }
                        strOperandos += /* strCadena.Replace(strSubcadena,*/
                            "CE07 " + ConvertirAOrdenacionPostfijo(strSubcadena) + " CE08";
                    }
                }
                i++;
            }
            return strOperandos + " " + strOperadores;

        }

        #endregion



        /**
         *  Métodos no usados y/o no implementados.
         */
        #region Test

        //public async Task<string> BottomUpPausado(string LineaCodigo, int TokensTotales, int TokensActuales)
        //{
        //    txtConsola.Text = "Analizando la línea " + LineaCodigo + " de " + TokensTotales + " de longitud. Buscando producciones de " + TokensActuales + ".";
        //    await teclaEnter.Task;
        //    teclaEnter = new TaskCompletionSource<object>();

        //    int VecesIterar = TokensTotales - TokensActuales;
        //    int CaracteresAEvaluar = LineaCodigo.Length - 5 * (TokensTotales - TokensActuales);
        //    string SubCadenaEvaluar, CadenaAux = "";

        //    if (LineaCodigo != "S" && LineaCodigo != "ERROR DE SINTAXIS")
        //    {

        //        for (int i = 0; i <= VecesIterar; i++)
        //        {
        //            if (TokensActuales > 1)
        //            {
        //                SubCadenaEvaluar = ObtenerSubcadenaEvaluar(LineaCodigo, CaracteresAEvaluar, i * 5);
        //                CadenaAux = CargarQuery("select Produccion from Sintaxis where Definicion = '" + SubCadenaEvaluar + "' and CantidadTokens = " + TokensActuales);
        //                if (CadenaAux != "")
        //                {
        //                    if (CadenaAux != "S")
        //                    {
        //                        txtConsola.Text = "Se ha reducido la subcadena " + SubCadenaEvaluar + " a " + CadenaAux + " de acuerdo a la producción identificada. Continua el analisis sintáctico";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        string NuevaLinea = CambiarLineaCodigo(LineaCodigo, i * 5, (i * 5) + SubCadenaEvaluar.Length - 1, CadenaAux);
        //                        return NuevaLinea;
        //                    }
        //                    else
        //                    {
        //                        txtConsola.Text = "Se ha reducido la subadena " + SubCadenaEvaluar + " a S de acuerdo a la producción encontrada. La sintaxis es correcta.";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        return "S";
        //                    }
        //                }
        //                else
        //                {
        //                    if (i < VecesIterar)
        //                    {
        //                        txtConsola.Text = "Evaluando siguiente conjunto de tokens.";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        txtConsola.Text = "No se ha encontrado una producción válida en la gramática. Se va a reducir la cantidad de tokens seleccionados para buscar producciones más pequeñas..";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        await BottomUpPausado(LineaCodigo, TokensTotales, TokensActuales - 1);
        //                    }
        //                }
        //            }
        //            else
        //            {

        //                SubCadenaEvaluar = ObtenerSubcadenaEvaluar(LineaCodigo, CaracteresAEvaluar, i * 5);
        //                if (SubCadenaEvaluar.Contains("CN") && SubCadenaEvaluar != "CNXX")
        //                {
        //                    SubCadenaEvaluar = "CNXX";
        //                    txtConsola.Text = "La subcadena es una constante numérica entera.";
        //                    await teclaEnter.Task;
        //                    teclaEnter = new TaskCompletionSource<object>();
        //                }
        //                if (SubCadenaEvaluar.Contains("CR") && SubCadenaEvaluar != "CRXX")
        //                {
        //                    SubCadenaEvaluar = "CRXX";
        //                    txtConsola.Text = "La subcadena es una constante numérica real.";
        //                    await teclaEnter.Task;
        //                    teclaEnter = new TaskCompletionSource<object>();
        //                }
        //                if (SubCadenaEvaluar.Contains("ID") && SubCadenaEvaluar != "IDXX")
        //                {
        //                    SubCadenaEvaluar = "IDXX";
        //                    {
        //                        SubCadenaEvaluar = "IDXX";
        //                        txtConsola.Text = "La subcadena es un identificador.";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        string NuevaLinea = CambiarLineaCodigo(LineaCodigo, i * 5, (i * 5) + SubCadenaEvaluar.Length - 1, SubCadenaEvaluar);
        //                        return NuevaLinea;
        //                    }

        //                }
        //                if (SubCadenaEvaluar == "ERRL")
        //                {
        //                    txtConsola.Text = "No se ha encontrado una producción válida en la gramática. Esto es un error de sintaxis.";
        //                    await teclaEnter.Task;
        //                    teclaEnter = new TaskCompletionSource<object>();
        //                    return "ERROR DE SINTAXIS";
        //                }

        //                CadenaAux = CargarQuery("select Produccion from Sintaxis where Definicion LIKE '%" + SubCadenaEvaluar + "%' and CantidadTokens = 1");
        //                if (CadenaAux != "")
        //                {
        //                    if (CadenaAux != "S")
        //                    {
        //                        txtConsola.Text = "Se ha reducido la subcadena " + SubCadenaEvaluar + " a " + CadenaAux + " de acuerdo a la producción identificada. Continua el analisis sintáctico";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        string NuevaLinea = CambiarLineaCodigo(LineaCodigo, i * 5, (i * 5) + SubCadenaEvaluar.Length - 1, CadenaAux);
        //                        return NuevaLinea;
        //                    }
        //                    else
        //                    {
        //                        txtConsola.Text = "Se ha reducido la subadena " + SubCadenaEvaluar + " a S de acuerdo a la producción encontrada. La sintaxis es correcta.";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        return "S";
        //                    }
        //                }
        //                else
        //                {
        //                    if (i < VecesIterar)
        //                    {
        //                        txtConsola.Text = "Evaluando siguiente token.";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        txtConsola.Text = "No se ha encontrado una producción válida en la totalidad de la gramática. Esto es un error de sintaxis.";
        //                        await teclaEnter.Task;
        //                        teclaEnter = new TaskCompletionSource<object>();
        //                        return "ERROR DE SINTAXIS";
        //                    }
        //                }
        //            }
        //        }
        //        // Aquí tengo duda, no entiendo bien por qué esta esto aquí, por si acaso dejo el código de la pausa comentado
        //        //txtConsola.Text = "Esto se mostrará en la caja de consola, aquí debe ir la indicación del paso que se realizó, o por qué se hizo la pausa.";
        //        //await teclaEnter.Task;
        //        //teclaEnter = new TaskCompletionSource<object>();
        //        return "S";
        //    }
        //    else
        //    {
        //        // Ni tampoco entendí esto, igual que antes, dejo el código por si acaso
        //        //txtConsola.Text = "Esto se mostrará en la caja de consola, aquí debe ir la indicación del paso que se realizó, o por qué se hizo la pausa.";
        //        //await teclaEnter.Task;
        //        //teclaEnter = new TaskCompletionSource<object>();
        //        return "";
        //    }
        //}

        //public async void AnalisisSintacticoPausado()
        //{
        //    txtConsola.Text = "Comenzando análisis léxico";
        //    await teclaEnter.Task;
        //    teclaEnter = new TaskCompletionSource<object>();

        //    int CantidadTokens = 1;
        //    string Linea = "";
        //    int NumeroLinea = 1;

        //    foreach (string LineaTokens in txtTokens.Lines)
        //    {
        //        txtConsola.Text = "Leyendo la línea " + NumeroLinea + " del archivo de tokens.";
        //        await teclaEnter.Task;
        //        teclaEnter = new TaskCompletionSource<object>();

        //        if (LineaTokens != "")
        //        {
        //            if (LineaTokens[LineaTokens.Length - 1] == ' ')
        //            {
        //                for (int i = 0; i < LineaTokens.Length - 1; i++)
        //                    Linea += LineaTokens[i];
        //            }

        //        }

        //        if (Linea != "")
        //        {
        //            txtGramatica.Text += Linea + "\n";
        //            do
        //            {
        //                Linea = BottomUpPausado(Linea, ObtenerCantidadTokens(Linea), ObtenerCantidadTokens(Linea)).Result;
        //                //txtConsola.Text = "Leyendo la línea " + NumeroLinea + " del archivo de tokens.";
        //                //await teclaEnter.Task;
        //                //teclaEnter = new TaskCompletionSource<object>();
        //                txtGramatica.Text += Linea + "\n";
        //            } while (Linea != "S" && Linea != "ERROR DE SINTAXIS");
        //            txtGramatica.Text += "\n";
        //            txtSintaxis.Text += Linea + "\n";
        //            if (Linea == "S")
        //            {
        //                txtConsola.Text = "La sintaxis de esta línea es correcta.";
        //            }
        //            else
        //            {
        //                txtConsola.Text = "Esta línea contiene un error de sintaxis.";
        //            }

        //            await teclaEnter.Task;
        //            teclaEnter = new TaskCompletionSource<object>();

        //            txtConsola.Text = "Se terminó de analizar la linea " + NumeroLinea + " del archivo de tokens.";
        //            await teclaEnter.Task;
        //            teclaEnter = new TaskCompletionSource<object>();
        //        }


        //        CantidadTokens = 1;
        //        Linea = "";
        //        NumeroLinea++;
        //    }
        //}

        private async void AnalisisLexicoPausado()
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
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtConsola.Text = "Ocurrió un error inesperado.";
                return;
            }
            txtCodigo.ReadOnly = false;
            btnSiguientePaso.Enabled = false;
        }

        #endregion
    }
}
