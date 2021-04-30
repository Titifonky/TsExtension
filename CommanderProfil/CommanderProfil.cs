using LogDebugging;
using Outils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TopSolid.Cad.Design.Automating;
using TopSolid.Kernel.Automating;

namespace CommanderProfils
{
    using Ts = TopSolidHost;
    using Tsd = TopSolidDesignHost;

    class CommanderProfil : BoutonBase
    {
        static void Main(string[] args)
        {
            StartTs();

            var Dcs = Ts.Documents;
            var Bms = Tsd.Boms;

            DocumentId docId = Dcs.EditedDocument;

            if (docId.GetTypeDoc() != TypeDoc.Nomenclature) return;

            var rr = Bms.GetRootRow(docId);

            if (rr == 0) return;

            var cc = Bms.GetColumnCount(docId);

            if (cc == 0) return;

            // On récupère le numéro des colonnes

            var idNo = -1;
            var idRep = -1;
            var idQte = -1;
            var idProfil = -1;
            var idMatiere = -1;
            var idLg = -1;
            var idA = -1;
            var idB = -1;

            for (int i = 0; i < cc; i++)
            {
                var cd = Bms.GetColumnPropertyDefinition(docId, i);

                if (cd.IsEmpty) continue;

                if (Ts.Pdm.SearchPropertyDefinitionInfo(cd, out string dn, out string n, out _) &&
                    dn == "Document"
                    && n == "Description du produit")
                {
                    idProfil = i;
                    continue;
                }

                switch (cd.Domain)
                {
                    case "$TopSolid.Cad.Design.DB":
                        switch (cd.Name)
                        {
                            case "FirstTrimmingAngle":
                                idA = i; break;
                            case "SecondTrimmingAngle":
                                idB = i; break;
                        }
                        break;
                    case "$TopSolid.Cad.Design.DB.Bom":
                        switch (cd.Name)
                        {
                            case "Index":
                                idNo = i; break;
                            case "ManufacturingIndex":
                                idRep = i; break;
                            case "Quantity":
                                idQte = i; break;
                        }
                        break;
                    case "$TopSolid.Cad.Design.DB.Materials":
                        switch (cd.Name)
                        {
                            case "Name":
                                idMatiere = i; break;
                        }
                        break;
                    case "$TopSolid.Kernel.TX.Properties":
                        switch (cd.Name)
                        {
                            //case "Description":
                            //    idProfil = i; break;
                            case "Length":
                                idLg = i; break;
                        }
                        break;
                    default:
                        break;
                }
            }

            var rc = Bms.GetRowChildrenRows(docId, rr);

            if (rc.Count == 0) return;

            var LgMax = 6000.0;
            var lg = "6000";

            if (Interaction.InputBox("Lg nominale des barres", "Lg :", ref lg) == DialogResult.OK)
            {
                if (!String.IsNullOrWhiteSpace(lg) && (lg.eToDouble() != 0))
                    LgMax = lg.eToDouble();
            }

            var MiseEnBarre = new MiseEnBarre(LgMax);

            

            foreach (var idLigne in rc)
            {
                Bms.GetRowContents(docId, idLigne, out List<Property> ListeProp, out List<String> ListeTexte);

                MiseEnBarre.AjouterElement(
                    idQte < 0 ? -1 : (String.IsNullOrEmpty(ListeTexte[idQte]) ? "0" : ListeTexte[idQte]).SupprimerUnites().eToInteger(),
                    idNo < 0 ? "" : ListeTexte[idNo],
                    idRep < 0 ? "" : ListeTexte[idRep],
                    idMatiere < 0 ? "" : ListeTexte[idMatiere],
                    idProfil < 0 ? "" : ListeTexte[idProfil],
                    idLg < 0 ? -1 : (String.IsNullOrEmpty(ListeTexte[idLg]) ? "0" : ListeTexte[idLg]).SupprimerUnites().eToDouble(),
                    idA < 0 ? 0 : (String.IsNullOrEmpty(ListeTexte[idA]) ? "0" : ListeTexte[idA]).SupprimerUnites().eToDouble(),
                    idB < 0 ? 0 : (String.IsNullOrEmpty(ListeTexte[idB]) ? "0" : ListeTexte[idB]).SupprimerUnites().eToDouble()
                    );
            }

            var pdmOidDoc = Ts.Documents.GetPdmObject(docId);
            var parent = Ts.Pdm.GetOwner(pdmOidDoc);
            var rev = Ts.Pdm.GetMajorRevisionText(Ts.Pdm.GetLastMajorRevision(pdmOidDoc));
            var nom = "Commande profil - Ind " + rev;

            var fichier = MiseEnBarre.Calculer(nom);

            if (fichier != null)
            {
                Ts.Pdm.GetConstituents(parent, out  _, out List<PdmObjectId> listeDocuments);

                var listeDelete = new List<PdmObjectId>();

                foreach (var f in listeDocuments)
                    if (Ts.Pdm.GetName(f) == nom) listeDelete.Add(f);

                if(listeDelete.Count > 0)
                    Ts.Pdm.DeleteSeveral(listeDelete);

                var fichierTexte = Ts.Pdm.ImportFile(fichier, parent, nom);
            }

            StopTs();
        }
    }
}
