using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Framework
{
    public class DlgRechercherUnFichier : IDisposable
    {

        public DlgRechercherUnFichier() { }

        private OpenFileDialog _Dialogue = new OpenFileDialog();

        public String Titre { get { return _Dialogue.Title; } set { _Dialogue.Title = value; } }
        public Boolean AjouterExtension { get { return _Dialogue.AddExtension; } set { _Dialogue.AddExtension = value; } }
        public Boolean TestFichierExiste { get { return _Dialogue.CheckFileExists; } set { _Dialogue.CheckFileExists = value; } }
        public Boolean TestCheminExiste { get { return _Dialogue.CheckPathExists; } set { _Dialogue.CheckPathExists = value; } }
        public String ExtensionParDefaut { get { return _Dialogue.DefaultExt; } set { _Dialogue.DefaultExt = value; } }
        public String Filtre { get { return _Dialogue.Filter; } set { _Dialogue.Filter = value; } }
        public int IndexDuFiltre { get { return _Dialogue.FilterIndex; } set { _Dialogue.FilterIndex = value; } }
        public String DossierInitial { get { return _Dialogue.InitialDirectory; } set { _Dialogue.InitialDirectory = value; } }
        public Boolean RestaurerLeDossierActif { get { return _Dialogue.RestoreDirectory; } set { _Dialogue.RestoreDirectory = value; } }
        public Boolean AfficherAide { get { return _Dialogue.ShowHelp; } set { _Dialogue.ShowHelp = value; } }
        public Boolean AfficherLectureSeule { get { return _Dialogue.ShowReadOnly; } set { _Dialogue.ShowReadOnly = value; } }
        public Boolean ExtensionsMultiple { get { return _Dialogue.SupportMultiDottedExtensions; } set { _Dialogue.SupportMultiDottedExtensions = value; } }
        public Boolean LectureSeuleSelectionne { get { return _Dialogue.ReadOnlyChecked; } set { _Dialogue.ReadOnlyChecked = value; } }

        #region "Méthodes"

        /// <summary>
        /// Filtrer les fichiers SW
        /// </summary>
        /// <param name="TypeDesFichiersFiltres"></param>
        public void FiltreSW(TypeFichier_e TypeDesFichiersFiltres, Boolean FiltreDistinct = true)
        {
            String TxtFiltre;

            List<String> Filtre = new List<string>();

            if (FiltreDistinct)
            {
                foreach (TypeFichier_e T in Enum.GetValues(typeof(TypeFichier_e)))
                {
                    if (TypeDesFichiersFiltres.HasFlag(T))
                        Filtre.Add(CONSTANTES.InfoFichier(T, InfoFichier_e.cNom) + " (*" + CONSTANTES.InfoFichier(T) + "|*" + CONSTANTES.InfoFichier(T));
                }

                TxtFiltre = String.Join("|", Filtre);
            }
            else
            {
                foreach (TypeFichier_e T in Enum.GetValues(typeof(TypeFichier_e)))
                {
                    if (TypeDesFichiersFiltres.HasFlag(T))
                        Filtre.Add("*" + CONSTANTES.InfoFichier(T));
                }

                TxtFiltre = "Fichier SolidWorks (" + String.Join(", ", Filtre) + ")" + "|" + String.Join("; ", Filtre);
            }

            _Dialogue.Filter = TxtFiltre;
        }

        /// <summary>
        /// Selectionner un fichier et renvoi le chemin
        /// </summary>
        /// <param name="CheminComplet"></param>
        /// <returns></returns>
        public String SelectionnerUnFichier(Boolean CheminComplet = true)
        {
            _Dialogue.Multiselect = false;

            if (_Dialogue.ShowDialog() == DialogResult.OK)
            {
                if (CheminComplet)
                    return _Dialogue.FileName;
                else
                    return _Dialogue.SafeFileName;
            }
            else
                return "";
        }

        /// <summary>
        /// Selectionner un fichier et renvoi l'objet FichierSW
        /// </summary>
        /// <param name="CheminComplet"></param>
        /// <returns></returns>
        public eFichierSW SelectionnerUnFichierSW()
        {
            eFichierSW pFichierSW = new eFichierSW();
            if (pFichierSW.Init(_SW))
            {
                pFichierSW.Chemin = SelectionnerUnFichier(true);
                if ((pFichierSW.TypeDuFichier == TypeFichier_e.cAssemblage) || (pFichierSW.TypeDuFichier == TypeFichier_e.cPiece) || (pFichierSW.TypeDuFichier == TypeFichier_e.cDessin))
                    return pFichierSW;
            }

            return null;
        }

        /// <summary>
        /// Selectionner des fichiers et renvoi un tableau de chemins
        /// </summary>
        /// <param name="CheminComplet"></param>
        /// <returns></returns>
        public ArrayList SelectionnerPlusieursFichiers(Boolean CheminComplet = true)
        {
            _Dialogue.Multiselect = true;
            ArrayList pArrayFichiers = new ArrayList();

            if (_Dialogue.ShowDialog() == DialogResult.OK)
            {
                if (CheminComplet)
                    pArrayFichiers = new ArrayList(_Dialogue.FileNames);
                else
                    pArrayFichiers = new ArrayList(_Dialogue.SafeFileNames);
            }

            return pArrayFichiers;
        }

        /// <summary>
        /// Selectionner des fichiers et renvoi un tableau d'objet FichierSW
        /// </summary>
        /// <param name="CheminComplet"></param>
        /// <returns></returns>
        public ArrayList SelectionnerPlusieursFichierSW()
        {
            ArrayList pArrayFichiers = new ArrayList();

            foreach (String CheminFichier in SelectionnerPlusieursFichiers(true))
            {
                eFichierSW pFichierSW = new eFichierSW();
                if (pFichierSW.Init(_SW))
                {
                    pFichierSW.Chemin = CheminFichier;
                    if ((pFichierSW.TypeDuFichier == TypeFichier_e.cAssemblage) || (pFichierSW.TypeDuFichier == TypeFichier_e.cPiece) || (pFichierSW.TypeDuFichier == TypeFichier_e.cDessin))
                        pArrayFichiers.Add(pFichierSW);
                }
            }

            return pArrayFichiers;
        }

        #endregion

        void IDisposable.Dispose()
        {
            _Dialogue.Dispose();
        }
    }
}
