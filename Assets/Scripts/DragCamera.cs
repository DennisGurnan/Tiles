using UnityEngine;
using System.Collections;

public class DragCamera : MonoBehaviour
{
    public float dragSpeed = 2;
    public float scrollSpeed = 2;
    private Vector3 dragOrigin;

    public bool cameraDragging = true;

    public float zoomMin = 1;
    public float zoomMax = 8;
    public float outerLeft = -10f;
    public float outerRight = 10f;
    public float outerUp = -10f;
    public float outerDown = 10f;

    void Update()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        float ScrollWheelChange = Input.mouseScrollDelta.y;
        if(ScrollWheelChange > 0)
        {
            if(this.transform.position.y < zoomMax) transform.Translate(new Vector3(0, ScrollWheelChange * scrollSpeed, 0), Space.World);
        }
        else if(ScrollWheelChange < 0)
        {
            if (this.transform.position.y > zoomMin) transform.Translate(new Vector3(0, ScrollWheelChange * scrollSpeed, 0), Space.World);
        }

        float left = Screen.width * 0.2f;
        float right = Screen.width - Screen.width * 0.2f;
        float up = Screen.height * 0.2f;
        float down = Screen.height - Screen.height * 0.2f;

        if ((mousePosition.x < left) || (mousePosition.x > right) || (mousePosition.y < up) || (mousePosition.y > down))
        {
            cameraDragging = true;
        }

        if (cameraDragging)
        {

            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (!Input.GetMouseButton(1)) return;

            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

            if (move.x > 0f)
            {
                if (this.transform.position.x < outerRight) transform.Translate(move, Space.World);
            }
            else
            {
                if (this.transform.position.x > outerLeft) transform.Translate(move, Space.World);
            }
            if(move.y > 0f)
            {
                if (this.transform.position.z < outerUp) transform.Translate(move, Space.World);
            }
            else
            {
                if (this.transform.position.z > outerDown) transform.Translate(move, Space.World);
            }
        }
    }


}

