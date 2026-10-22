using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Proyeccion_sobre_segmentos
{
    public class Segmento
    {
        public Punto A { get; set; }
        public Punto B { get; set; }

        public Segmento(Punto a, Punto b)
        {
            A = a;
            B = b;
        }

        public double Longitud()
        {
            double dx = B.X - A.X;
            double dy = B.Y - A.Y;

            return System.Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
