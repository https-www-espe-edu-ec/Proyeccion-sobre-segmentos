using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Proyeccion_sobre_segmentos
{
    public partial class Home : Form
    {
        private Punto puntoA;
        private Punto puntoB;
        private Punto puntoP;
        private Proyeccion proyeccion;

        private bool graficar = false;

        public Home()
        {
            InitializeComponent();

            // Configurar el panel para dibujar
            panel1.BackColor = Color.White;

            // Asociar los eventos
            btnCalcular.Click += btnCalcular_Click;
            btnLimpiar.Click += btnLimpiar_Click;
            panel1.Paint += panel1_Paint;

            // Establecer los datos iniciales
            txtA.Text = "2,2";
            txtB.Text = "8,4";
            txtP.Text = "5,6";

            lblQ.Text = "Punto Q: ";
            lblDistancia.Text = "Distancia minima: ";
        }

        // Convertir el texto en un punto
        private Punto LeerPunto(string texto)
        {
            string[] datos = texto.Split(',');

            if (datos.Length != 2)
            {
                throw new Exception(
                    "Ingrese las coordenadas como X,Y"
                );
            }

            double x;
            double y;

            bool validoX = double.TryParse(
                datos[0].Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out x
            );

            bool validoY = double.TryParse(
                datos[1].Trim(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out y
            );

            if (!validoX || !validoY ||
                double.IsNaN(x) || double.IsNaN(y) ||
                double.IsInfinity(x) || double.IsInfinity(y))
            {
                throw new Exception(
                    "Las coordenadas deben ser numeros validos."
                );
            }

            return new Punto(x, y);
        }

        // Calcular la proyeccion y mostrar los resultados
        private void btnCalcular_Click(
            object sender, EventArgs e)
        {
            try
            {
                // Leer los puntos ingresados
                Punto a = LeerPunto(txtA.Text);
                Punto b = LeerPunto(txtB.Text);
                Punto p = LeerPunto(txtP.Text);

                // Crear el segmento AB
                Segmento segmento = new Segmento(a, b);

                // Crear y calcular la proyeccion
                Proyeccion resultado = new Proyeccion();
                resultado.Calcular(segmento, p);

                // Guardar los resultados
                puntoA = a;
                puntoB = b;
                puntoP = p;
                proyeccion = resultado;

                // Mostrar las coordenadas de Q
                lblQ.Text = string.Format(
                    "Punto Q: ({0:F3}, {1:F3})",
                    proyeccion.Q.X,
                    proyeccion.Q.Y
                );

                // Mostrar la distancia minima
                lblDistancia.Text = string.Format(
                    "Distancia minima: {0:F3} unidades",
                    proyeccion.DistanciaMinima
                );

                // Activar el dibujo
                graficar = true;
                panel1.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Dibujar el plano cartesiano y los puntos
        private void panel1_Paint(
            object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            if (!graficar || proyeccion == null)
                return;

            Punto q = proyeccion.Q;

            // Obtener los limites de los puntos
            double minX = Math.Min(
                Math.Min(puntoA.X, puntoB.X),
                Math.Min(puntoP.X, q.X)
            );

            double maxX = Math.Max(
                Math.Max(puntoA.X, puntoB.X),
                Math.Max(puntoP.X, q.X)
            );

            double minY = Math.Min(
                Math.Min(puntoA.Y, puntoB.Y),
                Math.Min(puntoP.Y, q.Y)
            );

            double maxY = Math.Max(
                Math.Max(puntoA.Y, puntoB.Y),
                Math.Max(puntoP.Y, q.Y)
            );

            // Incluir el origen del plano
            minX = Math.Min(minX, 0);
            maxX = Math.Max(maxX, 0);
            minY = Math.Min(minY, 0);
            maxY = Math.Max(maxY, 0);

            // Agregar espacio alrededor del grafico
            double rangoX = Math.Max(maxX - minX, 1);
            double rangoY = Math.Max(maxY - minY, 1);

            double margenX = rangoX * 0.15;
            double margenY = rangoY * 0.15;

            minX -= margenX;
            maxX += margenX;
            minY -= margenY;
            maxY += margenY;

            // Definir el area disponible
            float margen = 45;

            float ancho = panel1.ClientSize.Width - 2 * margen;
            float alto = panel1.ClientSize.Height - 2 * margen;

            if (ancho <= 0 || alto <= 0)
                return;

            // Calcular la escala uniforme
            double escala = Math.Min(
                ancho / (maxX - minX),
                alto / (maxY - minY)
            );

            // Centrar el plano dentro del panel
            double anchoReal = (maxX - minX) * escala;
            double altoReal = (maxY - minY) * escala;

            double origenX = (panel1.ClientSize.Width - anchoReal) / 2;
            double origenY = (panel1.ClientSize.Height - altoReal) / 2;

            // Convertir coordenadas matematicas a pixeles
            Func<double, float> X = x =>
                (float)(origenX + (x - minX) * escala);

            Func<double, float> Y = y =>
                (float)(panel1.ClientSize.Height -
                origenY - (y - minY) * escala);

            // Dibujar la cuadricula
            using (Pen cuadricula = new Pen(
                Color.Gainsboro, 1))
            {
                double pasoX = PasoCuadricula(maxX - minX);
                double pasoY = PasoCuadricula(maxY - minY);

                for (double x = Math.Ceiling(minX / pasoX) * pasoX;
                     x <= maxX; x += pasoX)
                {
                    g.DrawLine(
                        cuadricula,
                        X(x), Y(minY),
                        X(x), Y(maxY)
                    );
                }

                for (double y = Math.Ceiling(minY / pasoY) * pasoY;
                     y <= maxY; y += pasoY)
                {
                    g.DrawLine(
                        cuadricula,
                        X(minX), Y(y),
                        X(maxX), Y(y)
                    );
                }
            }

            // Dibujar los ejes cartesianos
            using (Pen eje = new Pen(Color.Gray, 2))
            {
                g.DrawLine(
                    eje,
                    X(minX), Y(0),
                    X(maxX), Y(0)
                );

                g.DrawLine(
                    eje,
                    X(0), Y(minY),
                    X(0), Y(maxY)
                );
            }

            // Dibujar la trayectoria AB
            using (Pen trayectoria = new Pen(
                Color.Blue, 3))
            {
                g.DrawLine(
                    trayectoria,
                    X(puntoA.X), Y(puntoA.Y),
                    X(puntoB.X), Y(puntoB.Y)
                );
            }

            // Dibujar la distancia desde P hasta Q
            using (Pen distancia = new Pen(
                Color.Orange, 2))
            {
                distancia.DashStyle = DashStyle.Dash;

                g.DrawLine(
                    distancia,
                    X(puntoP.X), Y(puntoP.Y),
                    X(q.X), Y(q.Y)
                );
            }

            // Dibujar los puntos con sus etiquetas
            DibujarPunto(g, X(puntoA.X), Y(puntoA.Y),
                Color.Blue, "A");

            DibujarPunto(g, X(puntoB.X), Y(puntoB.Y),
                Color.Blue, "B");

            DibujarPunto(g, X(puntoP.X), Y(puntoP.Y),
                Color.Red, "P");

            DibujarPunto(g, X(q.X), Y(q.Y),
                Color.Green, "Q");
        }

        // Calcular una separacion adecuada para la cuadricula
        private double PasoCuadricula(double rango)
        {
            double paso = Math.Pow(
                10, Math.Floor(Math.Log10(rango / 8))
            );

            double cantidad = rango / paso;

            if (cantidad > 16)
                paso *= 5;
            else if (cantidad > 8)
                paso *= 2;

            return paso;
        }

        // Dibujar un punto y su nombre
        private void DibujarPunto(
            Graphics g, float x, float y,
            Color color, string nombre)
        {
            float radio = 6;

            using (SolidBrush pincel = new SolidBrush(color))
            {
                g.FillEllipse(
                    pincel,
                    x - radio,
                    y - radio,
                    radio * 2,
                    radio * 2
                );
            }

            using (Font fuente = new Font("Arial", 10,
                FontStyle.Bold))
            using (SolidBrush texto = new SolidBrush(color))
            {
                g.DrawString(
                    nombre,
                    fuente,
                    texto,
                    x + 8,
                    y - 20
                );
            }
        }

        // Limpiar los resultados y el plano
        private void btnLimpiar_Click(
            object sender, EventArgs e)
        {
            txtA.Clear();
            txtB.Clear();
            txtP.Clear();

            lblQ.Text = "Punto Q: ";
            lblDistancia.Text = "Distancia minima: ";

            puntoA = null;
            puntoB = null;
            puntoP = null;
            proyeccion = null;

            graficar = false;

            panel1.Invalidate();
            txtA.Focus();
        }
    }
}
