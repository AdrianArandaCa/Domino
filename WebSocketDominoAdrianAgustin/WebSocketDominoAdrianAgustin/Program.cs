using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using WebSocketDominoAdrianAgustin;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;

Console.Title = "Server";
int torn = 0;
int tornJugada = 0;
int dobleSis = 6;
int tornMaxim = 4;
int numMaximValorFitxa = 7;
bool colocadaCorrectament = false;
bool esTorn = false;
string taulerDeJoc = "";
bool repartides = false;
var builder = WebApplication.CreateBuilder();
List<Fitxa> fitxesInmutable = new List<Fitxa>();
List<Fitxa> fitxesEnTauler = new List<Fitxa>();
List<Jugador> jugadors = new List<Jugador>();
List<Jugador> users = new List<Jugador>();
List<Fitxa> fitxesOriginals = generarFitxes();
List<Fitxa> fitxesInvertir = generarFitxesTotals();

builder.WebHost.UseUrls("http://localhost:6666");
var app = builder.Build();

app.UseWebSockets();
app.Map("/ws/{nom}", async (string nom, HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {

        using (var webSocket = await context.WebSockets.AcceptWebSocketAsync())
        {
            //Afegim jugadors fins tenir 4
            if (torn < tornMaxim)
            {
                users.Add(new Jugador(nom, webSocket, torn));
                await webSocket.SendAsync(Encoding.UTF8.GetBytes("Hola " + nom + ":"), WebSocketMessageType.Text, true, CancellationToken.None);
                torn += 1;
            }

            byte[] rcvBufferName;
            var rcvBytes = new byte[256];
            var cts = new CancellationTokenSource();
            ArraySegment<byte> rcvBuffer = new ArraySegment<byte>(rcvBytes);
            string texto;
            //Comprovem que hi hagin 4 jugadors conectats per començar a jugar
            if (torn == tornMaxim)
            {
                if (!repartides)
                {
                    foreach (Jugador j in users)
                    {
                        //Repartim les mans de fitxes a tots el jugadors
                        repartirFitxes(j);
                        foreach (Fitxa f in j.fitxes)
                        {
                            //Indiquem el jugador que te la fitxa dobleSis per poder començar la partida
                            if (f.right == dobleSis && f.left == dobleSis)
                            {
                                rcvBufferName = Encoding.UTF8.GetBytes("?" + f.valor);
                            }
                            //Si no, repartim la fitxa al jugador amb normalitat
                            else
                            {
                                rcvBufferName = Encoding.UTF8.GetBytes(f.valor);
                            }
                            await j.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    repartides = true;
                }
            }
            //Comencem a escoltar
            while (true)
            {
                WebSocketReceiveResult rcvResult = await webSocket.ReceiveAsync(rcvBuffer, cts.Token);
                byte[] msgBytes = rcvBuffer.Take(rcvResult.Count).ToArray();
                texto = System.Text.Encoding.UTF8.GetString(msgBytes);
                //Escoltem els missatges de text, es poden enviar en qualsevol moment.
                if (texto.StartsWith("-"))
                {
                    if (texto.Equals("-??"))
                    {
                        rcvBufferName = Encoding.UTF8.GetBytes(texto);
                        foreach (Jugador u in users)
                        {
                            if (u.sock == webSocket)
                            {
                                texto = texto + "Guanya el jugador: " + u.nom;
                                rcvBufferName = Encoding.UTF8.GetBytes(texto);
                            }
                        }
                        foreach (Jugador u in users)
                        {
                            await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    else {
                        texto = texto.Substring(1);
                        texto = nom + ": " + texto;
                        rcvBufferName = Encoding.UTF8.GetBytes(texto);
                        foreach (Jugador u in users)
                        {
                            if (u.sock != webSocket)
                            {
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                }
                else if (texto.Equals("pasar"))
                {
                    if (tornJugada >= 3)
                    {
                        foreach (Jugador u in users)
                        {
                            if (tornJugada == u.torn)
                            {
                                rcvBufferName = Encoding.UTF8.GetBytes("tornAcabat");
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                        tornJugada = 0;
                        foreach (Jugador u in users)
                        {
                            if (tornJugada == u.torn)
                            {
                                rcvBufferName = Encoding.UTF8.GetBytes("tornComençat");
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                    else
                    {
                        foreach (Jugador u in users)
                        {
                            if (tornJugada == u.torn)
                            {
                                rcvBufferName = Encoding.UTF8.GetBytes("tornAcabat");
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                        tornJugada++;
                        foreach (Jugador u in users)
                        {
                            if (tornJugada == u.torn)
                            {
                                rcvBufferName = Encoding.UTF8.GetBytes("tornComençat");
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                }
                //Escoltem les fitxes de la partida
                else
                {
                    esTorn = false;
                    //Trobem a quin jugador li pertoca el torn
                    foreach (var j in users)
                    {
                        if (webSocket == j.sock && tornJugada == j.torn)
                        {
                            esTorn = true;
                        }
                    }
                    colocadaCorrectament = false;
                    if (esTorn)
                    {
                        //Comprovem que la fitxa es coloca correctament al taulell de joc.
                        logicaJoc(texto);
                        //Es colocada
                        if (colocadaCorrectament)
                        {
                            //Retorem el taulell amb la fitxa colocada.
                            rcvBufferName = Encoding.UTF8.GetBytes(" " + taulerDeJoc);
                            foreach (Jugador u in users)
                            {
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            //Control per a pasar el torn
                            if (tornJugada >= 3)
                            {
                                foreach (Jugador u in users)
                                {
                                    if (tornJugada == u.torn)
                                    {
                                        rcvBufferName = Encoding.UTF8.GetBytes("tornAcabat");
                                        await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                                tornJugada = 0;
                                foreach (Jugador u in users)
                                {
                                    if (tornJugada == u.torn)
                                    {
                                        rcvBufferName = Encoding.UTF8.GetBytes("tornComençat");
                                        await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                            }
                            else
                            {
                                foreach (Jugador u in users)
                                {
                                    if (tornJugada == u.torn)
                                    {
                                        rcvBufferName = Encoding.UTF8.GetBytes("tornAcabat");
                                        await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                                tornJugada++;
                                foreach (Jugador u in users)
                                {
                                    if (tornJugada == u.torn)
                                    {
                                        rcvBufferName = Encoding.UTF8.GetBytes("tornComençat");
                                        await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                            }
                        }
                        //La fitxa no es pot colocar al taulell
                        else
                        {
                            rcvBufferName = Encoding.UTF8.GetBytes("error");
                            foreach (Jugador u in users)
                            {
                                if (u.sock == webSocket)
                                {
                                    await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                        }
                    }
                    //No es torn del jugador, ha d'esperar
                    else
                    {
                        rcvBufferName = Encoding.UTF8.GetBytes("errorTorn");
                        foreach (Jugador u in users)
                        {
                            if (u.sock == webSocket)
                            {
                                await u.sock.SendAsync(rcvBufferName, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                }
            }
        }
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});

await app.RunAsync();


void logicaJoc(string fitxaJugador)
{
    fitxesInmutable = generarFitxesTotals();
    Fitxa fitxa = new Fitxa();
    Fitxa fitxainvertida = new Fitxa();
    foreach (var fi in fitxesInmutable)
    {
        if (fi.valor == fitxaJugador)
        {
            fitxa = fi;
        }
    }

    if (fitxa != null)
    {
        if (fitxesEnTauler.Count == 0)
        {
            if (fitxa.left == dobleSis || fitxa.right == dobleSis)
            {
                fitxesEnTauler.Add(fitxa);
                taulerDeJoc += fitxaJugador + " ";
                colocadaCorrectament = true;
            }
        }
        else
        {
            if (fitxesEnTauler.First().left == fitxa.right || fitxesEnTauler.LastOrDefault().right == fitxa.left)
            {
                if (fitxesEnTauler.First().left == fitxa.right)
                {
                    List<Fitxa> fitxesTemp = new List<Fitxa>();
                    fitxesTemp.Add(fitxa);
                    fitxesTemp.AddRange(fitxesEnTauler);
                    fitxesEnTauler = fitxesTemp;
                    taulerDeJoc = fitxaJugador + " " + taulerDeJoc;
                    colocadaCorrectament = true;
                }
                else
                {
                    fitxesEnTauler.Add(fitxa);
                    taulerDeJoc += fitxaJugador + " ";
                    colocadaCorrectament = true;
                }
            }
            else if (fitxesEnTauler.First().left == fitxa.left || fitxesEnTauler.LastOrDefault().right == fitxa.right)
            {

                foreach (var fitxaInv in fitxesInvertir)
                {
                    if (fitxaInv.left == fitxa.right && fitxaInv.right == fitxa.left)
                    {
                        fitxainvertida = fitxaInv;
                    }
                }

                if (fitxesEnTauler.First().left == fitxa.left)
                {
                    List<Fitxa> fitxesTemp = new List<Fitxa>();
                    fitxesTemp.Add(fitxainvertida);
                    fitxesTemp.AddRange(fitxesEnTauler);
                    fitxesEnTauler = fitxesTemp;
                    fitxaJugador = fitxainvertida.valor.ToString();
                    taulerDeJoc = fitxaJugador + " " + taulerDeJoc;
                    colocadaCorrectament = true;
                }
                else
                {
                    fitxesEnTauler.Add(fitxainvertida);
                    fitxaJugador = fitxainvertida.valor.ToString();
                    taulerDeJoc += fitxaJugador + " ";
                    colocadaCorrectament = true;
                }
            }
        }
    }
}

void repartirFitxes(Jugador jugador)
{
    Random rnd = new Random();

    List<Fitxa> fitxesJugador = new List<Fitxa>();
    fitxesJugador = fitxesOriginals.OrderBy(x => rnd.Next()).Take(numMaximValorFitxa).ToList();
    jugador.fitxes = fitxesJugador;
    for (int i = 0; i < fitxesJugador.Count; i++)
    {
        Fitxa fitxa = fitxesOriginals.Where(a => a.valor.Equals(fitxesJugador[i].valor)).FirstOrDefault();
        if (fitxa.right == dobleSis && fitxa.left == dobleSis)
        {
            tornJugada = jugador.torn;
        }
        fitxesOriginals.Remove(fitxa);
    }
}

List<Fitxa> generarFitxes()
{
    int pos = 2;
    byte[] bytesUtf16 = Encoding.Unicode.GetBytes("\U0001F031");
    List<Fitxa> fitxes = new List<Fitxa>();
    for (int i = 0; i < numMaximValorFitxa; i++)
    {
        for (int j = 0; j < numMaximValorFitxa; j++)
        {
            Fitxa fitxa = new Fitxa(Encoding.Unicode.GetString(bytesUtf16).ToString());
            fitxa.left = i;
            fitxa.right = j;
            bytesUtf16[pos]++;
            if (!fitxes.Exists(f => f.right == i && f.left == j))
            {
                fitxes.Add(fitxa);
            }
        }
    }
    return fitxes;
}

List<Fitxa> generarFitxesTotals()
{
    int pos = 2;
    byte[] bytesUtf16 = Encoding.Unicode.GetBytes("\U0001F031");
    List<Fitxa> fitxes = new List<Fitxa>();
    for (int i = 0; i < numMaximValorFitxa; i++)
    {
        for (int j = 0; j < numMaximValorFitxa; j++)
        {
            Fitxa fitxa = new Fitxa(Encoding.Unicode.GetString(bytesUtf16).ToString());
            fitxa.left = i;
            fitxa.right = j;
            bytesUtf16[pos]++;
            fitxes.Add(fitxa);
        }
    }
    return fitxes;
}
