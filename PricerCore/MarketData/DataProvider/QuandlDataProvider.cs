using Newtonsoft.Json.Linq;
using QuandlCS.Connection;
using QuandlCS.Interfaces;
using QuandlCS.Requests;
using QuandlCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PricerCore
{
    //not staic because of the interaface, it is a singleton instead
    public class QuandlDataProvider : IMarketDataProvider
    {
        private static readonly string Quandl_APIKey = "uQPfXBXPrqMrxoPFG4cs";

        private QuandlDataProvider() { }

        private static QuandlDataProvider instance;

        public static QuandlDataProvider Instance
        {
            get
            {
                if (instance == null) instance = new QuandlDataProvider();
                return instance;
            }
        }

        dynamic GetLastAvailableYieldCurveUntil(DateTime date)
        {
            QuandlDownloadRequest request = new QuandlDownloadRequest();
            request.APIKey = Quandl_APIKey;
            request.Datacode = new Datacode("USTREASURY", "YIELD");
            request.Format = FileFormats.JSON;
            request.Frequency = Frequencies.None;
            request.StartDate = date.AddDays(-5);
            request.EndDate = date;
            //request.Truncation = 150;
            request.Sort = SortOrders.Ascending;
            request.Transformation = Transformations.None;

            IQuandlConnection connection = new QuandlConnection();
            string data = connection.Request(request);

            dynamic dyn = JObject.Parse(data);

            //the last available data point
            dynamic lastcurve = dyn.data.Last;

            return lastcurve;
        }

        public YieldCurve GetYieldCurve()
        {
            return GetYieldCurve(DateTime.Now);
        }

        public YieldCurve GetYieldCurve(DateTime date)
        {
            dynamic lastcurve = GetLastAvailableYieldCurveUntil(date);

            var ret = new CubicSpline(
                new List<Point>
                {
                    new Point(0.0, (2*(double)lastcurve[1]-(double)lastcurve[2])/100.0),
                    new Point(1.0/12.0, (double)lastcurve[1]/100.0),
                    new Point(3.0/12.0, (double)lastcurve[2]/100.0),
                    new Point(6.0/12.0, (double)lastcurve[3]/100.0),
                    new Point(1.0, (double)lastcurve[4]/100.0),
                    new Point(2.0, (double)lastcurve[5]/100.0),
                    new Point(3.0, (double)lastcurve[6]/100.0),
                    new Point(5.0, (double)lastcurve[7]/100.0),
                    new Point(7.0, (double)lastcurve[8]/100.0),
                    new Point(10.0, (double)lastcurve[9]/100.0),
                    new Point(20.0, (double)lastcurve[10]/100.0),
                    new Point(30.0, (double)lastcurve[11]/100.0),
                }, 
                "USTREASURY/YIELD"
                );

            return new YieldCurve(ret);
        }
    }
}
