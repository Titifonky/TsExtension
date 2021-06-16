using LogDebugging;
using Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSolid.Cad.Design.Automating;
using TopSolid.Kernel.Automating;

namespace CadrerVueDvp
{
    using Ts = TopSolidHost;
    using Tsd = TopSolidDesignHost;

    public class CadrerVueDvp : BoutonBase
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

                var av = Ts.Visualization3D.GetActiveView(docId);
                Ts.Visualization3D.ZoomToFitView(docId, av);

                var tc = Ts.Visualization3D.GetTopCamera(docId);
                Ts.Visualization3D.GetCameraDefinition(tc, out Point3D ep, out Direction3D dir, out Direction3D UpDir, out double angle, out double rayon);
                Ts.Visualization3D.SetViewCamera(docId, av, ep, dir, UpDir, angle, rayon);
                Ts.Visualization3D.ZoomToFitView(docId, av);

                Ts.Visualization3D.RedrawView(docId, av);

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
