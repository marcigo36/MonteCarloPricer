using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class HullWhiteCalibrator
    {
        public Func<double, double> Theta { get; private set; }
        public Func<double, double> ForwardRate { get; private set; }

        
        public HullWhiteProcess Calibrate(DiscountCurve disc)
        {

            //preset parameters, calibrating these is skipped due to the lack of openly accessible market data
            //might worth implementing later
            double a = 0.31, sigma = 0.008;

            //market instantaneous forward rate
            ForwardRate = disc.ForwardRate;

            Theta = 
                
                t => Utils.Derive(x => ForwardRate(x), t) 
                   + a* ForwardRate(t) 
                   + sigma*sigma/(2.0*a*a)*(1.0 - Math.Exp(-2.0*a*t));


            Func<double, double> alpha =

                t => a * ForwardRate(t)
                   + sigma * sigma / (2.0 * a * a) * (1.0 - Math.Exp(-2.0 * a * t));



            return new HullWhiteProcess(Theta, a, sigma);
        }
    }
}
