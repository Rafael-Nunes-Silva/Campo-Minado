using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

static public class Connector
{
    static TcpClient tcpClient;

    public static bool Connect(string ip, int port, string name)
    {
        try
        {
            tcpClient = new TcpClient(ip, port);
            Write(name);

            // Task.Run(Listen);

            return true;
        }
        catch (Exception e) { Console.WriteLine("Falha ao conectar com o servidor"); }

        return false;
    }

    public static void Disconnect()
    {
        tcpClient.Close();
    }

    public static bool IsConnected()
    {
        return tcpClient.Connected;
    }

    /*
    public static void Listen()
    {
        while (tcpClient.Connected)
        {
            try
            {
                string[] msg = Read().Split('|');

                switch (msg[0])
                {
                    case "CONNECTED":
                        Console.WriteLine(msg[1]);
                        break;
                }
            }
            catch (Exception e) { break; }
        }
    }
    */

    public static bool Write(string msg)
    {
        try
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(msg);
            tcpClient.GetStream().Write(buffer, 0, buffer.Length);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }

    public static string Read()
    {
        Byte[] buffer = new Byte[256];
        int size = tcpClient.GetStream().Read(buffer, 0, buffer.Length);

        return Encoding.UTF8.GetString(buffer).Substring(0, size);
    }

    public static string GetRooms()
    {
        Write("GET_ROOMS");
        return Read();
    }

    public static bool CreateRoom(string roomName, int maxPlayers)
    {
        try
        {
            Write($"CREATE_ROOM|{roomName}|{maxPlayers}");
            if (Read() == "SUCCESS")
                return true;
        }
        catch(Exception e)
        {
            Console.WriteLine("Falha ao criar sala");
        }
        return false;
    }
}