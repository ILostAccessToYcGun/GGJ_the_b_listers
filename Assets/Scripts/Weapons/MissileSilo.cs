using UnityEngine;

public class MissileSilo : WeaponBase
{
    [Header("MissileSilo Stats")]
    [SerializeField] float homingStrength;
    [SerializeField] float homingDuration;
    [Range(0f, 15f)][SerializeField] float accuracy; //the angle variation in degrees
    [SerializeField] float angleChangeThreshhold;
    [SerializeField] Vector2 angleChangeFrequency;
    [SerializeField] Vector2 noiseRange;
    [SerializeField] Vector2 noiseFrequency;
    [SerializeField] GameObject target;


    //basically we are going to shoot one bullet and go on cooldown for a duration
    //if we're on cooldown or reloading return immediatly,
    //if we're out of ammo, reload
    public override void Fire()
    {
        if (FireCheck() == 1) return;

        currentAmmo--;

        Missile m = Instantiate(projectile, shootingPoint.transform.position, shootingPoint.transform.rotation).GetComponent<Missile>();
        m.damage = damage;
        m.speed = speed;
        m.lifeTime = lifeTime;
        m.homingStrength = homingStrength;
        m.homingDuration = homingDuration;
        m.angle = Random.Range(-accuracy, accuracy);
        m.angleChangeThreshhold = angleChangeThreshhold;
        m.angleChangeFrequency = angleChangeFrequency;
        m.noiseRange = noiseRange;
        m.noiseFrequency = noiseFrequency;
        m.target = target;

        StartCoroutine(Cooldown());
    }
}
