using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSolid.Cad.Design.Automating;
using TopSolid.Cad.Drafting.Automating;
using TopSolid.Kernel.Automating;

namespace Outils
{
    public class BoutonBase
    {
        public static void StartTs()
        {
            TopSolidHost.Connect();
            TopSolidDesignHost.Connect();
            TopSolidDraftingHost.Connect();
        }

        public static void StopTs()
        {
            TopSolidDraftingHost.Disconnect();
            TopSolidDesignHost.Disconnect();
            TopSolidHost.Disconnect();
        }
    }
}
