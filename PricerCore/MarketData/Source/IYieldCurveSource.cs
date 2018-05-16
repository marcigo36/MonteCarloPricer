using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public interface IYieldCurveSource
    {
        YieldCurve get();
    }
}
