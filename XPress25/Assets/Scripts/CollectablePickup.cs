using UnityEngine;

public class CollectablePickup : MonoBehaviour
{

    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            levelManager.AddItem();
            Destroy(gameObject);
        }
    }
}
