using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codext
{
    public class Tripleta : IEquatable<Object>
    {
        int intIndice;
        Object objDatoObjeto;
        Object objDatoFuente;
        Object objOperador;
        bool esInstruccion;

        public int Indice
        {
            get { return intIndice; }
            set { intIndice = value; }
        }
        public Object DatoObjeto
        {
            get { return objDatoObjeto; }
            set { objDatoObjeto = value; }
        }
        public Object DatoFuente
        {
            get { return objDatoFuente; }
            set { objDatoFuente = value; }
        }
        public Object Operador
        {
            get { return objOperador; }
            set { objOperador = value; }
        }
        public bool EsInstruccion
        {
            get { return esInstruccion; }
            set { esInstruccion = value; }
        }

        public Tripleta() { }

        public Tripleta(int indice, Object datoObjeto, Object datoFuente, Object operador)
        {
            intIndice = indice;
            objDatoObjeto = datoObjeto;
            objDatoFuente = datoFuente;
            objOperador = operador;
        }

        public override bool Equals(object obj)
        {
            return obj is Tripleta tripleta &&
                   Indice == tripleta.Indice &&
                   EqualityComparer<object>.Default.Equals(DatoObjeto, tripleta.DatoObjeto) &&
                   EqualityComparer<object>.Default.Equals(DatoFuente, tripleta.DatoFuente) &&
                   EqualityComparer<object>.Default.Equals(Operador, tripleta.Operador);
        }

        public static bool operator ==(Tripleta left, Tripleta right)
        {
            return EqualityComparer<Tripleta>.Default.Equals(left, right);
        }

        public static bool operator !=(Tripleta left, Tripleta right)
        {
            return !(left == right);
        }
    }
}
