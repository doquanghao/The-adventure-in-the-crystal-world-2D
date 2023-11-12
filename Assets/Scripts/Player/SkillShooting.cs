using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShooting : MonoBehaviour
{
    public float speed;
    public float timeExistsShooting;

    public int damage;


    private float direction;

    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > timeExistsShooting) Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            hit = true;
            anim.SetTrigger("explode");
            if (collision.gameObject.GetComponent<Enemy_behaviour>() != null)
            {
                transform.position = collision.gameObject.transform.position;
                collision.gameObject.GetComponent<Enemy_behaviour>().UpdateHpEnemy(damage, gameObject);
            }
            else if (collision.gameObject.GetComponent<Enemy_behaviour_idle>() != null)
            {
                transform.position = collision.gameObject.transform.position;
                collision.gameObject.GetComponent<Enemy_behaviour_idle>().UpdateHpEnemy(damage);
            }
        }
    }
    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;

        if (_direction > 0)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
    private void Deactivate()
    {
        Destroy(gameObject);
    }
}
