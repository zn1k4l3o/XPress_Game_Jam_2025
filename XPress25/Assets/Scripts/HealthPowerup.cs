using UnityEngine;

public class HealthPowerup : MonoBehaviour
{
    public float addHealth = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().AddHealth(addHealth);
            Destroy(gameObject);
        }
    }
}
