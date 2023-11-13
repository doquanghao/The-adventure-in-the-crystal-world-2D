using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBringerOfDeath : MonoBehaviour
{
    #region Public Variables
    public GameObject item;
    public float moveSpeed;
    public Vector2 positionOffSetSkill;
    public float timer; //Timer for cooldown between attacks
    public int hpEnemy = 100;
    public Slider hp;
    public GameObject bossUI;
    public GameObject boxAttack;
    public GameObject attackRange;
    public GameObject player;
    public ContactFilter2D enemyContactFilter;
    #endregion

    #region Private Variables
    private Transform target;
    private Animator anim;
    private bool attackMode;
    private bool cooling; //Check if Enemy is cooling after attack
    private bool canMove = true; //Check if Enemy is cooling after attack
    private float intTimer;
    private int hpEnemyLeft;
    private int countAttack;
    public GameObject spell;
    #endregion

    void Awake()
    {
        intTimer = timer; //Store the inital value of timer
        anim = GetComponent<Animator>();
        hpEnemyLeft = hpEnemy;
        countAttack = 0;
        hp.value = (float)hpEnemyLeft / hpEnemy;
    }

    void Update()
    {
        if (player != null)
        {
            if (!attackMode)
            {
                Move();
            }
            if (AttackRange())
            {
                EnemyLogic();
            }
            else
            {
                StopAttack();
            }
            Flip();
        }
    }
    public bool AttackRange()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(attackRange.GetComponent<Collider2D>(), enemyContactFilter, colliders);
        foreach (Collider2D c in colliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true;
            }
        }
        return false;
    }
    public void UpdateHpEnemy(int damage)
    {
        hpEnemyLeft -= damage;
        if (hpEnemyLeft <= 0)
        {
            var itemClone = Instantiate(item);
            itemClone.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            var itemMode = itemClone.GetComponent<ItemMode>();
            itemMode.InitItem();
            Destroy(bossUI);
            Destroy(transform.gameObject);
        }
        else
        {
            anim.SetTrigger("damage");
            hp.value = (float)hpEnemyLeft / hpEnemy;
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            player = trig.gameObject;

            target = trig.transform;
        }
    }

    void EnemyLogic()
    {
        if (!AttackRange())
        {
            StopAttack();
        }
        else if (AttackRange() && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
            anim.SetBool("AttackCast", false);
        }
    }

    void Move()
    {
        if (!canMove) return;
        anim.SetBool("canWalk", canMove);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Transform transformPlayer = player.transform;
            Vector2 targetPosition = new Vector2(transformPlayer.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 2)
            {

                canMove = false;
                anim.SetBool("canWalk", canMove);
                StartCoroutine(OnRun());
            }
        }
    }
    IEnumerator OnRun()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 3));
        canMove = true;
    }

    void Attack()
    {
        timer = intTimer; //Reset Timer when Player enter Attack Range
        attackMode = true; //To check if Enemy can still attack or not
        if (countAttack > UnityEngine.Random.Range(1, 3))
        {
            anim.SetBool("canWalk", false);
            anim.SetBool("AttackCast", true);
        }
        else
        {
            anim.SetBool("canWalk", false);
            anim.SetBool("Attack", true);
        }

    }
    public void InstantiateSpell()
    {
        var shootingClone = Instantiate(spell);
        var position = new Vector3(player.transform.position.x + positionOffSetSkill.x, player.transform.position.y + positionOffSetSkill.y, player.transform.position.z);
        shootingClone.transform.position = position;
        countAttack = 0;
        timer = 3;
    }
    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
        anim.SetBool("AttackCast", false);
    }

    public void TriggerCooling()
    {
        countAttack++;
        cooling = true;
    }
    void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 180;
        }
        else
        {
            rotation.y = 0;
        }
        transform.eulerAngles = rotation;
    }
    public void DamagePlayer()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(boxAttack.GetComponent<Collider2D>(), enemyContactFilter, colliders);
        foreach (Collider2D c in colliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                c.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            }
        }
    }

}
