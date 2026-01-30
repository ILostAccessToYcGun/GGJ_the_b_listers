using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Mask { Neutral, Yellow, Red, Green };
    public static GameManager instance;
    public Player player;
    public PlayerMovement playerMovement;

    private void Awake()
    {
        instance = this;
        player = FindFirstObjectByType<Player>();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }
}
