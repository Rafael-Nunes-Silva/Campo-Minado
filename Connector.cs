using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

static public class Connector
{
    static TcpClient tcpConn;

    static List<KeyValuePair<string,string[]>> messageQueue = new List<KeyValuePair<string, string[]>>(0);

    public static bool Connect(string ip, int port, string name)
    {
        try
        {
            tcpConn = new TcpClient(ip, port);
            Write("NAME", name);

            Task.Run(Listen);

            return true;
        }
        catch (Exception e) { Console.WriteLine("Falha ao conectar com o servidor"); }

        return false;
    }

    public static void Disconnect()
    {
        tcpConn.Close();
    }

    public static bool IsConnected()
    {
        return tcpConn.Connected;
    }

    public static bool Write(params string[] msgParts)
    {
        try
        {
            string msg = "";
            if (msgParts.Length > 1)
            {
                msg = $"|{msgParts[0]}?";
                for (int i = 1; i < msgParts.Length; i++)
                    msg += $"{msgParts[i]}{(i < msgParts.Length ? "&" : "")}";
                msg += "|";
            }
            else msg = $"|{msgParts[0]}|";

            Byte[] buffer = Encoding.UTF8.GetBytes(msg);
            tcpConn.GetStream().Write(buffer, 0, buffer.Length);
        }
        catch (Exception e) { return false; }
        return true;
    }

    public static bool Read(string msgName)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < 30)
        {
            for (int i = 0; i < messageQueue.Count; i++)
            {
                if (messageQueue[i].Key == msgName)
                {
                    messageQueue.RemoveAt(i);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool Read(out string[] msg, string msgName)
    {
        DateTime start = DateTime.Now;
        while ((DateTime.Now - start).TotalSeconds < 30)
        {
            for (int i = 0; i < messageQueue.Count; i++)
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
        }
        msg = new string[0];
        return false;
    }

    static void Listen()
    {
        while (tcpConn.Connected)
        {
            try
            {
                Byte[] buffer = new Byte[512];
                int size = tcpConn.GetStream().Read(buffer, 0, buffer.Length);

                string[] msg = Encoding.UTF8.GetString(buffer).Substring(0, size).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (msg.Length > 1)
                    messageQueue.Add(new KeyValuePair<string, string[]>(msg[0], msg[1].Split('&')));
                else messageQueue.Add(new KeyValuePair<string, string[]>(msg[0], new string[] { "" }));

                /* KeyValuePair<string, string[]>
                List<string> msgArr = new List<string>(0);
                foreach (string s in Encoding.UTF8.GetString(buffer).Substring(0, size).Split('|'))
                    msgArr.Add(s);

                messageQueue.Add(msgArr.ToArray());
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                break;
            }
        }
    }

    public static string GetRooms()
    {
        Write("GET_ROOMS");
        if (Read(out string[] msgArr, "ROOMS"))
            return msgArr[0];
        return "Servidor não respondeu";
    }

    public static bool CreateRoom(string roomName, int maxPlayers, Table.Difficulty difficulty)
    {
        try
        {
            Write($"CREATE_ROOM|{roomName}|{maxPlayers}|{difficulty}");
            if (Read("SUCCESS"))
            {
                Console.WriteLine("Sala criada com sucesso");
                return true;
            }
        }
        catch(Exception e) { Console.WriteLine("Falha ao criar sala"); }
        return false;
    }

    public static bool EnterRoom(string roomName)
    {
        try
        {
            Write($"ENTER_ROOM|{roomName}");
            if (Read("SUCCESS"))
            {
                Console.WriteLine("Conectado com sucesso");
                return true;
            }
        }
        catch (Exception e) { Console.WriteLine("Falha ao entrar na sala"); }
        return false;
    }
}