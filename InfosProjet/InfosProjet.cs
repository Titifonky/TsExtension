using LogDebugging;
using Outils;
using System;
using System.Collections.Generic;
using TopSolid.Cad.Design.Automating;
using TopSolid.Kernel.Automating;

namespace InfosProjet
{
    using Ts = TopSolidHost;

    class InfosProjet : BoutonBase
    {
        private static DocumentId docId;

        private static String Racine = "Projets";
        private static String Archive = "X - Archives";
        private static String NoClient = "";
        private static String NomClient = "";
        private static String NoChantier = "";
        private static String NomChantier = "";
        private static String Lot = "";
        private static String Article = "";

        static void Main(string[] args)
        {
            StartTs();

            try
            {
                var Dcs = Ts.Documents;

                docId = Dcs.EditedDocument;
                var oIdProjet = Ts.Pdm.GetProject(Ts.Documents.GetPdmObject(docId));

                var NomDossierN0 = "";
                var NomDossierN1 = "";
                var NomDossierN2 = "";
                var NomDossierN3 = "";

                NomDossierN0 = Ts.Pdm.GetName(oIdProjet);

                var IdDossier = Ts.Pdm.GetProjectOwner(oIdProjet);
                NomDossierN1 = Ts.Pdm.GetProjectFolderName(IdDossier);

                if (NomDossierN1 == Racine || NomDossierN1 == Archive)
                    NomDossierN1 = "";
                else
                {
                    IdDossier = Ts.Pdm.GetProjectFolderOwner(IdDossier);
                    if (!IdDossier.IsEmpty)
                    {
                        NomDossierN2 = Ts.Pdm.GetProjectFolderName(IdDossier);
                        if (NomDossierN2 == Racine || NomDossierN2 == Archive)
                            NomDossierN2 = "";
                        else
                        {
                            IdDossier = Ts.Pdm.GetProjectFolderOwner(IdDossier);
                            if (!IdDossier.IsEmpty)
                            {
                                NomDossierN3 = Ts.Pdm.GetProjectFolderName(IdDossier);
                                if (NomDossierN3 == Racine || NomDossierN3 == Archive)
                                    NomDossierN3 = "";
                            }
                        }
                    }
                }

                //System.Windows.Forms.MessageBox.Show("N0 " + NomDossierN0 + Environment.NewLine + "N1 " + NomDossierN1 + Environment.NewLine + "N2 " + NomDossierN2 + Environment.NewLine + "N3 " + NomDossierN3);

                // Rècupère les infos

                //  Projet
                //      - N3 Client
                //          - N2 Chantier ?Lot
                //              - N1 Lot
                //                  - N0 ?Article dans le nom du projet
                if (!String.IsNullOrEmpty(NomDossierN3))
                {
                    RemplirClient(NomDossierN3);
                    RemplirChantier(NomDossierN2);
                    if (String.IsNullOrEmpty(Lot))
                        RemplirLot(NomDossierN1);
                    RemplirArticle(NomDossierN0);
                }
                //  Projet
                //      - N2 Client
                //          - N1 Chantier ?Lot
                //              - N0 ?Article dans le nom du projet
                else if (!String.IsNullOrEmpty(NomDossierN2))
                {
                    RemplirClient(NomDossierN2);
                    RemplirChantier(NomDossierN1);
                    RemplirArticle(NomDossierN0);
                }
                //  Projet
                //      - N1 Client
                //          - N0 Chantier ? Lot
                else if (!String.IsNullOrEmpty(NomDossierN1))
                {
                    RemplirClient(NomDossierN1);
                    RemplirChantier(NomDossierN0);
                }
                //  Projet
                //      - N0 Client
                else
                {
                    RemplirClient(NomDossierN0);
                }

                // On rempli les propriétés du projet

                // Propriétés standard
                //===============================================================================
                if (!TopSolidHost.Application.StartModification("My Action", false)) return;

                try
                {
                    Log.Message("Modification");

                    TopSolidHost.Documents.EnsureIsDirty(ref docId);

                    Boolean PropArchi = false;

                    foreach (var ide in Ts.Parameters.GetParameters(docId))
                    {
                        var t = Ts.Elements.GetName(ide);

                        switch (t)
                        {
                            case "$TopSolid.Kernel.TX.Properties.CustomerIdentificationNumber":
                                Ts.Parameters.SetTextValue(ide, NoClient); break;
                            case "$TopSolid.Kernel.TX.Properties.Customer":
                                Ts.Parameters.SetTextValue(ide, NomClient); break;
                            case "$TopSolid.Kernel.TX.Properties.CustomerProjectIdentificationNumber":
                                Ts.Parameters.SetTextValue(ide, NoChantier); break;
                            case "$TopSolid.Kernel.TX.Properties.CustomerProjectDescription":
                                Ts.Parameters.SetTextValue(ide, NomChantier); break;
                            case "$TopSolid.Kernel.TX.Properties.PartNumber":
                                Ts.Parameters.SetTextValue(ide, NoChantier); break;
                            case "Architecte":
                                PropArchi = true; break;
                            default:
                                break;
                        }
                    }
                    if (!PropArchi)
                    {

                        String Architecte = "";
                        Interaction.InputBox("Cabinet d'architecte", "Architecte :", ref Architecte);
                        ElementId idArchi = Ts.Parameters.CreateTextParameter(docId, Architecte);
                        Ts.Elements.SetName(idArchi, "Architecte");
                    }

                    TopSolidHost.Application.EndModification(true, true);
                }
                catch (Exception e)
                {
                    Log.Message(e);
                    TopSolidHost.Application.EndModification(false, false);
                }

                // Propriétés utilisateur
                //===============================================================================
                if (!TopSolidHost.Application.StartModification("My Action", false)) return;

                try
                {
                    Log.Message("Modification");

                    TopSolidHost.Documents.EnsureIsDirty(ref docId);

                    ProprieteUtilisateur("Article", Article);
                    ProprieteUtilisateur("Lot", Lot);
                    ProprieteUtilisateur("Intitulé de la société", "Métallerie Ferronnerie du Bavaisis");
                    ProprieteUtilisateur("Acronyme de la société", "Mfb");

                    TopSolidHost.Application.EndModification(true, true);
                }
                catch (Exception e)
                {
                    Log.Message(e);
                    TopSolidHost.Application.EndModification(false, false);
                }
            }
            catch (Exception e)
            {
                Log.Message(e);
            }

            StopTs();
        }

        public static void ProprieteUtilisateur(String prop, String valeur)
        {
            PdmObjectId oidProp = TopSolidHost.Pdm.SearchDocumentByName(PdmObjectId.Empty, prop)[0];
            if (oidProp.IsEmpty) return;

            DocumentId didProp = TopSolidHost.Documents.GetDocument(oidProp);
            if (didProp.IsEmpty) return;

            ElementId eidParam = TopSolidHost.Parameters.SearchUserPropertyParameter(docId, didProp);
            if (eidParam.IsEmpty)
                eidParam = TopSolidHost.Parameters.CreateUserPropertyParameter(docId, didProp);

            if (eidParam.IsEmpty) return;

            TopSolidHost.Parameters.SetTextValue(eidParam, valeur);
        }

        public static void RemplirClient(String s)
        {
            var t = s.Split(new String[] { " - " }, StringSplitOptions.None);
            RemplirT(ref t, 0, ref NoClient);
            RemplirT(ref t, 1, ref NomClient);
        }

        public static void RemplirChantier(string s)
        {
            var t = s.Split(new String[] { " - " }, StringSplitOptions.None);
            RemplirT(ref t, 0, ref NoChantier);
            RemplirT(ref t, 1, ref NomChantier);
            RemplirT(ref t, 2, ref Lot);
            RemplirLot(Lot);
        }

        public static void RemplirLot(string s)
        {
            Lot = s;
            Lot = Lot.Replace("Lot", "");
            Lot = Lot.Replace("lot", "").Trim();
        }

        public static void RemplirArticle(string s)
        {
            var t = s.Split(new String[] { " " }, StringSplitOptions.None);
            if(t.Length > 0)
            {
                var a = t[0];
                if (char.IsDigit(a[0]))
                    Article = a;
            }
        }

        public static void RemplirT(ref String[] t, int i, ref String s)
        {
            if (t.Length > i)
                s = t[i].Trim();
            else
                s = "";
        }
    }
}
