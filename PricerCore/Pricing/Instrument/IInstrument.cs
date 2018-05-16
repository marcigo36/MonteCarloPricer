using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public interface IInstrument
    {
        double Price(IProcess process, IState st0, double t0, double step, int n);
    }
}
