using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;
using System.Security.Cryptography;

public class Opponent : MonoBehaviour
{
    Thread thread;
    public bool connected;
    public GameObject opponent;
    public GameObject tracersA1;
    public GameObject tracersA2;
    public GameObject tracers6;
    public GameObject tracers12;
    public GameObject etracersA1;
    public GameObject etracersA2;
    public GameObject etracers6;
    public GameObject etracers12;
    public float timeDiff;
    public float ping;
    public bool w, e, rw;
    public bool a, a2, q, rq;
    public bool target;
    public float[] T, V, C, W;
    public float[] Cool;
    public float casting;
    public bool flashable;
    public Transform[] skills;
    public Vector2[] S, E, D, R;
    public Vector2 temp;
    public int r;
    byte x;
    public Queue<byte[]> task;
    IPEndPoint IP;
    UdpClient udp;
    public byte[] msg;
    public byte[] msg2;

    public String debug;


    // Start is called before the first frame update
    void Start()
    {
        task = new Queue<byte[]>();
        skills = new Transform[15];
        skills[0] = transform.GetChild(0);
        skills[1] = transform.GetChild(1);
        skills[2] = transform.GetChild(2);
        skills[3] = transform.GetChild(3);
        skills[4] = transform.GetChild(4);
        skills[5] = transform.GetChild(5);
        skills[6] = transform.GetChild(6);
        skills[7] = transform.GetChild(7);
        skills[8] = transform.GetChild(8);
        skills[9] = transform.GetChild(9);
        skills[10] = transform.GetChild(10);
        skills[11] = transform.GetChild(11);
        skills[12] = transform.GetChild(11);
        skills[13] = transform.GetChild(11);
        skills[14] = transform.GetChild(11);
        S = new Vector2[20];
        E = new Vector2[20];
        D = new Vector2[20];
        R = new Vector2[20];
        T = new float[20];
        V = new float[20];
        C = new float[20];
        W = new float[20];
        Cool = new float[20];
        udp = new UdpClient(11449);
        connected = false;
        flashable = false;
        e = false;
        r = 0;
        V[0] = 3.25f;
        V[1] = 20;
        V[2] = 20;
        V[4] = 15;
        V[6] = 22.5f;
        V[7] = 14.5f;
        V[8] = 17.5f;
        V[11] = 22.5f;
        V[12] = 14.5f;
        V[13] = 17.5f;

        W[1] = 4.5f;
        W[2] = 12;
        W[3] = 15;
        W[4] = 40;
        W[6] = 6;
        W[7] = 10;
        W[8] = 12;
        W[9] = 30;

        C[0] = 0;
        C[1] = 0.25f;
        C[2] = 0.25f;
        C[3] = 0.25f;
        C[4] = 1;
        C[6] = 0.25f;
        C[7] = 10;
        C[8] = 0.25f;
        C[9] = 0;
        C[11] = 0.25f;
        C[12] = 10;
        C[13] = 0.25f;
        msg = new byte[21];
        msg2 = new byte[21];
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
        if (flashable)
            flash();
        ping = Master.ping;
        timeDiff = Master.timeDiff;
        Cool[14] -= Time.deltaTime;
        Cool[15] -= Time.deltaTime;
        casting -= Time.deltaTime;

        while (connected && casting <= 0 && task.Count != 0)
        {
            msg = task.Dequeue();
            x = msg[0];
            a = a2 = q = rq = false;
            target = false;
            switch (x)
            {
                case 3:
                    e = true;
                    break;
                case 5:
                    a = true;
                    break;
                case 6:
                    q = true;
                    break;
                case 7:
                    if (Cool[14] > 0 && Cool[14] <= 4)
                    {
                        Cool[14] = 0;
                        transform.position = S[7];
                        continue;
                    }
                    break;
                case 9:
                    if (Cool[15] > 0 && Cool[15] <= 4)
                    {
                        Cool[15] = 0;
                        transform.position = S[12];
                        continue;
                    }
                    break;
                case 10:
                    a2 = true;
                    break;
                case 11:
                    rq = true;
                    break;
                case 25:
                    Invoke("traceA", 0.25f - ping);
                    target = true;
                    break;
                case 26:
                    Invoke("trace6", 0.25f - ping);
                    target = true;
                    break;
                case 30:
                    Invoke("traceA2", 0.25f - ping);
                    target = true;
                    break;
                case 31:
                    Invoke("trace12", 0.25f - ping);
                    target = true;
                    break;
                case 55:
                    etracersA1.SetActive(false);
                    continue;
                case 56:
                    debug = "yes";  
                    etracers6.SetActive(false);
                    continue;
                case 60:
                    etracersA2.SetActive(false);
                    continue;
                case 61:
                    etracers12.SetActive(false);
                    continue;
            }
            if (!target)
            {
                S[x] = new Vector2(-BitConverter.ToSingle(msg, 1), -BitConverter.ToSingle(msg, 5));
                E[x] = new Vector2(-BitConverter.ToSingle(msg, 9), -BitConverter.ToSingle(msg, 13));
                T[x] = BitConverter.ToSingle(msg, 17);
            }
            else
                T[x -= 20] = BitConverter.ToSingle(msg, 1) + timeDiff;
            D[0] = Vector2.zero;
            casting = C[x];
            D[x] = E[x] - S[x];
            R[x] = D[x] / D[x].magnitude;
        }
        if (task.Count != 0)
        {
            switch (task.Peek()[0])
            {
                case 25:
                    Invoke("traceA", 0.25f - ping);
                    task.Dequeue();
                    target = true;
                    break;
                case 26:
                    Invoke("trace6", 0.25f - ping);
                    task.Dequeue();
                    target = true;
                    break;
                case 30:
                    Invoke("traceA2", 0.25f - ping);
                    task.Dequeue();
                    target = true;
                    break;
                case 31:
                    Invoke("trace12", 0.25f - ping);
                    task.Dequeue();
                    target = true;
                    break;
            }
        }
        if (connected && Time.time - T[0] <= D[0].magnitude / V[0]) //move
            transform.position = new Vector2(S[0].x + R[0].x * (Time.time - T[0]) * V[0], S[0].y + R[0].y * (Time.time - T[0]) * V[0]);

        if (connected && Time.time - T[1] - C[1] >= 0 && Time.time - T[1] - C[1] < 11.50f / V[1]) //q
        {
            skills[1].gameObject.SetActive(true);
            skills[1].position = new Vector2(S[1].x + R[1].x * (Time.time - T[1] - C[1]) * V[1], S[1].y + R[1].y * (Time.time - T[1] - C[1]) * V[1]);
            skills[1].rotation = Quaternion.Euler(0, 0, Mathf.Atan2(R[1].y, R[1].x) * Mathf.Rad2Deg);
        }
        else
            skills[1].gameObject.SetActive(false);

        if (connected && Time.time - T[2] - C[2] >= 0 && Time.time - T[2] - C[2] < 11.50f / V[2]) //w
        {
            skills[2].gameObject.SetActive(true);
            skills[2].position = new Vector2(S[2].x + R[2].x * (Time.time - T[2] - C[2]) * V[2], S[2].y + R[2].y * (Time.time - T[2] - C[2]) * V[2]);
            skills[2].rotation = Quaternion.Euler(0, 0, Mathf.Atan2(R[2].y, R[2].x) * Mathf.Rad2Deg);
        }
        else
            skills[2].gameObject.SetActive(false);

        if (connected && e && Time.time - T[3] - C[3] >= -0.05f)//e
        {
            e = false;
            if (Vector2.Distance(S[3], E[3]) < 4.75f)
                transform.position = E[3];
            else
                transform.position = S[3] + R[3] * 4.75f;
            e = false;
        }

        if (connected && Time.time - T[4] - C[4] >= 0 && Time.time - T[4] - C[4] < 100 / V[4]) //r
        {
            skills[4].gameObject.SetActive(true);
            skills[4].position = new Vector2(S[4].x + R[4].x * (Time.time - T[4] - C[4]) * V[4], S[4].y + R[4].y * (Time.time - T[4] - C[4]) * V[4]);
            skills[4].rotation = Quaternion.Euler(0, 0, Mathf.Atan2(R[4].y, R[4].x) * Mathf.Rad2Deg);
        }
        else
            skills[4].gameObject.SetActive(false);

        if (connected && Cool[5] <= 0 && a && Vector2.Distance(transform.position, opponent.transform.position) <= 5.25f) //a
            a = false;
        else if (a)
        {
            S[5] = transform.position;
            D[5] = (temp = opponent.transform.position) - S[5];
            R[5] = D[5] / D[5].magnitude;
            transform.position = new Vector2(S[5].x + R[5].x * Time.deltaTime * V[0], S[5].y + R[5].y * Time.deltaTime * V[0]);
        }

        if (connected && q && Vector2.Distance(transform.position, opponent.transform.position) <= 7) //q
            q = false;
        else if (q)
        {
            S[6] = transform.position;
            D[6] = (temp = opponent.transform.position) - S[6];
            R[6] = D[6] / D[6].magnitude;
            transform.position = new Vector2(S[6].x + R[6].x * Time.deltaTime * V[0], S[6].y + R[6].y * Time.deltaTime * V[0]);
        }



        if (connected && Time.time - T[7] <= Math.Min(D[7].magnitude, 6) / V[7]) //Lv w
        {
            w = true;
            E[7] = S[7] + R[7] * Math.Min(D[7].magnitude, 6);
            transform.position = new Vector2(S[7].x + R[7].x * (Time.time - T[7]) * V[7], S[7].y + R[7].y * (Time.time - T[7]) * V[7]);
        }
        else if (w)
        {
            w = false;
            casting = 0;
            transform.position = E[7];
            Cool[14] = 4.2f;
        }

        if (connected && Time.time - T[8] - C[8] >= 0 && Time.time - T[8] - C[8] < 8.65f / V[8]) //e
        {
            skills[8].gameObject.SetActive(true);
            skills[8].position = new Vector2(S[8].x + R[8].x * (Time.time - T[8] - C[8]) * V[8], S[8].y + R[8].y * (Time.time - T[8] - C[8]) * V[8]);
            skills[8].rotation = Quaternion.Euler(0, 0, Mathf.Atan2(R[8].y, R[8].x) * Mathf.Rad2Deg);
        }
        else
            skills[8].gameObject.SetActive(false);

        if (connected && Cool[10] <= 0 && a2 && Vector2.Distance(transform.position, opponent.transform.position) <= 5.25f) //a2
            a2 = false;
        else if (a2)
        {
            S[10] = transform.position;
            D[10] = (temp = opponent.transform.position) - S[10];
            R[10] = D[10] / D[10].magnitude;
            transform.position = new Vector2(S[10].x + R[10].x * Time.deltaTime * V[0], S[10].y + R[10].y * Time.deltaTime * V[0]);
        }

        if (connected && rq && Vector2.Distance(transform.position, opponent.transform.position) <= 7) //rq
            rq = false;
        else if (rq)
        {
            S[11] = transform.position;
            D[11] = (temp = opponent.transform.position) - S[11];
            R[11] = D[11] / D[11].magnitude;
            transform.position = new Vector2(S[11].x + R[11].x * Time.deltaTime * V[0], S[11].y + R[11].y * Time.deltaTime * V[0]);
        }

        if (connected && Time.time - T[12] <= Math.Min(D[12].magnitude, 6) / V[12]) //Lv rw
        {
            rw = true;
            E[12] = S[12] + R[12] * Math.Min(D[12].magnitude, 6);
            transform.position = new Vector2(S[12].x + R[12].x * (Time.time - T[12]) * V[12], S[12].y + R[12].y * (Time.time - T[12]) * V[12]);
        }
        else if (rw)
        {
            rw = false;
            casting = 0;
            transform.position = E[12];
            Cool[15] = 4.2f;
        }
        if (connected && Time.time - T[13] - C[13] >= 0 && Time.time - T[13] - C[13] < 8.65f / V[13]) //re
        {
            skills[13].gameObject.SetActive(true);
            skills[13].position = new Vector2(S[13].x + R[13].x * (Time.time - T[13] - C[13]) * V[13], S[13].y + R[13].y * (Time.time - T[13] - C[13]) * V[13]);
            skills[13].rotation = Quaternion.Euler(0, 0, Mathf.Atan2(R[13].y, R[13].x) * Mathf.Rad2Deg);
        }
        else
            skills[13].gameObject.SetActive(false);
        if (flashable)
            flash();
    }
    void flash()
    {
        flashable = false;

        D[0] = Vector2.zero;
        D[7] = Vector2.zero;
        S[5] = new Vector2(-BitConverter.ToSingle(msg2, 1), -BitConverter.ToSingle(msg2, 5));
        E[5] = new Vector2(-BitConverter.ToSingle(msg2, 9), -BitConverter.ToSingle(msg2, 13));
        D[5] = E[5] - S[5];
        R[5] = D[5] / D[5].magnitude;
        Debug.Log(msg2[0]);
        task.Clear();
        if (w || rw)
            casting = 0;
        a = a2 = rw = w = false;
        if (Vector2.Distance(S[5], E[5]) < 4)
            transform.position = E[5];
        else
            transform.position = S[5] + R[5] * 4;
        debug = S[5].ToString();
        Move(1);
        Move(2);
        Move(3);
        Move(8);
        Move(13);
    }
    void Move(int x)
    {
        S[x] = transform.position;
        D[x] = E[x] - S[x];
        R[x] = D[x] / D[x].magnitude;
    }
    void traceA()
    {
        tracersA1.gameObject.SetActive(true);
        tracersA1.gameObject.GetComponent<Tracer>().trace();
    }
    void traceA2()
    {
        tracersA2.gameObject.SetActive(true);
        tracersA2.gameObject.GetComponent<Tracer>().trace();
    }
    void trace6()
    {
        tracers6.gameObject.SetActive(true);
        tracers6.gameObject.GetComponent<Tracer>().trace();
    }
    void trace12()
    {
        tracers12.gameObject.SetActive(true);
        tracers12.gameObject.GetComponent<Tracer>().trace();
    }
    void Receive()
    {
        byte[] msg = new byte[21];
        while (true)
        {
            msg = udp.Receive(ref IP);
            if (msg[0] == 44)
            {
                msg2 = msg;
                flashable = true;
            }
            else
                task.Enqueue(msg);
        }
    }
}
