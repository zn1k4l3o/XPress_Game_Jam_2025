using UnityEngine;

public class StaminaPowerup : MonoBehaviour
{
    public float addStamina = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().AddStamina(addStamina);
            Destroy(gameObject);
        }
    }
}
