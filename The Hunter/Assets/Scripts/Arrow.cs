using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D myCollider;
    private float damageToDeal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Troll>().TakeDamage(damageToDeal);
        }
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        myCollider.enabled = false;
        Destroy(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }

    public void SetDamage(float newValue){damageToDeal = newValue;}
}
