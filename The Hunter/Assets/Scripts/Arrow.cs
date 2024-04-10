using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float damageToDeal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("AI"))
        {
            collision.collider.GetComponent<StaticAI>().TakeDamage(damageToDeal);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("AI"))
        {
            collider.GetComponent<StaticAI>().TakeDamage(damageToDeal);
        }
        Destroy(gameObject);
    }

    public void SetDamage(float newValue){damageToDeal = newValue;}
}
