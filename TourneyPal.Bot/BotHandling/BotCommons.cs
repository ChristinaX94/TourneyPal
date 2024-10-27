using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Service;

namespace TourneyPal.BotHandling
{
    public static class BotCommons
    {
        public static ITourneyPalService service { get; set; } = default!;
    }
}
