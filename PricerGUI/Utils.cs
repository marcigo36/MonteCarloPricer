using OxyPlot;
using PricerCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PricerGUI
{
    public class Utils
    {
        public static IEnumerable<DataPoint> Plot(Func<double, double> f, double from, double to, double step)
        {
            for (double t = from; t <= to; t += step)
            {
                yield return new DataPoint(t, f(t));
            }
        }

        public static IEnumerable<DataPoint> Plot(ICurve c, double from, double to, double step)
        {
            for (double t = from; t <= to; t += step)
            {
                yield return new DataPoint(t, c[t]);
            }
        }

        //deep clone from msdn
        public static Object DeepClone(Object original)
        {
            // Construct a temporary memory stream
            MemoryStream stream = new MemoryStream();

            // Construct a serialization formatter that does all the hard work
            BinaryFormatter formatter = new BinaryFormatter();

            // This line is explained in the "Streaming Contexts" section
            formatter.Context = new StreamingContext(StreamingContextStates.Clone);

            // Serialize the object graph into the memory stream
            formatter.Serialize(stream, original);

            // Seek back to the start of the memory stream before deserializing
            stream.Position = 0;

            // Deserialize the graph into a new set of objects
            // and return the root of the graph (deep copy) to the caller
            return (formatter.Deserialize(stream));
        }
    }
}
