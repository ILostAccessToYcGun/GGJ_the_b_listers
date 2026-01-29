using System.Collections;
using UnityEngine;

public class RailBeam : MonoBehaviour
{
    public float damage;
    public float lifeTime;


    private void Start()
    {
        StartCoroutine(LifetimeDestroy());
    }

    IEnumerator LifetimeDestroy()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
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
