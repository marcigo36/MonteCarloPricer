using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class BondOption : IInstrument
    {
        public BondOption()
        {
            UnderlyingBond = new Bond();
            StrikePrice = 0.9;
            ExecutionTime = 3;
            Type = OptionType.Call;
        }
        public enum OptionType { Call, Put };

        public OptionType Type { get; set; }

        public Bond UnderlyingBond { set; get; }

        //this amount will be paid to the buyer after maturity
        public double StrikePrice { set; get; }

        //the borrower can decide if he/she can buy the given instrument at the given price
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

                double bondPriceAtStrike = UnderlyingBond.Price(process, path.Last(), ExecutionTime, step, n2);
                if (Type == OptionType.Call)
                {
                    ret.Add(Math.Max(bondPriceAtStrike - StrikePrice, 0));
                }
                else //Put
                {
                    ret.Add(Math.Max(StrikePrice - bondPriceAtStrike, 0));
                }

            }

            return ret.Average();

        }
    }
}
