using System.Net;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera _cam;
    private Vector2 midpoint;

    public void Init()
    {
        _cam = Camera.main;
    }

    public void Runtime(Vector2 playerPosition)
    {
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        midpoint = (playerPosition + mousePos) / 2.0f;
        //clamp the midpoint to a distance from the player
        transform.position = midpoint;

    }
}
