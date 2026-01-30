using System.Collections;
using System.Drawing;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifeTime;
    public float homingStrength;
    public float homingDuration;
    public float angle;
    public float angleChangeThreshhold;
    public Vector2 angleChangeFrequency;
    public Vector2 noiseRange;
    public Vector2 noiseFrequency;

    float noise = 1.0f;
    float noiseMult = 1.0f;

    public GameObject target;

    bool homing = false;
    bool angling = false;


    private void Start()
    {
        transform.Rotate(Vector3.forward, angle);
        StartCoroutine(LifetimeDestroy());
        StartCoroutine(DisableHoming());
        StartCoroutine(Noise());
    }

    private void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
        if (!target) return;
        if (homing)
        {
            if (Vector3.Distance(transform.position, target.transform.position) < 5) noiseMult = 0.1f;
            else noiseMult = 0.1f;

            Vector3 posVec = ((target.transform.position + ((Vector3)Random.insideUnitCircle * noise * noiseMult)) - transform.position).normalized;
            float rotAngle = Vector3.SignedAngle(transform.right, posVec, Vector3.forward);

            if (Mathf.Abs(rotAngle) <= angleChangeThreshhold) return;
            if (!angling)
            {
                angling = true;
                StartCoroutine(AngleChange(rotAngle));
            }
            
                

            //Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, (target.transform.position - transform.position).normalized);
            //
            //transform.rotation = Quaternion.RotateTowards(
            //    transform.rotation,
            //    targetRotation,
            //    homingStrength * Time.deltaTime
            //);
        }
    }
    IEnumerator AngleChange(float rotAngle)
    {
        Debug.Log("angling: " + angling);
        if (rotAngle < 0)
        {
            transform.Rotate(Vector3.forward, -homingStrength * noise * Time.deltaTime);
        }
        else if (rotAngle > 0)
        {
            transform.Rotate(Vector3.forward, homingStrength * noise * Time.deltaTime);
        }
        yield return new WaitForSeconds(Random.Range(angleChangeFrequency.x, angleChangeFrequency.y));
        angling = false;
    }

    IEnumerator LifetimeDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    IEnumerator DisableHoming()
    {
        yield return new WaitForSeconds(0.5f);
        homing = true;
        yield return new WaitForSeconds(homingDuration);
        homing = false;
    }

    IEnumerator Noise()
    {
        do
        {
            noise = Random.Range(noiseRange.x, noiseRange.y);
            yield return new WaitForSeconds(Random.Range(noiseFrequency.x, noiseFrequency.y));
        }
        while (homing);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamage dmg = collision.GetComponent<IDamage>();
        if (dmg != null)
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
