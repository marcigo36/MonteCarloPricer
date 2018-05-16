using System;

namespace PricerCore
{
    public class MarketCurve : IMarketCurve
    {
        public MarketCurve(ICurve impl)
        {
            implementation = impl;
        }

        private ICurve implementation;

        public double this[double t] => implementation[t];

        public double T => implementation.T;

        public ICurve Transform(Func<Point, double> f) => implementation.Transform(f);
    }
}