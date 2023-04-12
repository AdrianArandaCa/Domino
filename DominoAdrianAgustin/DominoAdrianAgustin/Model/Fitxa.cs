using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominoAdrianAgustin.Model
{
    public class Fitxa
    {
        public string valor { get; set; }
        public Jugador jugador { get; set; }
        public int left { get; set; }
        public int right { get; set; }

        public Fitxa(string valor)
        {
            this.valor = valor;
        }
        public Fitxa()
        {
        }
    }

}
