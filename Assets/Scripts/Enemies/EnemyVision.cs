using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    Enemy parent;
    [SerializeField] float range = 200;
    [SerializeField] Vector2 reactionTime;
    [SerializeField] private LayerMask lineOfSightMask;
    public void Init(Enemy _parent)
    {
        parent = _parent;//this will be used to update some boolean/stats for decision making
        StartCoroutine(ScanForPlayer());
    }

    // Update is called once per frame
    public void Runtime()
    {
        //unused?
        if (parent.PlayerInRange)
        {
            parent.weapon.Fire();
        }

        if (parent.CanSeePlayer)
        {
            parent.weapon.TurnTowardsTarget(GameManager.instance.playerMovement.gameObject);
        }
    }

    IEnumerator ScanForPlayer()
    {
        yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
        while (gameObject)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, GameManager.instance.playerMovement.transform.position - transform.position, range, lineOfSightMask);
            if (hit.collider.gameObject == GameManager.instance.playerMovement.gameObject)
            {
                Debug.Log("player in range and in view" + hit.distance);
                parent.CanSeePlayer = true;
            }
            else
                parent.CanSeePlayer = false;

            if (hit.distance < parent.weapon.range && hit.collider.gameObject == GameManager.instance.playerMovement.gameObject)
                parent.PlayerInRange = true;

            else
                parent.PlayerInRange = false;

            yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
        }
        yield return null;
    }
}
