using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codext
{
    // Instrucción de código, cada uno de los elementos del programa, separados por delimitadores.
    public class Instrucción
    {
        private string _strCadena;
        private string _strToken;

        public string Token
        {
            get { return _strToken; }
            set { _strToken = value; }
        }
        public string Cadena
        {
            get { return _strCadena; }
            set { _strCadena = value; }
        }
    }
}
