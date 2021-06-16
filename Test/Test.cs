using LogDebugging;
using Outils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSolid.Kernel.Automating;
using TopSolid.Kernel.TX;

namespace Test
{
    class Test : BoutonBase
    {
        private static DocumentId docId;
        static void Main(string[] args)
        {
            StartTs();

            docId = TopSolidHost.Documents.EditedDocument;

            if (docId.IsEmpty) return;

            try
            {
                Log.Message("Modification");

                var doc = TopSolid.Kernel.TX.Documents.DocumentStore.EditedDocument;

                System.Windows.Forms.MessageBox.Show(doc.LocalizedName);
            }
            catch (Exception e)
            {
                Log.Message(e);
            }

            

            //var typeDoc = TopSolidHost.Documents.GetTypeFullName(docId);

            //System.Windows.Forms.MessageBox.Show(typeDoc);

            //MajPropriete("Document", "Repere", "Document", "No");
            //MajPropriete("Document", "Description courte", "Document", "Description courte du produit");
            //MajPropriete("Document", "Description nomenclature", "Document", "Description nomenclature du produit");
            //MajPropriete("Document", "Description du matériau", "Document", "Description du produit");
            //MajPropriete("Document", "Type de matériau", "Document", "Type de produit");

            StopTs();
        }

        private static void MajPropriete(String oldDomain, String oldName, String nvDomain, String nvName, String valeur = "")
        {
            Log.Message("MajPropriete");

            PdmObjectId idOld = TopSolidHost.Pdm.SearchDocumentByName(PdmObjectId.Empty, oldName)[0];
            if(idOld.IsEmpty) return;
            Log.Message("   idOld -> Ok");

            PdmObjectId idNv = TopSolidHost.Pdm.SearchDocumentByName(PdmObjectId.Empty, nvName)[0];
            if (idNv.IsEmpty) return;
            Log.Message("   idNv -> Ok");


            if (!TopSolidHost.Application.StartModification("My Action", false)) return;

            try
            {
                Log.Message("Modification");

                TopSolidHost.Documents.EnsureIsDirty(ref docId);

                DocumentId idOldDoc = TopSolidHost.Documents.GetDocument(idOld);
                if (!idOldDoc.IsEmpty)
                {
                    Log.Message("       idOldDoc -> Ok");

                    ElementId idOldParam = TopSolidHost.Parameters.SearchUserPropertyParameter(docId, idOldDoc);
                    if (!idOldParam.IsEmpty)
                    {
                        Log.Message("       idOldParam -> Ok");

                        DocumentId idNvDoc = TopSolidHost.Documents.GetDocument(idNv);
                        if (!idNvDoc.IsEmpty)
                        {
                            Log.Message("       idNvDoc -> Ok");

                            ElementId idNvParam = TopSolidHost.Parameters.SearchUserPropertyParameter(docId, idNvDoc);
                            if (idNvParam.IsEmpty)
                                idNvParam = TopSolidHost.Parameters.CreateUserPropertyParameter(docId, idNvDoc);

                            if (idNvParam.IsEmpty)
                            {
                                Log.Message("       idNvParam -> Ok");

                                if (String.IsNullOrWhiteSpace(valeur))
                                {
                                    valeur = TopSolidHost.Parameters.GetTextValue(idOldParam);
                                    TopSolidHost.Parameters.SetTextValue(idNvParam, valeur);
                                }
                            }
                        }
                    }
                }

                TopSolidHost.Application.EndModification(true, true);
            }
            catch
            {
                Log.Message("Erreur");
                TopSolidHost.Application.EndModification(false, false);
            }
        }

    }
}
