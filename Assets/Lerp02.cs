using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lerp02 : MonoBehaviour
{

    public Transform target01;
    public Transform target02;
    public Transform player;
    public float fract;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Rotate the arrow
        Vector3 dir = player.transform.TransformPoint(target02.position);
        float a = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float b = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        a += 180;
        target01.transform.localEulerAngles = new Vector3(0, 0, a);
        //transform.LookAt(target02);
    }
}
