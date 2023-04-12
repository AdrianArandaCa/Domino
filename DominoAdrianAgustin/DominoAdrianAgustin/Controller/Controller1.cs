using DominoAdrianAgustin.Model;
using DominoAdrianAgustin.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DominoAdrianAgustin.Controller
{
    public class Controller1
    {
        Form1 f;
        int sizeFontPanel = 55;
        int sizeFontFitxa = 50;
        int sizeLabelX = 80;
        int sizeLabelY = 70;
        int locationInitialX = 250;
        int locationMultiplierX = 88;
        int locationY = 510;
        int sizePanelX = 1072;
        int sizePanelY = 170;
        List<string> fitxes = new List<string>();
        List<Fitxa> fitxesEnTauler = new List<Fitxa>();
        List<Fitxa> fitxesInvertir = new List<Fitxa>();
        CancellationTokenSource cts;
        ClientWebSocket socket = new ClientWebSocket();
        Jugador jugador = new Jugador();
        Label fitxaEnviada;
        string ultimaFitxa;
        bool esOculta = false;
        object ultimSender;
        bool maPlena = false;
        string fitxaJugador = "";
        string ws = "ws://localhost:6666/ws/";

        public Controller1()
        {
            f = new Form1();
            loadData();
            initListener();
            Application.Run(f);
        }
        //Carreguem dades de la vista
        private void loadData()
        {
            f.servidorTB.Text = ws.ToString();
            f.conectarBtn.BackColor = Color.LightGreen;
            Font font = new Font(f.labelTablero.Font.FontFamily, sizeFontPanel);
            f.labelTablero.Size = new Size(sizePanelX, sizePanelY);
            f.labelTablero.Font = font;
            f.labelTablero.TextAlign = ContentAlignment.MiddleCenter;
            f.tornLBL.Text = "Toca esperar!";
            f.tornLBL.BackColor = Color.Red;
            f.tornLBL.Visible = true;
        }

        private void initListener()
        {
            f.conectarBtn.Click += ConectarBtn_Click;
            f.enviarBtn.Click += EnviarBtn_Click;
            f.pasarBtn.Click += PasarBtn_Click;
        }

        private void PasarBtn_Click(object sender, EventArgs e)
        {
            Enviar(socket, "pasar");
        }

        private void EnviarBtn_Click(object sender, EventArgs e)
        {
            esOculta = false;
            Enviar(socket, "");
        }
        //Conectem amb el server
        public async Task Conectar(string nom)
        {
            cts = new CancellationTokenSource();
            string wsUri = ws + nom;
            await socket.ConnectAsync(new Uri(wsUri), cts.Token);
        }

        private async void ConectarBtn_Click(object sender, EventArgs e)
        {
            if (!f.conectarBtn.Text.Equals("Desconectar"))
            {
                f.conectarBtn.Text = "Desconectar";
                f.conectarBtn.BackColor = Color.Red;
                f.conectarBtn.ForeColor = Color.White;
                string nom = f.nomTB.Text.ToString();
                await Conectar(nom);
                if (socket.State == WebSocketState.Open)
                {
                    await Escoltar(socket);
                }
            }
            else
            {
                f.conectarBtn.Text = "Conectar";
                f.conectarBtn.BackColor = Color.Green;
                f.conectarBtn.ForeColor = Color.White;
            }
        }

        //Escoltem que ens diu el webSocket
        public async Task Escoltar(ClientWebSocket socket)
        {
            cts = new CancellationTokenSource();
            var buffer = new byte[256];
            if (socket.State == WebSocketState.Open)
            {
                var rcvBytes = new byte[256];
                var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                while (true)
                {
                    WebSocketReceiveResult rcvResult = await socket.ReceiveAsync(rcvBuffer, cts.Token);
                    if (rcvResult.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                    else
                    {
                        byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                        string rcvMsg = Encoding.UTF8.GetString(msgBytes).ToString();
                        //Si es un missatge
                        if (rcvMsg.Contains(":"))
                        {
                            f.missatgeLB.Items.Add(rcvMsg);
                        }
                        //Si algú ha guanyat la partida
                        else if (rcvMsg.StartsWith("-??"))
                        {
                            rcvMsg = rcvMsg.Substring(3);
                            MessageBox.Show(rcvMsg);
                        }
                        //Si el jugador en actiu intenta colocar una fitxa incorrecta
                        else if (rcvMsg.Equals("error"))
                        {
                            MessageBox.Show("No es pot colocar aquesta fitxa");

                        }
                        //Si un jugador intenta colocar una fitxa fora de torn
                        else if (rcvMsg.Equals("errorTorn"))
                        {
                            MessageBox.Show("No es el teu torn!");
                        }
                        //El taulell amb les fitxes
                        else if (rcvMsg.StartsWith(" "))
                        {
                            f.labelTablero.Text = rcvMsg;
                            for (var m = 0; m < jugador.ma.Count; m++)
                            {
                                if (jugador.ma[m].Text == ultimaFitxa)
                                {
                                    jugador.ma[m].Visible = false;
                                    jugador.fitxes.Remove(ultimaFitxa);
                                }
                            }
                            if (jugador.fitxes.Count == 0)
                            {
                                Enviar(socket, "??");
                            }
                        }
                        //Acaba torn
                        else if (rcvMsg.Equals("tornAcabat"))
                        {
                            f.tornLBL.BackColor = Color.Red;
                            f.tornLBL.Text = "No es el teu torn!";
                        }
                        //Comença torn
                        else if (rcvMsg.Equals("tornComençat"))
                        {
                            f.tornLBL.Text = "Et toca!";
                            f.tornLBL.BackColor = Color.LawnGreen;
                            f.tornLBL.Visible = true;
                        }
                        //Torna fitxa
                        else
                        {
                            //Omple la ma del jugador
                            if (!maPlena)
                            {
                                if (rcvMsg.StartsWith("?"))
                                {
                                    f.tornLBL.Text = "Et toca!";
                                    rcvMsg = rcvMsg.Substring(1);
                                    f.tornLBL.BackColor = Color.LawnGreen;
                                    f.tornLBL.Visible = true;
                                }
                                jugador.fitxes.Add(rcvMsg);
                                if (jugador.fitxes.Count == 7)
                                {
                                    mostrarFitxesJugador(jugador);
                                    maPlena = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public Boolean Enviar(ClientWebSocket socket, string fitxa)
        {
            cts = new CancellationTokenSource();
            if (socket.State == WebSocketState.Open)
            {
                //Enviem missatge de text
                if (fitxa == "")
                {
                    string missatge = "-" + f.missatgeTB.Text.ToString();
                    f.missatgeLB.Items.Add(missatge);
                    byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
                    var sendBuffer = new ArraySegment<byte>(sendBytes);
                    socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
                    f.missatgeTB.Clear();
                    return true;
                }
                //Accio de pasar
                else if (fitxa == "pasar")
                {
                    string missatge = fitxa;
                    byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
                    var sendBuffer = new ArraySegment<byte>(sendBytes);
                    socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
                    return true;
                }
                //Enviem fitxa
                else
                {
                    ultimaFitxa = fitxa;
                    string missatge = fitxa;
                    byte[] sendBytes = Encoding.UTF8.GetBytes(missatge);
                    var sendBuffer = new ArraySegment<byte>(sendBytes);
                    socket.SendAsync(sendBuffer, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: cts.Token);
                    return true;
                }
            }
            return false;
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            var label = sender as Label;
            MessageBox.Show(label.Text);
        }

        public void mostrarFitxesJugador(Jugador jugador)
        {
            fitxes = jugador.fitxes;
            for (int i = 0; i < fitxes.Count; i++)
            {
                Label labelJ = new Label();
                Font font = new Font(labelJ.Font.FontFamily, sizeFontFitxa);
                labelJ.Size = new Size(sizeLabelX, sizeLabelY);
                labelJ.Font = font;
                labelJ.Text = fitxes[i].ToString();
                labelJ.Location = new Point(locationInitialX + (i * locationMultiplierX), locationY);
                labelJ.Click += LabelJ_Click;
                jugador.ma.Add(labelJ);
                f.Controls.Add(labelJ);
            }
        }

        private void LabelJ_Click(object sender, EventArgs e)
        {
            fitxaEnviada = sender as Label;
            ultimSender = sender;
            fitxaJugador = fitxaEnviada.Text.ToString();
            Enviar(socket, fitxaJugador);
        }
    }
}