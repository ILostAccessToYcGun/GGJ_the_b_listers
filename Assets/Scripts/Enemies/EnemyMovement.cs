using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    Enemy parent;
    public Rigidbody2D rb;
    private float maxSpeed = 12f;
    public float lineDamp;
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


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Enemy _parent)
    {
        parent = _parent;
        EnemyManager.instance.AddEnemy(rb);
        StartCoroutine(RandomizeNoise());
        lineDamp = rb.linearDamping;
    }

    public void Destroy()
    {
        EnemyManager.instance.RemoveEnemy(rb);
    }

    public void Runtime()
    {
        //i think there should be kinda 2 modes, moving and hovering, but for now lets focus on hovering

        //okay so the plan for enemy movement is that we are going to use steering behaviours to manipulate a single velocity vector
        //because we are using rigidbodies, we'll use the linearVelocity component or more specifically, using rb.addforce
        
        //lets think about the logistics behind how they want to move first, lets setup the system for them to:
        // - stay above sea level * 
        // - dont touch eachother -> separation
        // - group up -> cohesian

        //okay so now that we have some basic movement in place, lets start thinking about logic
        //i think for a basic dumb AI, if we see the player, move towards them and shoot
        //dont reposition, dont do nothing, just move towards them and shoot
        //this means we're gonna need some shared variables for vision and states

        //Sea Level
        if (transform.position.y <= EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist)
        {
            float dist = ((EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist) - transform.position.y) / seaLevelDist;
            rb.AddForce(dist * noise * seaLevelStrength * Time.deltaTime * transform.up);
        }

        List<Rigidbody2D> nearby = new List<Rigidbody2D>();

        foreach (Rigidbody2D enemy in EnemyManager.instance.enemies)
        {
            //Separation
            if (Vector3.Distance(transform.position, enemy.transform.position) <= separationDist)
            {
                rb.AddForce((transform.position - enemy.transform.position).normalized * separationStrength * noise * Time.deltaTime);
            }

            //Cohesion
            if (Vector3.Distance(transform.position, enemy.transform.position) <= cohesionDist)
            {
                nearby.Add(enemy);
            }
        }

        //more Cohesion
        Vector3 avePos = Vector3.zero;
        Vector3 totalPos = Vector3.zero;
        foreach (Rigidbody2D enemy in nearby)
        {
            totalPos += enemy.transform.position;
        }
        avePos = totalPos / nearby.Count;
        rb.AddForce((avePos - transform.position).normalized * cohesionStrength * noise * Time.deltaTime);


        //if we can see the player, get into range
        if (parent.CanSeePlayer && !parent.PlayerInRange)
        {
            rb.AddForce((GameManager.instance.playerMovement.transform.position - transform.position).normalized * playerStrength * noise * Time.deltaTime);
            //Vector3 hold = ((transform.position - GameManager.instance.playerMovement.transform.position).normalized * parent.weapon.range) - trans
            //rb.AddForce( * playerStrength * noise * Time.deltaTime);
        }

        if (parent.PlayerInRange) rb.linearDamping = lineDamp * 4;
        else rb.linearDamping = lineDamp;



        //Max Speed Clamping
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
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
}
