using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class EnemyVision : MonoBehaviour
{
    Enemy parent;
    PlayerMovement player;
    [SerializeField] float range = 200;
    [SerializeField] float viewConeAngle = 45;
    [SerializeField] Vector2 reactionTime;
    [SerializeField] private LayerMask lineOfSightMask;
    public void Init(Enemy _parent)
    {
        parent = _parent;//this will be used to update some boolean/stats for decision making
        player = GameManager.instance.playerMovement;
        StartCoroutine(ScanForPlayer());
    }

    // Update is called once per frame
    public void Runtime()
    {
        //unused?
        if (parent.PlayerInRange && parent.PlayerInView)
        {
            parent.weapon.Fire();
        }
    }

    IEnumerator ScanForPlayer()
    {
        yield return new WaitForSeconds(Random.Range(reactionTime.x, reactionTime.y));
        while (gameObject)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position, range, lineOfSightMask);
            if (hit.collider != null)
            {
                //canSeePlayer
                if (hit.collider.gameObject == player.gameObject)
                {
                    Debug.Log("player in range and in view" + hit.distance);
                    parent.CanSeePlayer = true;
                }
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
}
