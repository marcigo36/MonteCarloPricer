using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    //https://www.math.ntnu.no/emner/TMA4215/2008h/cubicsplines.pdf
    public class CubicSpline : ICurve
    {
        public CubicSpline(List<Point> Data, string Name = "")
        {
            this.Name = Name;
            if (Data.First().x != 0) throw new ArgumentException("first data point should be at zero");
            data = new List<Point>(Data);

            N = data.Count - 1;
            h = new double[N+1];
            b = new double[N+1];
            v = new double[N+1];
            u = new double[N+1];
            z = new double[N+1];

            for (int i = 0; i <= N - 1; i++)
            {
                h[i] = data[i + 1].x - data[i].x;
                b[i] = (data[i + 1].y - data[i].y)/ h[i];
                if (i > 0)
                {
                    v[i] = 2 * (h[i - 1] + h[i]);
                    u[i] = 6 * (b[i] - b[i-1]);
                }
            }
            z[0] = 0;
            z[N] = 0;
            var zz = TridiagonalSolver(
                h.ToList().GetRange(1, N - 2).ToArray(),
                v.ToList().GetRange(1, N - 1).ToArray(),
                h.ToList().GetRange(1, N - 2).ToArray(),
                u.ToList().GetRange(1, N - 1).ToArray()
                );
            for (int i = 1; i <= N - 1; i++)
            {
                z[i] = zz[i-1];
            }

        }


        private List<Point> data;
        int N;
        double[] h, b, v, u, z;

        private static double[] TridiagonalSolver(double[] a, double[] d, double[] c, double[] b)
        {
            int N = d.Length;

            double[] x = new double[N];

            //lower diagonal
            for (int i = 1; i <= N-1; i++)
            {
                double m = a[i - 1] / d[i - 1];
                d[i] = d[i] - m * c[i - 1];
                b[i] = b[i] - m * b[i - 1];
            }

            //backward substitution
            x[N-1] = b[N-1] / d[N-1];
            for (int i = N-2; i >= 0; i--)
            {
                x[i] = (b[i] - c[i] * x[i + 1]) / d[i];
            }

            return x;
        }

        public double this[double t]
        {
            get
            {
                if (t > T) throw new ArgumentOutOfRangeException("t is too big to be defined by this BSplineCurve");
                if (t < 0.0) throw new ArgumentOutOfRangeException("negative t is not defined by this BSplineCurve");
                if (t == T) t -= 0.000001;

                double x0 = 0, x1 = 0, f0 = 0, f1 = 0;
                int intervalIndex = -1;

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
                        intervalIndex++;
                    }
                }

                Func<int, double, double> S = (i, x) 
                    => z[i + 1] / 6 / h[i] * Math.Pow(x - data[i].x, 3) 
                    +  z[i] / 6 / h[i] * Math.Pow(data[i+1].x - x, 3)
                    +  (data[i+1].y/h[i] - z[i+1] / 6 * h[i]) * (x - data[i].x)
                    +  (data[i].y / h[i] - z[i] / 6 * h[i]) * (data[i+1].x - x);

                //cubic interpolation
                return S(intervalIndex, t);
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

            return new CubicSpline(newdata);
        }
    }
}
