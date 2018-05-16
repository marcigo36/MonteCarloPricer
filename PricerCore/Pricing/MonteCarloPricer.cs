using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PricerCore
{
    public class MonteCarloPricer
    {
        public MonteCarloPricer(double step, int n)
        {
            this.step = step;
            this.n = n;
        }

        double step;
        int n;

        public double Price(PricingRequest request)
        {
            DiscountCurve dc = (DiscountCurve)request.Ycs.get();
            IProcess calibratedProcess = RSCalibrator.Calibrate(dc);

            double r0 = dc.ForwardRate(0.01);
            return request.Instrument.Price(calibratedProcess,new RSProcess.State(r0,0),0,step,n);
        }

    }
}
