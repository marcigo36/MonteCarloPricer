using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class WienerProcess : IProcess
    {
        public class State : IState
        {
            public State(double r)
            {
                this.r = r;
            }

            public double r { get; set; }
        }

        public WienerProcess(double nu, double sigma)
        {
            Nu = nu;
            Sigma = sigma;
        }

        public double Nu { get; set; }

        public double Sigma { get; set; }

        public IState this[double t]
        {
            get
            {
                return new State( new NormalDistribution( Nu, Sigma*t).NextVal() );
            }
        }

        public IEnumerable<IState> generatePath(double step, double maxt)
        {
            return generatePathFrom(new State(Nu), 0, step, maxt);
        }

        public IEnumerable<IState> generatePathFrom(IState X0, double t0, double step, double maxt)
        {
            double t = t0;
            double X = X0.r;

            for (; t < maxt; t += step)
            {
                yield return new State(X);

                X += NormalDistribution.GetValue(0.0, Sigma * step);
            }
        }
    }
}
