using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifeTime;
    public float homingStrength;
    public float homingDuration;
    public float angle;

    float noise = 1.0f;

    public GameObject target;

    bool homing = false;


    private void Start()
    {
        transform.Rotate(Vector3.forward, angle);
        StartCoroutine(LifetimeDestroy());
        StartCoroutine(DisableHoming());
        StartCoroutine(Noise());
    }

    private void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime);
        if (homing)
        {
            //float rotAngle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Vector3.forward, (target.transform.position - transform.position).normalized));
            float rotAngle = Vector3.SignedAngle(transform.right, (target.transform.position - transform.position).normalized, Vector3.forward);

            Debug.Log(rotAngle);
            if (rotAngle < 0)
            {
                transform.Rotate(Vector3.forward, homingStrength * noise * Time.deltaTime);
            }
            else if (rotAngle > 0)
            {
                transform.Rotate(Vector3.forward, -homingStrength * noise * Time.deltaTime);
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
            noise = Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(Random.Range(0.3f, 2f));
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
