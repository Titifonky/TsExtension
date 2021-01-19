using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outils
{
    [Flags]
    public enum TypeDoc
    {
        [Intitule("Vide"), ExtFichier("")]
        Vide = 0,
        [Intitule("Inconnu"), ExtFichier("")]
        Inconnu = 1,
        [Intitule("Pièce"), ExtFichier(""), ExtGabarit("")]
        Piece = 2,
        [Intitule("Assemblage"), ExtFichier(""), ExtGabarit("")]
        Assemblage = 4,
        [Intitule("Dessin"), ExtFichier(""), ExtGabarit("")]
        Dessin = 8,
        [Intitule("Nomenclature"), ExtFichier(""), ExtGabarit("")]
        Nomenclature = 16
        
    }
}
