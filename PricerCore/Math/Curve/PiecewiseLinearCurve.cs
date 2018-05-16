using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class PiecewiseLinearCurve : ICurve
    {
        public PiecewiseLinearCurve(List<Point> Data, string Name = "")
        {
            this.Name = Name;
            if(Data.First().x != 0) throw new ArgumentException("first data point should be at zero");
            data = new List<Point>(Data);
        }

        //yearfraction -> discount
        private List<Point> data;

        public double this[double t]
        {
            get
            {
                if (t > T) throw new ArgumentOutOfRangeException("t is too big to be defined by this PiecewiseLinearCurve");
                if (t < 0.0) throw new ArgumentOutOfRangeException("negative t is not defined by this PiecewiseLinearCurve");

                double x0 = 0, x1 = 0, f0 = 0, f1 = 0;

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
                    }
                }

                //linear interpolation
                return ( f0*(x1-t) + f1*(t-x0) ) / (x1-x0);
            }
        }

        public double T
        {
            get => data.Max(e => e.x);
        }

        public string Name { get; private set; }

        public ICurve Transform(Func<Point, double> f)
        {
            var newdata = data.Select( p => new Point( p.x,f(p)) ).ToList();

            return new PiecewiseLinearCurve(newdata);
        }
    }
}
