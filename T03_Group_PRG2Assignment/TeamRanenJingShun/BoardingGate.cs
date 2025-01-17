using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    internal class BoardingGate
    {
        public string GateName { get; set; }
        public bool SupportsCFFT { get; set; }
        public bool SupportsDDJB { get; set; }
        public bool SupportsLWTT { get; set; }

        public Flight Flight { get; set; }

        public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT)
        {
            GateName = gateName;
            SupportsCFFT = supportsCFFT;
            SupportsDDJB = supportsDDJB;
            SupportsLWTT = supportsLWTT;
        }

        public double CalculateFees()
        {
            double total = 0;

            if (SupportsCFFT && Flight.GetType() == typeof(CFFTFlight))
            {
                CFFTFlight cfftFlight = (CFFTFlight)Flight;
                total += cfftFlight.CalculateFees();
            }
            else if (SupportsDDJB && Flight.GetType() == typeof(DDJBFlight))
            {
                DDJBFlight ddjbFlight = (DDJBFlight)Flight;
                total += ddjbFlight.CalculateFees();
            }
            else if (SupportsLWTT && Flight.GetType() == typeof(LWTTFlight))
            {
                LWTTFlight lwttFlight = (LWTTFlight)Flight;
                total += lwttFlight.CalculateFees();
            }

            return total;
        }

        public override string ToString()
        {
            return "Gate Name: " + GateName + "\tSupports CFFT: " + SupportsCFFT + "\tSupports DDJB: " + SupportsDDJB + "\tSupports LWTT: " + SupportsLWTT + Flight.ToString();
        }
    }
}
