using LogDebugging;
using Outils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TopSolid.Cad.Design.Automating;
using TopSolid.Kernel.Automating;

namespace InverserVueDvp
{
    using Ts = TopSolidHost;
    using Tsd = TopSolidDesignHost;

    public class InverserVueDvp : BoutonBase
    {
        private static DocumentId docId;

        static void Main(string[] args)
        {
            StartTs();

            var Dcs = Ts.Documents;

            docId = Dcs.EditedDocument;

            var l = Ts.Operations.GetOperations(docId);

            ElementId? TmpEidDvp = null;

            foreach (var eid in l)
            {
                if(Ts.Elements.GetTypeFullName(eid) == "TopSolid.Cad.Design.DB.SheetMetal.Unfolding.UnfoldingResultOperation")
                {
                    TmpEidDvp = eid;
                    break;
                }
            }

            if (TmpEidDvp == null) return;

            ElementId eidDvp = (ElementId)TmpEidDvp;

            Log.Message(Ts.Elements.GetName(eidDvp));

            var tst = Ts.Entities.GetOccurrenceDefinition(eidDvp);

            Log.Message(tst != null);

            if (!Ts.Application.StartModification("My Action", false)) return;

            try
            {
                Log.Message("Modification");

                Ts.Documents.EnsureIsDirty(ref docId);

                

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
