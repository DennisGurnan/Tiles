using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle
{
    public Vector2[] points = new Vector2[4];
    public bool[] neighbours = new bool[4];
    public Vector2 center;
    public float height;
    public float width;
    public float angle;

    public Rectangle(Vector2 _center, float _width, float _height, float _angle)
    {
        center = _center;
        angle = _angle;
        height = _height;
        width = _width;
        float dx = width / 2;
        float dy = height / 2;
        for (int i = 0; i < neighbours.Length; i++) neighbours[i] = false;
        points[0] = Rotate(new Vector2(-dx, -dy), center, angle);
        points[1] = Rotate(new Vector2(dx, -dy), center, angle);
        points[2] = Rotate(new Vector2(dx, dy), center, angle);
        points[3] = Rotate(new Vector2(-dx, dy), center, angle);
    }

    private Vector2 Rotate(Vector2 p, Vector2 center, float angle)
    {
        float nx = p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle) + center.x;
        float ny = p.x * Mathf.Sin(angle) + p.y * Mathf.Cos(angle) + center.y;
        return new Vector2(nx, ny);
    }

    public bool isChecked()
    {
        bool result = true;
        foreach (bool i in neighbours) result &= i;
        return result;
    }

    public bool IsIntersecting(Rectangle rectangle)
    {
        foreach(Rectangle rect in new[] {this, rectangle })
        {
            for(int i1 = 0; i1 < 4; i1++)
            {
                int i2 = (i1 + 1) % 4;
                Vector2 p1 = rect.points[i1];
                Vector2 p2 = rect.points[i2];
                Vector2 normal = new Vector2(p2.y - p1.y, p1.x - p2.x);
                float? minA = null, maxA = null;
                foreach(Vector2 p in this.points)
                {
                    float projected = normal.x * p.x + normal.y * p.y;
                    if (minA == null || projected < minA) minA = projected;
                    if (maxA == null || projected > maxA) maxA = projected;
                }
                float? minB = null, maxB = null;
                foreach (Vector2 p in rectangle.points)
                {
                    float projected = normal.x * p.x + normal.y * p.y;
                    if (minB == null || projected < minB) minB = projected;
                    if (maxB == null || projected > maxB) maxB = projected;
                }
                if (maxA < minB || maxB < minA) return false;
            }
        }
        return true;
    }
}
