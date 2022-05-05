using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleSquare : MonoBehaviour
{
    private LineRenderer lineRender;
    // Start is called before the first frame update
    void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
    }

    public void SetLeftUpCone(Vector3 point)
    {
        lineRender.SetPosition(0, point);
        Vector3 tmp = lineRender.GetPosition(1);
        tmp.z = point.z;
        lineRender.SetPosition(1, tmp);
        tmp = lineRender.GetPosition(3);
        tmp.x = point.x;
        lineRender.SetPosition(3, tmp);
    }

    public void SetRightDownCone(Vector3 point)
    {
        lineRender.SetPosition(2, point);
        Vector3 tmp = lineRender.GetPosition(1);
        tmp.x = point.x;
        lineRender.SetPosition(1, tmp);
        tmp = lineRender.GetPosition(3);
        tmp.z = point.z;
        lineRender.SetPosition(3, tmp);
    }
}
