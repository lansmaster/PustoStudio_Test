using UnityEngine;

public class DragArrow : MonoBehaviour
{
    private void Start()
    {
        //
    }

    void OnMouseDrag()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 arrowPos = transform.parent.position;
        Vector2 direction = mousePos - arrowPos;

        transform.parent.up = direction;
    }
}
