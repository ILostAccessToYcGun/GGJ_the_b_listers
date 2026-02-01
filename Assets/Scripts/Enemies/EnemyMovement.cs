using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum MoveState { Hit, Run, Search, Flee, LastResort}
    public MoveState currentState;

    protected Enemy parent;
    public Rigidbody2D rb;
    protected PlayerMovement player;

    [Header("Movement Stats")]
    [SerializeField] protected float engineForce = 3500f;
    [SerializeField] protected float maxSpeed = 24;
    public float rotateSpeed = 90;
    public bool engine = true;

    [Header("Noise")]
    [SerializeField] Vector2 noiseFrequency;
    [SerializeField] Vector2 noiseStrength;
    [SerializeField] protected float noise = 1;

    [Header("Sea Level Avoidance")]
    [SerializeField] protected float seaLevelDist = 30; //if you wanna change the hovering height for the enemies, change the dist here
    [SerializeField] float seaLevelStrength = 1000;

    [Header("Enemy Separation")]
    [SerializeField] protected float separationDist = 10;

    [Header("Run")]
    [SerializeField] protected float runTimer = 0.0f;
    [SerializeField] protected float runTime = 2.0f;
    [SerializeField] protected float fleeTime = 3.0f;
    [SerializeField] protected float runDistance = 50f;


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

    public virtual void Runtime()
    {
        if (engine)
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
    public bool TurnTowardsTarget(Vector3 target, float turnStrength, float threshold = 1)
    {
        Vector3 posVec = (target - transform.position).normalized;
        float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

        if (Mathf.Abs(rotAngle) < threshold) return true;

        if (rotAngle < 0)
            transform.Rotate(Vector3.forward, -turnStrength * Time.deltaTime);

        else if (rotAngle > 0)
            transform.Rotate(Vector3.forward, turnStrength * Time.deltaTime);

        return false;
    }

    public bool TurnAwayFromTarget(Vector3 target, float turnStrength)
    {
        Vector3 posVec = (target - transform.position).normalized;
        float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

        if (rotAngle < 0)
            transform.Rotate(Vector3.forward, turnStrength * Time.deltaTime);

        else if (rotAngle > 0)
            transform.Rotate(Vector3.forward, -turnStrength * Time.deltaTime);

        if (Mathf.Abs(rotAngle) < 179) return true;
        else return false;
    }

    protected IEnumerator VeerAway()
    {
        for (int i = 0; i < 20; i++)
        {
            TurnAwayFromTarget(player.transform.position, rotateSpeed);
            yield return null;
        }
    }

    public IEnumerator EngineRestart()
    {
        Debug.Log("Restarting Engines");
        engine = false;
        yield return new WaitForSeconds(1);
        engine = true;
    }
}
