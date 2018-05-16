using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    class NormalDistribution
    {
        public NormalDistribution(double Nu, double Sigma)
        {
            nu = Nu;
            sigma = Sigma;
        }

        private double nu, sigma;
        private static readonly Random rnd = new Random((int)System.DateTime.Now.Ticks);

        public double Nu { get => nu; }
        public double Sigma { get => sigma; }

        public double NextVal()
        {
            return GetValue(nu, sigma);
        }

        public static double GetValue(double Nu, double Sigma)
        {
            //Box-Muller method
            double U = rnd.NextDouble();
            double V = rnd.NextDouble();

            double val = Math.Sqrt(-2.0 * Math.Log(U))
                        * Math.Cos(2.0 * Math.PI * V);

            return Nu + val*Sigma;
        }
    }
}
