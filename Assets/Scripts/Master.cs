using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{
    Thread thread;
    public long time;
    Socket serverSock;
    Socket transferSock;
    Socket clientSock;
    public bool connected;
    public GameObject player;
    public GameObject opponent;

    public byte[] msg;
    
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
        if(connected)
        {
            player.GetComponent<Player>().Setup((IPEndPoint)transferSock.RemoteEndPoint);
            opponent.GetComponent<Opponent>().Setup((IPEndPoint)transferSock.RemoteEndPoint);
            connected = false;
        }
    }
    void Server()
    {
        serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        serverSock.Bind(new IPEndPoint(IPAddress.Any, 11450));
        serverSock.Listen(100);
        transferSock = serverSock.Accept();
        msg[0] = 127;
        sw.Start();
        transferSock.Send(msg);
        transferSock.Receive(msg);
        sw.Stop();
        transferSock.Send(msg);
        time = sw.ElapsedMilliseconds;
        connected = true;
    }
    void Client()
    {
        clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        clientSock.Connect(new IPEndPoint(IPAddress.Loopback, 11450));
        clientSock.Receive(msg);
        sw.Start();
        clientSock.Send(msg);
        clientSock.Receive(msg);
        sw.Stop();
        time = sw.ElapsedMilliseconds;
     //   connected = true;
    }
}
