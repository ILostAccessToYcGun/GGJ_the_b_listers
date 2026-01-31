using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponry : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPoint;

    [Header("Weapon Settings")]
    [Header("Gun")]
    [SerializeField] private float timeBetweenShots;
    [Header("Lance")]
    [SerializeField] private List<float> chargeTimings;

    private float _lanceHoldTime;

    private Coroutine _currentFireCoroutine;

    public void Init()
    {
        
    }

    public void Runtime(CharacterInput characterInput)
    {
        if (characterInput.Shoot && _currentFireCoroutine == null)
        {
            _currentFireCoroutine = StartCoroutine(ContinuousFire());
        }
        else if (!characterInput.Shoot && _currentFireCoroutine != null)
        {
            StopCoroutine(_currentFireCoroutine);
            _currentFireCoroutine = null;
        }
    }

    IEnumerator ContinuousFire()
    {
        while (true)
        {
            FireBullet();
            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void FireBullet()
    {
        Quaternion offset = Quaternion.Euler(0, 0, 90f);
        Instantiate(bulletPrefab, shootingPoint.position, transform.rotation * offset);
    }
}