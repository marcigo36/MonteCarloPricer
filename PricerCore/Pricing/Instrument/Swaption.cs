using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class Swaption : IInstrument
    {
        public Swaption()
        {
            UnderlyingSwap = new Swap();
            ExecutionTime = 3;
        }

        public Swap UnderlyingSwap { set; get; }

        public double ExecutionTime { set; get; }


        public double Price(IProcess process, IState st0, double t0, double step, int n)
        {
            var ret = new List<double>();


            List<IEnumerable<IState>> pathsToStrike = Enumerable.Range(0, n).Select(_ => process.generatePathFrom(st0, t0, step, ExecutionTime)).ToList();

            int n2 = (int)Math.Sqrt(n);
            foreach (var path in pathsToStrike)
            {
                double zbp = 1;
                foreach (var x in path)
                {
                    zbp *= Math.Exp(-x.r * step);
                }

                ret.Add(
                    Math.Max(UnderlyingSwap.Price(process, path.Last(), ExecutionTime, step, n2), 0) * zbp
                    );
            }


            return ret.Average();
        }
    }
}
