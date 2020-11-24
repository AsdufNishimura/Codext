using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codext
{
    /**
     *  Métodos no usados y/o no implementados.
     */
    class Test
    {
        

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


//        private async void AnalisisLexicoPausado()
//        {
//            txtCodigo.ReadOnly = true;
//            btnSiguientePaso.Enabled = true;

//            try
//            {
//                dgvTSIdentificadores.Rows.Clear();
//                dgvTSConstantesNumericas.Rows.Clear();
//                IDXX = 0;
//                CNXX = 0;
//                CRXX = 0;

//                txtConsola.Text = "Comenzando proceso.";
//                txtTokens.Text = "";
//                txtEvaluacion.Text = "";
//                string strTokenAux;
//                ObtenerSubcadenas();
//                int renglonActual = 0;
//                int intPosRenglon = 0;
//                int intPosRenglonTokens = 0;
//                txtRenglones.Text = txtCodigo.Lines.Count().ToString();
//                txtRenglonActual.Text = renglonActual.ToString();

//                foreach (List<string> Subcadenas in lstRenglones)
//                {
//                    txtCodigo.BackColor = colorResaltado;
//                    txtRenglonActual.Text = (renglonActual + 1).ToString();
//                    txtConsola.Text = "Leyendo el renglón " + (renglonActual + 1);
//                    txtCodigo.Focus();
//                    txtCodigo.Select(intPosRenglon, txtCodigo.Lines[renglonActual].Length);
//                    //await Task.Delay(2000);
//                    this.Focus();
//                    await teclaEnter.Task;
//                    teclaEnter = new TaskCompletionSource<object>();
//                    //await Task.Run(() =>
//                    //                    {
//                    //                        void frmCodext_KeyDowna(object senderinterno, KeyEventArgs einterno)
//                    //                        {
//                    //                            if (einterno.KeyCode == Keys.Enter)
//                    //                                return;
//                    //                        }
//                    //                    });
//                    txtCodigo.BackColor = Color.FromArgb(255, 240, 240, 240);
//                    foreach (string Subcadena in Subcadenas)
//                    {
//                        //Aquí debería pausar xd creo
//                        txtSubcadena.Text = Subcadena;
//                        txtSubcadena.BackColor = colorResaltado;
//                        txtConsola.Text = "Leyendo la subcadena " + Subcadena;
//                        //await Task.Delay(2000);
//                        this.Focus();
//                        await teclaEnter.Task;
//                        teclaEnter = new TaskCompletionSource<object>();
//                        txtSubcadena.BackColor = Color.FromArgb(255, 240, 240, 240);

//                        miInstruccion = new Instrucción
//                        {
//                            Cadena = Subcadena
//                        };


//                        string tsres;

//                        if (Subcadena.Contains("_"))
//                        {
//                            tsres = VerificarTablasDeSimbolos("ID");

//                            if (tsres != "")
//                            {
//                                strTokenAux = tsres;
//                                dgvTSIdentificadores.Select();
//                                txtEvaluacion.Text += strTokenAux + " ";
//                                txtEvaluacion.BackColor = colorResaltado;
//                                txtConsola.Text = "Se encontró el identificador en la tabla de símbolos, su token es " + strTokenAux;
//                                //await Task.Delay(2000);
//                                this.Focus();
//                                await teclaEnter.Task;
//                                teclaEnter = new TaskCompletionSource<object>();
//                                txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
//                                break;
//                            }
//                        }

//                        if (Subcadena.Contains("#"))
//                        {
//                            if (Subcadena.Contains(".") || Subcadena.Contains("E"))
//                            {
//                                tsres = VerificarTablasDeSimbolos("CR");
//                                if (tsres != "")
//                                {
//                                    strTokenAux = tsres;
//                                    dgvTSConstantesNumericas.Select();
//                                    txtEvaluacion.Text += strTokenAux + " ";
//                                    txtEvaluacion.BackColor = colorResaltado;
//                                    txtConsola.Text = "Se encontró la constante numérica real en la tabla de símbolos, su token es " + strTokenAux;
//                                    //await Task.Delay(2000);
//                                    this.Focus();
//                                    await teclaEnter.Task;
//                                    teclaEnter = new TaskCompletionSource<object>();
//                                    txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
//                                    break;
//                                }
//                            }
//                            else
//                            {
//                                tsres = VerificarTablasDeSimbolos("CN");
//                                if (tsres != "")
//                                {
//                                    strTokenAux = tsres;
//                                    dgvTSConstantesNumericas.Select();
//                                    txtEvaluacion.Text += strTokenAux + " ";
//                                    txtEvaluacion.BackColor = colorResaltado;
//                                    txtConsola.Text = "Se encontró la constante numérica entera en la tabla de símbolos, su token es " + strTokenAux;
//                                    //await Task.Delay(2000);
//                                    this.Focus();
//                                    await teclaEnter.Task;
//                                    teclaEnter = new TaskCompletionSource<object>();
//                                    txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
//                                    break;
//                                }
//                            }

//                        }

//                        strTokenAux = EncontrarToken(miInstruccion, 0, "0");

//                        if (strTokenAux == "ERRL")
//                        {
//                            txtEvaluacion.Text += strTokenAux + " ";
//                            txtEvaluacion.BackColor = colorResaltado;
//                            txtConsola.Text = "Error detectado: no se reconoció el elemento. Se asignará un token de error léxico (ERRL)";
//                            //await Task.Delay(2000);
//                            this.Focus();
//                            await teclaEnter.Task;
//                            teclaEnter = new TaskCompletionSource<object>();
//                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);
//                        }
//                        else
//                        {
//                            //Aquí también  
//                            txtEvaluacion.Text += strTokenAux + " ";
//                            txtEvaluacion.BackColor = colorResaltado;
//                            txtConsola.Text = "Se identifico la subcadena " + Subcadena + " con el token " + strTokenAux;
//                            //await Task.Delay(2000);
//                            this.Focus();
//                            await teclaEnter.Task;
//                            teclaEnter = new TaskCompletionSource<object>();
//                            txtEvaluacion.BackColor = Color.FromArgb(255, 240, 240, 240);

//                            string tipoToken = strTokenAux.Substring(0, 2);
//                            string numToken = strTokenAux.Substring(2, 2);

//                            if (tipoToken == "ID")
//                            {
//                                dgvTSIdentificadores.Rows.Add(numToken, Subcadena, "-", "-");
//                                dgvTSIdentificadores.Select();
//                                txtConsola.Text = "Se agregó el identificador con el token " + strTokenAux + " a la tabla de símbolos.";
//                                //await Task.Delay(2000);
//                                this.Focus();
//                                await teclaEnter.Task;
//                                teclaEnter = new TaskCompletionSource<object>();
//                            }
//                            else if (tipoToken == "CN")
//                            {
//                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
//                                dgvTSConstantesNumericas.Select();
//                                txtConsola.Text = "Se agregó la constante numérica entera con el token " + strTokenAux + " a la tabla de símbolos.";
//                                //await Task.Delay(2000);
//                                this.Focus();
//                                await teclaEnter.Task;
//                                teclaEnter = new TaskCompletionSource<object>();
//                            }
//                            else if (tipoToken == "CR")
//                            {
//                                dgvTSConstantesNumericas.Rows.Add(numToken, Subcadena.Substring(0, Subcadena.Length - 1));
//                                dgvTSConstantesNumericas.Select();
//                                txtConsola.Text = "Se agregó la constante numérica real con el token " + strTokenAux + " a la tabla de símbolos.";
//                                //await Task.Delay(2000);
//                                this.Focus();
//                                await teclaEnter.Task;
//                                teclaEnter = new TaskCompletionSource<object>();
//                            }


//                            txtSubcadena.Text = "";
//                        }
//                    }
//                    //Aquí pausa igual                    
//                    txtTokens.Text += txtEvaluacion.Text + "\n";
//                    txtTokens.BackColor = colorResaltado;
//                    txtConsola.Text = "Se ha llegado al final del renglón " + (renglonActual + 1);
//                    txtTokens.Focus();
//                    txtTokens.Select(intPosRenglonTokens, txtTokens.Lines[renglonActual].Length);
//                    //await Task.Delay(2000);
//                    this.Focus();
//                    await teclaEnter.Task;
//                    teclaEnter = new TaskCompletionSource<object>();
//                    txtTokens.BackColor = Color.FromArgb(255, 240, 240, 240);

//                    txtEvaluacion.Text = "";

//                    intPosRenglon += (txtCodigo.Lines[renglonActual].Length + 1);
//                    intPosRenglonTokens += (txtTokens.Lines[renglonActual].Length + 1);
//                    renglonActual++;

//                }
//                txtConsola.Text = "Ha finalizado el análisis léxico.";
//                this.Focus();
//                return;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                txtConsola.Text = "Ocurrió un error inesperado.";
//                return;
//            }
//#pragma warning disable CS0162 // Se detectó código inaccesible
//            txtCodigo.ReadOnly = false;
//#pragma warning restore CS0162 // Se detectó código inaccesible
//            btnSiguientePaso.Enabled = false;
//        }

    }
}
