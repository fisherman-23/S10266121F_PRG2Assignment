//==========================================================
// Student Number : S10267339B
// Student Name : Ranen Sim
// Partner Name : Ooi Jing Shun
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    internal class LWTTFlight : Flight
    {
        public double RequestFee { get; set; }
        public LWTTFlight(string flightNo, string origin, string dest, DateTime et, string status) : base(flightNo, origin, dest, et, status)
        {
            RequestFee = 500;
        }
        public override double CalculateFees()
        {
            return base.CalculateFees() + RequestFee;
        }

        public override string ToString()
        {
            return base.ToString() + " RequestFee: " + RequestFee;
        }
    }
}
