using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class CustomYieldCurveSource : IYieldCurveSource
    {
        public CustomYieldCurveSource(YieldCurve yc)
        {
            this.yc = yc;
        }
        YieldCurve yc;
        public YieldCurve get()
        {
            return yc;
        }
    }
}
