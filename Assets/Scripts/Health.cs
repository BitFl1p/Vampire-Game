using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] public int maxHealth, health;
    public GameObject me;
    protected Rigidbody rb;
    public Slider healthSlider;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
    }
    protected virtual void Update()
    {
        if (health <= 0) Destroy(me ? me : gameObject);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    public virtual void Damage(int damage, Collider dealer, float knockback)
    {
        health -= damage;
        rb.velocity = (transform.position * 10 - dealer.transform.position * 10).normalized * knockback;
    }
    public IEnumerator InvincibilityFrames(float knockTimer)
    {
        while (knockTimer > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f);
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        yield return null;
    }
}