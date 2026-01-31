using System.Collections;
using UnityEngine;

public class MachineGun : WeaponBase
{
    [Header("Machine Gun Stats")]
    [Range(0f, 15f)][SerializeField] float accuracy; //the angle variation in degrees


    //basically we are going to shoot one bullet and go on cooldown for a duration
    //if we're on cooldown or reloading return immediatly,
    //if we're out of ammo, reload
    public override void Fire()
    {
        if (FireCheck() == 1) return;

        currentAmmo--;
        
        Bullet b = Instantiate(projectile, shootingPoint.transform.position, shootingPoint.transform.rotation).GetComponent<Bullet>();
        b.damage = damage;
        b.speed = speed;
        b.lifeTime = lifeTime;
        b.angle = Random.Range(-accuracy, accuracy);
        b.owner = gameObject;

        StartCoroutine(Cooldown());
    }
}
