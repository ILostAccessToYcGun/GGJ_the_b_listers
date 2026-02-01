using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponBase : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] float fireRate; //delay between shots;
    [SerializeField] float reloadTime; //time to reload
    public float range;
    [SerializeField] int magazineSize;
    [SerializeField] public int currentAmmo;
    public bool reloading;
    bool cooldown;

    [Header("Projectile Stats")]
    [SerializeField] protected float damage;
    [SerializeField] protected float speed;
    [SerializeField] protected float lifeTime;

    [Header("Components")]
    [SerializeField] protected GameObject projectile;
    [SerializeField] protected GameObject shootingPoint;


    private void Awake()
    {
        currentAmmo = magazineSize;
    }

    //basically we are going to shoot one bullet and go on cooldown for a duration
    //if we're on cooldown or reloading return immediatly,
    //if we're out of ammo, reload
    public virtual void Fire()
    {

    }

    protected int FireCheck()
    {
        if (reloading || cooldown) return 1;
        if (currentAmmo == 0)
        {
            StartCoroutine(Reload());
            return 1;
        }
        return 0;
    }

    public IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineSize;
        reloading = false;
    }

    protected IEnumerator Cooldown()
    {
        cooldown = true;
        yield return new WaitForSeconds(fireRate);
        cooldown = false;
    }
}
