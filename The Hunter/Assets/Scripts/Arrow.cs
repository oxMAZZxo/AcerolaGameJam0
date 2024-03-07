using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Arrow : MonoBehaviour
{
    public bool isGlowing;
    [SerializeField]private Light2D light2D;
    private Rigidbody2D rb;
    private BoxCollider2D myCollider;
    private float damageToDeal;
    private bool shot;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!shot){return;}
        if(collision.collider.CompareTag("AI"))
        {
            collision.collider.GetComponent<StaticAI>().TakeDamage(damageToDeal);
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

    public void SetLightIntensity(float newValue)
    {
        if(!isGlowing) {return;}
        light2D.intensity = newValue;
    }
}
