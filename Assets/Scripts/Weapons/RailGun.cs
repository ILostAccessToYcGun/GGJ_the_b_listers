using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RailGun : WeaponBase
{
    [Header("Rail Gun Stats")]
    [SerializeField] float chargeTime; //the angle variation in degrees
    [SerializeField] float recoil; //the angle variation in degrees

    bool isShooting = false;

    [Header("Rail Gun Components")]
    [SerializeField] GameObject pointer;
    [SerializeField] Rigidbody2D rb;

    //basically we are going to shoot one bullet and go on cooldown for a duration
    //if we're on cooldown or reloading return immediatly,
    //if we're out of ammo, reload
    public override void Fire()
    {
        if (FireCheck() == 1) return;

        //wind up here
        if (!isShooting) StartCoroutine(Shoot());

        
    }

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        pointer.SetActive(true);
        yield return new WaitForSeconds(chargeTime);
        currentAmmo--;

        RailBeam r = Instantiate(projectile, shootingPoint.transform.position, shootingPoint.transform.rotation).GetComponentInChildren<RailBeam>();
        r.damage = damage;
        r.lifeTime = lifeTime;

        //recoil here
        rb.AddForce(-transform.right * recoil);

        pointer.SetActive(false);
        isShooting = false;

        StartCoroutine(Cooldown());
    }
}
