using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    //one factor hjm
    public class RSProcess : IProcess
    {
        public class State : IState
        {
            public double r { get; set; }

            public double phi { get; set; }

            public State(double r, double phi)
            {
                this.r = r;
                this.phi = phi;
            }
        }

        public RSProcess(double kappa, double eta, Func<double, double> f)
        {
            this.kappa = kappa;
            this.eta = eta;
            this.f = f;
        }

        public IState this[double t]
        {
            get
            {
                const double defaultTimeStep = 0.0001;

                return generatePath(defaultTimeStep, t).Last();
            }
        }

        private double kappa, eta;
        private Func<double, double> f;

        public IEnumerable<IState> generatePath(double step, double maxt)
        {
            return generatePathFrom(new State(0,0), 0, step, maxt);
        }

        public IEnumerable<IState> generatePathFrom(IState X0_, double t0, double step, double maxt)
        { 
            double t = t0;
            State X0 = (State)X0_;
            double r = X0.r;
            double phi = X0.phi;

            for (; t < maxt; t += step)
            {
                yield return new State(r,phi);

                //step by step solution to the RS formula
                double nu = kappa * (f(t) - r) + phi + Utils.Derive(f, t);
                r += nu * step + NormalDistribution.GetValue(0.0, eta * step);
                phi += (eta * eta - 2 * kappa * phi) * step;
            }
        }
    }
}
