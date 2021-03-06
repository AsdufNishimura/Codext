﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace Codext
{
    public partial class FrmCodext : Form
    {   
        /**
         *  Propiedades y métodos propios del Form principal. 
         */
        Instrucción miInstruccion;
        readonly List<Instrucción> lstInstrucciones = new List<Instrucción>();
        List<string> lstSubcadenas;
        List<List<string>> lstRenglones;
        int IDXX = 0, CNXX = 0, CRXX = 0;
        readonly Color colorResaltado = Color.FromArgb(226, 130, 27);
        readonly Color colorUnresaltado = Color.FromArgb(255, 240, 240, 240);
        TaskCompletionSource<object> teclaEnter = new TaskCompletionSource<object>();
        List<List<Tripleta>> tripletas = new List<List<Tripleta>>();
        int intContadorVariablesTripleta = 0;
        string usuario = "asduf";

        public FrmCodext()
        {
            InitializeComponent();
        }

        private void FrmCodext_Load(object sender, EventArgs e)
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

            dgvTripleta.Columns.Add("columnIndice","Índice");
            dgvTripleta.Columns.Add("columnDatoObjeto","Dato objeto");
            dgvTripleta.Columns.Add("columnDatoFuente","Dato fuente");
            dgvTripleta.Columns.Add("columnOperador","Operador");

            dgvTripletaV.Columns.Add("columnIndice", "Índice");
            dgvTripletaV.Columns.Add("columnDatoObjeto", "Dato objeto");
            dgvTripletaV.Columns.Add("columnDatoFuente", "Dato fuente");
            dgvTripletaV.Columns.Add("columnOperador", "Operador");

            dgvTripletaF.Columns.Add("columnIndice", "Índice");
            dgvTripletaF.Columns.Add("columnDatoObjeto", "Dato objeto");
            dgvTripletaF.Columns.Add("columnDatoFuente", "Dato fuente");
            dgvTripletaF.Columns.Add("columnOperador", "Operador");
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
                OpenFileDialog dialogEntrada = new OpenFileDialog
                {
                    Filter = ".txt files (*.txt)|*.txt"
                };
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
            //bool resultado_léxico, resultado_sintaxis;
            AnalisisLexico();
            //btnSiguientePaso.Enabled = true;
            AnalisisSintactico();
            //if(resultado_léxico) { AnalisisSintacticoPausado(); } else { return; } 
            AnalisisSemantico();
            CodigoIntermedio();
            CodigoFinal();

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
                Subcadena += Cadena[Posicion];
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
                        NuevaCadena += Cambio;
                        Bandera = false;
                    }
                }
                else
                {
                    NuevaCadena += Cadena[i];
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
         *  Fase de análisis léxico 
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

                    txtCodigo.BackColor = colorUnresaltado;  /*Color.FromArgb(255, 240, 240, 240);*/
                    foreach (string Subcadena in Subcadenas)
                    {
                        txtSubcadena.Text = Subcadena;
                        txtSubcadena.BackColor = colorResaltado;
                        txtConsola.Text = "Leyendo la subcadena " + Subcadena;
                        txtSubcadena.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/

                        miInstruccion = new Instrucción
                        {
                            Cadena = Subcadena
                        };


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
                                txtEvaluacion.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/
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
                                    txtEvaluacion.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/
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
                                    txtEvaluacion.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/
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
                            txtEvaluacion.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/
                        }
                        else
                        {                            
                            txtEvaluacion.Text += strTokenAux + " ";
                            txtEvaluacion.BackColor = colorResaltado;
                            txtConsola.Text = "Se identifico la subcadena " + Subcadena + " con el token " + strTokenAux;
                            txtEvaluacion.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/

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
                    txtTokens.BackColor = colorUnresaltado; /*Color.FromArgb(255, 240, 240, 240);*/

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
#pragma warning disable CS0219 // La variable está asignada pero nunca se usa su valor
            int CantidadTokens;
#pragma warning restore CS0219 // La variable está asignada pero nunca se usa su valor
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
                //int CantidadTokens = 1;
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


                    //CantidadTokens = 1;
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
            string strIden;
            string strValor;
            //bool boolIden = false;
            //bool boolValor = false;



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
                        if(!txtTokens.Lines[i].Contains("PR08") && !txtTokens.Lines[i].Contains("PR13"))
                        {
                            txtPostFijo.Text += txtTokens.Lines[i] + "\n";
                            j = intCantidadTokens;
                            break;
                        }
                        s = txtTokens.Lines[i].Substring((j * 4) + j, 4);
                        if (banderaSublista == false)
                        {
                            if (s == "PR08" || s == "PR13")
                            {
                                banderaSublista = true;
                                intInicioSublista = ((j * 4) + j);
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

                                txtPostFijo.Text += txtTokens.Lines[i].Substring(0, intInicioSublista) + "POST " + ConvertirAOrdenacionPostfijo(strSubcadena) + "POST" + txtTokens.Lines[i].Substring(intFinSublista) + "\n";/* strCadena.Replace(strSubcadena,*/
                                    
                            }
                        }
                        j++;
                    }
                }
            }

            //  Conversión a tripletas
            GenerarTripletas();
            MostrarTripletas();
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
                       s.StartsWith("CR") ||
                       s.Equals("PR16"))
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

        public void GenerarTripletas()
        {
            /*
             *  4 tipos de tripletas:
             *  1 -> Tripletas de asignación (puede contener operaciones aritméticas o lógicas)
             *  2 -> Tripletas aritmeticas
             *  3 -> Tripletas de selección (contiene una expresión lógica)
             *  4 -> Tripletas de iteración (contiene una expresión lógica)
             * 
             */
            string s = "";
            int contadorLinea = 0;
            int contadorApertura = 0;
            int contadorCierre = 0;
            bool banderaSeleccion = false;
            bool banderaCiclo = false;
            bool banderaTabla = false;
            string cadenaOriginal = "";

            while (contadorLinea < txtPostFijo.Lines.Count() - 1)
            {
                int cantidadTokens = ObtenerCantidadTokens(txtPostFijo.Lines[contadorLinea].Trim());
                int contadorToken = 0;
                string temp = txtPostFijo.Lines[contadorLinea];



                if (!banderaSeleccion && !banderaCiclo)
                {
                    if (banderaTabla)
                    {
                        if (txtPostFijo.Lines[contadorLinea].Contains("PR03"))
                        {
                            contadorApertura++;
                        }
                        if (txtPostFijo.Lines[contadorLinea].Contains("PR04"))
                        {
                            contadorCierre++;
                        }
                        if (contadorApertura == contadorCierre)
                        {
                            banderaTabla = false;
                            List<Tripleta> listemp = new List<Tripleta>();
                            listemp.Add(new Tripleta(0, "PR09", "</table>", "html"));
                            tripletas.Add(listemp);
                        }
                    }
                    while (contadorToken < cantidadTokens)
                    {
                        s = txtPostFijo.Lines[contadorLinea].Substring((contadorToken * 4) + contadorToken, 4);

                        //  Instruccion
                        if (s == "PR01" ||
                            s == "PR02" ||
                            s == "PR05" ||
                            s == "PR06" ||
                            s == "PR07" ||
                            s == "PR10" ||
                            s == "PR17" ||
                            s == "PR18" ||
                            s == "PR19" )
                        {
                            tripletas.Add(TripletaInstruccion(txtPostFijo.Lines[contadorLinea]));
                        }

                        //  Tabla
                        if (s == "PR09")
                        {
                            banderaTabla = true;
                            tripletas.Add(TripletaInstruccion(txtPostFijo.Lines[contadorLinea]));
                        }

                        //  Selección
                        if (s == "PR11")
                        {
#pragma warning disable IDE0059 // Asignación innecesaria de un valor
                            banderaSeleccion = true;
#pragma warning restore IDE0059 // Asignación innecesaria de un valor
                        }
                        //  Iteración
                        if (s == "PR14")
                        {
#pragma warning disable IDE0059 // Asignación innecesaria de un valor
                            banderaCiclo = true;
#pragma warning restore IDE0059 // Asignación innecesaria de un valor
                        }
                        contadorToken++;
                    }
                }
                else
                {
                    if (cadenaOriginal != "")
                    {
                        cadenaOriginal += " ";
                    }
                    cadenaOriginal += txtPostFijo.Lines[contadorLinea - 1].Trim() + "\n";

                    if (txtPostFijo.Lines[contadorLinea - 1].Contains("PR04"))
                    {
                        if (banderaSeleccion)
                        {
                            if (!txtPostFijo.Lines[contadorLinea].Contains("PR12"))
                            {
                                banderaSeleccion = false;
                                tripletas.Add(TripletaExpresionCondicional(cadenaOriginal));
                                contadorLinea--;
                            }
                        }
                        if (banderaCiclo)
                        {
                            banderaCiclo = false;
                            cadenaOriginal += txtPostFijo.Lines[contadorLinea] + "\n";
                            tripletas.Add(TripletaExpresionIteracion(cadenaOriginal));
                        }
                        //if (banderaTabla)
                        //{
                        //    banderaTabla = false;
                        //    cadenaOriginal += txtPostFijo.Lines[contadorLinea] + "\n";
                        //    tripletas.Add(TripletaTabla(cadenaOriginal));
                        //    //tripletas.Add(new Tripleta(0, "PR09", "<table>", "html"));
                        //    //foreach (Tripleta t in TripletaTabla(cadenaOriginal))
                        //    //{
                        //    //    tripletas.Add(t);
                        //    //}
                        //    //tripletas.Add(new Tripleta(0, "PR09", "</table>", "html"));
                        //}
                    }
                                       
                }
                contadorLinea++;
            }
            
        }

        public List<Tripleta> GenerarTripletas(string instrucciones)
        {
            List<Tripleta> tripletaTemp = new List<Tripleta>();
            string s = "";
            int contadorLinea = 0;
            bool banderaSeleccion = false;
            bool banderaCiclo = false;
            bool banderaTabla = false;
            string cadenaOriginal = "";
            string[] arregloString = instrucciones.Split('\n');

            while (contadorLinea < arregloString.Count() - 1)
            {
                int cantidadTokens = ObtenerCantidadTokens(arregloString[contadorLinea].Trim());
                int contadorToken = 0;
                string temp = arregloString[contadorLinea];

                if (!banderaSeleccion && !banderaCiclo && !banderaTabla)
                {
                    while (contadorToken < cantidadTokens && arregloString[contadorLinea] != "")
                    {
                        s = arregloString[contadorLinea].Substring((contadorToken * 4) + contadorToken, 4);

                        //  Instruccion
                        if (s == "PR01" ||
                            s == "PR02" ||
                            s == "PR05" ||
                            s == "PR06" ||
                            s == "PR07" ||
                            s == "PR17" ||
                            s == "PR18" ||
                            s == "PR19")
                        {
                            foreach(Tripleta t in TripletaInstruccion(arregloString[contadorLinea]))
                            {
                                tripletaTemp.Add(t);
                            }
                            //tripletas.Add(TripletaInstruccion(arregloString[contadorLinea]));
                        }

                        //  Tabla
                        if (s == "PR09" ||
                            s == "PR10")
                        {
                            banderaTabla = true;
                        }

                        //  Selección
                        if (s == "PR11")
                        {
#pragma warning disable IDE0059 // Asignación innecesaria de un valor
                            banderaSeleccion = true;
#pragma warning restore IDE0059 // Asignación innecesaria de un valor
                        }
                        //  Iteración
                        if (s == "PR14")
                        {
#pragma warning disable IDE0059 // Asignación innecesaria de un valor
                            banderaCiclo = true;
#pragma warning restore IDE0059 // Asignación innecesaria de un valor
                        }
                        contadorToken++;
                    }
                }
                else
                {
                    if (cadenaOriginal != "")
                    {
                        cadenaOriginal += " ";
                    }
                    cadenaOriginal += arregloString[contadorLinea - 1].Trim() + "\n";

                    if (arregloString[contadorLinea - 1].Contains("PR04"))
                    {
                        if (banderaSeleccion)
                        {
                            if (!arregloString[contadorLinea].Contains("PR12"))
                            {
                                banderaSeleccion = false;
                                foreach (Tripleta t in TripletaExpresionCondicional(cadenaOriginal))
                                {
                                    tripletaTemp.Add(t);
                                }
                                //tripletas.Add(TripletaExpresionCondicional(cadenaOriginal));
                                contadorLinea--;
                            }
                        }
                        if (banderaCiclo)
                        {
                            banderaCiclo = false;
                            cadenaOriginal += arregloString[contadorLinea] + "\n";
                            foreach (Tripleta t in TripletaExpresionIteracion(cadenaOriginal))
                            {
                                tripletaTemp.Add(t);
                            }
                            //tripletas.Add(TripletaExpresionIteracion(cadenaOriginal));
                        }
                        if (banderaTabla)
                        {
                            banderaTabla = false;
                            cadenaOriginal += arregloString[contadorLinea] + "\n";
                            tripletaTemp.Add(new Tripleta(0, "PR09", "<table>", "html"));
                            foreach (Tripleta t in TripletaTabla(cadenaOriginal))
                            {
                                tripletaTemp.Add(t);
                            }
                            tripletaTemp.Add(new Tripleta(0, "PR09", "</table>", "html"));
                        }
                    }

                }
                contadorLinea++;
            }
            return tripletaTemp;
        }

        public void MostrarTripletas()
        {
            dgvTripleta.Rows.Clear();
            dgvTripletaV.Rows.Clear();
            dgvTripletaF.Rows.Clear();
            foreach(List<Tripleta> lista in tripletas)
            {
                foreach(Tripleta t in lista)
                {
                    dgvTripleta.Rows.Add(
                        t.Indice, (t.DatoObjeto == null) ? "" :  t.DatoObjeto.ToString(),
                        (t.DatoFuente == null) ? "" : 
                        (t.DatoObjeto != null && t.DatoObjeto.ToString().Equals("ETIQT")) ? t.DatoFuente :
                        (t.DatoObjeto != null && t.DatoObjeto.ToString().Equals("ETIQF")) ? t.DatoFuente : 
                        t.DatoFuente.ToString(), 
                        (t.Operador == null) ? "" : t.Operador.ToString()
                        );
                }
            }
        }

        public List<Tripleta> TripletaExpresionAritmetica(string cadenaOriginal)
        {
            /*  
             *  Paso 1: Declarar una variable para la cadena original de la expresión
             *  Paso 2: Cadena auxiliar donde se concatenarán todos los tokens hasta que se encuentre un operador.
             *  Paso 3: Cadena auxiliar 2, donde se tome el operador detectado y los 2 operandos previos.
             *  Paso 4: Agregar a la tripleta 2 registros, uno que sea T1 recibe un identificador, el otro realiza la operación al mismo temporal.
             *  Paso 5: En la cadena auxiliar 1, se reemplaza la cadena auxiliar 2 por un temporal (TEXX)
             *  Paso 6: Si la cadena original termina, entonces se termina, si no, regresa al paso 2. (Ciclo hasta que termine la cadena).
             */

            List<Tripleta> trResultado = new List<Tripleta>();
            //  Definir TIMES como 1, esto en caso de que se halle esta instrucción dentro de una estructura de iteración
            trResultado.Add(new Tripleta(0, "TIMES", 1, "ORAS"));

            string s;
            string cadenaAuxiliar1 = "";
            string cadenaAuxiliar2;
            int i = 0;
            int intContadorRegistro = 1;
            int intContadorTemporales = 0;
            int intCantidadTokens = ObtenerCantidadTokens(cadenaOriginal.Trim());

            while (i < intCantidadTokens)
            {
                s = cadenaOriginal.Substring((i * 4) + i, 4);
                if (cadenaAuxiliar1 != "")
                {
                    cadenaAuxiliar1 += " ";
                }
                cadenaAuxiliar1 += s;
                if (s == "OPAS" ||   /*  +  */
                    s == "OPAR" ||   /*  -  */
                    s == "OPAM" ||   /*  *  */
                    s == "OPAD" ||   /*  /  */
                    s == "OPAP" ||   /*  ** */
                    s == "OPAC"      /*  %  */)
                {
                    cadenaAuxiliar2 = cadenaOriginal.Substring(((i - 2) * 4) + (i - 2));

                    //  Agregar registros en la tripleta
                    trResultado.Add(new Tripleta(intContadorRegistro, "TE" + (intContadorTemporales + 1).ToString("00"), cadenaAuxiliar2.Substring(0, 4),"OPAS"));
                    intContadorRegistro++;
                    trResultado.Add(new Tripleta(intContadorRegistro, "TE" + (intContadorTemporales + 1).ToString("00"), cadenaAuxiliar2.Substring(5,4), cadenaAuxiliar2.Substring(10)));
                    intContadorRegistro++;

                    cadenaAuxiliar1 = Regex.Replace(cadenaAuxiliar1, cadenaAuxiliar2, "TE" + intContadorTemporales.ToString("##"));
                    intContadorTemporales++;
                }
                intCantidadTokens++;
            }
            return trResultado;
        }

        public List<Tripleta> TripletaExpresionCondicional(string instruccionCondicional)
        {
            int intContadorRegistro = 2;
            int intContadorTemporales = 0;
            int intContadorTemporalesRelacionales = 0;
            string cadenaAuxiliar1 = "";
            string cadenaAuxiliar2 = "";

            List<Tripleta> trCondicional = new List<Tripleta>();
            List<Tripleta> trTrue = new List<Tripleta>();
            List<Tripleta> trFalse = new List<Tripleta>();

            //  Definir TIMES como 1, esto en caso de que se halle esta instrucción dentro de una estructura de iteración
            trCondicional.Add(new Tripleta(0, "TIMES", 1, "ORAS"));
            trCondicional.Add(new Tripleta(0, "PR11", null, null));


            //  Definir tripleta condicional, verdadera y falsa
            string s = "";

            string[] arregloString = instruccionCondicional.Split('\n');

            if (arregloString[0].Contains("OPLA") ||   /*  &  */
                arregloString[0].Contains("OPLO") ||   /*  |  */
                arregloString[0].Contains("OPLN")      /*  !  */)
            {
                //  Condición compleja                
                int i = 0;
                int intCantidadTokens = ObtenerCantidadTokens(arregloString[0].Trim());
                while (i < intCantidadTokens)
                {
                    s = arregloString[0].Substring((i * 4) + i, 4);
                    if (cadenaAuxiliar1 != "")
                    {
                        cadenaAuxiliar1 += " ";
                    }
                    cadenaAuxiliar1 += s;
                    if (s == "OPAS" ||   /*  +   */
                        s == "OPAR" ||   /*  -   */
                        s == "OPAM" ||   /*  *   */
                        s == "OPAD" ||   /*  /   */
                        s == "OPAP" ||   /*  **  */
                        s == "OPAC" ||   /*  %   */
                        s == "OPRM" ||   /*  >   */
                        s == "OPRm" ||   /*  <   */
                        s == "ORMI" ||   /*  >=   */
                        s == "ORmI" ||   /*  <=   */
                        s == "OPRD" ||   /*  <>  */
                        s == "OPRI"      /*  =   */
                        )
                    {
                        cadenaAuxiliar2 = arregloString[0].Substring(((i - 2) * 4) + (i - 2));

                        //  Agregar registros en la tripleta
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE" + (intContadorTemporales + 1).ToString("00"), cadenaAuxiliar2.Substring(0, 4), "OPAS"));
                        intContadorRegistro++;
                        intContadorTemporales++;
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE" + (intContadorTemporales + 1).ToString("00"), cadenaAuxiliar2.Substring(5, 4), "OPAS"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE" + (intContadorTemporales + 2).ToString("00"), "TE" + (intContadorTemporales + 1).ToString("00"), cadenaAuxiliar2.Substring(10, 4)));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE" + (intContadorTemporales + 2).ToString("00"), "TRUE", "ETIQT"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE" + (intContadorTemporales + 2).ToString("00"), "FALSE", "ETIQF"));
                        intContadorRegistro++;
                        intContadorTemporales++;

                        cadenaAuxiliar1 = Regex.Replace(cadenaAuxiliar1, cadenaAuxiliar2, "TE" + (intContadorTemporales + 1).ToString("00"));
                    }
                    if (s == "OPLN")
                    {
                        cadenaAuxiliar2 = arregloString[0].Substring(((i - 1) * 4) + (i - 1));

                        //  Agregar registros en la tripleta
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE" + intContadorTemporales.ToString("00"), cadenaAuxiliar2.Substring(0, 4), "OPLN"));
                        intContadorRegistro++;
                        intContadorTemporales++;

                        cadenaAuxiliar1 = Regex.Replace(cadenaAuxiliar1, cadenaAuxiliar2, "TE" + intContadorTemporales.ToString("00"));
                    }
                    if (s == "OPLA")
                    {
                        cadenaAuxiliar2 = arregloString[0].Substring(((i - 2) * 4) + (i - 2));

                        trCondicional.Add(new Tripleta(trCondicional.Count, cadenaAuxiliar2.Substring(0, 4), cadenaAuxiliar2.Substring(5, 4), "OPLA"));
                        intContadorRegistro++;

                        trCondicional.Add(new Tripleta(trCondicional.Count, intContadorTemporalesRelacionales.ToString("##"), "TRUE", "ETIQT"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, intContadorTemporalesRelacionales.ToString("##"), "FALSE", "ETIQF"));
                        intContadorTemporalesRelacionales++;

                        cadenaAuxiliar1 = Regex.Replace(cadenaAuxiliar1, cadenaAuxiliar2, "TE" + intContadorTemporales.ToString("00"));
                    }
                    if (s == "OPLO")
                    {
                        cadenaAuxiliar2 = arregloString[0].Substring(((i - 2) * 4) + (i - 2));

                        trCondicional.Add(new Tripleta(trCondicional.Count, cadenaAuxiliar2.Substring(0, 4), cadenaAuxiliar2.Substring(5, 4), "OPLO"));
                        intContadorRegistro++;

                        trCondicional.Add(new Tripleta(trCondicional.Count, intContadorTemporalesRelacionales.ToString("##"), "TRUE", "ETIQT"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, intContadorTemporalesRelacionales.ToString("##"), "FALSE", "ETIQF"));
                        intContadorTemporalesRelacionales++;

                        cadenaAuxiliar1 = Regex.Replace(cadenaAuxiliar1, cadenaAuxiliar2, "TE" + intContadorTemporales.ToString("00"));
                    }
                    i++;
                }


            }
            else
            {
                //  Condición simple
                int i = 0;
                int intCantidadTokens = ObtenerCantidadTokens(arregloString[0].Trim());
                while (i < intCantidadTokens)
                {
                    s = arregloString[0].Substring((i * 4) + i, 4);
                    if (cadenaAuxiliar1 != "")
                    {
                        cadenaAuxiliar1 += " ";
                    }
                    cadenaAuxiliar1 += s;

                    if (s == "OPAS" ||   /*  +   */
                        s == "OPAR" ||   /*  -   */
                        s == "OPAM" ||   /*  *   */
                        s == "OPAD" ||   /*  /   */
                        s == "OPAP" ||   /*  **  */
                        s == "OPAC" ||   /*  %   */
                        s == "OPRM" ||   /*  >   */
                        s == "OPRm" ||   /*  <   */
                        s == "ORMI" ||   /*  >=   */
                        s == "ORmI" ||   /*  <=   */
                        s == "OPRD" ||   /*  <>  */
                        s == "OPRI"      /*  =   */
                        )
                    {
                        cadenaAuxiliar2 = arregloString[0].Substring(((i - 2) * 4) + (i - 2));

                        //  Agregar registros en la tripleta
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE01", cadenaAuxiliar2.Substring(0, 4), "OPAS"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE02", cadenaAuxiliar2.Substring(5, 4), "OPAS"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE01", "TE02", cadenaAuxiliar2.Substring(10, 4)));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE01", "TRUE", "ETIQT"));
                        trCondicional.Add(new Tripleta(trCondicional.Count, "TE01", "FALSE", "ETIQF"));

                        cadenaAuxiliar1 = Regex.Replace(cadenaAuxiliar1, cadenaAuxiliar2, "TE" + intContadorTemporales.ToString("00"));
                    }
                    i++;
                }

                //trCondicional.Add(new Tripleta(1, "TE01", arregloString[0].Substring(5, 4), "ORAS"));
                //trCondicional.Add(new Tripleta(2, "TE02", arregloString[0].Substring(10, 4), "ORAS"));
                //trCondicional.Add(new Tripleta(3, "TE01", "TE02", arregloString[0].Substring(15, 4)));
                //trCondicional.Add(new Tripleta(4, "TR01", "TRUE", "ETIQT"));
                //trCondicional.Add(new Tripleta(5, "TR01", "FALSE", "ETIQF"));
                //intContadorRegistro = 6;

            }

            
            //  Obtener tripleta verdadera
            int intRenglonRecorrido = 2;
            //string parteVerdadera = "";
            //do
            //{
            //    parteVerdadera += arregloString[intRenglonRecorrido] + "\n";
            //    intRenglonRecorrido++;
            //}
            //while (!arregloString[intRenglonRecorrido].Contains("PR04"));
            //foreach(Tripleta t in GenerarTripletas(parteVerdadera))
            //{
            //    trTrue.Add(t);
            //}
            do
            {
                foreach (Tripleta tr in TripletaInstruccion(arregloString[intRenglonRecorrido]))
                {
                    trTrue.Add(tr);
                }
                intRenglonRecorrido++;
            }
            while (!arregloString[intRenglonRecorrido].Contains("PR04"));

            //  Obtener tripleta falsa
            intRenglonRecorrido++;
            if (intRenglonRecorrido != arregloString.Length)
            {
                //string parteFalsa = "";
                //do
                //{
                //    parteFalsa += arregloString[intRenglonRecorrido] + "\n";
                //    intRenglonRecorrido++;
                //}
                //while (intRenglonRecorrido < arregloString.Length);
                //foreach(Tripleta t in GenerarTripletas(parteFalsa))
                //{
                //    trFalse.Add(t);
                //}
                do
                {
                    foreach (Tripleta tr in TripletaInstruccion(arregloString[intRenglonRecorrido]))
                    {
                        trFalse.Add(tr);
                    }
                    intRenglonRecorrido++;
                }
                while (intRenglonRecorrido < arregloString.Length);
            }


            //  Agregar etiquetas a la tripleta condicional
            trCondicional.Add(new Tripleta(trCondicional.Count, "ETIQT", trTrue, null));
            trCondicional.Add(new Tripleta(trCondicional.Count, "ETIQ", null, trCondicional.Count + 2));
            trCondicional.Add(new Tripleta(trCondicional.Count, "ETIQF", trFalse, null));
            trCondicional.Add(new Tripleta(trCondicional.Count, "FIN", null, null));

            return trCondicional;
        }

        public List<Tripleta> TripletaExpresionIteracion(string instruccionIteracion)
        {
            List<Tripleta> trResultado = new List<Tripleta>();

            string[] arregloString = instruccionIteracion.Split('\n');

            arregloString[0] = arregloString[arregloString.Length - 2];
            arregloString[arregloString.Length - 2] = "";

            string temp = "";

            /* 
             * Se usa una variable temporal para hacer una estructura como la siguiente
             * TIL ( condicion )
             * ->
             *      -Codigo-
             * <-
             */
            foreach (string linea in arregloString)
            {
                if (!String.IsNullOrEmpty(linea))
                    temp += linea + "\n";
            }

            // Se utiliza una tripleta temporal donde se guarda la condición del ciclo
            List<Tripleta> tempCondicion = TripletaExpresionCondicional(temp);

            // Ciclo para agregar lo obtenido en el método TripletaExpresionCondicional a trResultado
            foreach (Tripleta tripleta in tempCondicion)
            {
                // Solamente se agrega hasta ETIQ trTrue
                if (tripleta.DatoObjeto.ToString().Equals("ETIQ") && tripleta.DatoFuente == null)
                    break;
                if (tripleta.DatoObjeto.ToString().Equals("PR11"))
                {
                    tripleta.DatoObjeto = "PR14";
                    tripleta.Operador = "js";
                }
                trResultado.Add(tripleta);

            }

            // Agregar tripletas faltantes
            trResultado.Add(new Tripleta(trResultado.Count, "TIMES", 1, "OPAS"));
            foreach (Tripleta tripleta in trResultado)
            {
                // Se agrega la tripleta que volverá al incio de la condición
                if (tripleta.DatoFuente != null && tripleta.DatoFuente.ToString().Equals("TRUE"))
                {
                    trResultado.Add(new Tripleta(trResultado.Count, "ETIQ", null, tripleta.Indice - 1));
                    break;
                }
            }
            trResultado.Add(new Tripleta(trResultado.Count, "FIN", null, null));

            // Ciclo para cambiar el apuntador (operador) de las tripletas que van a FIN
            trResultado.ForEach(tripleta => {
                if (tripleta.Operador != null && tripleta.Operador.ToString().Equals("ETIQF"))
                    tripleta.Operador = trResultado.Count - 1;
            });


            return trResultado;
        }

        public List<Tripleta> TripletaTabla(string instruccionTabla)
        {
            int intContadorRegistro = 1;

            List<Tripleta> trResultado = new List<Tripleta>();


            //  Definir tripleta condicional, verdadera y falsa
            string s = "";

            string[] arregloString = instruccionTabla.Split('\n');

            trResultado.Add(new Tripleta(0, "PR09", "<table>", "html"));

            for (int x = 1; x < arregloString.Length - 1; x++)
            {
                if(arregloString[x] == "")
                {
                    break;
                }

                if (arregloString[x].Contains("PR01") ||
                    arregloString[x].Contains("PR02") ||
                    arregloString[x].Contains("PR05") ||
                    arregloString[x].Contains("PR06") ||
                    arregloString[x].Contains("PR07") ||
                    arregloString[x].Contains("PR17") ||
                    arregloString[x].Contains("PR18") ||
                    arregloString[x].Contains("PR19"))
                {
                    foreach (Tripleta t in TripletaInstruccion(arregloString[x]))
                    {
                        trResultado.Add(t);
                    }
                }

                //  Selección
                if (arregloString[x].Contains("PR11"))
                {
                    string condicion = "";
                    int cuentaApertura = 0;
                    int cuentaCierre = 0;
                    while (cuentaApertura != cuentaCierre && cuentaApertura != 0)
                    {
                        condicion = condicion + arregloString[x] + "\n";
                        x++;
                        if (arregloString[x].Contains("PR03"))
                        {
                            cuentaApertura++;
                        }
                        if (arregloString[x].Contains("PR04"))
                        {
                            cuentaCierre++;
                        }
                    }
                    foreach (Tripleta t in TripletaExpresionCondicional(condicion))
                    {
                        trResultado.Add(t);
                    }
                }

                if (arregloString[x].Contains("PR14"))
                {
                    string ciclo = "";
                    while (!arregloString[x].Contains("PR15"))
                    {
                        ciclo = ciclo + arregloString[x] + "\n";
                        x++;
                    }
                    x++;
                    ciclo = ciclo + arregloString[x] + "\n";
                    foreach (Tripleta t in TripletaExpresionIteracion(ciclo))
                    {
                        trResultado.Add(t);
                    }
                }                
                else
                {
                    trResultado.Add(new Tripleta(x, "PR10", "<tr>", "html"));
                    int i = 0;
                    int intCantidadTokens = ObtenerCantidadTokens(arregloString[x].Trim());
                    while (i < intCantidadTokens)
                    {
                        s = arregloString[x].Trim().Substring((i * 4) + i, 4);
                        if (!s.StartsWith("CE") && !s.StartsWith("PR"))
                        {
                            string Param = s;
                            int CantidadColumnas = BuscarEnTablasSimbolos(s) != null ? BuscarEnTablasSimbolos(s).Cells.Count : 0;
                            switch (CantidadColumnas)
                            {
                                case 2:
                                    Param = BuscarEnTablasSimbolos(s).Cells[1].Value.ToString();
                                    break;
                                case 4:
                                    Param = BuscarEnTablasSimbolos(s).Cells[3].Value.ToString();
                                    break;
                                default:
                                    Param = s;
                                    break;
                            }
                            string a = "\\";
                            string b = "\"";
                            Param = Regex.Replace(Param, "<", "");
                            Param = Regex.Replace(Param, ">", "");
                            Param = Regex.Replace(Param, b, "");
                            Param = Regex.Replace(Param, "#", "");
                            if (Regex.Matches(Param, "#").Count <= 1)
                            {
                                Param = Regex.Replace(Param, "#", "");
                            }
                            trResultado.Add(new Tripleta(intContadorRegistro, "PR10", "<td>" + Param + "</td>", "html"));
                            intContadorRegistro++;
                        }
                        i++;
                    }
                    trResultado.Add(new Tripleta(intContadorRegistro, "PR10", "</tr>", "html"));
                }
                
                intContadorRegistro++;
            }
            trResultado.Add(new Tripleta(intContadorRegistro, "PR09", "</table>", "html"));
            return trResultado;
        }

        private void dgvTripleta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((string)dgvTripleta.Rows[e.RowIndex].Cells[1].Value == "ETIQT")
                {
                    dgvTripletaV.Rows.Clear();
                    foreach (Tripleta t in (List<Tripleta>)dgvTripleta.Rows[e.RowIndex].Cells[2].Value)
                    {
                        dgvTripletaV.Rows.Add(t.Indice, t.DatoObjeto, t.DatoFuente, t.Operador);
                    }
                }
                if ((string)dgvTripleta.Rows[e.RowIndex].Cells[1].Value == "ETIQF")
                {
                    dgvTripletaF.Rows.Clear();
                    foreach (Tripleta t in (List<Tripleta>)dgvTripleta.Rows[e.RowIndex].Cells[2].Value)
                    {
                        dgvTripletaF.Rows.Add(t.Indice, t.DatoObjeto, t.DatoFuente, t.Operador);
                    }
                    dgvTripletaV.Rows.Clear();
                }
            }
            catch(Exception ex) { }
            
        }

        public List<Tripleta> TripletaInstruccion(string instruccion)
        {
            //string css = "animation: type 3s steps(22);overflow: hidden;white-space: nowrap;width: 22ch;"

            List<Tripleta> trResultado = new List<Tripleta>();
            /*
             *  Las palabras reservadas que generarán tripletas son:
             *  01 -> DOC_START
             *  02 -> DOC_END
             *  05 -> TITLE
             *  06 -> PAGE_JUMP
             *  07 -> NEXT_LINE
             *  09 -> TABLE
             *  10 -> TABLE_ROW
             *  17 -> VAR
             *  18 -> VALUE
             *  19 -> VIEW
             */

            //  DOC_START
            if (instruccion.Contains("PR01"))
            {
                trResultado.Add(new Tripleta(0, "PR01",
                    "<!DOCTYPE html>\n" +
                        "<head>\n" +
                            "<link href=\"C:\\Users\\" + usuario + "\\Documents\\Codext\\tmpDocumento.css\" rel=\"stylesheet\">\n" +
                            "<script src=\"C:\\Users\\" + usuario + "\\Documents\\Codext\\tmpDocumento.js\"></script>\n" +
                        "</head>\n" +
                    "<body>\n" +
                        "<div>\n"
                    , "html"));

            }

            //  DOC_END
            if (instruccion.Contains("PR02"))
            {
                trResultado.Add(new Tripleta(0, "PR02", "</div></body></html>", "html"));
            }

            /*   TITLE
            *    Param1 -> Tamaño
            *    Param2 -> Texto
            */
            if (instruccion.Contains("PR05"))
            {
                string Param1 = instruccion.Substring(10, 4);
                string Param2 = instruccion.Substring(20, 4);

                //if (Param1 == "POST")
                //{
                //    List<Tripleta> arit = TripletaExpresionAritmetica(instruccion.Substring(10, instruccion.Length - 15));
                    
                //    trResultado.Add(new Tripleta(0, "PR05", "do { documen.write(\"<br>\")"arit., "js"));
                //}

                int CantidadColumnas = BuscarEnTablasSimbolos(Param1) != null ? BuscarEnTablasSimbolos(Param1).Cells.Count : 0;
                {
                    switch (CantidadColumnas)
                    {
                        case 2:
                            Param1 = BuscarEnTablasSimbolos(Param1).Cells[1].Value.ToString();
                            break;
                        case 4:
                            Param1 = BuscarEnTablasSimbolos(Param1).Cells[3].Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
                CantidadColumnas = BuscarEnTablasSimbolos(Param2) != null ? BuscarEnTablasSimbolos(Param2).Cells.Count : 0;
                {
                    switch (CantidadColumnas)
                    {
                        case 2:
                            Param2 = BuscarEnTablasSimbolos(Param2).Cells[1].Value.ToString();
                            break;
                        case 4:
                            Param2 = BuscarEnTablasSimbolos(Param2).Cells[3].Value.ToString();
                            break;
                        default:
                            break;
                    }
                }
                string a = "\\";
                string b = "\"";
                Param1 = Regex.Replace(Param1, "<", "");
                Param1 = Regex.Replace(Param1, ">", "");
                Param1 = Regex.Replace(Param1, b, "");
                if (Regex.Matches(Param1, "#").Count <= 1)
                {
                    Param1 = Regex.Replace(Param1, "#", "");
                }

                Param2 = Regex.Replace(Param2, "<", "");
                Param2 = Regex.Replace(Param2, ">", "");
                Param2 = Regex.Replace(Param2, b, "");
                if (Regex.Matches(Param2, "#").Count <= 1)
                {
                    Param2 = Regex.Replace(Param2, "#", "");
                }

                trResultado.Add(new Tripleta(0, "PR05", "<p style='font-size:" + Param1 + "ch;'>" + Param2 + "</p>", "html")) ;

            }

            //  PAGE_JUMP
            if (instruccion.Contains("PR06"))
            {
                trResultado.Add(new Tripleta(0, "PR06", "</div><hr><div>", "html"));
            }

            /*  NEXT_LINE
             *  *Param1 -> Cantidad de lineas a saltar
             */
            if (instruccion.Contains("PR07"))
            {
                try
                {
                    string Param1 = instruccion.Substring(10, 4);

                    int CantidadColumnas = BuscarEnTablasSimbolos(Param1) != null ? BuscarEnTablasSimbolos(Param1).Cells.Count : 0;
                    {
                        switch (CantidadColumnas)
                        {
                            case 2:
                                Param1 = BuscarEnTablasSimbolos(Param1).Cells[1].Value.ToString();
                                break;
                            case 4:
                                Param1 = BuscarEnTablasSimbolos(Param1).Cells[3].Value.ToString();
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    trResultado.Add(new Tripleta(0, "PR07", "<br>", "html"));
                }


                //int x = 0;
                //try
                //{
                //    string Param1 = instruccion.Substring(10, 4);

                //    if (Param1 == "POST")
                //    {
                //        List<Tripleta> arit = TripletaExpresionAritmetica(instruccion.Substring(10, instruccion.Length - 5));
                //        trResultado.Add(new Tripleta(0, "PR07", "i = 0; do { document.write(\"<br>\") } while i < (" + arit., "js"));
                //    }

                //    int CantidadColumnas = BuscarEnTablasSimbolos(Param1) != null ? BuscarEnTablasSimbolos(Param1).Cells.Count : 0;
                //    {
                //        switch (CantidadColumnas)
                //        {
                //            case 2:
                //                Param1 = BuscarEnTablasSimbolos(Param1).Cells[1].Value.ToString();
                //                break;
                //            case 4:
                //                Param1 = BuscarEnTablasSimbolos(Param1).Cells[3].Value.ToString();
                //                break;
                //            default:
                //                break;
                //        }
                //    }
                //    trResultado.Add(new Tripleta(0, "PR07", Param1, "OPRI"));
                //    for (x = 0; x < int.Parse(Param1); x++)
                //    {
                //        trResultado.Add(new Tripleta(x + 1, "PR07", "<br>", "html"));
                //    }
                //}
                //catch (Exception ex)
                //{
                //    trResultado.Add(new Tripleta(x + 1, "PR07", "<br>", "html"));
                //}
            }



            //if (instruccion.Contains("PR09"))
            //{

            //}
            ////  TABLE
            ////  *Param1 -> Cantidad de col
            ///

            if (instruccion.Contains("PR09"))
            {
                trResultado.Add(new Tripleta(0, "PR09", "<table>", "html"));
            }
            if (instruccion.Contains("PR10"))
            {
                trResultado.Add(new Tripleta(0, "PR10", "<tr>", "html"));
                string Param1 = instruccion.Trim().Substring(10, 4);
                string Param2 = instruccion.Trim().Substring(20, 4);

                int CantidadColumnas = BuscarEnTablasSimbolos(Param1) != null ? BuscarEnTablasSimbolos(Param1).Cells.Count : 0;
                switch (CantidadColumnas)
                {
                    case 2:
                        Param1 = BuscarEnTablasSimbolos(Param1).Cells[1].Value.ToString();
                        break;
                    case 4:
                        Param1 = BuscarEnTablasSimbolos(Param1).Cells[3].Value.ToString();
                        break;
                    default:
                        break;
                }

                CantidadColumnas = BuscarEnTablasSimbolos(Param2) != null ? BuscarEnTablasSimbolos(Param2).Cells.Count : 0;
                switch (CantidadColumnas)
                {
                    case 2:
                        Param2 = BuscarEnTablasSimbolos(Param2).Cells[1].Value.ToString();
                        break;
                    case 4:
                        Param2 = BuscarEnTablasSimbolos(Param2).Cells[3].Value.ToString();
                        break;
                    default:
                        break;
                }
                string a = "\\";
                string b = "\"";
                Param1 = Regex.Replace(Param1, "<", "");
                Param1 = Regex.Replace(Param1, ">", "");
                Param1 = Regex.Replace(Param1, b, "");
                if (Regex.Matches(Param1, "#").Count <= 1)
                {
                    Param1 = Regex.Replace(Param1, "#", "");
                }

                Param2 = Regex.Replace(Param2, "<", "");
                Param2 = Regex.Replace(Param2, ">", "");
                Param2 = Regex.Replace(Param2, b, "");
                if (Regex.Matches(Param2, "#").Count <= 1)
                {
                    Param2 = Regex.Replace(Param2, "#", "");
                }
                trResultado.Add(new Tripleta(1, "PR10", "<td>" + Param1 + "</td>" + "<td>" + Param2 + "</td>", "html"));
                        
                    
                
                trResultado.Add(new Tripleta(2, "PR10", "</tr>", "html"));
            }

                //if (instruccion.Contains("PR09"))
                //{
                //    try
                //    {
                //        string Param1 = instruccion.Substring(10, 4);
                //        int CantidadColumnas = BuscarEnTablasSimbolos(Param1).Cells.Count;
                //        {
                //            switch (CantidadColumnas)
                //            {
                //                case 2:
                //                    Param1 = BuscarEnTablasSimbolos(Param1).Cells[1].Value.ToString();
                //                    break;
                //                case 4:
                //                    Param1 = BuscarEnTablasSimbolos(Param1).Cells[3].Value.ToString();
                //                    break;
                //                default:
                //                    break;
                //            }
                //        }
                //        for (int x = 0; x < int.Parse(Param1); x++)
                //        {
                //            trResultado.Add(new Tripleta(x, "PR07", "<br>", "html"));
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        trResultado.Add(new Tripleta(x, "PR07", "<br>", "html"));
                //    }
                //}

                ////  TABLE_ROW
                //if (instruccion.Contains("PR10"))
                //{
                //    trResultado.Add(new Tripleta(0, "PR10", null, null));
                //}

            //  VAR
            // Param1 -> Nombre de la variable
            if (instruccion.Contains("PR17"))
            {
                string Param1 = instruccion.Substring(10, 4);
                trResultado.Add(new Tripleta(0, "PR17", "var " + Param1 + "\n", "js"));
            }

            //  VALUE
            //  Param1 -> Identificador
            //  Param2 -> Valor
            if (instruccion.Contains("PR18"))
            {
                //  Asignación
                //if (s == "PR18")
                //{
                //    while (Regex.Matches(cadenaOriginal, "POST").Count < 2)
                //    {
                //        s = txtPostFijo.Lines[contadorLinea].Substring((contadorToken * 4) + contadorToken, 4);
                //        if (cadenaOriginal != "")
                //        {
                //            cadenaOriginal += " ";
                //        }
                //        cadenaOriginal += s;
                //        contadorToken++;
                //    }
                //}
                bool esAritmetica = instruccion.Contains("POST");
                string Param1 = instruccion.Substring(10, 4);
                string Param2 = instruccion.Substring(20, 4);

                if (Param2 == "POST")
                {
                    List<Tripleta> arit = TripletaExpresionAritmetica(instruccion.Substring(20, instruccion.Length - 25));
                    trResultado.Add(new Tripleta(0, "PR18", arit, "OPRI"));
                    Param2 = instruccion.Substring(30, 4);
                }

                int CantidadColumnas = BuscarEnTablasSimbolos(Param2) != null ? BuscarEnTablasSimbolos(Param2).Cells.Count : 0;
                switch (CantidadColumnas)
                {
                    case 2:
                        Param2 = BuscarEnTablasSimbolos(Param2).Cells[1].Value.ToString();
                        break;
                    case 4:
                        Param2 = BuscarEnTablasSimbolos(Param2).Cells[3].Value.ToString();
                        break;
                    default:
                        Param2 = "\"" + Param2 + "\"";
                        break;
                }
                if (!esAritmetica)
                    trResultado.Add(new Tripleta(0, "PR18", Param2, "OPRI"));
                trResultado.Add(new Tripleta(1, "PR18", Param1 + " = " + Param2 + "\n", "js"));
            }

            //  VIEW
            //  Param1 -> Objeto a mostrar
            if (instruccion.Contains("PR19"))
            {
                string Param1 = instruccion.Trim().Substring(10, 4);

                int CantidadColumnas = BuscarEnTablasSimbolos(Param1) != null ? BuscarEnTablasSimbolos(Param1).Cells.Count : 0;
                switch (CantidadColumnas)
                {
                    case 2:
                        Param1 = BuscarEnTablasSimbolos(Param1).Cells[1].Value.ToString();
                        break;
                    case 4:
                        Param1 = BuscarEnTablasSimbolos(Param1).Cells[3].Value.ToString();
                        break;
                    default:
                        break;
                }
                string a = "\\";
                string b = "\"";
                Param1 = Regex.Replace(Param1, "<", "");
                Param1 = Regex.Replace(Param1, ">", "");
                Param1 = Regex.Replace(Param1, b, "");
                if (Regex.Matches(Param1, "#").Count <= 1)
                {
                    Param1 = Regex.Replace(Param1, "#", "");
                }

                trResultado.Add(new Tripleta(0, "PR19", "<p>  " + Param1 + " </p>", "html"));
            }

            foreach (Tripleta t in trResultado)
            {
                t.EsInstruccion = true;
            }
            return trResultado;
        }

        #endregion

        public DataGridViewRow BuscarEnTablasSimbolos(string Token)
        {
            if(Token.StartsWith("CN") || Token.StartsWith("CR"))
            {
                foreach(DataGridViewRow d in dgvTSConstantesNumericas.Rows)
                {
                    if (d.Cells[0].Value.ToString() == Token.Substring(2,2))
                    {
                        return d;
                    }
                }
            }
            if(Token.StartsWith("ID"))
            {
                foreach (DataGridViewRow d in dgvTSIdentificadores.Rows)
                {
                    if (d.Cells[0].Value.ToString() == Token.Substring(2, 2))
                    {
                        return d;
                    }
                }
            }
            return null;
        }


        /**
         *  Fase de optimización de tripletas 
         */
        #region Optimizaciones

        public void Optimizaciones()
        {
            OptimizacionLocales2();
            OptimizacionLocales4();
            MostrarTripletas();
        }

        public void OptimizacionLocales2()
        {
            List<object> objVariablesUsadas = new List<object>();
            foreach (List<Tripleta> listTripletas in tripletas)
            {
                foreach (Tripleta t in listTripletas)
                {
                    if (!objVariablesUsadas.Contains(t.DatoFuente))
                    {
                        objVariablesUsadas.Add(t.DatoFuente);
                    }
                }
            }

            foreach (List<Tripleta> listTripletas in tripletas)
            {
                for (int i = 0; i <= listTripletas.Count - 1; i++)
                {
                    if (!objVariablesUsadas.Contains(listTripletas[i].DatoObjeto) &&
                        !listTripletas[i].EsInstruccion &&
                        listTripletas[i].DatoObjeto != null &&
                        !(listTripletas[i].DatoObjeto.ToString() == "FIN" ||
                        listTripletas[i].DatoObjeto.ToString() == "ETIQ" ||
                        listTripletas[i].DatoObjeto.ToString() == "ETIQT" ||
                        listTripletas[i].DatoObjeto.ToString() == "ETIQF"))
                    {
                        listTripletas.Remove(listTripletas[i]);
                        i--;
                    }
                }
            }
        }

        public void OptimizacionLocales4()
        {
            foreach (List<Tripleta> listTripletas in tripletas)
            {
                for (int i = 0; i <= listTripletas.Count - 1; i++)
                {
                    if (listTripletas[i].DatoFuente != null && listTripletas[i].Operador != null)
                    {
                        if (listTripletas[i].DatoFuente is List<Tripleta>)
                        {
                            foreach (Tripleta t in (List<Tripleta>)listTripletas[i].DatoFuente)
                            {
                                if (t.DatoFuente is int && (int)t.DatoFuente == 0 && t.Operador.ToString() == "OPAS")
                                {
                                    listTripletas.Remove(listTripletas[i]);
                                    i--;
                                }
                                else if (t.DatoFuente is int && (int)t.DatoFuente == 1 && t.Operador.ToString() == "OPAM")
                                {
                                    listTripletas.Remove(listTripletas[i]);
                                    i--;
                                }
                                else if (t.DatoFuente == t.DatoObjeto && t.Operador.ToString() == "OPRI")
                                {
                                    listTripletas.Remove(listTripletas[i]);
                                    i--;
                                }
                            }
                        }
                        if (listTripletas[i].DatoFuente is int && (int)listTripletas[i].DatoFuente == 0 && listTripletas[i].Operador.ToString() == "OPAS")
                        {
                            listTripletas.Remove(listTripletas[i]);
                            i--;
                        }
                        else if (listTripletas[i].DatoFuente is int && (int)listTripletas[i].DatoFuente == 1 && listTripletas[i].Operador.ToString() == "OPAM")
                        {
                            listTripletas.Remove(listTripletas[i]);
                            i--;
                        }
                        else if (listTripletas[i].DatoFuente == listTripletas[i].DatoObjeto && listTripletas[i].Operador.ToString() == "OPRI")
                        {
                            listTripletas.Remove(listTripletas[i]);
                            i--;
                        }
                    }
                }
            }
        }



        #endregion



        /**
         *  Fase de código final
         */
        #region CodigoFinal

        //  Este método se encarga de recorrer cada registro de la tripleta. Por cada registro, escribe una instrucción
        //  en el archivo correspondiente, dependiendo de lo que represente ese registro.
        public void CodigoFinal()
        {
            StreamWriter swHTML = new StreamWriter(@"C:\Users\" + usuario + @"\Documents\Codext\tmpDocumento.html", false);
            swHTML.Close();
            swHTML = new StreamWriter(@"C:\Users\" + usuario + @"\Documents\Codext\tmpDocumento.html", true);

            StreamWriter swJS = new StreamWriter(@"C:\Users\"+usuario+@"\Documents\Codext\tmpDocumento.js", false);
            swJS.Close();
            swJS = new StreamWriter(@"C:\Users\" + usuario + @"\Documents\Codext\tmpDocumento.js", true);

            swJS.WriteLine("function graficar() {");
            swJS.WriteLine("document.write(\"<!DOCTYPE html>" +
                        "<head>" +
                            "<link href='C:\\\\Users\\\\" + usuario + "\\\\Documents\\\\Codext\\\\tmpDocumento.css' rel='stylesheet'>" +
                            "<script src='C:\\\\Users\\\\" + usuario + "\\\\Documents\\\\Codext\\\\tmpDocumento.js'></script>" +
                        "</head>" +
                    "<body>" +
                        "<div>\")");
            swJS.WriteLine("var TIMES;");

            foreach (List<Tripleta> listTripletas in tripletas)
            {
                //if (listTripletas.Count > 0 && listTripletas.ElementAt<Tripleta>(0).DatoObjeto.ToString() == "PR09")
                //{
                //    int contadorRegistro = 1;
                //    bool banderaCondicion;
                //    bool banderaCiclo = false;

                //    for (contadorRegistro = 1; contadorRegistro < listTripletas.Count; contadorRegistro++)
                //    {
                //        if (listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto.ToString() == "PR14")
                //        {
                //            List<List<object>> variables = new List<List<object>>();
                //            string temp = "";
                //            swJS.WriteLine("TIMES = 0");
                //            swJS.WriteLine("do{");
                //            for (contadorRegistro = contadorRegistro; contadorRegistro < listTripletas.Count; contadorRegistro++)
                //            {
                //                if (listTripletas[contadorRegistro].Operador != null)
                //                {
                //                    if (listTripletas[contadorRegistro].Operador.ToString() == "OPAS" || listTripletas[contadorRegistro].Operador.ToString() == "OPRI")
                //                    {
                //                        if (listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("CN") || listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("CR") || listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("ID"))
                //                        {

                //                            int CantidadColumnas = BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()) != null ? BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()).Cells.Count : 0;
                //                            {
                //                                switch (CantidadColumnas)
                //                                {
                //                                    case 2:
                //                                        listTripletas[contadorRegistro].DatoFuente = BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()).Cells[1].Value.ToString();
                //                                        break;
                //                                    case 4:
                //                                        listTripletas[contadorRegistro].DatoFuente = BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()).Cells[3].Value.ToString();
                //                                        break;
                //                                    default:
                //                                        break;
                //                                }
                //                            }
                //                            List<object> variable = new List<object>();
                //                            //Nombre
                //                            variable.Add(listTripletas[contadorRegistro].DatoObjeto);
                //                            //Valor
                //                            variable.Add(listTripletas[contadorRegistro].DatoFuente);
                //                            variables.Add(variable);
                //                        }

                //                        if (listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente.ToString().Equals("PR16"))
                //                        {
                //                            swJS.WriteLine("TIMES++");
                //                            List<object> variable = new List<object>();
                //                            //Nombre
                //                            variable.Add(listTripletas[contadorRegistro].DatoObjeto);
                //                            //Valor
                //                            variable.Add("TIMES");
                //                            variables.Add(variable);
                //                        }
                //                    }

                //                }
                //                if (listTripletas[contadorRegistro].DatoObjeto != null && listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("TE") && listTripletas[contadorRegistro].DatoObjeto.ToString().StartsWith("TE"))
                //                {
                //                    string operador = "";
                //                    switch (listTripletas[contadorRegistro].Operador.ToString())
                //                    {
                //                        case "OPRI":
                //                            operador = "=";
                //                            break;
                //                        case "OPRM":
                //                            operador = ">";
                //                            break;
                //                        case "OPRm":
                //                            operador = "<";
                //                            break;
                //                        case "OPRD":
                //                            operador = "<>";
                //                            break;
                //                        case "OPMI":
                //                            operador = ">=";
                //                            break;
                //                        case "OPmI":
                //                            operador = "<=";
                //                            break;
                //                        case "OPLA":
                //                            operador = "&";
                //                            break;
                //                        case "OPLO":
                //                            operador = "|";
                //                            break;
                //                        case "OPLN":
                //                            operador = "!";
                //                            break;

                //                    }
                //                    foreach (List<object> objetos in variables)
                //                    {
                //                        if (temp != "")
                //                            temp += operador;
                //                        temp += objetos[1].ToString();
                //                    }
                //                }
                //                if (listTripletas[contadorRegistro].DatoObjeto != null && listTripletas[contadorRegistro].DatoObjeto.ToString() == "ETIQT")
                //                {
                //                    if (listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente is List<Tripleta>)
                //                    {
                //                        foreach (Tripleta t1 in (List<Tripleta>)listTripletas[contadorRegistro].DatoFuente)
                //                        {
                //                            if (t1.DatoObjeto.ToString().StartsWith("PR"))
                //                            {
                //                                switch (t1.Operador.ToString())
                //                                {
                //                                    case "html":
                //                                        swJS.WriteLine("document.write(\"" + t1.DatoFuente.ToString() + "\");");
                //                                        break;
                //                                    case "js":
                //                                        swJS.WriteLine(t1.DatoFuente.ToString());
                //                                        break;
                //                                }
                //                            }
                //                            if (t1.Operador.ToString().StartsWith("O"))
                //                            {
                //                                swJS.WriteLine(t1.DatoObjeto + "=" + t1.DatoObjeto + t1.Operador + t1.DatoFuente);
                //                            }
                //                        }
                //                        swJS.WriteLine("}");
                //                    }
                //                }

                //            }
                //            swJS.WriteLine("while(" + temp + ");");
                //        }
                //        else
                //        {
                //            Tripleta t = listTripletas.ElementAt<Tripleta>(contadorRegistro);

                //            //  Si es una instrucción (incluyendo tabla)
                //            if (t.DatoObjeto.ToString().StartsWith("PR"))
                //            {
                //                if (t.DatoObjeto.ToString().StartsWith("PR01"))
                //                {
                //                    swHTML.WriteLine(t.DatoFuente.ToString());
                //                }
                //                else
                //                {
                //                    if (t.Operador != null && t.DatoFuente != null)
                //                    {
                //                        switch (t.Operador.ToString())
                //                        {
                //                            case "html":
                //                                swJS.WriteLine("document.write(\"" + t.DatoFuente.ToString() + "\")");
                //                                break;
                //                            case "js":
                //                                swJS.WriteLine(t.DatoFuente.ToString());
                //                                break;
                //                        }
                //                    }
                //                }
                //            }

                //            //  Si es una operación
                //            if (t.Operador != null)
                //            {
                //                if (t.Operador.ToString().StartsWith("O") && !t.DatoObjeto.ToString().StartsWith("PR") && t.DatoObjeto.ToString() != "TIMES")
                //                {
                //                    string operador = "";
                //                    switch (t.Operador.ToString())
                //                    {
                //                        case "OPAS":
                //                            operador = "+";
                //                            break;
                //                        case "OPAR":
                //                            operador = "-";
                //                            break;
                //                        case "OPAM":
                //                            operador = "*";
                //                            break;
                //                        case "OPAD":
                //                            operador = "/";
                //                            break;
                //                        case "OPAP":
                //                            operador = "**";
                //                            break;
                //                        case "OPAC":
                //                            operador = "%";
                //                            break;
                //                        case "OPRI":
                //                            operador = "=";
                //                            break;
                //                        case "OPRM":
                //                            operador = ">";
                //                            break;
                //                        case "OPRm":
                //                            operador = "<";
                //                            break;
                //                        case "OPRD":
                //                            operador = "<>";
                //                            break;
                //                        case "OPMI":
                //                            operador = ">=";
                //                            break;
                //                        case "OPmI":
                //                            operador = "<=";
                //                            break;
                //                        case "OPLA":
                //                            operador = "&";
                //                            break;
                //                        case "OPLO":
                //                            operador = "|";
                //                            break;
                //                        case "OPLN":
                //                            operador = "!";
                //                            break;
                //                    }
                //                    swJS.WriteLine(t.DatoObjeto + "=" + t.DatoObjeto + operador + t.DatoFuente);
                //                }
                //            }
                //        }
                //    }
                //}

                //else 
                if (listTripletas.Count > 0 && listTripletas.ElementAt<Tripleta>(0).DatoObjeto.ToString() == "TIMES")                    
                {
                    
                    int contadorRegistro = 1;
                    bool banderaCondicion;
                    bool banderaCiclo = false;

                    

                    //  Generación del código final de una condición
                    if (listTripletas.ElementAt<Tripleta>(1).DatoObjeto.ToString() == "PR11")
                    {
                        swJS.WriteLine("if(");
                        for (contadorRegistro = 1; contadorRegistro < listTripletas.Count; contadorRegistro++)
                        {
                            if (listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto.ToString().StartsWith("PR"))
                            {
                                if(listTripletas.ElementAt<Tripleta>(contadorRegistro).Operador != null)
                                {
                                    switch (listTripletas.ElementAt<Tripleta>(contadorRegistro).Operador.ToString())
                                    {
                                        case "html":
                                            swJS.WriteLine("document.write(\"" + listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoFuente.ToString() + "\");");
                                            break;
                                        case "js":
                                            swJS.WriteLine(listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoFuente.ToString());
                                            break;
                                    }
                                }
                                
                            }
                            if (listTripletas.ElementAt<Tripleta>(contadorRegistro).Operador != null && listTripletas.ElementAt<Tripleta>(contadorRegistro).Operador.ToString().StartsWith("O"))
                            {
                                swJS.WriteLine(listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto + "=" + listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto + listTripletas.ElementAt<Tripleta>(contadorRegistro).Operador + listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoFuente);
                            }
                            if (listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto != null && listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto.ToString() == "ETIQT")
                            {
                                swJS.WriteLine(") {");
                                List<Tripleta> trTrue = (List<Tripleta>)listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoFuente;
                                foreach(Tripleta t1 in trTrue)
                                {
                                    if (t1.DatoObjeto.ToString().StartsWith("PR"))
                                    {
                                        switch (t1.Operador.ToString())
                                        {
                                            case "html":
                                                swJS.WriteLine("document.write(\"" + t1.DatoFuente.ToString() + "\");");
                                                break;
                                            case "js":
                                                swJS.WriteLine(t1.DatoFuente.ToString());
                                                break;
                                        }
                                    }
                                    if (t1.Operador != null && t1.Operador.ToString().StartsWith("O"))
                                    {
                                        swJS.WriteLine(t1.DatoObjeto + "=" + t1.DatoObjeto + t1.Operador + t1.DatoFuente);
                                    }
                                }
                                swJS.WriteLine("}");
                            }
                            if (listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoObjeto.ToString() == "ETIQF")
                            {
                                swJS.WriteLine("else {");
                                List<Tripleta> trFalse = (List<Tripleta>)listTripletas.ElementAt<Tripleta>(contadorRegistro).DatoFuente;
                                foreach (Tripleta t2 in trFalse)
                                {
                                    if (t2.DatoObjeto.ToString().StartsWith("PR"))
                                    {
                                        switch (t2.Operador.ToString())
                                        {
                                            case "html":
                                                swJS.WriteLine("document.write(\"" + t2.DatoFuente.ToString() + "\");");
                                                break;
                                            case "js":
                                                swJS.WriteLine(t2.DatoFuente.ToString());
                                                break;
                                        }
                                    }
                                    if (t2.Operador.ToString().StartsWith("O"))
                                    {
                                        swJS.WriteLine(t2.DatoObjeto + "=" + t2.DatoObjeto + t2.Operador + t2.DatoFuente);
                                    }
                                }
                                swJS.WriteLine("}");
                            }
                            contadorRegistro++;
                        }
                    }

                    //  Generación del código de un ciclo
                    if(listTripletas.ElementAt<Tripleta>(1).DatoObjeto.ToString() == "PR14")
                    {
                        List<List<object>> variables = new List<List<object>>();
                        string temp = "";
                        swJS.WriteLine("TIMES = 0");
                        swJS.WriteLine("do{");
                        for (contadorRegistro = 1; contadorRegistro < listTripletas.Count; contadorRegistro++)
                        {
                            if(listTripletas[contadorRegistro].Operador != null)
                            {
                                if (listTripletas[contadorRegistro].Operador.ToString() == "OPAS" || listTripletas[contadorRegistro].Operador.ToString() == "OPRI")
                                {
                                    if (listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("CN") || listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("CR") || listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("ID"))
                                    {

                                        int CantidadColumnas = BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()) != null ? BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()).Cells.Count : 0;
                                        {
                                            switch (CantidadColumnas)
                                            {
                                                case 2:
                                                    listTripletas[contadorRegistro].DatoFuente = BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()).Cells[1].Value.ToString();
                                                    break;
                                                case 4:
                                                    listTripletas[contadorRegistro].DatoFuente = BuscarEnTablasSimbolos(listTripletas[contadorRegistro].DatoFuente.ToString()).Cells[3].Value.ToString();
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        List<object> variable = new List<object>();
                                        //Nombre
                                        variable.Add(listTripletas[contadorRegistro].DatoObjeto);
                                        //Valor
                                        variable.Add(listTripletas[contadorRegistro].DatoFuente);
                                        variables.Add(variable);
                                    }

                                    if (listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente.ToString().Equals("PR16"))
                                    {
                                        swJS.WriteLine("TIMES++");
                                        List<object> variable = new List<object>();
                                        //Nombre
                                        variable.Add(listTripletas[contadorRegistro].DatoObjeto);
                                        //Valor
                                        variable.Add("TIMES");
                                        variables.Add(variable);
                                    }
                                }

                            }
                            if (listTripletas[contadorRegistro].DatoObjeto != null && listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente.ToString().StartsWith("TE") && listTripletas[contadorRegistro].DatoObjeto.ToString().StartsWith("TE"))
                            {
                                string operador = "";
                                switch (listTripletas[contadorRegistro].Operador.ToString())
                                {
                                    case "OPRI":
                                        operador = "=";
                                        break;
                                    case "OPRM":
                                        operador = ">";
                                        break;
                                    case "OPRm":
                                        operador = "<";
                                        break;
                                    case "OPRD":
                                        operador = "<>";
                                        break;
                                    case "OPMI":
                                        operador = ">=";
                                        break;
                                    case "OPmI":
                                        operador = "<=";
                                        break;
                                    case "OPLA":
                                        operador = "&";
                                        break;
                                    case "OPLO":
                                        operador = "|";
                                        break;
                                    case "OPLN":
                                        operador = "!";
                                        break;

                                }
                                foreach (List<object> objetos in variables)
                                {
                                    if (temp != "")
                                        temp += operador;
                                    temp += objetos[1].ToString();
                                }
                            }
                            if(listTripletas[contadorRegistro].DatoObjeto != null && listTripletas[contadorRegistro].DatoObjeto.ToString() == "ETIQT")
                            {
                                if(listTripletas[contadorRegistro].DatoFuente != null && listTripletas[contadorRegistro].DatoFuente is List<Tripleta>)
                                {
                                    foreach (Tripleta t1 in (List<Tripleta>)listTripletas[contadorRegistro].DatoFuente)
                                    {
                                        if (t1.DatoObjeto.ToString().StartsWith("PR"))
                                        {
                                            switch (t1.Operador.ToString())
                                            {
                                                case "html":
                                                    swJS.WriteLine("document.write(\"" + t1.DatoFuente.ToString() + "\");");
                                                    break;
                                                case "js":
                                                    swJS.WriteLine(t1.DatoFuente.ToString());
                                                    break;
                                            }
                                        }
                                        if (t1.Operador.ToString().StartsWith("O"))
                                        {
                                            swJS.WriteLine(t1.DatoObjeto + "=" + t1.DatoObjeto + t1.Operador + t1.DatoFuente);
                                        }
                                    }
                                    swJS.WriteLine("}");
                                }
                            }
                           
                        }
                        swJS.WriteLine("while(" + temp + ");");
                    }                    
                }
                else
                {     
                    foreach (Tripleta t in listTripletas)
                    {
                        //  Si es una instrucción (incluyendo tabla)
                        if (t.DatoObjeto.ToString().StartsWith("PR"))
                        {
                            if (t.DatoObjeto.ToString().StartsWith("PR01"))
                            {
                                swHTML.WriteLine(t.DatoFuente.ToString());
                            }
                            else 
                            {
                                if (t.Operador != null && t.DatoFuente != null)
                                {
                                    switch (t.Operador.ToString())
                                    {
                                        case "html":
                                            swJS.WriteLine("document.write(\"" + t.DatoFuente.ToString() + "\")");
                                            break;
                                        case "js":
                                            swJS.WriteLine(t.DatoFuente.ToString());
                                            break;
                                    }
                                }
                            }
                        }

                        //  Si es una operación
                        if (t.Operador != null)
                        {
                            if (t.Operador.ToString().StartsWith("O") && !t.DatoObjeto.ToString().StartsWith("PR") && t.DatoObjeto.ToString() != "TIMES")
                            {
                                string operador = "";
                                switch(t.Operador.ToString())
                                {
                                    case "OPAS":
                                        operador = "+";
                                        break;
                                    case "OPAR":
                                        operador = "-";
                                        break;
                                    case "OPAM":
                                        operador = "*";
                                        break;
                                    case "OPAD":
                                        operador = "/";
                                        break;
                                    case "OPAP":
                                        operador = "**";
                                        break;
                                    case "OPAC":
                                        operador = "%";
                                        break;
                                    case "OPRI":
                                        operador = "=";
                                        break;
                                    case "OPRM":
                                        operador = ">";
                                        break;
                                    case "OPRm":
                                        operador = "<";
                                        break;
                                    case "OPRD":
                                        operador = "<>";
                                        break;
                                    case "OPMI":
                                        operador = ">=";
                                        break;
                                    case "OPmI":
                                        operador = "<=";
                                        break;
                                    case "OPLA":
                                        operador = "&";
                                        break;
                                    case "OPLO":
                                        operador = "|";
                                        break;
                                    case "OPLN":
                                        operador = "!";
                                        break;
                                }
                                swJS.WriteLine(t.DatoObjeto + "=" + t.DatoObjeto + operador + t.DatoFuente);
                            }
                        }
                    }
                }
            }
            swHTML.Close();

            swJS.WriteLine("}");
            swJS.WriteLine("window.onload = graficar;");
            swJS.Close();


            System.Diagnostics.Process.Start(@"C:\Users\" + usuario + @"\Documents\Codext\tmpDocumento.html");

            //using (DocumentView dv = new DocumentView())
            //{
            //    dv.EstablecerUrl(@"C:\Users\" + usuario + @"\Documents\Codext\tmpDocumento.html");
            //    dv.ShowDialog();
            //}
        }

        #endregion

    }
}
