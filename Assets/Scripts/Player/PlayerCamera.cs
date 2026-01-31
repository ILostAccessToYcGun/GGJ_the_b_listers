using System.Net;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Camera _cam;
    private Vector2 midpoint;

    [SerializeField] private float camDistance = 5f;

    public void Init()
    {
        _cam = Camera.main;
    }

    public void Runtime(Vector2 playerPosition)
    {
        Vector2 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        var direction = mousePos - playerPosition;

        var clamped_direction = Vector2.ClampMagnitude(direction, camDistance);

        var final_position = playerPosition + clamped_direction;

        //midpoint = (playerPosition + final_position) / 2.0f;
        midpoint = playerPosition;

        transform.position = new Vector3(midpoint.x, midpoint.y, transform.position.z);
    }
}
