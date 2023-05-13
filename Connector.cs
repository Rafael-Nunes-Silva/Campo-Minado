using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

public static class Connector
{
    static string name;

    static TcpClient tcpConn;
    static Dictionary<string, string[]> messageQueue = new Dictionary<string, string[]>(0);
    static Object messageQueueLock = new Object();

    public static bool gaming = false;

    public static bool Connect(string ip, int port, string name)
    {
        Connector.name = name;

        try { tcpConn = new TcpClient(ip, port); }
        catch (Exception e)
        {
            Console.WriteLine("Falha ao conectar com o servidor");
            return false;
        }
        if (!Write("NAME", name))
            return false;

        Task.Run(Listen);

        // WaitForMsg("STARTGAME", (content) => { gaming = true; }, true);
        // WaitForMsg("ENDGAME", (content) => { gaming = false; }, true);

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

    public static void WaitForMsg(string msgName, Action<string[]> receiveCallback, bool repeat = false)
    {
        Task.Run(() =>
        {
            while (tcpConn.Connected)
            {
                if (Read(msgName, out string[] strArr))
                {
                    receiveCallback(strArr);
                    if (!repeat)
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

    public static bool Read(string msgName, int maxWaitTime = 10000)
    {
        DateTime start = DateTime.Now;
        do
        {
            lock (messageQueueLock)
            {
                if (messageQueue.ContainsKey(msgName))
                {
                    messageQueue.Remove(msgName);
                    return true;
                }
            }
            Thread.Sleep(100);
        } while ((DateTime.Now - start).TotalMilliseconds < maxWaitTime);
        return false;
    }

    public static bool Read(string msgName, out string[] content, int maxWaitTime = 10000)
    {
        DateTime start = DateTime.Now;
        do
        {
            lock (messageQueueLock)
            {
                if (messageQueue.ContainsKey(msgName))
                {
                    content = messageQueue[msgName];
                    messageQueue.Remove(msgName);
                    return true;
                }
            }
            Thread.Sleep(100);
        } while ((DateTime.Now - start).TotalMilliseconds < maxWaitTime);
        content = new string[0];
        return false;
    }

    static void Listen()
    {
        while (tcpConn.Connected)
        {
            Byte[] buffer = new Byte[4096];
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

            Thread.Sleep(100);
        }
    }

    public static bool CreateRoom(string roomName, int maxPlayers, Table.Difficulty difficulty)
    {
        Write("CREATE_ROOM", roomName, maxPlayers.ToString(), ((int)difficulty).ToString());

        if (Read("CREATE_ROOM_SUCCESS"))
        {
            Console.WriteLine("Sala criada com sucesso");
            return true;
        }
        Console.WriteLine("Falha ao criar sala");
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

        if (Read("ROOMS", out string[] rooms))
            return rooms[0];
        return "O servidor não respondeu a tempo";
    }

    public static string GetPlayers()
    {
        Write("GET_PLAYERS");

        if (Read("PLAYERS", out string[] players))
            return players[0];
        return "O servidor não respondeu a tempo";
    }
}
