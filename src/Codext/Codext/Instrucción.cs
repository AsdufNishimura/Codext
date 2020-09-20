using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codext
{
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
