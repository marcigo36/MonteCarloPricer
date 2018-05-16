using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class DiscountCurve : MarketCurve
    {
        public DiscountCurve(ICurve impl) : base(impl) { }

        public static explicit operator YieldCurve(DiscountCurve _this)
        {
            return new YieldCurve(_this.Transform(p => (1.0 - p.x) / (p.y * p.x)));
        }

        public Func<double, double> ForwardRate
        {
            get
            {
                return T => -(this[T + Utils.eps] - this[T]) / (this[T + Utils.eps] * Utils.eps);
            }
        }
    }
}
  