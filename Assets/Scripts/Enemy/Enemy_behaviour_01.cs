using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_behaviour_01 : MonoBehaviour
{
    public GameObject item;  // Đối tượng Item để tạo khi Enemy bị hủy
    public Transform rayCast;  // Đối tượng dùng để thực hiện raycast để xác định Player
    public LayerMask raycastMask;  // LayerMask để chỉ định loại đối tượng Player trong raycast
    public float rayCastLength;  // Độ dài của raycast để xác định Player
    public float attackDistance; // Khoảng cách tấn công tối thiểu
    public float moveSpeed;  // Tốc độ di chuyển của Enemy
    public float timer; // Thời gian chờ giữa các lần tấn công
    public int hpEnemy = 100;  // Máu của Enemy
    public GameObject hp;  // Đối tượng hiển thị thanh máu
    public GameObject boxAttack;  // Hộp va chạm dùng để xác định vùng tấn công
    public Transform leftLimit;  // Giới hạn bên trái cho di chuyển của Enemy
    public Transform rightLimit;  // Giới hạn bên phải cho di chuyển của Enemy
    public ContactFilter2D enemyContactFilter;  // Bộ lọc va chạm với đối tượng Enemy

    private RaycastHit2D hit;  // Lưu trữ kết quả từ raycast
    private Transform target;  // Đối tượng Player
    private Animator anim;  // Animator của Enemy
    private float distance; // Lưu trữ khoảng cách giữa Enemy và Player
    private bool attackMode;  // Kiểm tra xem Enemy có thể tấn công hay không
    private bool cooling; // Kiểm tra xem Enemy đang trong thời gian chờ giữa các lần tấn công hay không
    private float intTimer;  // Lưu trữ giá trị ban đầu của timer
    private int hpEnemyLeft;  // Máu còn lại của Enemy
    private bool inRange; // Kiểm tra xem Người chơi có ở trong phạm vi không

    void Awake()
    {
        SelectTarget();
        intTimer = timer;  // Lưu giữ giá trị ban đầu của timer
        anim = GetComponent<Animator>();
        hpEnemyLeft = hpEnemy;
        hp.transform.localScale = new Vector3((float)hpEnemyLeft / hpEnemy, 1, 1);
    }

    void Update()
    {
        if (!attackMode)
        {
            Move();
        }

        // Kiểm tra xem có nằm trong giới hạn không và không ở trong tầm tấn công, cũng không đang ở trạng thái tấn công
        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            SelectTarget();
        }

        // Nếu trong tầm tấn công
        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        // Khi phát hiện người chơi
        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;  // Nếu không phát hiện người chơi, đặt inRange về false
        }

        if (inRange == false)
        {
            StopAttack();
        }
    }
    public void UpdateHpEnemy(int damage)
    {
        hpEnemyLeft -= damage;
        if (hpEnemyLeft <= 0)
        {
            // Nếu HP dưới 0, tạo ra một item và hủy đối tượng Enemy
            var itemClone = Instantiate(item);
            itemClone.transform.position = transform.position;
            Destroy(transform.parent.gameObject);
        }
        else
        {
            // Nếu HP còn, thực hiện hiệu ứng "đau" và giảm độ chính xác của Enemy
            anim.SetTrigger("damage");
            hp.transform.localScale = new Vector3((float)hpEnemyLeft / hpEnemy, 1, 1);
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            target = trig.transform;
            inRange = true;
            Flip();  // Gọi phương thức Flip để đảo hướng của Enemy khi gặp người chơi
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
            Attack();  // Nếu người chơi trong tầm tấn công, thực hiện tấn công
        }

        if (cooling)
        {
            Cooldown();  // Nếu đang trong trạng thái cooldown, giảm cooldown và chuyển trạng thái tấn công về false
            anim.SetBool("Attack", false);
        }
    }


    void Move()
    {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            // Thiết lập vị trí mục tiêu dựa trên vị trí của người chơi
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            // Di chuyển Enemy đến vị trí mục tiêu
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        timer = intTimer; // Đặt lại thời gian cooldown khi người chơi vào phạm vi tấn công
        attackMode = true; // Để kiểm tra xem Enemy có thể tấn công hay không

        anim.SetBool("canWalk", false); // Dừng hoạt động di chuyển khi tấn công
        anim.SetBool("Attack", true); // Kích hoạt trạng thái tấn công trong Animator
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
        // Hiển thị tia Raycast trong Scene Editor để theo dõi phạm vi tấn công của Enemy
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
        // Kiểm tra xem Enemy có nằm trong giới hạn di chuyển không
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    private void SelectTarget()
    {
        // Chọn một hướng di chuyển dựa trên khoảng cách đến các giới hạn bên trái và bên phải
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

        Flip(); // Đảo hướng của Enemy để hướng về target
    }

    void Flip()
    {
        // Xác định hướng quay của Enemy dựa trên vị trí của target
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
        {
            rotation.y = 180; // Quay về hướng ngược lại nếu target ở bên trái
        }
        else
        {
            rotation.y = 0; // Quay về hướng ban đầu nếu target ở bên phải
        }
        transform.eulerAngles = rotation;
    }

    public void DamagePlayer()
    {
        // Tạo danh sách collider để lưu trữ các collider va chạm
        List<Collider2D> colliders = new List<Collider2D>();

        // Sử dụng OverlapCollider để lấy tất cả collider va chạm với boxAttack và lưu vào danh sách colliders
        Physics2D.OverlapCollider(boxAttack.GetComponent<Collider2D>(), enemyContactFilter, colliders);

        // Duyệt qua danh sách colliders và gọi phương thức TakeDamage trên PlayerHealth nếu có collider thuộc layer "Player"
        foreach (Collider2D c in colliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                c.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
            }
        }
    }

}
