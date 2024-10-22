using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.DataHandling.DataObjects;

namespace TourneyPal.Service
{
    public class TourneyPalService: ITourneyPalService
    {
        public string ping() 
        {
            return "Hello!";
        }

        public void initializeData()
        {
            GeneralData.GeneralDataInitialize();
        }
    }
}
