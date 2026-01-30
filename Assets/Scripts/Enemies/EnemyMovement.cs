using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Sea Level Avoidance")]
    [SerializeField] float seaLevelDist;
    [SerializeField] float seaLevelStrength;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        //i think there should be kinda 2 modes, moving and hovering, but for now lets focus on hovering

        //okay so the plan for enemy movement is that we are going to use steering behaviours to manipulate a single velocity vector
        //because we are using rigidbodies, we'll use the linearVelocity component
        
        //lets think about the logistics behind how they want to move first, lets setup the system for them to:
        // - stay above sea level
        // - dont touch eachother -> separation
        // - group up -> cohesian

        //

        if (transform.position.y <= EnemyManager.instance.seaLevel.transform.position.y + seaLevelDist)
        {
            float dist = transform.position.y - EnemyManager.instance.seaLevel.transform.position.y;
            //rb.AddForce(transform.up * seaLevelStrength * Time.deltaTime * )
        }




    }
}
