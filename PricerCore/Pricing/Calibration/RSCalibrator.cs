using PricerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerCore
{
    public class RSCalibrator
    {

        public static RSProcess Calibrate(DiscountCurve dc)
        {
            return new RSProcess(0.31,0.008,dc.ForwardRate);
        }
    }
}
