using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class EnemyVision : MonoBehaviour
{
    Enemy parent;
    PlayerMovement player;
    [SerializeField] float range = 200;
    [SerializeField] float reactionRange = 25;
    [SerializeField] float viewConeAngle = 45;
    [SerializeField] Vector2 reactionTime;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    private bool isVolley = false;
    public void Init(Enemy _parent)
    {
        parent = _parent;//this will be used to update some boolean/stats for decision making
        player = GameManager.instance.playerMovement;
        StartCoroutine(ScanForPlayer());
        StartCoroutine(ScanForObstacles());

        MissileSilo miss = parent.weapon.GetComponent<MissileSilo>();
        if (miss != null)
        {
            miss.target = player.gameObject;
        }
    }

    // Update is called once per frame
    public void Runtime()
    {
        //unused?
        if (parent.PlayerInRange && parent.PlayerInView)
        {
            if (!isVolley) StartCoroutine(Volley());
        }
    }

    IEnumerator ScanForPlayer()
    {
        yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
        while (gameObject)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, range, playerMask);
            if (hit.collider != null)
            {
                //canSeePlayer
                if (hit.collider.gameObject == player.gameObject)
                    parent.CanSeePlayer = true;

                else
                    parent.CanSeePlayer = false;

                //PlayerInView
                Vector3 posVec = (player.transform.position - transform.position).normalized;
                float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

                if (Mathf.Abs(rotAngle) < viewConeAngle) parent.PlayerInView = true;
                else parent.PlayerInView = false;

                //PlayerInRange
                float rangeMult = 1f;
                if (parent.PlayerInView) rangeMult = 1.5f;
                else rangeMult = 0.75f;

                if (hit.distance < parent.weapon.range * rangeMult && hit.collider.gameObject == player.gameObject)
                    parent.PlayerInRange = true;

                else
                    parent.PlayerInRange = false;

                yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator ScanForObstacles()
    {
        yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
        
        while (gameObject)
        {
            if (parent.movement.engine)
            {
                bool hit = false;
                RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, (transform.right * 2 + transform.up).normalized, reactionRange, groundMask);
                RaycastHit2D hitRight = Physics2D.Raycast(transform.position, (transform.right * 2 - transform.up).normalized, reactionRange, groundMask);

                float leftDist = 0.0f;
                float rightDist = 0.0f;
                if (hitLeft.collider != null) leftDist = hitLeft.distance; hit = true;
                if (hitRight.collider != null) rightDist = hitRight.distance; hit = true;

                if (leftDist < rightDist)
                    parent.movement.TurnAwayFromTarget((Vector3)hitLeft.point, parent.movement.rotateSpeed * 2.5f);
                else if (leftDist > rightDist)
                    parent.movement.TurnAwayFromTarget((Vector3)hitRight.point, parent.movement.rotateSpeed * 2.5f);

                if (hit)
                    yield return null;
                else
                    yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator Volley()
    {
        isVolley = true;
        while (parent.weapon.currentAmmo >= 0 && !parent.weapon.reloading)
        {
            parent.weapon.Fire();
            yield return null;
        }
        isVolley = false;
        yield return null;
    }
}
