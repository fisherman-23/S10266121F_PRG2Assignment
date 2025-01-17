using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamRanenJingShun
{
    internal class DDJBFlight : Flight
    {
        public double RequestFee { get; set; }

        public DDJBFlight(string flightNo, string origin, string dest, DateTime et, string status) : base(flightNo, origin, dest, et, status)
        {
            RequestFee = 300;
        }
        public override double CalculateFees()
        {
            return base.CalculateFees() + RequestFee;
        }

        public override string ToString()
        {
            return base.ToString() + "RequestFee: " + RequestFee;
        }

    }
}
