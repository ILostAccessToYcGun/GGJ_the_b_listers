using UnityEngine;

public class GameManager : MonoBehaviour
{
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
