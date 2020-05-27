using System.Net;
using System.Net.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

// connected = 연결확인; S = 출발위치; E = 목표위치; D= 목표 방향; R= 목표각도; skills = 자식오브젝트; cloneTask = 전송; task = 대기열; T = 사용 시간; V = 속도 ;C = 캐스팅 시간; W = 재사용 대기시간;

public class Player : MonoBehaviour
{
    public Camera cam;
    public GameObject opponent;
    public GameObject tracersA1;
    public GameObject tracersA2;
    public GameObject tracers6;
    public GameObject tracers12;

    public bool connected;
    public bool w, e, rw;
    public bool a,a2, q, rq;
    public bool target;
    public float[] T, V, C, W;
    public float[] Cool;
    public float casting;
    public bool flashable;
    public Transform[] skills;
    public Vector2[] S, E, D, R;
    public Vector2 temp;
    public int character;
    public int r;
    public Queue<KeyValuePair<byte, Vector2>> task;
    public Queue<byte[]> cloneTask;
    UdpClient udp;

    public String debug;


    // Start is called before the first frame update
    void Start()
    {
        task = new Queue<KeyValuePair<byte, Vector2>>();
        cloneTask = new Queue<byte[]>();
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
        udp = new UdpClient(11451);
        connected = false;
        flashable = true;
        e = false;
        character = 5;
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
        C[5] = 0.1f;
        C[6] = 0.25f;
        C[7] = 10;
        C[8] = 0.25f;
        C[9] = 0;
        C[10] = 0.1f;
        C[11] = 0.25f;
        C[12] = 10;
        C[13] = 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        if (connected && Input.GetKeyDown(KeyCode.X))
        {
            flashable = true;
            for (int i = 0; i < 20; ++i)
                Cool[i] = 0;
        }
        casting -= Time.deltaTime;
        for (int i = 0; i < 20; ++i)
            Cool[i] -= Time.deltaTime;
        if (connected && Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform.gameObject == opponent)
                Enqueue(5);
            else
                Enqueue(0);
        }
        if (character == 0)
        {
            if (connected && Input.GetKeyDown(KeyCode.Q))
                Enqueue(1);
            if (connected && Input.GetKeyDown(KeyCode.W))
                Enqueue(2);
            if (connected && Input.GetKeyDown(KeyCode.E))
                Enqueue(3);
            if (connected && Input.GetKeyDown(KeyCode.R))
                Enqueue(4);
        }
        else if (character == 5)
        {
            if (connected && Input.GetKeyDown(KeyCode.Q))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider != null && hit.collider.transform.gameObject == opponent)
                    Enqueue(6);
            }
            if (connected && Input.GetKeyDown(KeyCode.W))
                Enqueue(7);
            if (connected && Input.GetKeyDown(KeyCode.E))
                Enqueue(8);
            if (connected && Input.GetKeyDown(KeyCode.R))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider != null && hit.collider.transform.gameObject == opponent)
                    target = true;
                else
                    target = false;
                Enqueue(9);
            }
        }
        if (connected && flashable && Input.GetKeyDown(KeyCode.F))//f
        {
            flashable = false;
            q = rq = a =a2 = false;
            D[0] = Vector2.zero;
            D[7] = Vector2.zero;
            S[5] = transform.position;
            E[5] = cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            D[5] = E[5] - S[5];
            R[5] = D[5] / D[5].magnitude;
            task.Clear();
            if (w || rw)
                casting = 0;
            a= a2= rw = w = false;
            if (Vector2.Distance(S[5], E[5]) < 4)
                transform.position = E[5];
            else
                transform.position = S[5] + R[5] * 4;
            Move(1);
            Move(2);
            Move(3);
            Move(8);
            Move(13);
        }

        debug = task.Count.ToString();

        while (connected && casting <= 0 && task.Count != 0)
            SetPacket(task.Dequeue());

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
       
        if (connected && a && Vector2.Distance(transform.position, opponent.transform.position) <= 5.25f) //a
        {
            a = false;
            Cool[5] = 1.6f;
            casting = C[5];
            byte[] msg = new byte[5];
            msg[0] = 105;
            float[] t = new float[1];
            t[0] = Time.time;
            Buffer.BlockCopy(t, 0, msg, 1, 4);
            udp.Send(msg, 5);
            Invoke("traceA", 0.25f);
        }
        else if (a)
        {
            S[5] = transform.position;
            D[5] = (temp = opponent.transform.position) - S[5];
            R[5] = D[5] / D[5].magnitude;
            transform.position = new Vector2(S[5].x + R[5].x * Time.deltaTime * V[0], S[5].y + R[5].y * Time.deltaTime * V[0]);
        }
     
        if (connected && q && Vector2.Distance(transform.position, opponent.transform.position) <= 7) //q
        {
            q = false;
            Cool[6] = W[6];
            casting = C[6];
            byte[] msg = new byte[5];
            msg[0] = 106;
            float[] t = new float[1];
            t[0] = Time.time;
            Buffer.BlockCopy(t, 0, msg, 1, 4);
            udp.Send(msg, 5);
            Invoke("trace6", 0.25f);
        }
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

        if (connected && a2 && Vector2.Distance(transform.position, opponent.transform.position) <= 5.25f) //a2
        {
            a2 = false;
            Cool[10] = 1.6f;
            casting = C[10];
            byte[] msg = new byte[5];
            msg[0] = 110;
            float[] t = new float[1];
            t[0] = Time.time;
            Buffer.BlockCopy(t, 0, msg, 1, 4);
            udp.Send(msg, 5);
            Invoke("traceA2", 0.25f);
        }
        else if (a2)
        {
            S[10] = transform.position;
            D[10] = (temp = opponent.transform.position) - S[10];
            R[10] = D[10] / D[10].magnitude;
            transform.position = new Vector2(S[10].x + R[10].x * Time.deltaTime * V[0], S[10].y + R[10].y * Time.deltaTime * V[0]);
        }

        if (connected && rq && Vector2.Distance(transform.position, opponent.transform.position) <= 7) //rq
        {
            rq = false;
            Cool[11] = W[11];
            casting = C[11];
            byte[] msg = new byte[5];
            msg[0] = 111;
            float[] t = new float[1];
            t[0] = Time.time;
            Buffer.BlockCopy(t, 0, msg, 1, 4);
            udp.Send(msg, 5);
            Invoke("trace12", 0.25f);
        }
        else if (rq)
        {
            S[11] = transform.position;
            D[11] = (temp = opponent.transform.position) - S[11];
            R[11] = D[11] / D[11].magnitude;
            transform.position = new Vector2(S[11].x + R[11].x * Time.deltaTime * V[0], S[11].y + R[11].y * Time.deltaTime * V[11]);
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
    public void Setup(IPEndPoint IP)
    {
        udp.Connect(new IPEndPoint(IP.Address, 11449));
        connected = true;
    }
    void Move(int x)
    {
        S[x] = transform.position;
        D[x] = E[x] - S[x];
        R[x] = D[x] / D[x].magnitude;
    }
    void Enqueue(byte x)
    {
        KeyValuePair<byte, Vector2> p = new KeyValuePair<byte, Vector2>(x, cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)));
        task.Enqueue(p);
    }
    void SetPacket(KeyValuePair<byte, Vector2> p)
    {
        float[] piece = new float[5];
        byte[] msg = new byte[21];
        byte x = p.Key;
        if (Cool[x] >= 0)
            switch (x)
            {
                case 7:
                    if (Cool[14] > 0 && Cool[14] <= 4)
                    {
                        Cool[14] = 0;
                        transform.position = S[7];
                        byte[] m = new byte[1];
                        m[0] = 7;
                        udp.Send(m, 1);
                    }
                    return;
                case 9:
                    if (Cool[15] > 0 && Cool[15] <= 4)
                    {
                        Cool[15] = 0;
                        transform.position = S[12];
                        byte[] m = new byte[1];
                        m[0] = 9;
                        udp.Send(m, 1);
                    }
                    return;
                default:
                    return;
            }
        Cool[x] = W[x];
        q = rq = a = a2 = false;
        switch (x)
        {
            case 0:
                break;
            case 3:
                e = true;
                break;
            case 5:
                a = true;
                break;
            case 6:
                r = 2;
                Cool[6] = -1;
                q = true;
                break;
            case 7:
                r = 3;
                break;
            case 8:
                r = 4;
                break;
            case 9:
                if (r == 0)
                {
                    Cool[9] = -1;
                    return;
                }
                x += (byte)r;
                if (r == 2)
                {
                    Cool[9] = -1;
                    rq = true;
                }
                break;
            case 10:
                a2 = true;
                break;
        }
        D[0] = Vector2.zero;
        msg[0] = x;
        casting = C[x];
        E[x] = p.Value;
        piece[2] = E[x].x;
        piece[3] = E[x].y;
        S[x] = transform.position;
        T[x] = piece[4] = Time.time;
        piece[0] = S[x].x;
        piece[1] = S[x].y;
        Buffer.BlockCopy(piece, 0, msg, 1, 20);
        udp.Send(msg, 21);
        D[x] = E[x] - S[x];
        R[x] = D[x] / D[x].magnitude;
    }
}