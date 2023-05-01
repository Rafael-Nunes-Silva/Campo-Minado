using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

public static class Connector
{
    static TcpClient tcpConn;
    static Dictionary<string, string[]> messageQueue = new Dictionary<string, string[]>(0);
    static Object messageQueueLock = new Object();

    static Object rommsLock = new Object(),
        playersLock = new Object(),
        startGameLock = new Object();
    // public static string rooms, players;
    public static bool startGame = false;

    public static bool Connect(string ip, int port, string name)
    {
        // rooms = "Servidor não respondeu";
        // players = "Servidor não respondeu";

        try { tcpConn = new TcpClient(ip, port); }
        catch (Exception e)
        {
            Console.WriteLine("Falha ao conectar com o servidor");
            return false;
        }
        if (!Write("NAME", name))
            return false;

        Task.Run(Listen);

        // WaitForMsg("ROOMS", (rommsData) => { lock (rommsLock) { rooms = rommsData[0]; } }, false);
        // WaitForMsg("PLAYERS", (playersData) => { lock (playersLock) { players = playersData[0]; } }, false);
        WaitForMsg("STARTGAME", (start) => { lock (startGameLock) { startGame = true; } }, false);

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

    public static bool Read(string msgName, int waitTime = 1000)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalMilliseconds <= waitTime)
        {
            lock (messageQueueLock)
            {
                if (messageQueue.ContainsKey(msgName))
                {
                    messageQueue.Remove(msgName);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool Read(out string[] msg, string msgName, int waitTime = 1000)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalMilliseconds <= waitTime)
        {
            lock (messageQueueLock)
            {
                if (messageQueue.ContainsKey(msgName))
                {
                    msg = messageQueue[msgName];
                    return true;
                }
            }
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

                lock (messageQueueLock)
                {
                    if (messageQueue.ContainsKey(msg[0]))
                        messageQueue[msg[0]] = (msg.Length > 1 ? msg[1].Split('&') : new string[] { "" });
                    else messageQueue.Add(msg[0], (msg.Length > 1 ? msg[1].Split('&') : new string[] { "" }));
                }
            }
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

    public static string GetRooms()
    {
        Write("GET_ROOMS");
        if(Read(out string[] rooms, "ROMMS"))
            return rooms[0];
        return "O servidor não respondeu a tempo";
    }

    public static string GetPlayers()
    {
        Write("GET_PLAYERS");
        if (Read(out string[] players, "PLAYERS"))
            return players[0];
        return "O servidor não respondeu a tempo";
    }
}
