//==========================================================
// Student Number : S10266121F
// Student Name : Ooi Jing Shun
// Partner Name : Ranen Sim
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    class CFFTFlight : Flight
    {

        public CFFTFlight(string flightNo, string origin, string dest, DateTime et, string status) : base(flightNo, origin, dest, et, status)
        {
            RequestFee = 150;
        }

        public double RequestFee { get; set; }
        public override double CalculateFees()
        {
            return base.CalculateFees() + RequestFee;

        }
        
        public override string ToString()
        {
            return base.ToString() + $" RequestFee: {RequestFee}";
        }

    }
}
