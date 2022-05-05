using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public float square;

    public void GenerateMesh(Rectangle rect, Vector3 UpLeft, Vector3 DownRight)
    {
        Vector2[] sq = new Vector2[4];
        sq[0] = new Vector2(UpLeft.x, UpLeft.z);
        sq[1] = new Vector2(DownRight.x, UpLeft.z);
        sq[2] = new Vector2(DownRight.x, DownRight.z);
        sq[3] = new Vector2(UpLeft.x, DownRight.z);
        List<Vector3> coners = new List<Vector3>();
        for(int i = 0; i < 4; i++)
        {
            Vector2 p1 = rect.points[i];
            Vector2 p2 = new Vector2();
            if (i == 3) p2 = rect.points[0];
            else p2 = rect.points[i + 1];
            if (IsInRect(p1, sq) && IsInRect(p2, sq)) coners.Add(new Vector3(p1.x, 0, p1.y));
            else if(IsInRect(p1, sq) && !IsInRect(p2, sq))
            {
                coners.Add(new Vector3(p1.x, 0, p1.y));
                Vector2[] itr = GetIntersection(p1, p2, sq);
                coners.Add(new Vector3(itr[0].x, 0, itr[0].y));
            }else if(!IsInRect(p1,sq) && IsInRect(p2, sq))
            {
                Vector2[] itr = GetIntersection(p1, p2, sq);
                coners.Add(new Vector3(itr[0].x, 0, itr[0].y));
            }else if(!IsInRect(p1,sq) && !IsInRect(p2, sq))
            {
                bool isCone = false;
                for(int j = 0; j < 4; j++)
                {
                    if(IsInRect(sq[j], rect.points))
                    {
                        coners.Add(new Vector3(sq[j].x, 0, sq[j].y));
                        isCone = true;
                        break;
                    }
                }
                if (!isCone)
                {
                    Vector2[] itr = GetIntersection(p1, p2, sq);
                    if(itr.Length > 0)
                    {
                        coners.Add(new Vector3(itr[0].x, 0, itr[0].y));
                        coners.Add(new Vector3(itr[1].x, 0, itr[1].y));
                    }
                }
            }
        }
        /*
        for(int i1 = 0; i1 < 4; i1++)
        {
            if (IsInRect(rect.points[i1], sq)) coners.Add(new Vector3(rect.points[i1].x, 0, rect.points[i1].y));
            for(int j1 = 0; j1 < 4; j1++)
            {
                int i2 = i1 + 1;
                if (i2 > 3) i2 = 0;
                int j2 = j1 + 1;
                if (j2 > 3) j2 = 0;
                if(IsIntersection(rect.points[i1], rect.points[i2], sq[j1], sq[j2]))
                {
                    Vector2 itr = GetIntersection(rect.points[i1], rect.points[i2], sq[j1], sq[j2]);
                    coners.Add(new Vector3(itr.x, 0, itr.y));
                }
                if (IsInRect(sq[j2], rect.points)) coners.Add(new Vector3(sq[j2].x, 0, sq[j2].y));
            }
        }
        */
        Mesh mesh = new Mesh();
        mesh.vertices = coners.ToArray();

        int trnum = (coners.Count - 2);
        square = 0;
        List<int> tri = new List<int>();
        for(int i = 0; i < trnum; i++)
        {
            tri.Add(i + 2);
            tri.Add(i + 1);
            tri.Add(0);
            square += GetSquare(coners[0], coners[i + 1], coners[i + 2]);
        }
        mesh.triangles = tri.ToArray();

        List<Vector3> normals = new List<Vector3>();
        Vector2 normal = Vector3.Cross(coners[1] - coners[0], coners[2] - coners[0]).normalized;
        for (int i = 0; i < coners.Count; i++) normals.Add(-normal);
        mesh.normals = normals.ToArray();

        List<Vector2> uv = new List<Vector2>();
        Vector2 rp = new Vector2();
        foreach(Vector3 c in coners)
        {
            uv.Add(new Vector2(c.x, c.z));
            if((rp == Vector2.zero) && (IsInRect(uv[uv.Count - 1], sq))) rp = uv[uv.Count - 1];
        }
        for(int i = 0; i < uv.Count; i++)
        {
            uv[i] = (RotatePoint(rp, uv[i], -rect.angle) - rp) / rect.width;
        }
        mesh.uv = uv.ToArray();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private Vector2 RotatePoint(Vector2 t0, Vector2 t, float angle)
    {
        float nx = t0.x + (t.x - t0.x) * Mathf.Cos(angle) - (t.y - t0.y) * Mathf.Sin(angle);
        float ny = t0.y + (t.x - t0.x) * Mathf.Sin(angle) + (t.y - t0.y) * Mathf.Cos(angle);
        return new Vector2(nx, ny);
    }

    private bool IsIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float Xa, Ya, A1, A2, b1, b2;

        if(b.x < a.x)
        {
            Vector2 temp = a;
            a = b;
            b = temp;
        }
        if(d.x < c.x)
        {
            Vector2 temp = c;
            c = d;
            d = temp;
        }
        if (b.x < c.x) return false;

        if((a.x - b.x == 0) && (c.x-d.x == 0))
        {
            if(a.x == c.x)
            {
                if (!((Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)) || (Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y)))) return true;
            }
            return false;
        }

        if(a.x - b.x == 0)
        {
            Xa = a.x;
            A2 = (c.y - d.y) / (c.x - d.x);
            b2 = c.y - A2 * c.x;
            Ya = A2 * Xa + b2;
            if (c.x <= Xa && d.x >= Xa && Mathf.Min(a.y, b.y) <= Ya && Mathf.Max(a.y, b.y) >= Ya) return true;
            return false;
        }

        if(c.x - d.x == 0)
        {
            Xa = c.x;
            A1 = (a.y - b.y) / (a.x - b.x);
            b1 = a.y - A1 * a.x;
            Ya = A1 * Xa + b1;
            if (a.x <= Xa && b.x >= Xa && Mathf.Min(c.y, d.y) <= Ya && Mathf.Max(c.y, d.y) >= Ya) return true;
            return false;
        }

        A1 = (a.y - b.y) / (a.x - b.x);
        A2 = (c.y - d.y) / (c.x - d.x);
        b1 = a.y - A1 * a.x;
        b2 = c.y - A2 * c.x;
        if (A1 == A2) return false;
        Xa = (b2 - b1) / (A1 - A2);
        if (Xa < Mathf.Max(a.x, c.x) || Xa > Mathf.Min(b.x, d.x)) return false;
        return true;
    }

    private Vector2[] GetIntersection(Vector2 a, Vector2 b, Vector2[] rect)
    {
        List<Vector2> intersections = new List<Vector2>();
        for(int i = 0; i < 4; i++)
        {
            Vector2 ra = rect[i];
            Vector2 rb = new Vector2();
            if (i == 3) rb = rect[0];
            else rb = rect[i + 1];
            if (IsIntersection(a, b, ra, rb)) intersections.Add(GetIntersection(a, b, ra, rb));
        }
        return intersections.ToArray();
    }

    private Vector2 GetIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float dm = (a.x - b.x) * (d.y - c.y) - (a.y - b.y) * (d.x - c.x);
        float da = (a.x - c.x) * (d.y - c.y) - (a.y - c.y) * (d.x - c.x);
        float db = (a.x - b.x) * (a.y - c.y) - (a.y - b.y) * (a.x - c.x);
        float ta = da / dm;
        float tb = db / dm;
        float dx = a.x + ta * (b.x - a.x);
        float dy = a.y + ta * (b.y - a.y);
        return new Vector2(dx, dy);
    }

    private bool IsInRect(Vector2 point, Vector2[] rect)
    {
        float p1 = Product(point, rect[0], rect[1]);
        float p2 = Product(point, rect[1], rect[2]);
        float p3 = Product(point, rect[2], rect[3]);
        float p4 = Product(point, rect[3], rect[0]);
        return (p1 < 0 && p2 < 0 && p3 < 0 && p4 < 0) || (p1 > 0 && p2 > 0 && p3 > 0 && p4 > 0); 
    }

    private float Product(Vector2 p, Vector2 a, Vector2 b)
    {
        return (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
    }

    private float GetSquare(Vector3 a, Vector3 b, Vector3 c)
    {
        float A = GetLength(a, b);
        float B = GetLength(b, c);
        float C = GetLength(c, a);
        float p = (A + B + C) / 2;
        return Mathf.Sqrt(p * (p - A) * (p - B) * (p - C));
    }

    private float GetLength(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(Mathf.Pow(b.x - a.x, 2) + Mathf.Pow(b.z - a.z, 2));
    }
}
