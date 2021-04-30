using LogDebugging;
using Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSolid.Cad.Design.Automating;
using TopSolid.Kernel.Automating;

namespace MajProprietes
{
    using Ts = TopSolidHost;
    using Tsd = TopSolidDesignHost;

    class MajProprietes : BoutonBase
    {
        private static DocumentId docId;

        static void Main(string[] args)
        {
            StartTs();

            var Dcs = Ts.Documents;

            docId = Dcs.EditedDocument;

            if (!TopSolidHost.Application.StartModification("My Action", false)) return;

            try
            {
                Log.Message("Modification");

                TopSolidHost.Documents.EnsureIsDirty(ref docId);

                Tsd.Parts.SetPhysicalPropertiesManagementRefreshAuto(docId, true);
                Tsd.Parts.SetMassPropertyManagement(docId, true, null);
                Tsd.Parts.SetCenterOfMassPropertyManagement(docId, true, null);

                Tsd.Parts.SetSurfaceAreaPropertyManagement(docId, true, null);
                Tsd.Parts.SetVolumePropertyManagement(docId, true, null);

                TopSolidHost.Application.EndModification(true, true);
            }
            catch (Exception e)
            {
                Log.Message(e);
                TopSolidHost.Application.EndModification(false, false);
            }

            StopTs();
        }
    }
}
