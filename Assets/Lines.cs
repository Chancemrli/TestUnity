using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{
    // Start is called before the first frame update

    public LineRenderer lineRen;
    public Transform origin;
    public Transform direction;
    void Start()
    {
        lineRen= GetComponent<LineRenderer>();
        lineRen.startWidth = 0.1f;
        lineRen.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        lineRen.SetPosition(0,origin.position);
        lineRen.SetPosition(1,direction.position);
    }
}
