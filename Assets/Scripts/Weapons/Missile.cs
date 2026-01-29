using System.Collections;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float damage;
    public float speed;
    public float lifeTime;
    public float homingStrength;
    public float homingDuration;
    public GameObject target;

    bool homing = false;


    private void Start()
    {
        StartCoroutine(LifetimeDestroy());
        StartCoroutine(DisableHoming());
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (homing)
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                homingStrength * Time.deltaTime
            );
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
