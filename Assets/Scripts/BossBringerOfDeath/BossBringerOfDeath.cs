using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBringerOfDeath : MonoBehaviour
{
    public GameObject item; // Đối tượng item
    public float moveSpeed; // Tốc độ di chuyển
    public Vector2 positionOffSetSkill; // Vị trí offset cho skill
    public float timer; // Thời gian chờ giữa các cuộc tấn công
    public int hpEnemy = 100; // Máu của Enemy
    public Slider hp; // Thanh máu
    public GameObject bossUI; // UI của Boss
    public GameObject boxAttack; // Hộp tấn công
    public GameObject attackRange; // Phạm vi tấn công
    public GameObject player; // Đối tượng người chơi
    public GameObject victoryScreen; // Màn hình chiến thắng
    public ContactFilter2D enemyContactFilter; // Bộ lọc va chạm với Enemy
    private Transform target; // Đối tượng mục tiêu
    private Animator anim; // Animator
    private bool attackMode; // Chế độ tấn công
    private bool cooling; // Kiểm tra xem Enemy có đang làm mát sau cuộc tấn công không
    private bool canMove = true; // Kiểm tra xem Enemy có thể di chuyển không
    private float intTimer; // Thời gian khởi tạo
    private int hpEnemyLeft; // Số máu còn lại của Enemy
    private int countAttack; // Số lần tấn công
    public GameObject spell; // Đối tượng spell

    void Awake()
    {
        victoryScreen.SetActive(false); // Ẩn màn hình chiến thắng khi bắt đầu
        intTimer = timer; // Lưu giá trị ban đầu của timer
        anim = GetComponent<Animator>(); // Lấy thành phần Animator của đối tượng
        hpEnemyLeft = hpEnemy; // Gán giá trị máu còn lại bằng giá trị máu ban đầu
        countAttack = 0; // Đặt số lần tấn công về 0
        hp.value = (float)hpEnemyLeft / hpEnemy; // Thiết lập giá trị thanh máu dựa trên máu còn lại so với máu ban đầu
    }
    void Update()
    {
        if (player != null)
        {
            // Nếu không ở trong chế độ tấn công và không ở trong phạm vi tấn công
            if (!attackMode && !AttackRange())
            {
                Move(); // Di chuyển
            }
            else
            {
                anim.SetBool("canWalk", false); // Thiết lập trạng thái di chuyển trong Animator thành false
            }

            // Nếu ở trong phạm vi tấn công
            if (AttackRange())
            {
                EnemyLogic(); // Xử lý logic của Enemy
            }

            Flip(); // Lật hình ảnh nếu cần
        }
    }

    public bool AttackRange()
    {
        List<Collider2D> colliders = new List<Collider2D>();

        // Kiểm tra các va chạm trong phạm vi tấn công sử dụng bộ lọc và lưu vào danh sách colliders
        Physics2D.OverlapCollider(attackRange.GetComponent<Collider2D>(), enemyContactFilter, colliders);

        // Duyệt qua danh sách colliders
        foreach (Collider2D c in colliders)
        {
            // Nếu đối tượng kích thước Player xuất hiện trong danh sách colliders
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                return true; // Trả về true, đối tượng ở trong phạm vi tấn công
            }
        }

        return false; // Nếu không có đối tượng nào là Player trong danh sách colliders, trả về false
    }
    public void UpdateHpEnemy(int damage)
    {
        // Giảm máu của Enemy dựa trên lượng sát thương nhận được
        hpEnemyLeft -= damage;

        // Nếu máu của Enemy dưới hoặc bằng 0
        if (hpEnemyLeft <= 0)
        {
            // Tạo một bản sao của đối tượng item
            var itemClone = Instantiate(item);

            // Đặt vị trí của itemClone
            itemClone.transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);

            // Lấy thành phần ItemMode từ itemClone
            var itemMode = itemClone.GetComponent<ItemMode>();

            // Khởi tạo item
            itemMode.InitItem();

            // Hiển thị màn hình chiến thắng
            victoryScreen.SetActive(true);

            // Hủy bỏ UI của Boss
            Destroy(bossUI);

            // Hủy bỏ đối tượng Enemy
            Destroy(transform.gameObject);
        }
        else
        {
            // Triggers animation "damage" để phản ánh sự tổn thương
            anim.SetTrigger("damage");

            // Cập nhật giá trị thanh máu dựa trên máu còn lại so với máu ban đầu
            hp.value = (float)hpEnemyLeft / hpEnemy;
        }
    }


    void OnTriggerEnter2D(Collider2D trig)
    {
        // Kiểm tra nếu đối tượng va chạm có tag là "Player"
        if (trig.gameObject.CompareTag("Player"))
        {
            // Lưu đối tượng Player vào biến player
            player = trig.gameObject;

            // Lưu transform của đối tượng Player vào biến target
            target = trig.transform;
        }
    }


    void EnemyLogic()
    {
        // Nếu đối tượng ở trong phạm vi tấn công và không trong trạng thái cooling
        if (AttackRange() && cooling == false)
        {
            Attack(); // Thực hiện cuộc tấn công
        }

        // Nếu đang trong trạng thái cooling
        if (cooling)
        {
            Cooldown(); // Thực hiện quá trình làm mát
            anim.SetBool("Attack", false); // Đặt trạng thái tấn công trong Animator về false
            anim.SetBool("AttackCast", false); // Đặt trạng thái tấn công đặc biệt trong Animator về false
        }
    }


    void Move()
    {
        // Nếu không thể di chuyển, thoát khỏi hàm
        if (!canMove) return;

        // Đặt trạng thái di chuyển trong Animator thành true
        anim.SetBool("canWalk", canMove);

        // Nếu không ở trong trạng thái tấn công
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            // Lấy transform của đối tượng Player
            Transform transformPlayer = player.transform;

            // Tính toán vị trí mục tiêu dọc theo trục x
            Vector2 targetPosition = new Vector2(transformPlayer.position.x, transform.position.y);

            // Di chuyển đối tượng đến vị trí mục tiêu
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Nếu đối tượng gần với vị trí mục tiêu
            if (Vector2.Distance(transform.position, targetPosition) < 2)
            {
                // Không thể di chuyển, cập nhật trạng thái di chuyển trong Animator và bắt đầu coroutine OnRun
                canMove = false;
                anim.SetBool("canWalk", canMove);
            }
        }
    }

    void Attack()
    {
        // Đặt lại Timer khi Player vào phạm vi tấn công
        timer = intTimer;

        // Đặt biến attackMode để kiểm tra xem Enemy có thể tấn công hay không
        attackMode = true;

        // Nếu số lượng tấn công hiện tại lớn hơn một giá trị ngẫu nhiên trong khoảng từ 1 đến 3
        if (countAttack > UnityEngine.Random.Range(1, 3))
        {
            // Dừng trạng thái di chuyển, đặt trạng thái tấn công đặc biệt trong Animator thành true
            anim.SetBool("canWalk", false);
            anim.SetBool("AttackCast", true);
        }
        else
        {
            // Dừng trạng thái di chuyển, đặt trạng thái tấn công trong Animator thành true
            anim.SetBool("canWalk", false);
            anim.SetBool("Attack", true);
        }
    }

    public void InstantiateSpell()
    {
        // Tạo một bản sao của đối tượng spell
        var shootingClone = Instantiate(spell);

        // Tính toán vị trí mục tiêu dựa trên vị trí của Player và offset của kỹ năng
        var position = new Vector3(player.transform.position.x + positionOffSetSkill.x, player.transform.position.y + positionOffSetSkill.y, player.transform.position.z);

        // Đặt vị trí của bản sao spell tại vị trí đã tính toán
        shootingClone.transform.position = position;

        // Đặt lại số lượng tấn công và timer
        countAttack = 0;
        timer = 3;
    }

    void Cooldown()
    {
        // Giảm giá trị của timer dựa trên thời gian đã trôi qua
        timer -= Time.deltaTime;

        // Nếu timer giảm xuống dưới hoặc bằng 0 và đang trong trạng thái cooling
        if (timer <= 0 && cooling)
        {
            // Tắt trạng thái cooling và đặt lại giá trị timer
            cooling = false;
            timer = intTimer;
        }
    }


    public void TriggerCooling()
    {
        // Tăng số lượng tấn công
        countAttack++;

        // Bắt đầu trạng thái cooling và tắt trạng thái tấn công
        cooling = true;
        attackMode = false;

        // Đặt lại trạng thái tấn công trong Animator về false
        anim.SetBool("Attack", false);

        // Đặt lại trạng thái tấn công đặc biệt trong Animator về false
        anim.SetBool("AttackCast", false);
    }

    void Flip()
    {
        // Lấy giá trị góc quay hiện tại của đối tượng
        Vector3 rotation = transform.eulerAngles;

        // Nếu vị trí của đối tượng lớn hơn vị trí của mục tiêu theo trục x
        if (transform.position.x > target.position.x)
        {
            // Đặt giá trị góc quay theo trục y về 180 độ (quay hình ảnh ngược lại)
            rotation.y = 180;
        }
        else
        {
            // Ngược lại, đặt giá trị góc quay theo trục y về 0 độ (hình ảnh bình thường)
            rotation.y = 0;
        }

        // Cập nhật giá trị góc quay của đối tượng
        transform.eulerAngles = rotation;
    }

    public void DamagePlayer()
    {
        // Tạo danh sách chứa tất cả Collider2D gặp phải trong phạm vi của boxAttack
        List<Collider2D> colliders = new List<Collider2D>();

        // Kiểm tra và lấy tất cả Collider2D gặp phải trong boxAttack bằng cách sử dụng OverlapCollider
        Physics2D.OverlapCollider(boxAttack.GetComponent<Collider2D>(), enemyContactFilter, colliders);

        // Duyệt qua danh sách các Collider2D để kiểm tra va chạm với đối tượng Player
        foreach (Collider2D c in colliders)
        {
            // Nếu Collider2D thuộc layer "Player"
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // Gọi phương thức TakeDamage trên đối tượng Player và truyền giá trị là 1
                c.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            }
        }
    }

}
