using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class YieldCurve : MarketCurve
    {
        public YieldCurve(ICurve impl) : base(impl) { }

        public static explicit operator DiscountCurve(YieldCurve _this)
        {
            return new DiscountCurve( _this.Transform( p => 1.0 / (1.0 + p.x*p.y ) ) );
        }

        //plottable, for debug
        public double F(double x) { return this[x]; }
    }
}
