using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
    public static float timeDiff;
    public static float ping;
    Thread thread;
    public long time;
    public float now;
    Socket serverSock;
    Socket transferSock;
    Socket clientSock;
    public bool connected;
    public GameObject player;
    public GameObject opponent;
    UdpClient udp;
    public byte[] msg;
    public string connection;
    IPEndPoint ip;
    Stopwatch sw;




    public string Debug;
    public string Debug2;


    void Start()
    {
        sw = new Stopwatch();
        Application.runInBackground = true;
        msg = new byte[1];
    }
    public void SetupServer()
    {
        thread = new Thread(Server);
        thread.Start();
    }
    public void SetupClient()
    {
        thread = new Thread(Client);
        thread.Start();
    }


    void Update()
    {
        now = Time.time;
        if(connected)
        {
            player.GetComponent<Player>().Setup(ip);
            opponent.GetComponent<Opponent>().Setup(ip);
            connected = false;
        }
    }
    void Server()
    {
        byte[] shake = new byte[4];
        byte[] shake2 = new byte[4];
        serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        serverSock.Bind(new IPEndPoint(IPAddress.Any, 11450));
        serverSock.Listen(100);
        transferSock = serverSock.Accept();
        ip = (IPEndPoint)transferSock.RemoteEndPoint;
        msg[0] = 127;
        connected = true;
        transferSock.Send(msg);
        udp = new UdpClient(11445);
        udp.Connect(ip.Address, 11445);
        while (true)
        {
            shake = BitConverter.GetBytes(now);
            sw.Start();
            udp.Send(shake, 4);
            shake2 = udp.Receive(ref ip);
            sw.Stop();
            udp.Send(shake, 4);
            time = sw.ElapsedMilliseconds;
            ping = time / 2000.0f;
            timeDiff =now - BitConverter.ToSingle(shake2, 0);
            Debug = ping.ToString();
            Debug2 = timeDiff.ToString();
        }
    }
    void Client()
    {
        byte[] shake = new byte[4];
        byte[] shake2 = new byte[4];
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        clientSock.Connect(new IPEndPoint(IPAddress.Loopback, 11450));
        clientSock.Receive(msg);
        //  udp = new UdpClient(11445);
        //  udp.Connect(((IPEndPoint)transferSock.RemoteEndPoint).Address, 11445);
        //   connected = true;
      //  udp.Connect(ip.Address, 11445);
        while (true)
        {
            shake = BitConverter.GetBytes(now);
            shake2 = udp.Receive(ref ip);
            sw.Start();
            udp.Send(shake, 4);
            shake2 = udp.Receive(ref ip);
            sw.Stop();
            time = sw.ElapsedMilliseconds;
            ping = time / 2000.0f;
            timeDiff = now - BitConverter.ToSingle(shake2, 0);
        }
    }
}
