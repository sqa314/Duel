using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Opponent : MonoBehaviour
{
    Thread thread;
    public bool connected;
    public Vector2 playerMoveDest;
    public Vector2 playerMoveStart;
    public Vector2 playerMoveDir;
    public Vector2 playerMoveRot;
    public float timeMove;
    public float speed;
    public float[] fa;
    public byte[] msg;
    public Queue<byte[]> task;
    UdpClient udp;
    IPEndPoint IP;

    public string debug;

    void Start()
    {
        task = new Queue<byte[]>();
        fa = new float[5];
        msg = new byte[21];
        udp = new UdpClient(11449);
        connected = false;
        timeMove = 0;
        speed = 4;
    }
    public void Setup(IPEndPoint ip)
    {
        IP = ip;
        thread = new Thread(Receive);
        thread.Start();
        connected = true;
    }

    void Update()
    {
        if (connected && task.Count != 0)
        {
            msg = task.Dequeue();
            switch (msg[0])
            {
                case 15:
                    move(msg);
                    break;
            }
        }
       
        if (connected && Time.time - timeMove <= playerMoveDir.magnitude / speed)
            transform.position = new Vector2(playerMoveStart.x + playerMoveRot.x * (Time.time - timeMove) * speed, playerMoveStart.y + playerMoveRot.y * (Time.time - timeMove) * speed);

    }
    void move(byte[] msg)
    {
        playerMoveStart = new Vector2(-BitConverter.ToSingle(msg, 1), -BitConverter.ToSingle(msg, 5));
        playerMoveDest = new Vector2(-BitConverter.ToSingle(msg, 9), -BitConverter.ToSingle(msg, 13));
        timeMove = BitConverter.ToSingle(msg, 17);
        playerMoveDir = playerMoveDest - playerMoveStart;
        playerMoveRot = playerMoveDir / playerMoveDir.magnitude;
    }
    void Receive()
    {
        while (true)
        {
            task.Enqueue(udp.Receive(ref IP));
        }
    }
}
