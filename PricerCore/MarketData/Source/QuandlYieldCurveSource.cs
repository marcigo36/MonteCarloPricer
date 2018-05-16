using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class QuandlYieldCurveSource : IYieldCurveSource
    {
        public QuandlYieldCurveSource(DateTime when)
        {
            this.when = when;
        }

        DateTime when;
        public YieldCurve get()
        {
            return QuandlDataProvider.Instance.GetYieldCurve(when);
        }
    }
}
