using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class Player : MonoBehaviour
{
    public Camera cam;
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

    public String debug;


    // Start is called before the first frame update
    void Start()
    {
        task =new Queue<byte[]>();
        fa = new float[5];
        msg = new byte[21];
        udp = new UdpClient(11451);
        connected = false;
        timeMove = 0;
        speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (connected && Input.GetMouseButton(1) && Vector2.Distance(cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)), new Vector2(playerMoveDest.x, playerMoveDest.y)) > 0.5f)
        {
            timeMove = Time.time - Time.deltaTime;
            playerMoveStart = transform.position;
            playerMoveDest = cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            playerMoveDir = playerMoveDest - playerMoveStart;
            playerMoveRot = playerMoveDir / playerMoveDir.magnitude;
            SetPacket(15, playerMoveStart, playerMoveDest, timeMove);
        }
        if (connected && Time.time - timeMove <= playerMoveDir.magnitude / speed)
            transform.position = new Vector2(playerMoveStart.x + playerMoveRot.x * (Time.time - timeMove) * speed, playerMoveStart.y + playerMoveRot.y * (Time.time - timeMove) * speed);
        if (connected && task.Count != 0)
        {
            msg = task.Dequeue();
            for (int i = 0; i < 3; ++i)
                udp.Send(msg, 21);
        }
    }
    public void Setup(IPEndPoint IP)
    {
        udp.Connect(new IPEndPoint(IP.Address, 11449));
        connected = true;
    }
    void SetPacket(byte c, Vector2 a, Vector2 b, float t)
    {
        float[] farray = new float[5];
        byte[] m = new byte[21];
        farray[0] = a.x;
        farray[1] = a.y;
        farray[2] = b.x;
        farray[3] = b.y;
        farray[4] = t;
        m[0] = c;
        Buffer.BlockCopy(farray, 0, m, 1, 20);
        task.Enqueue(m);
    }
}
