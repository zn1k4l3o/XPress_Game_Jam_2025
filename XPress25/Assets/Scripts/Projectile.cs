using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 targetPos;
    public float timer = 3f;
    public float damage = 10f;

    void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
        timer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
