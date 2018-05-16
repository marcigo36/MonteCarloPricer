using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class Bond : IInstrument
    {
        public Bond()
        {
            FaceValue = 1;
            Maturity = 5;
            Coupon = CouponPaymentType.Annual;
            Rate = 0.01;
        }

        //this amount will be paid to the buyer after maturity
        public double FaceValue { set; get; }

        //the instrument will mature after this many years from now
        public double Maturity { set; get; }

        public CouponPaymentType Coupon { set; get; }

        //yearly rate to be paid
        public double Rate { set; get; }

        public double CouponInterval {
            get
            {
                switch (Coupon)
                {
                    case CouponPaymentType.Annual:
                        return 1;
                    case CouponPaymentType.SemiAnnual:
                        return 0.5;
                    case CouponPaymentType.Quarterly:
                        return 0.25;
                    default:
                        return 0;
                }
            }
        }

        double[] generateCouponTimes(double T)
        {
            int howManyCoupons = 0;

            switch (Coupon)
            {
                case CouponPaymentType.None:
                    //no coupon payments
                    return new double[0];

                case CouponPaymentType.Annual:
                    howManyCoupons = (T % 1 == 0 ? (int)(T - 1) : (int)T);
                    break;
                case CouponPaymentType.SemiAnnual:
                    howManyCoupons = ((2.0 * T) % 1 == 0 ? (int)((2.0 * T) - 1) : (int)(2.0 * T));
                    break;
                case CouponPaymentType.Quarterly:
                    howManyCoupons = ((4.0 * T) % 1 == 0 ? (int)((4.0 * T) - 1) : (int)(4.0 * T));
                    break;
                default:
                    //no coupon payments
                    return new double[0];
            }

            double[] coupontimes = new double[howManyCoupons];
            for (int i = 1; i <= coupontimes.Length; i++)
            {
                coupontimes[i - 1] = CouponInterval * i;
            }
            return coupontimes;
        }

        public double Price(IProcess process, IState st0, double t0, double step, int n)
        {
            double[] ret = new double[n];
            double T = Maturity;

            List<IEnumerable<IState>> paths = Enumerable.Range(0, n).Select(_ => process.generatePathFrom(st0,t0,step, T)).ToList();

            double[] couponTimes = generateCouponTimes(T);
            double[] couponsValues = new double[0];

            int reti = 0;
            foreach (var path in paths)
            {
                double zbp = 1;
                double t = 0;
                int nextcouponindex = 0;
                foreach (var x in path)
                {
                    t += step;
                    zbp *= Math.Exp(-x.r * step);
                    if (nextcouponindex < couponTimes.Count() && couponTimes[nextcouponindex] < t)
                    {
                        couponsValues[nextcouponindex] = zbp * FaceValue * Rate * CouponInterval;
                        nextcouponindex++;
                    }
                }

                //these coupons are now at present value
                ret[reti] = FaceValue * zbp + couponsValues.Sum();

                reti++;

            }

            return ret.Average();
        }
    }
}
