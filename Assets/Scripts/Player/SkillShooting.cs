using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShooting : MonoBehaviour
{
    // Tốc độ của đạn
    public float speed;

    // Thời gian tồn tại của viên đạn
    public float timeExistsShooting;

    // Sát thương của viên đạn
    public int damage;

    // Hướng di chuyển của viên đạn
    private float direction;

    // Biến xác định việc đạn đã trúng mục tiêu hay chưa
    private bool hit;

    // Thời gian sống của viên đạn
    private float lifetime;

    // Animator của viên đạn
    private Animator anim;

    // BoxCollider2D của viên đạn
    private BoxCollider2D boxCollider;


    private void Awake()
    {
        // Lấy tham chiếu đến Animator và BoxCollider2D của đối tượng
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Nếu đạn đã trúng mục tiêu, không làm gì cả
        if (hit) return;

        // Tính toán vận tốc di chuyển dựa trên thời gian và hướng
        float movementSpeed = speed * Time.deltaTime * direction;

        // Di chuyển đạn theo hướng và vận tốc đã tính toán
        transform.Translate(movementSpeed, 0, 0);

        // Tăng thời gian sống của viên đạn
        lifetime += Time.deltaTime;

        // Nếu thời gian sống vượt quá thời gian tồn tại, hủy đối tượng đạn
        if (lifetime > timeExistsShooting) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với đối tượng có layer là "Enemy"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Đánh dấu rằng đạn đã trúng mục tiêu
            hit = true;

            // Kích hoạt trigger "explode" trên Animator
            anim.SetTrigger("explode");

            // Di chuyển đạn đến vị trí của đối tượng va chạm
            transform.position = collision.gameObject.transform.position;

            // Kiểm tra loại đối tượng Enemy và cập nhật HP tương ứng
            if (collision.gameObject.GetComponent<Enemy_behaviour_01>() != null)
            {
                collision.gameObject.GetComponent<Enemy_behaviour_01>().UpdateHpEnemy(damage);
            }
            else if (collision.gameObject.GetComponent<Enemy_behaviour_idle>() != null)
            {
                collision.gameObject.GetComponent<Enemy_behaviour_idle>().UpdateHpEnemy(damage);
            }
            else if (collision.gameObject.GetComponent<BossBringerOfDeath>() != null)
            {
                collision.gameObject.GetComponent<BossBringerOfDeath>().UpdateHpEnemy(damage);
            }
        }
    }

    public void SetDirection(float _direction)
    {
        // Đặt lại thời gian sống, hướng di chuyển, và trạng thái trúng đạn
        lifetime = 0;
        direction = _direction;
        hit = false;

        // Kích hoạt BoxCollider để xử lý va chạm
        boxCollider.enabled = true;

        // Lấy giá trị localScaleX hiện tại của đối tượng
        float localScaleX = transform.localScale.x;

        // Nếu hướng di chuyển là về bên phải (_direction > 0), đảo ngược localScaleX
        if (_direction > 0)
            localScaleX = -localScaleX;

        // Cập nhật localScale của đối tượng để đảm bảo hướng di chuyển đúng
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        // Hủy đối tượng đạn
        Destroy(gameObject);
    }

}
