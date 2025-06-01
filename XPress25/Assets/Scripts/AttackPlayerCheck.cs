using UnityEngine;

public class AttackPlayerCheck : MonoBehaviour
{
    public PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.Find("player").GetComponent<PlayerController>();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!playerController.objectsInTrigger.Contains(other.gameObject) && other.tag == "Enemy")
        {
            playerController.AddEnemy(other.gameObject);
            Debug.Log("Entered: " + other.name);
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (!playerController.objectsInTrigger.Contains(other.gameObject) && other.tag == "Enemy")
        {
            playerController.AddEnemy(other.gameObject);
            Debug.Log("Staying: " + other.name);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (playerController.objectsInTrigger.Contains(other.gameObject))
        {
            playerController.RemoveEnemy(other.gameObject);
            Debug.Log("Exited: " + other.name);
        }
    }

}
