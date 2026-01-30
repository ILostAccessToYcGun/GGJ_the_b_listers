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

    public void Init()
    {

    }

    public void Runtime()
    {

    }
}
