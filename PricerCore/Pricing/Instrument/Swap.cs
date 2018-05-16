using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class Swap : IInstrument
    {
        public Swap()
        {
            FaceValue = 1;
            Maturity = 5;
            Coupon = CouponPaymentType.Annual;
            Rate = 0.01;
        }
        //payment direction refers to the fixed leg
        public enum SwapType { Receiver, Payer }

        public SwapType Type { get; set; }

        //this amount will be paid to the buyer after maturity
        public double FaceValue { set; get; }

        //the instrument will mature after this many years from now
        public double Maturity { set; get; }

        public CouponPaymentType Coupon { set; get; }

        //yearly rate to be paid
        public double Rate { set; get; }

        public double Price(IProcess process, IState st0, double t0, double step, int n)
        {
            Bond b = new Bond {
                FaceValue = FaceValue,
                Maturity = Maturity,
                Coupon = Coupon,
                Rate = Rate
            };

            if (Type == SwapType.Receiver)
            {
                return b.Price(process, st0, t0, step, n) - FaceValue;
            }
            else
            {
                return FaceValue - b.Price(process, st0, t0, step, n);
            }
            
        }
    }
}
