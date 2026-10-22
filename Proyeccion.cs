using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Proyeccion_sobre_segmentos
{
    public class Proyeccion
    {
        public Punto Q { get; private set; }
        public double DistanciaMinima { get; private set; }
        public double T { get; private set; }

        public void Calcular(Segmento segmento, Punto p)
        {
            // Calcular el vector AB
            double dx = segmento.B.X - segmento.A.X;
            double dy = segmento.B.Y - segmento.A.Y;

            // Calcular el vector AP
            double px = p.X - segmento.A.X;
            double py = p.Y - segmento.A.Y;

            // Calcular la longitud al cuadrado de AB
            double longitud2 = dx * dx + dy * dy;

            // Verificar si A y B son el mismo punto
            if (longitud2 == 0)
            {
                T = 0;
                Q = new Punto(segmento.A.X, segmento.A.Y);
            }
            else
            {
                // Calcular el parametro de proyeccion
                double t = (px * dx + py * dy) / longitud2;

                // Restringir la proyeccion al segmento
                T = Math.Max(0, Math.Min(1, t));

                // Calcular las coordenadas del punto Q
                double qx = segmento.A.X + T * dx;
                double qy = segmento.A.Y + T * dy;

                Q = new Punto(qx, qy);
            }

            // Calcular la distancia minima entre P y Q
            double distanciaX = p.X - Q.X;
            double distanciaY = p.Y - Q.Y;

            DistanciaMinima = Math.Sqrt(
                distanciaX * distanciaX +
                distanciaY * distanciaY
            );
        }
    }
}
