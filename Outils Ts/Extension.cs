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
    public static class Extension
    {
        public static TypeDoc GetTypeDoc(this DocumentId docId)
        {
            if (docId.IsEmpty) return TypeDoc.Vide;

            var t = TopSolidHost.Documents.GetTypeFullName(docId);

            switch (t)
            {
                case "TopSolid.Cad.Design.DB.Bom.Documents.BomDocument":
                    return TypeDoc.Nomenclature;
                case "TopSolid.Cad.Design.DB.Bom.Documents.AssemblyDocument":
                    return TypeDoc.Assemblage;
                //case swDocumentTypes_e.swDocDRAWING:
                //    return TypeDoc.Dessin;
                //case swDocumentTypes_e.swDocLAYOUT:
                //    break;
                //case swDocumentTypes_e.swDocNONE:
                //    break;
                //case swDocumentTypes_e.swDocPART:
                //    return TypeDoc.Piece;
                //case swDocumentTypes_e.swDocSDM:
                //    break;
                default:
                    break;
            }

            return TypeDoc.Inconnu;
        }

        public static bool IsNotEmpty(this DocumentId docId)
        {
            return !docId.IsEmpty;
        }


    }
}
