using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PricerCore
{
    //data class all data needed for a priceable instrument
    public class PricingRequest
    {
        public PricingRequest(IInstrument instrument, IYieldCurveSource ycs)
        {
            this.instrument = instrument;
            this.ycs = ycs;
        }

        IInstrument instrument;
        IYieldCurveSource ycs;

        public IYieldCurveSource Ycs { get => ycs; set => ycs = value; }
        public IInstrument Instrument { get => instrument; set => instrument = value; }

    }
}
