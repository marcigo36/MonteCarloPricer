using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public interface IProcess
    {
        IState this[double t] { get; }

        IEnumerable<IState> generatePath(double step, double maxt);

        IEnumerable<IState> generatePathFrom(IState X0, double t0, double step, double maxt);
    }
}
