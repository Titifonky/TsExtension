using LogDebugging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CommanderProfils
{
    public enum eTypeSortie
    {
        ListeDebit = 1,
        ListeBarre = 2
    }

    public class MiseEnBarre
    {
        public List<String> ListeMateriaux = new List<String>();
        public List<String> ListeProfils = new List<String>();
        public int Quantite = 1;
        public eTypeSortie TypeSortie = eTypeSortie.ListeDebit;

        public String CheminFichier { get; private set; }

        public Double LgBarre = 6000;

        private ListeElement _listeElement;

        public MiseEnBarre(Double lgBarreMax = -1)
        {
            if (lgBarreMax > 0)
                LgBarre = lgBarreMax;

            _listeElement = new ListeElement(LgBarre);
        }

        public void AjouterElement(int Nb, String no, String reference, String materiau, String profil, Double lg, Double a, Double b)
        {
            try
            {
                _listeElement.AjouterElement(Nb * Quantite, no, reference, materiau, profil, lg, a, b);
            }
            catch (Exception e)
            {
                Log.Message(new Object[] { e });
            }
        }

        public String Calculer(String NomFichier)
        {
            String CheminFichier = null;
            try
            {
                //String NomFichier = "MiseEnBarre";

                String Complet = "";

                switch (TypeSortie)
                {
                    case eTypeSortie.ListeDebit:
                        {
                            var ListeBarre = _listeElement.ListeBarre();

                            String ResumeBarre = ListeBarre.ResumeNbBarre();
                            String ResumeListeDebit = ListeBarre.ResumeListeDebit();

                            Complet = ResumeBarre + "\r\n\r\n" + ResumeListeDebit;

                            Complet += "\r\n\r\n" + "Nb d'éléments : " + _listeElement.NbElement;
                            Complet += "\r\n" + "Nb de barres : " + ListeBarre.NbBarre;

                            Log.Message(Complet);
                        }
                        break;
                    case eTypeSortie.ListeBarre:
                        {
                            String ResumeListeBarre = _listeElement.ResumeListeBarre();
                            
                            Complet = ResumeListeBarre;

                            Log.Message(Complet);
                        }
                        break;
                    default:
                        break;
                }

                CheminFichier = Path.Combine(Path.GetTempPath(), NomFichier + ".txt");

                StreamWriter s = new StreamWriter(CheminFichier);
                s.Write(Complet);
                s.Close();

                Process.Start("notepad.exe", CheminFichier);

            }
            catch (Exception e)
            {
                Log.Message(new Object[] { e });
                CheminFichier = null;
            }

            return CheminFichier;
        }

        private class Element
        {
            public String No { get; private set; }
            public String Ref { get; private set; }
            public String Materiau { get; private set; }
            public String Profil { get; private set; }
            public Double Lg { get; private set; }
            public Double A { get; private set; }
            public Double B { get; private set; }

            public Element(String no, String reference, String materiau, String profil, Double lg, Double a, Double b)
            {
                No = no; Ref = reference; Materiau = materiau; Profil = profil; Lg = lg; A = a; B = b;
            }
        }

        private class Barre : List<Element>
        {
            public Double LgTT { get; private set; }
            public Double LgUtilise { get; private set; }
            public Double LgChute { get; private set; }

            public Barre(Double lgMax)
            {
                LgTT = lgMax;
                LgUtilise = 0;
                LgChute = LgTT;
            }

            public Barre(Double lgMax, Element e)
            {
                LgTT = lgMax;
                LgUtilise = 0;
                LgChute = LgTT;

                Ajouter(e);
            }

            public Boolean Ajouter(Element e)
            {
                if ((e.Lg <= LgChute) || (e.Lg > LgTT))
                {
                    Add(e);
                    LgUtilise += e.Lg;
                    LgChute = LgTT - LgUtilise;
                    return true;
                }

                return false;
            }
        }

        private class ListeLgProfil
        {
            private Double _LgBarre = 6000;
            private Dictionary<String, Dictionary<String, Double>> _dic = new Dictionary<string, Dictionary<string, Double>>();

            public Dictionary<String, Dictionary<String, Double>> DicLg { get { return _dic; } }

            public ListeLgProfil(Double lgBarre)
            {
                _LgBarre = lgBarre;
            }

            public void AjouterProfil(String materiau, String profil)
            {
                var DicMat = new Dictionary<String, Double>();

                if (_dic.ContainsKey(materiau))
                    DicMat = _dic[materiau];
                else
                    _dic.Add(materiau, DicMat);

                if (!DicMat.ContainsKey(profil))
                {
                    DicMat.Add(profil, _LgBarre);
                    NbProfils++;
                }
            }

            public Double LgMax(String materiau, String profil)
            {
                Double LgMax = _LgBarre;

                if (_dic.ContainsKey(materiau))
                {
                    if (_dic[materiau].ContainsKey(profil))
                        LgMax = _dic[materiau][profil];
                }

                return LgMax;
            }

            public int NbProfils { get; private set; }
        }

        private class ListeElement
        {
            private Dictionary<String, Dictionary<String, List<Element>>> _dic = new Dictionary<string, Dictionary<string, List<Element>>>();

            private ListeLgProfil _LgProfils;

            public ListeLgProfil ListeLgProfil { get { return _LgProfils; } }

            public ListeElement(Double lgBarre)
            {
                _LgProfils = new ListeLgProfil(lgBarre);
            }

            public ListeBarre ListeBarre()
            {
                Trier();

                ListeBarre ListeBarre = new ListeBarre(_LgProfils);

                foreach (var Materiau in _dic.Values)
                {
                    foreach (var ListProf in Materiau.Values)
                    {
                        foreach (var Element in ListProf)
                            ListeBarre.AjouterElement(Element);
                    }
                }

                return ListeBarre;
            }

            public void AjouterElement(int Nb, String no, String reference, String materiau, String profil, Double lg, Double a, Double b)
            {
                var DicMat = new Dictionary<String, List<Element>>();

                if (_dic.ContainsKey(materiau))
                    DicMat = _dic[materiau];
                else
                    _dic.Add(materiau, DicMat);

                var ListProf = new List<Element>();

                if (DicMat.ContainsKey(profil))
                    ListProf = DicMat[profil];
                else
                    DicMat.Add(profil, ListProf);

                for (int i = 0; i < Nb; i++)
                {
                    NbElement++;
                    ListProf.Add(new Element(no, reference, materiau, profil, lg, a, b));
                }

                _LgProfils.AjouterProfil(materiau, profil);
            }

            private void Trier()
            {
                foreach (var Materiau in _dic.Values)
                {
                    foreach (var ListProf in Materiau.Values)
                    {
                        ListProf.Sort(
                            (e1, e2) =>
                            {
                                if (e1.Lg == e2.Lg) return 0;
                                if (e1.Lg > e2.Lg) return -1;
                                return 1;
                            });
                    }
                }
            }

            public String ResumeListeBarre()
            {
                String text = "";

                foreach (var Materiau in _dic.Keys)
                {
                    text += "\r\n" + Materiau;
                    var DicMat = _dic[Materiau];

                    foreach (var Prof in DicMat.Keys)
                    {
                        var ListProf = DicMat[Prof];

                        int nbEle = 0;
                        double Lg = ListProf[0].Lg;

                        String format = "\r\n  {0,-30} {1,8:0.0}mm {2,5}";

                        foreach (var ele in ListProf)
                        {
                            if (ele.Lg != Lg)
                            {
                                text += String.Format(format, Prof, Lg, "×" + nbEle);
                                nbEle = 1;
                                Lg = ele.Lg;
                            }
                            else
                                nbEle += 1;
                        }

                        text += String.Format(format, Prof, Lg, "×" + nbEle);
                    }
                }

                return text;
            }

            public int NbElement { get; private set; }
        }

        private class ListeBarre
        {
            private Dictionary<String, Dictionary<String, List<Barre>>> _dic = new Dictionary<String, Dictionary<String, List<Barre>>>();

            private ListeLgProfil _ListeLgProfil;

            public ListeBarre(ListeLgProfil listeLgProfil)
            {
                _ListeLgProfil = listeLgProfil;
            }

            public void AjouterElement(Element element)
            {
                var DicMat = new Dictionary<String, List<Barre>>();

                if (_dic.ContainsKey(element.Materiau))
                    DicMat = _dic[element.Materiau];
                else
                    _dic.Add(element.Materiau, DicMat);

                var ListBarre = new List<Barre>();

                if (DicMat.ContainsKey(element.Profil))
                    ListBarre = DicMat[element.Profil];
                else
                    DicMat.Add(element.Profil, ListBarre);

                Double lgMax = _ListeLgProfil.LgMax(element.Materiau, element.Profil);

                if (ListBarre.Count == 0)
                {
                    NbBarre++;
                    ListBarre.Add(new Barre(lgMax));
                }

                // On essaye d'ajouter la barre
                // si c'est ok, on sort de la routine
                foreach (var barre in ListBarre)
                {
                    if (barre.Ajouter(element))
                        return;
                }

                // sinon on ajoute une nouvelle barre
                {
                    var barre = new Barre(lgMax);
                    barre.Ajouter(element);
                    NbBarre++;
                    ListBarre.Add(barre);
                }

            }

            public String ResumeNbBarre()
            {
                String text = "";

                foreach (var Materiau in _dic.Keys)
                {
                    text += Materiau;
                    var DicMat = _dic[Materiau];

                    foreach (var Prof in DicMat.Keys)
                    {
                        var ListBarre = DicMat[Prof];
                        text += String.Format("\r\n  {0,5}× {1}", ListBarre.Count, Prof);
                    }
                }

                return text;
            }

            public String ResumeListeDebit()
            {
                String text = "";

                foreach (var Materiau in _dic.Keys)
                {
                    text += "\r\n" + Materiau;
                    var DicMat = _dic[Materiau];

                    foreach (var Prof in DicMat.Keys)
                    {
                        var ListBarre = DicMat[Prof];
                        text += String.Format("\r\n {0,4}× {1}", ListBarre.Count, Prof);

                        for (int i = 0; i < ListBarre.Count; i++)
                        {
                            var barre = ListBarre[i];
                            text += String.Format("\r\n      {3}Barre {0,-3} : {1,4:0}mm \\ Chute : {2,4:0}mm", i + 1, barre.LgUtilise, barre.LgChute, barre.LgChute < 0 ? "!!!!!!! " : "");

                            foreach (var ele in barre)
                            {
                                text += String.Format("\r\n         {0}   {1,6:0.0}mm   {2,4:0.0}°   {3,4:0.0}°", ele.Ref, ele.Lg, ele.A, ele.B);
                            }

                            text += "\r\n";
                        }
                        text += "\r\n";
                    }
                }
                return text;
            }

            public int NbBarre { get; private set; }
        }
    }
}


