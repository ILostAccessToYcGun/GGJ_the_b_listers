using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    public enum MoveState { Hit, Run, Search, Flee, LastResort}
    public MoveState currentState;

    Enemy parent;
    public Rigidbody2D rb;
    PlayerMovement player;

    [Header("Movement Stats")]
    [SerializeField] float engineForce = 3500f;
    [SerializeField] float maxSpeed = 24;
    [SerializeField] float rotateSpeed = 90;

    [Header("Noise")]
    [SerializeField] Vector2 noiseFrequency;
    [SerializeField] Vector2 noiseStrength;
    [SerializeField] private float noise = 1;

    [Header("Sea Level Avoidance")]
    [SerializeField] float seaLevelDist = 30; //if you wanna change the hovering height for the enemies, change the dist here
    [SerializeField] float seaLevelStrength = 1000;

    [Header("Enemy Separation")]
    [SerializeField] float separationDist = 10;
    [SerializeField] float separationStrength = 2000;

    [Header("Enemy Cohesion")]
    [SerializeField] float cohesionDist = 40;
    [SerializeField] float cohesionStrength = 500;

    [Header("Player Cohesion")]
    [SerializeField] float playerStrength = 25;

    [Header("Run")]
    [SerializeField] float runTimer = 0.0f;
    [SerializeField] float runTime = 2.0f;
    [SerializeField] float fleeTime = 3.0f;
    [SerializeField] float runDistance = 50f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Enemy _parent)
    {
        parent = _parent;
        EnemyManager.instance.AddEnemy(rb);
        StartCoroutine(RandomizeNoise());
        player = GameManager.instance.playerMovement;
        currentState = MoveState.Search;
    }

    public void Destroy()
    {
        EnemyManager.instance.RemoveEnemy(rb);
    }

    public void Runtime()
    {
        //okay we want the mech to move more like a plane, therefore we are going to have 3 modes:
        // - hit
        // - run
        // - search

        //Sea Level
        if (transform.position.y <= EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist)
        {
            //float dist = ((EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist) - transform.position.y);
            //rb.AddForce(dist * noise * seaLevelStrength * Time.deltaTime * transform.up);
            TurnAwayFromTarget(EnemyManager.instance.seaLevel.gameObject, rotateSpeed * 2);
        }

        List<Rigidbody2D> nearby = new List<Rigidbody2D>();

        foreach (Rigidbody2D enemy in EnemyManager.instance.enemies)
        {
            //Separation
            if (Vector3.Distance(transform.position, enemy.transform.position) <= separationDist)
            {
                //rb.AddForce((transform.position - enemy.transform.position).normalized * separationStrength * noise * Time.deltaTime);
                TurnAwayFromTarget(enemy.gameObject, rotateSpeed);
            }
        }

        switch(currentState)
        {
            case MoveState.Hit:
                if (parent.CanSeePlayer)
                {
                    TurnTowardsTarget(player.gameObject, rotateSpeed);

                    Vector3 posVec = (player.transform.position - transform.position).normalized;
                    float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

                    if ((Mathf.Abs(rotAngle) > 45 || Vector3.Distance(transform.position, player.transform.position) <= separationDist) &&
                        Vector3.Distance(transform.position, player.transform.position) < runDistance)
                    {
                        currentState = MoveState.Run;
                        runTimer = 0.0f;
                        StartCoroutine(VeerAway());
                    }
                }
                break;
            case MoveState.Run:
                {
                    runTimer += Time.deltaTime;
                    if (Vector3.Distance(transform.position, player.transform.position) > runDistance)
                    {
                        //stop running and attack again
                        currentState = MoveState.Search;
                    }
                    if (runTimer > runTime)
                    {
                        currentState = MoveState.Flee;
                        runTimer = 0.0f;
                    }
                }
                break;
            case MoveState.Search:
                if (parent.CanSeePlayer) currentState = MoveState.Hit;
                break;
            case MoveState.Flee:
                runTimer += Time.deltaTime;
                TurnAwayFromTarget(player.gameObject, rotateSpeed);
                if (Vector3.Distance(transform.position, player.transform.position) > runDistance)
                {
                    //stop running and attack again
                    currentState = MoveState.Search;
                }
                if (runTimer > fleeTime)
                {
                    currentState = MoveState.LastResort;
                }
                break;

            case MoveState.LastResort:
                {
                    TurnTowardsTarget(player.gameObject, rotateSpeed * 3);
                    if (Vector3.Distance(transform.position, player.transform.position) > runDistance)
                    {
                        //stop running and attack again
                        currentState = MoveState.Search;
                    }
                }
                break;
        }


        
        if (currentState != MoveState.LastResort)
            rb.AddForce(transform.right * noise * engineForce * Time.deltaTime);

        //Max Speed Clamping
        if (rb.linearVelocity.magnitude > maxSpeed) rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    IEnumerator RandomizeNoise()
    {
        while (gameObject)
        {
            noise = Random.Range(noiseStrength.x, noiseStrength.y);
            yield return new WaitForSeconds(Random.Range(noiseFrequency.x, noiseFrequency.y));
        }
        yield return null;
    }

    //returns true once we're mostly aiming towards the player
    public bool TurnTowardsTarget(GameObject target, float turnStrength)
    {
        Vector3 posVec = (target.transform.position - transform.position).normalized;
        float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

        if (rotAngle < 0)
            transform.Rotate(Vector3.forward, -turnStrength * Time.deltaTime);

        else if (rotAngle > 0)
            transform.Rotate(Vector3.forward, turnStrength * Time.deltaTime);

        if (Mathf.Abs(rotAngle) < 1) return true;
        else return false;
    }

    public bool TurnAwayFromTarget(GameObject target, float turnStrength)
    {
        Vector3 posVec = (target.transform.position - transform.position).normalized;
        float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

        if (rotAngle < 0)
            transform.Rotate(Vector3.forward, turnStrength * Time.deltaTime);

        else if (rotAngle > 0)
            transform.Rotate(Vector3.forward, -turnStrength * Time.deltaTime);

        if (Mathf.Abs(rotAngle) < 179) return true;
        else return false;
    }

    IEnumerator VeerAway()
    {
        for (int i = 0; i < 20; i++)
        {
            TurnAwayFromTarget(player.gameObject, rotateSpeed);
            yield return null;
        }
    }
}
