using System.Net.WebSockets;

namespace WebSocketDominoAdrianAgustin
{
    public class Jugador
    {
        public string nom { get; set; }
        public WebSocket sock { get; set; }
        public List<Fitxa> fitxes { get; set; }
        public int torn { get; set; }

        public Jugador(string nom, WebSocket sock, int torn)
        {
            this.nom = nom;
            this.sock = sock;
            this.torn = torn;
        }

        public Jugador(List<Fitxa> fitxes)
        {
            this.fitxes = fitxes;
        }
    }
}
