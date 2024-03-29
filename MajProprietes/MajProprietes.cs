﻿using LogDebugging;
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

    public class MajProprietes : BoutonBase
    {
        private static DocumentId docId;

        static void Main(string[] args)
        {
            StartTs();

            var Dcs = Ts.Documents;

            docId = Dcs.EditedDocument;

            if (!Ts.Application.StartModification("My Action", false)) return;

            try
            {
                Log.Message("Modification");

                Ts.Documents.EnsureIsDirty(ref docId);

                Tsd.Parts.SetPhysicalPropertiesManagementRefreshAuto(docId, true);
                Tsd.Parts.SetMassPropertyManagement(docId, true, null);
                Tsd.Parts.SetCenterOfMassPropertyManagement(docId, true, null);

                Tsd.Parts.SetSurfaceAreaPropertyManagement(docId, true, null);
                Tsd.Parts.SetVolumePropertyManagement(docId, true, null);

                Ts.Application.EndModification(true, true);
            }
            catch (Exception e)
            {
                Log.Message(e);
                Ts.Application.EndModification(false, false);
            }

            StopTs();
        }
    }
}
