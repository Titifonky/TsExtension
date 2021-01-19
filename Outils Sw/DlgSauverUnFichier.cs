using System;
using System.Windows.Forms;

namespace Outils
{
    public class DlgSauverUnFichier : IDisposable
    {
        #region "Constructeur\Destructeur"

        public DlgSauverUnFichier() { }

        #endregion

        #region "Propriétés"

        private SaveFileDialog _Dialogue = new SaveFileDialog();

        public String Titre { get { return _Dialogue.Title; } set { _Dialogue.Title = value; } }
        public Boolean AjouterExtension { get { return _Dialogue.AddExtension; } set { _Dialogue.AddExtension = value; } }
        public Boolean TestFichierExiste { get { return _Dialogue.CheckFileExists; } set { _Dialogue.CheckFileExists = value; } }
        public Boolean TestCheminExiste { get { return _Dialogue.CheckPathExists; } set { _Dialogue.CheckPathExists = value; } }
        public Boolean DemanderAutorisationCreationFichier { get { return _Dialogue.CreatePrompt; } set { _Dialogue.CreatePrompt = value; } }
        public String ExtensionParDefaut { get { return _Dialogue.DefaultExt; } set { _Dialogue.DefaultExt = value; } }
        public String Filtre { get { return _Dialogue.Filter; } set { _Dialogue.Filter = value; } }
        public int IndexDuFiltre { get { return _Dialogue.FilterIndex; } set { _Dialogue.FilterIndex = value; } }
        public String DossierInitial { get { return _Dialogue.InitialDirectory; } set { _Dialogue.InitialDirectory = value; } }
        public Boolean RestaurerLeDossierActif { get { return _Dialogue.RestoreDirectory; } set { _Dialogue.RestoreDirectory = value; } }
        public Boolean AfficherAide { get { return _Dialogue.ShowHelp; } set { _Dialogue.ShowHelp = value; } }
        public Boolean ExtensionsMultiple { get { return _Dialogue.SupportMultiDottedExtensions; } set { _Dialogue.SupportMultiDottedExtensions = value; } }


        #endregion

        #region "Méthodes"

        public String SauverUnFichier()
        {

            if (_Dialogue.ShowDialog() == DialogResult.OK)
                return _Dialogue.FileName;
            else
                return "";
        }

        public String[] SauverPlusieursFichiers()
        {
            if (_Dialogue.ShowDialog() == DialogResult.OK)
                return _Dialogue.FileNames;
            else
                return null;
        }

        #endregion

        void IDisposable.Dispose()
        {
            _Dialogue.Dispose();
        }
    }
}
