using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public static class Utils
    {
        public const double eps = 0.00001;
        //poor man's derive
        public static double Derive(Func<double, double> f, double x)
        {
            try
            {
                double fx0 = f(x), fx1 = f(x + eps);
                double dy = fx1 - fx0;
                double dx = eps;

                return dy / dx;
            }
            catch (ArgumentOutOfRangeException e)
            {
                double dy = f(x) - f(x - eps);
                double dx = eps;

                return dy / dx;
            }
        }
    }
}
