using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

public static class Connector
{
    static TcpClient tcpConn;
    static List<KeyValuePair<string, string[]>> messageQueue = new List<KeyValuePair<string, string[]>>(0);
    static Mutex messageQueueMut = new Mutex();

    static Mutex rommsMut = new Mutex(),
        playersMut = new Mutex(),
        startGameMut = new Mutex();
    public static string rooms, players;
    public static bool startGame = false;

    public static bool Connect(string ip, int port, string name)
    {
        rooms = "Servidor não respondeu";
        players = "Servidor não respondeu";

        try { tcpConn = new TcpClient(ip, port); }
        catch (Exception e)
        {
            Console.WriteLine("Falha ao conectar com o servidor");
            return false;
        }
        if (!Write("NAME", name))
            return false;

        Task.Run(Listen);

        WaitForMsg("ROOMS", (rommsData) =>
        {
            rommsMut.WaitOne();
            rooms = rommsData[0];
            rommsMut.ReleaseMutex();
        }, false);
        WaitForMsg("PLAYERS", (playersData) =>
        {
            playersMut.WaitOne();
            players = playersData[0];
            playersMut.ReleaseMutex();
        }, false);
        WaitForMsg("STARTGAME", (start) =>
        {
            startGameMut.WaitOne();
            startGame = true;
            startGameMut.ReleaseMutex();
        }, false);

        return true;
    }

    public static void Disconnect()
    {
        tcpConn.Close();
    }

    public static bool IsConnected()
    {
        return tcpConn.Connected;
    }

    public static void WaitForMsg(string msg, Action<string[]> receiveCallback, bool once = true)
    {
        Task.Run(() =>
        {
            while (tcpConn.Connected)
            {
                if (Read(out string[] strArr, msg))
                {
                    receiveCallback(strArr);
                    if (once)
                        return;
                }
            }
        });
    }

    public static bool Write(params string[] msgParts)
    {
        string msg = "";
        if (msgParts.Length > 1)
        {
            msg = $"|{msgParts[0]}?";
            for (int i = 1; i < msgParts.Length; i++)
                msg += $"{msgParts[i]}{(i < msgParts.Length - 1 ? "&" : "")}";
            msg += "|";
        }
        else msg = $"|{msgParts[0]}|";

        try
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(msg);
            tcpConn.GetStream().Write(buffer, 0, buffer.Length);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public static bool Read(string msgName)
    {
        if (messageQueueMut.WaitOne())
        {
            for (int i = 0; i < messageQueue.Count; i++)
            {
                if (messageQueue[i].Key == msgName)
                {
                    messageQueue.RemoveAt(i);
                    return true;
                }
            }
            messageQueueMut.ReleaseMutex();
        }
        return false;
    }

    public static bool Read(out string[] msg, string msgName)
    {
        if (messageQueueMut.WaitOne())
        {
            for (int i = 0; i < messageQueue.Count; i++)
            {
                try
                {
                    if (messageQueue[i].Key == msgName)
                    {
                        msg = new string[messageQueue[i].Value.Length];
                        for (int j = 0; j < messageQueue[i].Value.Length; j++)
                            msg[j] = messageQueue[i].Value[j];
                        messageQueue.RemoveAt(i);
                        return true;
                    }
                }
                catch (Exception e) { break; }
            }
            messageQueueMut.ReleaseMutex();
        }
        msg = new string[0];
        return false;
    }

    static void Listen()
    {
        while (tcpConn.Connected)
        {
            Byte[] buffer = new Byte[512];
            int size;
            try { size = tcpConn.GetStream().Read(buffer, 0, buffer.Length); }
            catch (Exception e)
            {
                Console.WriteLine(e);
                break;
            }

            foreach (string receivedMsg in Encoding.UTF8.GetString(buffer).Substring(0, size).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] msg = receivedMsg.Split('?');

                if (messageQueueMut.WaitOne())
                {
                    if (msg.Length > 1)
                        messageQueue.Add(new KeyValuePair<string, string[]>(msg[0], msg[1].Split('&')));
                    else messageQueue.Add(new KeyValuePair<string, string[]>(msg[0], new string[] { "" }));
                    messageQueueMut.ReleaseMutex();
                }
            }
            Thread.Sleep(1000);
        }
    }

    public static bool CreateRoom(string roomName, int maxPlayers, Table.Difficulty difficulty)
    {
        try
        {
            Write("CREATE_ROOM", roomName, maxPlayers.ToString(), ((int)difficulty).ToString());
            if (Read("CREATE_ROOM_SUCCESS"))
            {
                Console.WriteLine("Sala criada com sucesso");
                return true;
            }
        }
        catch (Exception e) { Console.WriteLine("Falha ao criar sala"); }
        return false;
    }

    public static bool EnterRoom(string roomName)
    {
        try { Write("ENTER_ROOM", roomName); }
        catch (Exception e) { Console.WriteLine(e); }
        if (Read("ENTER_ROOM_SUCCESS"))
        {
            Console.WriteLine("Conectado com sucesso");
            return true;
        }
        Console.WriteLine("Falha ao entrar na sala");
        return false;
    }
}
