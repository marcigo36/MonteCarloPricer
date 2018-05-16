using OxyPlot;
using OxyPlot.Wpf;
using PricerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConvergenceTests
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        DiscountCurve dc;
        const int n = 200;
        const double step = 0.01;
        IProcess process;

        private void generatePlot()
        {
            dc = (DiscountCurve)QuandlDataProvider.Instance.GetYieldCurve();
            process = RSCalibrator.Calibrate(dc);

            //ZBP1/////////////////////////////////////////////////////////
            Bond b = new Bond() { FaceValue = 1, Maturity = 1, Rate = 0.0, Coupon = CouponPaymentType.None };
            double[] bondPrices1 = BondPrices(b);

            var bond1 = AvgConvergencePlot(bondPrices1);    

            double b1base = dc[1.0];
            var bond1base = new List<DataPoint>();
            bond1base.Add(new DataPoint(1, b1base));
            bond1base.Add(new DataPoint(n, b1base));

            var bond1error = bond1.Select(p => new DataPoint(p.X, Math.Abs(p.Y - b1base)/ b1base * 100)).ToList();
            /////////////////////////////////////////////////////////////

            //ZBP10/////////////////////////////////////////////////////////
            Bond b2 = new Bond() { FaceValue = 1, Maturity = 10, Rate = 0.0, Coupon = CouponPaymentType.None };
            double[] bondPrices2 = BondPrices(b2);

            var bond2 = AvgConvergencePlot(bondPrices2);

            double b2base = dc[10.0];
            var bond2base = new List<DataPoint>();
            bond2base.Add(new DataPoint(1, b2base));
            bond2base.Add(new DataPoint(n, b2base));

            var bond2error = bond2.Select(p => new DataPoint(p.X, Math.Abs(p.Y - b2base) / b2base * 100)).ToList();
            /////////////////////////////////////////////////////////////


            Bond b3 = new Bond() { FaceValue = 1, Maturity = 5, Rate = 0.02, Coupon = CouponPaymentType.Annual };
            var bond = AvgConvergencePlot(BondPrices(b3));

            BondOption bo = new BondOption() { ExecutionTime = 3, StrikePrice = 0.8, Type=BondOption.OptionType.Call, UnderlyingBond = b3 };
            var bondoption = AvgConvergencePlot(BondOptionPrices(bo));

            Swap sw = new Swap() { FaceValue = 1, Maturity = 5, Rate = 0.02, Coupon = CouponPaymentType.Annual, Type=Swap.SwapType.Receiver };
            var swap = AvgConvergencePlot(SwapPrices(sw));

            Swaption swo = new Swaption() { ExecutionTime = 3, UnderlyingSwap = sw };
            var swaption = AvgConvergencePlot(SwaptionPrices(swo));

            /////////////////////////////////////////////////////////////////

            Bond b4 = new Bond() { FaceValue = 1, Maturity = 5, Rate = 0.002, Coupon = CouponPaymentType.Annual };
            BondOption bo2 = new BondOption() { ExecutionTime = 3, Type = BondOption.OptionType.Call, UnderlyingBond = b3 };

            var boladder = new List<DataPoint>();
            //for (double sp = 0.9; sp < 1.0; sp += 0.005)
            //{
            //    bo2.StrikePrice = sp;
            //    double price = BondOptionPrices(bo2).Average();
            //    boladder.Add(new DataPoint(sp, price));

            //}

            //WIENER////

            WienerProcess wp = new WienerProcess(0, 1);

            double t = -1;
            var w1 = new List<DataPoint>(wp.generatePathFrom(new WienerProcess.State(0), 0, 1, 100).Select(fx => new DataPoint(t += 1, fx.r)));
            t = -1;
            var w2 = new List<DataPoint>(wp.generatePathFrom(new WienerProcess.State(0), 0, 1, 100).Select(fx => new DataPoint(t += 1, fx.r)));
            t = -1;
            var w3 = new List<DataPoint>(wp.generatePathFrom(new WienerProcess.State(0), 0, 1, 100).Select(fx => new DataPoint(t += 1, fx.r)));
            t = -1;
            var w4 = new List<DataPoint>(wp.generatePathFrom(new WienerProcess.State(0), 0, 1, 100).Select(fx => new DataPoint(t += 1, fx.r)));
            t = -1;
            var w5 = new List<DataPoint>(wp.generatePathFrom(new WienerProcess.State(0), 0, 1, 100).Select(fx => new DataPoint(t += 1, fx.r)));
            //////////

            ////GAUSS///////

            Func<double, double, double, double> nrd = (nu, sigma, x) => {
                return 1.0/Math.Sqrt(2*Math.PI*sigma*sigma)*Math.Exp(-(x-nu)*(x-nu)/(2*sigma*sigma));
            };

            var nrd1 = new List<DataPoint>();
            var nrd2 = new List<DataPoint>();
            var nrd3 = new List<DataPoint>();
            var nrd4 = new List<DataPoint>();

            for (double x = -5; x < 5; x+=0.05)
            {
                nrd1.Add(new DataPoint(x, nrd(0, 1, x)));
                nrd2.Add(new DataPoint(x, nrd(0, 0.5, x)));
                nrd3.Add(new DataPoint(x, nrd(0, 0.1, x)));
                nrd4.Add(new DataPoint(x, nrd(-2, 0.4, x)));
            }

            ////////////////

            this.DataContext = new
            {
                bond1 = bond1,
                bond1base = bond1base,
                bond1error = bond1error,
                bond2 = bond2,
                bond2base = bond2base,
                bond2error = bond2error,
                bond = bond,
                bondoption=bondoption,
                swap = swap,
                swaption = swaption,
                boladder = boladder,
                w1=w1,
                w2=w2,
                w3=w3,
                w4=w4,
                w5=w5,
                nrd1=nrd1,
                nrd2=nrd2,
                nrd3=nrd3,
                nrd4=nrd4
            };
        }

        private static List<DataPoint> AvgConvergencePlot(double[] prices)
        {
            var ret = new List<DataPoint>();
            double avg = prices[0];
            ret.Add(new DataPoint(1, avg));
            for (int i = 1; i < prices.Length; i++)
            {
                avg *= i / (i + 1.0);
                avg += 1.0 / (i + 1.0) * prices[i];

                ret.Add(new DataPoint(i + 1, avg));
            }

            return ret;
        }

        public double[] BondPrices(Bond b)
        { 
            double[] ret = new double[n];
            double T = b.Maturity;

            double r0 = dc.ForwardRate(0.01);
            List<IEnumerable<IState>> paths = Enumerable.Range(0, n).Select(_ => process.generatePathFrom(new RSProcess.State(r0,0),0.0,step, T)).ToList();

            double[] coupons = new double[0];
            double[] coupontimes = new double[0];

            switch (b.Coupon)
            {
                case CouponPaymentType.None:
                    //no coupon payments
                    break;

                case CouponPaymentType.Annual:
                    int cpn = (T % 1 == 0 ? (int)(T - 1) : (int)T);
                    coupons = new double[cpn];
                    coupontimes = new double[cpn];
                    for (int i = 1; i <= coupons.Length; i++)
                    {
                        coupontimes[i - 1] = i;
                    }
                    break;


                case CouponPaymentType.SemiAnnual:
                    cpn = ((2.0 * T) % 1 == 0 ? (int)((2.0 * T) - 1) : (int)(2.0 * T));
                    coupons = new double[cpn];
                    coupontimes = new double[cpn];
                    for (int i = 1; i <= coupons.Length; i++)
                    {
                        coupontimes[i - 1] = 0.5 * i;
                    }
                    break;

                case CouponPaymentType.Quarterly:
                    cpn = ((4.0 * T) % 1 == 0 ? (int)((4.0 * T) - 1) : (int)(4.0 * T));
                    coupons = new double[cpn];
                    coupontimes = new double[cpn];
                    for (int i = 1; i <= coupons.Length; i++)
                    {
                        coupontimes[i - 1] = 0.25 * i;
                    }
                    break;

                default:
                    //no coupon payments
                    break;
            }

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
                    if (nextcouponindex < coupontimes.Count() && coupontimes[nextcouponindex] < t)
                    {
                        coupons[nextcouponindex] = zbp * b.FaceValue * b.Rate * b.CouponInterval;
                        nextcouponindex++;
                    }
                }

                //these coupons are now at present value
                ret[reti] = b.FaceValue * zbp + coupons.Sum();

                reti++;

            }

            return ret;
        }

        public double[] BondOptionPrices(BondOption bo)
        {
            var ret = new List<double>();

            double r0 = dc.ForwardRate(0.01);
            List<IEnumerable<IState>> pathsToStrike = Enumerable.Range(0, n).Select(_ => process.generatePathFrom(new RSProcess.State(r0, 0), 0, step, bo.ExecutionTime)).ToList();

            foreach (var path in pathsToStrike)
            {
                double zbp = 1;
                foreach (var x in path)
                {
                    zbp *= Math.Exp(-x.r * step);
                }

                double bondPriceAtStrike = bo.UnderlyingBond.Price(process, path.Last(), bo.ExecutionTime, step, (int)Math.Sqrt(n));
                if (bo.Type == BondOption.OptionType.Call)
                {
                    ret.Add(Math.Max( bondPriceAtStrike - bo.StrikePrice, 0));
                }
                else //Put
                {
                    ret.Add(Math.Max(bo.StrikePrice - bondPriceAtStrike, 0));
                }

            }

            return ret.ToArray();

        }

        public double[] SwapPrices(Swap sw)
        {
            Bond b = new Bond
            {
                FaceValue = sw.FaceValue,
                Maturity = sw.Maturity,
                Coupon = sw.Coupon,
                Rate = sw.Rate
            };

            if (sw.Type == Swap.SwapType.Receiver)
            {
                return BondPrices(b).Select(p => (p - sw.FaceValue)).ToArray();
            }
            else
            {
                return BondPrices(b).Select(p => (sw.FaceValue - p)).ToArray();
            }

        }

        public double[] SwaptionPrices(Swaption swo)
        {
            var ret = new List<double>();

            double r0 = dc.ForwardRate(0.01);
            List<IEnumerable<IState>> pathsToStrike = Enumerable.Range(0, n).Select(_ => process.generatePathFrom(new RSProcess.State(r0, 0), 0, step, swo.ExecutionTime)).ToList();

            foreach (var path in pathsToStrike)
            {
                double zbp = 1;
                foreach (var x in path)
                {
                    zbp *= Math.Exp(-x.r * step);
                }

                ret.Add(
                    Math.Max(swo.UnderlyingSwap.Price(process, path.Last(), swo.ExecutionTime, step, (int)Math.Sqrt(n)), 0) * zbp
                    );
            }


            return ret.ToArray();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            generatePlot();
        }

        private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var thisTab = (TabItem)sender;

            var pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };
            pngExporter.ExportToFile(((Plot)thisTab.Content).ActualModel, "C:\\Users\\marcigo\\Desktop\\szakdoga\\diploma\\figures\\" + (string)thisTab.Header + "_plot.png");
        }
    }
}
