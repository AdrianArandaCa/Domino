using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DominoAdrianAgustin.Model
{
    public class Jugador
    {
        public int codi { get; set; }
        public string nom { get; set; }
        public List<string> fitxes { get; set; }
        public List<Label> ma { get; set; }
        public Jugador(List<string> fitxes)
        {
            this.fitxes = fitxes;
        }
        public Jugador()
        {
            fitxes = new List<string>();
            ma = new List<Label>();
        }
    }
}
