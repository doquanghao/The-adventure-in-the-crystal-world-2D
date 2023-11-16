using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_behaviour_idle : MonoBehaviour
{
    public float attackDistance; // Khoảng cách tối thiểu để tấn công
    public float timer; // Thời gian cooldown giữa các lần tấn công
    public int hpEnemy = 100; // Sức khỏe ban đầu của quái vật
    public GameObject hp; // Tham chiếu đến GameObject biểu thị sức khỏe của quái vật (có thể là một phần tử UI)
    public GameObject item; // Tham chiếu đến một vật phẩm rơi từ quái vật

    public GameObject boxAttack; // Tham chiếu đến hitbox hoặc collider của tấn công
    public ContactFilter2D enemyContactFilter; // Bộ lọc để phát hiện người chơi hoặc các đối tượng khác

    private Transform target; // Tham chiếu đến người chơi hoặc mục tiêu
    private Animator anim; // Tham chiếu đến thành phần Animator để điều khiển animation
    private float distance; // Lưu trữ khoảng cách giữa quái vật và người chơi
    private bool attackMode; // Chỉ định nếu quái vật đang ở chế độ tấn công
    private bool cooling; // Kiểm tra xem quái vật có đang cooldown sau khi tấn công hay không
    private float intTimer; // Giá trị ban đầu cho hẹn giờ cooldown
    private int hpEnemyLeft; // Sức khỏe hiện tại của quái vật
    private bool isAttackRight; // Chỉ định hướng của cuộc tấn công


    void Awake()
    {
        intTimer = timer; // Lưu trữ giá trị ban đầu của timer
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
            // Tạo một đối tượng mới từ item
            var itemClone = Instantiate(item);
            itemClone.transform.position = transform.position;
            // Hủy đối tượng cha của Enemy
            Destroy(transform.parent.gameObject);
        }
        else
        {
            // Kích hoạt trigger "damage" trên Animator để chơi hiệu ứng
            anim.SetTrigger("damage");

            // Cập nhật kích thước thanh máu
            hp.transform.localScale = new Vector3((float)hpEnemyLeft / hpEnemy, 1, 1);
        }
    }


    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            // Gán đối tượng Player vào biến target và thực hiện flip
            target = trig.transform;
            Flip();
        }
    }

    void EnemyLogic()
    {
        if (target == null) return;

        // Tính khoảng cách giữa Enemy và Player
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            // Nếu khoảng cách lớn hơn attackDistance, dừng tấn công
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            // Nếu khoảng cách nhỏ hơn hoặc bằng attackDistance và không trong trạng thái cooling, tấn công
            Attack();
        }

        if (cooling)
        {
            // Nếu đang cooling, thực hiện cooldown và đặt trạng thái tấn công trong Animator về false
            Cooldown();
            anim.SetBool(isAttackRight ? "AttackRight" : "AttackLeft", false);
        }
    }


    // Hàm xử lý tấn công của Enemy
    void Attack()
    {
        timer = intTimer; // Đặt lại Timer khi Player vào phạm vi tấn công
        attackMode = true; // Để kiểm tra xem Enemy có thể tấn công tiếp hay không
        anim.SetBool(isAttackRight ? "AttackRight" : "AttackLeft", true); // Đặt trạng thái tấn công trong Animator
    }

    // Hàm cooldown giữa các lần tấn công
    void Cooldown()
    {
        timer -= Time.deltaTime;

        // Kiểm tra nếu cooldown đã hết và đang trong trạng thái cooling và attackMode
        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false; // Tắt cooldown
            timer = intTimer; // Đặt lại Timer
        }
    }

    // Hàm dừng tấn công
    void StopAttack()
    {
        cooling = false; // Tắt cooldown
        attackMode = false; // Tắt trạng thái tấn công
        anim.SetBool(isAttackRight ? "AttackRight" : "AttackLeft", false); // Đặt trạng thái tấn công trong Animator về false
    }

    // Kích hoạt cooldown
    public void TriggerCooling()
    {
        cooling = true;
    }

    // Hàm đảo ngược hướng của Enemy tùy thuộc vào vị trí của Player
    void Flip()
    {
        // So sánh vị trí của Enemy và Player để xác định hướng tấn công
        if (transform.position.x > target.position.x)
        {
            isAttackRight = true;
        }
        else
        {
            isAttackRight = false;
        }
    }

    // Gây sát thương cho người chơi khi va chạm
    public void DamagePlayer()
    {
        List<Collider2D> colliders = new List<Collider2D>();

        // Kiểm tra va chạm giữa boxAttack và các collider khác trong enemyContactFilter
        Physics2D.OverlapCollider(boxAttack.GetComponent<Collider2D>(), enemyContactFilter, colliders);

        // Duyệt qua các collider va chạm
        foreach (Collider2D c in colliders)
        {
            // Kiểm tra nếu collider thuộc layer "Player"
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Gọi phương thức TakeDamage trên PlayerHealth của đối tượng người chơi và truyền giá trị 1
                c.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            }
        }
    }
}
