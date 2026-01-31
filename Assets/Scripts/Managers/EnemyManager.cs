using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public GameObject seaLevel;

    [Header("Enemy Info")]
    public List<Rigidbody2D> enemies;
    private void Awake()
    {
        instance = this;
    }

    public void ClearEnemyList()
    {
        enemies.Clear();
    }

    public void AddEnemy(Rigidbody2D newRb)
    {
        enemies.Add(newRb);
    }

    public void RemoveEnemy(Rigidbody2D newRb)
    {
        enemies.Remove(newRb);
    }


}
