using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class BSplineCurve : ICurve
    {
        public BSplineCurve(List<Point> Data, string Name = "")
        {
            this.Name = Name;
            if (Data.First().x != 0) throw new ArgumentException("first data point should be at zero");
            data = new List<Point>(Data);

            N = Data.Count - 1;
            A = new double[N];
            B = new double[N];
            C = new double[N];
            D = new double[N];

            this.Generate();
        }

        void Generate()     // output
        {
            if (old)
            {
                double[] w = new double[N];
                double[] h = new double[N];
                double[] ftt = new double[N + 1];

                for (int i = 0; i < N; i++)
                {
                    w[i] = (data[i + 1].x - data[i].x);
                    h[i] = (data[i + 1].y - data[i].y) / w[i];
                }

                ftt[0] = 0;
                for (int i = 0; i < N - 1; i++)
                    ftt[i + 1] = 3 * (h[i + 1] - h[i]) / (w[i + 1] + w[i]);
                ftt[N] = 0;

                for (int i = 0; i < N; i++)
                {
                    A[i] = (ftt[i + 1] - ftt[i]) / (6 * w[i]);
                    B[i] = ftt[i] / 2;
                    C[i] = h[i] - w[i] * (ftt[i + 1] + 2 * ftt[i]) / 6;
                    D[i] = data[i].y;
                }
            }
            else
            {
                double[] dw = new double[N];
                double[] dh = new double[N];
                double[] dif = new double[N + 1];

                for (int i = 0; i < N; i++)
                {
                    dw[i] = data[i + 1].x - data[i].x;
                    dh[i] = data[i + 1].y - data[i].y;
                }

                dif[0] = dh[0] / dw[0];
                dif[0] = dh[N - 1] / dw[N - 1];
                for (int i = 1; i < N; i++)
                {
                    dif[i] = (dh[i - 1] + dh[i]) / (dw[i - 1] + dw[i]);
                }

                for (int i = 0; i < N; i++)
                {               //abcd solution to downscaled problem       //polinomyals with downscaled x axis   //scaling up again
                    //F[i] = t =>   (dif[i+1] + dif[i] - 2.0*dh[i]/dw[i])     * Math.Pow((t - data[i].x) / dw[i], 3) * dw[i]
                    //            + (3.0*dh[i]/dw[i] - 2.0*dif[i] - dif[i+1]) * Math.Pow((t - data[i].x) / dw[i], 2) * dw[i]
                    //           + dif[i]                                    *         ((t - data[i].x) / dw[i])    * dw[i]
                    //            + data[i].y; //translate to the correct height
                    A[i] = (dif[i + 1] + dif[i] - 2.0 * dh[i] / dw[i]) * dw[i];
                    B[i] = (3.0 * dh[i] / dw[i] - 2.0 * dif[i] - dif[i + 1]) * dw[i];
                    C[i] = dif[i] * dw[i];
                    D[i] = data[i].y;
                }
            }
        }

        //yearfraction -> discount
        private List<Point> data;
        int N;
        double[] A, B, C, D;
        bool old = false;


        public double this[double t]
        {
            get
            {
                if (t > T) throw new ArgumentOutOfRangeException("t is too big to be defined by this BSplineCurve");
                if (t < 0.0) throw new ArgumentOutOfRangeException("negative t is not defined by this BSplineCurve");
                if (t == T) t -= 0.000001;

                double x0 = 0, x1 = 0, f0 = 0, f1 = 0;
                int i = -1;

                foreach (Point point in data)
                {
                    if (point.x > t)
                    {
                        x1 = point.x;
                        f1 = point.y;
                        break;
                    }
                    else
                    {
                        x0 = point.x;
                        f0 = point.y;
                        i++;
                    }
                }

                //cubic interpolation
                double dw = data[i + 1].x - data[i].x;
                double t_rescaled = (t - x0) / (old?1.0:dw);
                double Y = A[i] * Math.Pow(t_rescaled, 3) + B[i] * Math.Pow(t_rescaled, 2) + C[i] * (t_rescaled) + D[i];
                return f0 + (Y - f0) * dw;
            }
        }

        public double T
        {
            get => data.Max(e => e.x);
        }

        public string Name { get; private set; }

        public ICurve Transform(Func<Point, double> f)
        {
            var newdata = data.Select(p => new Point(p.x, f(p))).ToList();

            return new BSplineCurve(newdata);
        }
    }
}
