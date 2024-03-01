using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D myCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            myCollider.enabled = false;
            Destroy(gameObject,2f);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
    }
}
