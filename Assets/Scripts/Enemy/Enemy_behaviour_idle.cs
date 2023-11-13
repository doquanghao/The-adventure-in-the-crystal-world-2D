using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_behaviour_idle : MonoBehaviour
{
    #region Public Variables
    public float attackDistance; //Minimum distance for attack
    public float timer; //Timer for cooldown between attacks
    public int hpEnemy = 100;
    public GameObject hp;
    public GameObject item;
    #endregion

    #region Private Variables
    private Transform target;
    private Animator anim;
    private float distance; //Store the distance b/w enemy and player
    private bool attackMode;
    private bool cooling; //Check if Enemy is cooling after attack
    private float intTimer;
    private int hpEnemyLeft;
    private bool isAttackRight;
    #endregion

    void Awake()
    {
        intTimer = timer; //Store the inital value of timer
        anim = GetComponent<Animator>();
        hpEnemyLeft = hpEnemy;
        hp.transform.localScale = new Vector3((float)hpEnemyLeft / hpEnemy, 1, 1);
    }

    private void Update()
    {
        EnemyLogic();
    }

    public void UpdateHpEnemy(int damage)
    {
        hpEnemyLeft -= damage;
        if (hpEnemyLeft <= 0)
        {
            var itemClone = Instantiate(item);
            itemClone.transform.position = transform.position;
            var itemMode = itemClone.GetComponent<ItemMode>();
            System.Random random = new System.Random();
            TypeItem randomTypeItem = (TypeItem)random.Next(1, Enum.GetValues(typeof(TypeItem)).Length + 1);
            itemMode.typeItem = randomTypeItem;
            itemMode.InitItem();
            Destroy(transform.parent.gameObject);
        }
        else
        {
            anim.SetTrigger("damage");
            hp.transform.localScale = new Vector3((float)hpEnemyLeft / hpEnemy, 1, 1);
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            target = trig.transform;
            Flip();
        }
    }

    void EnemyLogic()
    {
        if (target == null) return;

        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool(isAttackRight ? "AttackRight" : "AttackLeft", false);
        }
    }


    void Attack()
    {
        timer = intTimer; //Reset Timer when Player enter Attack Range
        attackMode = true; //To check if Enemy can still attack or not
        anim.SetBool(isAttackRight ? "AttackRight" : "AttackLeft", true);
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
        anim.SetBool(isAttackRight ? "AttackRight" : "AttackLeft", false);
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    void Flip()
    {
        if (transform.position.x > target.position.x)
        {
            isAttackRight = true;
        }
        else
        {
            isAttackRight = false;
        }
    }
}
