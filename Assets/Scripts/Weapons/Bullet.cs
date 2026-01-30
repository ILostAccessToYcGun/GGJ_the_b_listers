using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject owner;
    public float damage;
    public float speed;
    public float lifeTime;
    public float angle;

    private void Start()
    {
        transform.Rotate(Vector3.forward, angle);
        StartCoroutine(LifetimeDestroy());
    }

    private void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);
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
