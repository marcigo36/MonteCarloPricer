using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class HullWhiteProcess : IProcess
    {
        public class State : IState
        {
            public State(double r)
            {
                this.r = r;
            }

            public double r { get; set; }
        }
        public HullWhiteProcess(Func<double, double> theta, double a, double sigma)
        {
            this.a = a;
            this.sigma = sigma;
            this.theta = theta;                                                                  
        }

        private double a, sigma;
        private Func<double, double> theta;

        public IState this[double t]
        {
            get
            {
                const double defaultTimeStep = 0.0001;

                return generatePath(defaultTimeStep, t).Last();
            }
        }

        public IEnumerable<IState> generatePath(double step, double maxt)
        {
            return generatePathFrom(new State(0), 0, step, maxt);
        }

        public IEnumerable<IState> generatePathFrom(IState X0,double t0, double step, double maxt)
        {
            double t = t0;
            double r = X0.r;

            for (; t < maxt; t += step)
            {
                yield return new State(r);

                //step by step solution to the HW formula
                r += (theta(t) - a * r) * step + NormalDistribution.GetValue(0.0, sigma * step);
            }
        }
    }   
}
