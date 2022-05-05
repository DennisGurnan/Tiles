using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum State
    {
        SELECT_CONER_A,
        SELECT_CONER_B,
        PUT_SEAM
    }
    public InputField UI_Seam;
    public InputField UI_Angle;
    public InputField UI_Bias;
    public InputField UI_Width;
    public InputField UI_Square;

    public RectangleSquare rectangle;

    public GameObject tilePrefab;
    public GameObject testObject;

    private float seam;
    private float angle;
    private float bias;
    private float width;
    private float square;

    private State state;

    private Plane floor = new Plane(Vector3.up, 0);

    private Vector3 ConerA = new Vector3(0, 0, 0);
    private Vector3 ConerB = new Vector3(0, 0, 0);
    private Vector3 centerPoint = new Vector3(0, 0, 0);


    // Start is called before the first frame update
    void Start()
    {
        seam = float.Parse(UI_Seam.text) * 0.001f;
        angle = float.Parse(UI_Angle.text) * (float)Math.PI / 180;
        bias = float.Parse(UI_Bias.text) * 0.001f;
        width = float.Parse(UI_Width.text) * 0.001f;
        centerPoint = new Vector3(0, 0, 0);
        ConerASelect(new Vector3(-1, 0, -1));
        ConerBSelect(new Vector3(1, 0, 1));
        state = State.SELECT_CONER_A;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            float distance;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (floor.Raycast(ray, out distance))
            {
                Vector3 mouseWordPosition = ray.GetPoint(distance);
                if (state == State.SELECT_CONER_A) ConerASelect(new Vector3(-1, 0, -1)); //ConerASelect(mouseWordPosition);
                else if (state == State.SELECT_CONER_B) ConerBSelect(new Vector3(1, 0, 1));// ConerBSelect(mouseWordPosition);
                else PlaceTile(new Vector3(0, 0, 0));//PlaceTile(mouseWordPosition);
            }
        }
        */
    }

    private void ConerASelect(Vector3 point)
    {
        rectangle.SetLeftUpCone(point);
        ConerA = point;
        state = State.SELECT_CONER_B;
    }

    private void ConerBSelect(Vector3 point)
    {
        rectangle.SetRightDownCone(point);
        ConerB = point;
        state = State.PUT_SEAM;
    }

    private void PlaceTile(Vector3 point)
    {
        centerPoint = point;
        UpdateFloor();
    }

    private void UpdateFloor()
    {
        if(transform.childCount > 0)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));
        }
        Vector2 sqCenter = new Vector2((ConerA.x + ConerB.x) / 2f, (ConerA.z + ConerB.z) / 2f);
        Rectangle sq = new Rectangle(sqCenter, Mathf.Abs(ConerB.x - ConerA.x), Mathf.Abs(ConerB.z - ConerA.z), 0f);
        List<Rectangle> tiles = FillSquare(sq);
        square = 0;
        foreach(Rectangle t in tiles)
        {
            GameObject tile = Instantiate(tilePrefab, transform);
            Tile tl = tile.GetComponent<Tile>();
            tl.GenerateMesh(t, ConerA, ConerB);
            square += tl.square;
        }
        UI_Square.text = square.ToString("#0.000");
    }

    private bool IsInArray(List<Rectangle> list, Rectangle rect)
    {
        foreach(Rectangle r in list)
        {
            if ((Mathf.Abs(r.center.x - rect.center.x) < width / 2) && (Mathf.Abs(r.center.y - rect.center.y) < width / 2)) return true;
        }
        return false;
    }

    private List<Rectangle> FillSquare(Rectangle sq)
    {
        List<Rectangle> tiles = new List<Rectangle>();
        Vector2 centerTile = new Vector2(centerPoint.x, centerPoint.z);
        Rectangle cr = new Rectangle(centerTile, width, width, angle);
        tiles.Add(cr);
        bool isComplite = false;
        while (!isComplite)
        {
            List<Rectangle> tiles_temp = new List<Rectangle>();
            isComplite = true;
            foreach(Rectangle r in tiles)
            {
                tiles_temp.Add(r);
                if (!r.isChecked())
                {
                    if (!r.neighbours[0])
                    {
                        isComplite = false;
                        float distance = width + seam;
                        float agl = angle;
                        float nx = r.center.x + distance * Mathf.Cos(agl);
                        float ny = r.center.y + distance * Mathf.Sin(agl); 
                        Rectangle n = new Rectangle(new Vector2(nx, ny), width, width, angle);
                        n.neighbours[2] = true;
                        if(n.IsIntersecting(sq) && !IsInArray(tiles_temp, n) && !IsInArray(tiles, n)) tiles_temp.Add(n);
                        r.neighbours[0] = true;
                    }
                    if (!r.neighbours[1])
                    {
                        isComplite = false;
                        float distance = Mathf.Sqrt(Mathf.Pow(width + seam, 2) + Mathf.Pow(bias, 2));
                        float agl = angle + Mathf.Asin((width + seam) / distance);
                        float nx = r.center.x + distance * Mathf.Cos(agl);
                        float ny = r.center.y + distance * Mathf.Sin(agl);
                        Rectangle n = new Rectangle(new Vector2(nx, ny), width, width, angle);
                        n.neighbours[3] = true;
                        if (n.IsIntersecting(sq) && !IsInArray(tiles_temp, n) && !IsInArray(tiles, n)) tiles_temp.Add(n);
                        r.neighbours[1] = true;
                    }
                    if (!r.neighbours[2])
                    {
                        isComplite = false;
                        float distance = width + seam;
                        float agl = Mathf.PI + angle;
                        float nx = r.center.x + distance * Mathf.Cos(agl);
                        float ny = r.center.y + distance * Mathf.Sin(agl);
                        Rectangle n = new Rectangle(new Vector2(nx, ny), width, width, angle);
                        n.neighbours[0] = true;
                        if (n.IsIntersecting(sq) && !IsInArray(tiles_temp, n) && !IsInArray(tiles, n)) tiles_temp.Add(n);
                        r.neighbours[2] = true;
                    }
                    if (!r.neighbours[3])
                    {
                        isComplite = false;
                        float distance = Mathf.Sqrt(Mathf.Pow(width + seam, 2) + Mathf.Pow(bias, 2));
                        float agl = angle + Mathf.Asin((width + seam) / distance) + Mathf.PI;
                        float nx = r.center.x + distance * Mathf.Cos(agl);
                        float ny = r.center.y + distance * Mathf.Sin(agl);
                        Rectangle n = new Rectangle(new Vector2(nx, ny), width, width, angle);
                        n.neighbours[1] = true;
                        if (n.IsIntersecting(sq) && !IsInArray(tiles_temp, n) && !IsInArray(tiles, n)) tiles_temp.Add(n);
                        r.neighbours[3] = true;
                    }
                }
            }
            tiles = tiles_temp;
            if (tiles.Count > 1000) isComplite = true;
        }
        return tiles;
    }

    public void OnValuesChanged()
    {
        try
        {
            seam = float.Parse(UI_Seam.text) * 0.001f;
            angle = float.Parse(UI_Angle.text) * (float)Math.PI/180;
            bias = float.Parse(UI_Bias.text) * 0.001f;
            width = float.Parse(UI_Width.text) * 0.001f;
            UpdateFloor();
        }catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
