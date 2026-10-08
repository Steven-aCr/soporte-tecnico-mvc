using System;
using System.Collections.Generic;
using System.Text;

namespace SoporteTecnico.BL.Excepciones
{
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }

        public ReglaNegocioException(string mensaje, Exception innerException)
            : base(mensaje, innerException) { }
    }
}