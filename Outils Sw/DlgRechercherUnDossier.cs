using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

namespace Outils
{
    public class DlgRechercherUnDossier : IDisposable
    {
        public DlgRechercherUnDossier() { }

        private FolderBrowserDialog _Dialogue = new FolderBrowserDialog();

        public String Description { get { return _Dialogue.Description; } set { _Dialogue.Description = value; } }
        public Environment.SpecialFolder DossierRacine { get { return _Dialogue.RootFolder; } set { _Dialogue.RootFolder = value; } }
        public Boolean BoutonNouveauDossier { get { return _Dialogue.ShowNewFolderButton; } set { _Dialogue.ShowNewFolderButton = value; } }

        /// <summary>
        /// Retourne le chemin du dossier sélectionné
        /// </summary>
        /// <returns></returns>
        public String SelectionnerUnDossier()
        {
            if (_Dialogue.ShowDialog() == DialogResult.OK)
                return _Dialogue.SelectedPath;

            return "";
        }

        /// <summary>
        /// Retourne la liste des fichiers du dossier sélectionné
        /// </summary>
        /// <param name="TypeDesFichiers"></param>
        /// <param name="NomARechercher"></param>
        /// <param name="ParcourirLesSousDossier"></param>
        /// <returns></returns>
        public ArrayList SelectionnerUnDossierEtRenvoyerFichierSW(TypeFichier_e TypeDesFichiers, String NomARechercher = "*", Boolean ParcourirLesSousDossier = false)
        {
            ArrayList pArrayFichiers = new ArrayList();

            String CheminDossier = SelectionnerUnDossier();

            SearchOption Options = SearchOption.TopDirectoryOnly;
            if (ParcourirLesSousDossier)
                Options = SearchOption.AllDirectories;

            if (Directory.Exists(CheminDossier))
            {
                foreach (String CheminFichier in Directory.EnumerateFiles(CheminDossier, NomARechercher, Options))
                {
                    eFichierSW pFichierSW = new eFichierSW();
                    if (pFichierSW.Init(_SW))
                    {
                        pFichierSW.Chemin = CheminFichier;
                        if ((pFichierSW.TypeDuFichier & TypeDesFichiers) != 0)
                        {
                            pArrayFichiers.Add(pFichierSW);
                        }
                    }
                }
            }

            return pArrayFichiers;
        }

        void IDisposable.Dispose()
        {
            _Dialogue.Dispose();
        }
    }
}
