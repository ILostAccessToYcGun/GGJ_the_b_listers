using UnityEngine;

public class RailGun : WeaponBase
{
    [Header("Rail Gun Stats")]
    [SerializeField] float chargeTime; //the angle variation in degrees
    [SerializeField] float recoil; //the angle variation in degrees


    //basically we are going to shoot one bullet and go on cooldown for a duration
    //if we're on cooldown or reloading return immediatly,
    //if we're out of ammo, reload
    public override void Fire()
    {
        if (FireCheck() == 1) return;

        //wind up here

        currentAmmo--;

        RailBeam r = Instantiate(projectile, shootingPoint.transform.position, shootingPoint.transform.rotation).GetComponent<RailBeam>();
        r.damage = damage;
        r.lifeTime = lifeTime;

        StartCoroutine(Cooldown());
    }
}
