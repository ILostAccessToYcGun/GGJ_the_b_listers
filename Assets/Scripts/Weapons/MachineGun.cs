using UnityEngine;

public class MachineGun : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] float fireRate; //delay between shots;
    [Range(0f, 1f)][SerializeField] float accuracy; //delay between shots;
    [SerializeField] int magazineSize;
    int currentAmmo;

    [Header("Bullet Stats")]
    [SerializeField] float damage; 
    [SerializeField] float speed; 
    [SerializeField] float lifeTime; 

    [Header("Components")]
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject aimTarget;


    private void Awake()
    {
        currentAmmo = magazineSize;
    }
}
