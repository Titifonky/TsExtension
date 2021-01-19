using Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSolid.Kernel.Automating;

namespace TypeDocument
{
    class TypeDocument : BoutonBase
    {
        static void Main(string[] args)
        {
            StartTs();

            DocumentId docId = TopSolidHost.Documents.EditedDocument;

            if (docId.IsEmpty) return;

            var typeDoc = TopSolidHost.Documents.GetTypeFullName(docId);

            System.Windows.Forms.MessageBox.Show(typeDoc);

            StopTs();
        }
    }
}
