using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public interface ICurve
    {
        //point on a curve
        double this[double t]
        {
            get;
        }

        //last defined point
        double T
        {
            get;
        }

        ICurve Transform(Func<Point, double> f);
    }
}
