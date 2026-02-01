using UnityEngine;

public class WaterTriggerHandler : MonoBehaviour
{
    [SerializeField] private LayerMask waterMask;
    [SerializeField] private GameObject splashParticles;

    private EdgeCollider2D edgeCollider;

    private InteractableWater water;

    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        water = GetComponent<InteractableWater>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((waterMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();

            if (rb != null)
            {
                //particles spawn
                Vector2 localPos = gameObject.transform.localPosition;
                Vector2 hitObjectPos = collision.transform.position;
                Bounds hitObjectBounds = collision.bounds;

                Vector3 spawnPos = Vector3.zero;
                if (collision.transform.position.y >= edgeCollider.points[1].y + edgeCollider.offset.y + localPos.y)
                {
                    //collision from above
                    spawnPos = hitObjectPos - new Vector2(0f, hitObjectBounds.extents.y);
                }
                else //not needed for this game but included for future use
                {
                    //hit from below
                    spawnPos = hitObjectPos + new Vector2(0f, hitObjectBounds.extents.y);
                }

                Instantiate(splashParticles, spawnPos, Quaternion.identity);

                //clamp splash point to a max velocity
                int multiplier = 1;
                if (rb.linearVelocityY < 0)
                {
                    multiplier = -1;
                }
                else
                {
                    multiplier = 1;
                }

                float vel = rb.linearVelocityY * water.forceMultiplier;
                vel = Mathf.Clamp(Mathf.Abs(vel), 0f, water.maxForce);
                vel *= multiplier;

                water.Splash(collision, vel);
            }
        }
    }
}
