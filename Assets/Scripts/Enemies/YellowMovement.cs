using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class YellowMovement : EnemyMovement
{
    public override void Runtime()
    {
        //okay we want the mech to move more like a plane, therefore we are going to have some modes:
        // - hit
        // - run
        // - search
        // - flee
        // - lastResort

        //Sea Level
        if (transform.position.y <= EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist)
        {
            //float dist = ((EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist) - transform.position.y);
            //rb.AddForce(dist * noise * seaLevelStrength * Time.deltaTime * transform.up);
            TurnAwayFromTarget(EnemyManager.instance.seaLevel.transform.position, rotateSpeed * 2);
            rb.AddForce(transform.right * noise * engineForce * Time.deltaTime);
        }

        List<Rigidbody2D> nearby = new List<Rigidbody2D>();

        foreach (Rigidbody2D enemy in EnemyManager.instance.enemies)
        {
            //Separation
            if (Vector3.Distance(transform.position, enemy.transform.position) <= separationDist)
            {
                //rb.AddForce((transform.position - enemy.transform.position).normalized * separationStrength * noise * Time.deltaTime);
                TurnAwayFromTarget(enemy.transform.position, rotateSpeed);
            }
        }

        switch (currentState)
        {
            case MoveState.Hit:
                if (parent.CanSeePlayer)
                {
                    TurnTowardsTarget(player.transform.position, rotateSpeed);

                    Vector3 posVec = (player.transform.position - transform.position).normalized;
                    float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

                    if ((Mathf.Abs(rotAngle) > 45 || Vector3.Distance(transform.position, player.transform.position) <= separationDist) &&
                        Vector3.Distance(transform.position, player.transform.position) < runDistance)
                    {
                        currentState = MoveState.Run;
                        runTimer = 0.0f;
                        StartCoroutine(VeerAway());
                        StartCoroutine(parent.weapon.Reload());
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
                TurnAwayFromTarget(player.transform.position, rotateSpeed);
                if (Vector3.Distance(transform.position, player.transform.position) > runDistance)
                {
                    //stop running and attack again
                    currentState = MoveState.Search;
                }
                if (runTimer > fleeTime)
                {
                    currentState = MoveState.LastResort;
                    engine = false;
                }
                break;

            case MoveState.LastResort:
                {
                    TurnTowardsTarget(player.transform.position, rotateSpeed * 3);
                    if (Vector3.Distance(transform.position, player.transform.position) > runDistance)
                    {
                        //stop running and attack again
                        currentState = MoveState.Search;
                        engine = true;
                    }
                }
                break;
        }



        if (engine)
            rb.AddForce(transform.right * noise * engineForce * Time.deltaTime);

        //Max Speed Clamping
        if (rb.linearVelocity.magnitude > maxSpeed) rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }
}
