using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D myCollider;
    private float damageToDeal;
    private bool shot;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!shot){return;}
        if(collision.collider.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Troll>().TakeDamage(damageToDeal);
        }
        if(collision.collider.CompareTag("Bunny"))
        {
            collision.gameObject.GetComponent<Bunny>().TakeDamage(damageToDeal);
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

    void Update()
    {
        if(shot) {return;}
        transform.position = PlayerCombat.Instance.GetFirepoint().position;
        // if(PlayerCombat.Instance.transform.localScale.x < 0)
        // {
        //     transform.localScale *= -1;
        // }else
        // {
        //     transform.localScale *= 1;
        // }
    }

    public void SetDamage(float newValue){damageToDeal = newValue;}

    public void Shoot(bool newValue)
    {
        shot = newValue;
        if(myCollider == null)
        {
            myCollider = GetComponent<BoxCollider2D>();
        }
        myCollider.enabled = newValue;
    }
}
