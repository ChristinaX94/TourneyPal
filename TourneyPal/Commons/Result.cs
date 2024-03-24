using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.Commons
{
    public class Result
    {
        public bool success;
        public string message;
        public object obj;

        public Result(bool success = false, string message = "")
        {
            this.success = success;
            this.message = message;
        }

    }
}
