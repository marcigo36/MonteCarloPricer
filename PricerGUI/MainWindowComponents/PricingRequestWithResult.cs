using PricerCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricerGUI.MainWindowComponents
{
    public class PricingRequestWithResult : INotifyPropertyChanged
    {
        public PricingRequestWithResult(PricingRequest request, double? result)
        {
            Request = request;
            Result = result;
        }

        private PricingRequest request;

        public PricingRequest Request
        {
            get { return request; }
            set {
                request = value;
                Result = null;
                OnPropertyChanged("Request");
                OnPropertyChanged("Name");
                OnPropertyChanged("Summary");
                OnPropertyChanged("MainInfo");
            }
        }

        private double? result;

        public double? Result
        {
            get { return result; }
            set { result = value; OnPropertyChanged("Result"); OnPropertyChanged("FormattedResult"); }
        }

        private bool isPricingOngoing;

        public bool IsPricingOngoing
        {
            get { return isPricingOngoing; }
            set { isPricingOngoing = value; OnPropertyChanged("IsPricingOngoing"); }
        }



        public String Name
        {
            get
            {
                return Request.Instrument.GetType().Name;
            }
        }

        public String Summary
        {
            get
            {
                Type t = Request.Instrument.GetType();
                if (t == typeof(Bond))
                {
                    Bond i = (Bond)(Request.Instrument);
                    if (i.Coupon == CouponPaymentType.None)
                    {
                        return "Zero coupon";
                    }
                    else
                    {
                        return String.Format("Coupon={0}\nRate={1}", i.Coupon.ToString(),i.Rate);
                    }
                    
                }
                if (t == typeof(BondOption))
                {
                    BondOption i = (BondOption)(Request.Instrument);
                    return String.Format("T_s={0}\nType={1}\nCoupon={2}\nRate={3}", i.ExecutionTime, i.Type.ToString(), i.UnderlyingBond.Coupon.ToString(), i.UnderlyingBond.Rate);
                }
                if (t == typeof(Swap))
                {
                    Swap i = (Swap)(Request.Instrument);
                    return String.Format("Type={0}\nCoupon={1}\nFixedRate={2}", i.Type.ToString(), i.Coupon.ToString(), i.Rate);
                }
                if (t == typeof(Swaption))
                {
                    Swaption i = (Swaption)(Request.Instrument);
                    return String.Format("T_s={0}\nType={1}\nFixedRate={2}", i.ExecutionTime, i.UnderlyingSwap.Type.ToString(), i.UnderlyingSwap.Rate);
                }
                return "";
            }
        }

        public String FormattedResult
        {
            get
            {
                if (!Result.HasValue)
                {
                    return "Not priced";
                }
                else
                {
                    return Result.Value.ToString("N4");
                }
            }
        }

        public String MainInfo
        {
            get
            {
                Type t = Request.Instrument.GetType();
                if (t == typeof(Bond))
                {
                    Bond i = (Bond)(Request.Instrument);
                    return String.Format("N={0}\nT={1}",i.FaceValue, i.Maturity);
                }
                if (t == typeof(BondOption))
                {
                    BondOption i = (BondOption)(Request.Instrument);
                    return String.Format("N={0}\nT={1}", i.UnderlyingBond.FaceValue, i.UnderlyingBond.Maturity);
                }
                if (t == typeof(Swap))
                {
                    Swap i = (Swap)(Request.Instrument);
                    return String.Format("N={0}\nT={1}", i.FaceValue, i.Maturity);
                }
                if (t == typeof(Swaption))
                {
                    Swaption i = (Swaption)(Request.Instrument);
                    return String.Format("N={0}\nT={1}", i.UnderlyingSwap.FaceValue, i.UnderlyingSwap.Maturity);
                }
                return "";

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
