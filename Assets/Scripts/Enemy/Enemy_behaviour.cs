using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeEnemy
{
    Enemy,
    EnemyFly
}
public class Enemy_behaviour : MonoBehaviour
{
    #region Public Variables
    public GameObject item;
    public Transform rayCast;
    public TypeEnemy typeEnemy;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance; //Minimum distance for attack
    public float moveSpeed;
    public float timer; //Timer for cooldown between attacks
    public int hpEnemy = 100;
    public GameObject hp;
    public GameObject boxAttack;
    public Transform leftLimit;
    public Transform rightLimit;
    public ContactFilter2D enemyContactFilter;
    #endregion

    #region Private Variables
    private RaycastHit2D hit;
    private Transform target;
    private Animator anim;
    private float distance; //Store the distance b/w enemy and player
    private bool attackMode;
    private bool cooling; //Check if Enemy is cooling after attack
    private bool canMove = true; //Check if Enemy is cooling after attack
    private float intTimer;
    private int hpEnemyLeft;
    #endregion

    void Awake()
    {
        SelectTarget();
        intTimer = timer; //Store the inital value of timer
        anim = GetComponent<Animator>();
        hpEnemyLeft = hpEnemy;
        hp.transform.localScale = new Vector3((float)hpEnemyLeft / 100, 1, 1);
    }

    void Update()
    {
        if (!attackMode)
        {
            Move();
        }

        if (!InsideOfLimits() && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            SelectTarget();
        }

        hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
        RaycastDebugger();

        //When Player is detected
        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            StopAttack();
        }

    }

    public void UpdateHpEnemy(int damage, GameObject player)
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

            Vector2 vector = transform.position - player.transform.position;
            StartCoroutine(DelayHurt(vector));
            anim.SetTrigger("damage");
            hp.transform.localScale = new Vector3((float)hpEnemyLeft / 100, 1, 1);
        }
    }
    IEnumerator DelayHurt(Vector2 vector)
    {
        if (vector.x > 0)
        {
            transform.position = new Vector2(transform.position.x + 3, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x + -3, transform.position.y);
        }
        yield return new WaitForSeconds(0.3f);
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
            anim.SetBool("Attack", false);
        }
    }

    void Move()
    {
        if (!canMove) return;
        anim.SetBool("canWalk", canMove);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector2 targetPosition = new Vector2(0, 0);
            switch (typeEnemy)
            {
                case TypeEnemy.Enemy:
                    targetPosition = new Vector2(target.position.x, transform.position.y);
                    break;
                case TypeEnemy.EnemyFly:
                    targetPosition = target.position;
                    break;
                default:
                    targetPosition = new Vector2(target.position.x, transform.position.y);
                    break;
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
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

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
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
    }

    void RaycastDebugger()
    {
        if (distance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    private void SelectTarget()
    {
        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }
        Flip();
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
