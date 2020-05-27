using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    // Start is called before the first frame update
    Vector2 S, D, R,temp;
    bool shot;
    public Transform enemy,player;
    float V = 22.5f;
    public void trace()
    {
        transform.position = player.position;
        shot = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (shot)
        {
            S = transform.position;
            D = (temp = enemy.position) - S;
            R = D / D.magnitude;
            transform.position = new Vector2(S.x + R.x * (Time.deltaTime) * V, S.y + R.y * (Time.deltaTime) * V);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(R.y, R.x) * Mathf.Rad2Deg);
        }
    }
}
