using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 10f; // Tốc độ di chuyển của nhân vật
    public float jumpForce = 16f; // Lực nhảy của nhân vật
    public float groundCheckRadius; // Bán kính kiểm tra đất dưới chân nhân vật
    public float dashTime; // Thời gian thực hiện Dash
    public float dashSpeed; // Tốc độ của Dash
    public float dashCoolDown; // Thời gian chờ giữa các lần thực hiện Dash
    public int damage = 20; // Lượng sát thương của nhân vật
    public int bulletCount = 100; // Số lượng đạn ban đầu
    public int amountOfJumps = 1; // Số lượng nhảy tối đa
    public int diamondCount; // Số lượng kim cương nhân vật có
    public LayerMask whatIsGround; // Layer chứa đất
    public LayerMask whatIsWater; // Layer chứa nước
    public Transform groundCheck; // Điểm kiểm tra đất dưới chân nhân vật
    public Transform positionShooting; // Vị trí bắn đạn
    public GameObject shooting; // Đối tượng đạn
    public GameObject boxAttack; // Hộp tấn công
    public AudioClip audioClipAttack; // Âm thanh tấn công
    public ContactFilter2D enemyContactFilter; // Bộ lọc va chạm với kẻ địch

    public TMP_Text textBulletCount; // Text hiển thị số lượng đạn
    public TMP_Text textDiamondCount; // Text hiển thị số lượng kim cương
    public TMP_Text textDiamondCountYouWin; // Text hiển thị số lượng kim cương khi chiến thắng

    private Rigidbody2D rb; // Rigidbody của nhân vật
    private PlayerHealth playerHealth; // PlayerHealth của nhân vật
    private Vector2 move; // Vector di chuyển
    private Animator _animatorController; // Điều khiển Animator của nhân vật
    private bool isFacingRight; // Nhân vật đang hướng về bên phải hay không
    private bool isCheckGround; // Kiểm tra có đang đứng trên mặt đất không
    private bool canJump; // Có thể nhảy hay không
    private bool isDashing; // Đang thực hiện Dash hay không
    private bool canMove = true; // Có thể di chuyển hay không

    // Biến lưu trữ chỉ số của cuộc tấn công hiện tại
    [Range(0, 1)] public float TimeAttack = .25f;

    public float TimeShooting = .25f;

    // Biến lưu trữ thời gian kể từ lần tấn công gần nhất
    private float timeSinceAttack = 0.0f;
    private float timeSkillShooting = 0.0f;

    private float dashTimeLeft; // Thời gian Dash còn lại
    private float lastDash = -100f; // Thời điểm thực hiện Dash cuối cùng
    private float facingDirection = 1; // Hướng nhìn của nhân vật (1 là bên phải, -1 là bên trái)

    private int amountOfJumpsLeft; // Số lượng nhảy còn lại

    private AudioSource audioSourcePlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy tham chiếu đến Rigidbody2D của nhân vật
        audioSourcePlayer = GetComponent<AudioSource>(); // Lấy tham chiếu đến AudioSource của nhân vật
        _animatorController = GetComponent<Animator>(); // Lấy tham chiếu đến Animator của nhân vật
        playerHealth = GetComponent<PlayerHealth>(); // Lấy tham chiếu đến PlayerHealth của nhân vật

        if (GameManager.instance.diamondCount == 0)
        {
            textDiamondCount.text =
                "0";
        }
        else
        {
            textDiamondCount.text =
                GameManager.instance.diamondCount.ToString();
            diamondCount = GameManager.instance.diamondCount;
        }

        if (GameManager.instance.bulletCount == 0)
        {
            textBulletCount.text = bulletCount.ToString(); // Hiển thị số lượng đạn trên giao diện
        }
        else
        {
            textBulletCount.text = GameManager.instance.bulletCount.ToString(); // Hiển thị số lượng đạn trên giao diện
            bulletCount = GameManager.instance.bulletCount;
        }

        amountOfJumpsLeft = amountOfJumps; // Khởi tạo số lượng nhảy còn lại bằng số lượng nhảy tối đa
        // Hiển thị số lượng kim cương trên giao diện (đang để trống vì không có giá trị ban đầu)
    }

    private void Update()
    {
        GatherInput(); // Gọi hàm lấy thông tin đầu vào từ người chơi
        SelectTarget(); // Gọi hàm chọn mục tiêu
        CheckItCanJump(); // Gọi hàm kiểm tra có thể nhảy hay không
        UpdateAmintions(); // Gọi hàm cập nhật các animation
        CheckDash(); // Gọi hàm kiểm tra việc thực hiện Dash
    }

    private void FixedUpdate()
    {
        timeSinceAttack += Time.deltaTime; // Cập nhật thời gian kể từ lần tấn công gần nhất
        timeSkillShooting += Time.deltaTime; // Cập nhật thời gian kể từ lần sử dụng kỹ năng bắn đạn gần nhất
        AppLyMovement(); // Gọi hàm áp dụng di chuyển của nhân vật
        CheckSurroundings(); // Gọi hàm kiểm tra môi trường xung quanh nhân vật
        CheckWater();
    }

    private void CheckSurroundings()
    {
        // Kiểm tra xem nhân vật có đang đứng trên mặt đất hay không
        isCheckGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    private void CheckWater()
    {
        // Kiểm tra xem nhân vật có chạm vào nước hay không
        var isCheckWater = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsWater);

        //Nếu trạm vào nước chết.
        if (isCheckWater)
        {
            playerHealth.Die();
        }
    }

    // Thu thập thông tin đầu vào từ người chơi
    private void GatherInput()
    {
        // Lấy vector di chuyển từ trục ngang và dọc
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Nếu người chơi nhấn nút nhảy
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Nếu người chơi nhấn nút Dash
        if (Input.GetButtonDown("Dash"))
        {
            // Kiểm tra xem có thể thực hiện Dash hay không
            if (Time.time >= (lastDash + dashCoolDown))
                AttemptToDash();
        }

        // Nếu người chơi nhấn chuột phải để dùng kĩ năng
        if (Input.GetMouseButtonDown(1))
        {
            AttackShooting();
        }

        // Nếu người chơi nhấn chuột trái tấn công
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    // Hàm thực hiện kỹ năng bắn đạn của nhân vật
    private void AttackShooting()
    {
        // Kiểm tra xem có thể thực hiện kỹ năng bắn đạn hay không
        if (timeSkillShooting > TimeShooting && bulletCount > 0)
        {
            // Kích hoạt animation kỹ năng bắn đạn
            _animatorController.SetTrigger("AttackFireball");
            // Reset thời gian kể từ lần sử dụng kỹ năng bắn đạn
            timeSkillShooting = 0.0f;
        }
    }


    // Hàm thực hiện khi nhân vật tấn công bằng cách sử dụng skill đánh bại
    public void InstantiateShooting()
    {
        // Giảm số đạn và cập nhật hiển thị trên giao diện
        bulletCount--;
        textBulletCount.text = bulletCount.ToString();
        // Tạo một đối tượng đạn mới và đặt vị trí khởi tạo
        var shootingClone = Instantiate(shooting);
        shootingClone.transform.position = positionShooting.position;
        // Đặt hướng của đạn dựa trên hướng của nhân vật
        shootingClone.GetComponent<SkillShooting>().SetDirection(facingDirection);
    }

    // Hàm thực hiện khi nhân vật tấn công bằng cách sử dụng kỹ năng đánh thường
    private void Attack()
    {
        // Kiểm tra xem có thể thực hiện tấn công hay không (thời gian kể từ lần tấn công gần nhất lớn hơn 0.25 giây và nhân vật không đang cuộn)
        if (timeSinceAttack > TimeAttack)
        {
            // Gọi một trong ba hoạt cảnh tấn công "Attack"
            _animatorController.SetTrigger("Attack");
            audioSourcePlayer.PlayOneShot(audioClipAttack);
            // Đặt lại thời gian kể từ lần tấn công gần nhất
            timeSinceAttack = 0.0f;
        }
    }

    // Hàm thực hiện khi nhân vật gây sát thương cho kẻ địch
    public void DamageEnemy()
    {
        // Tìm tất cả Collider2D nằm trong vùng tấn công
        List<Collider2D> colliders = new List<Collider2D>();
        Physics2D.OverlapCollider(boxAttack.GetComponent<Collider2D>(), enemyContactFilter, colliders);
        // Duyệt qua từng Collider2D và kiểm tra layer
        foreach (Collider2D c in colliders)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Nếu là đối tượng Enemy_behaviour
                if (c.gameObject.GetComponent<Enemy_behaviour_01>() != null)
                {
                    // Gọi hàm cập nhật HP của Enemy_behaviour
                    c.gameObject.GetComponent<Enemy_behaviour_01>().UpdateHpEnemy(damage);
                }
                // Nếu là đối tượng Enemy_behaviour_idle
                else if (c.gameObject.GetComponent<Enemy_behaviour_idle>() != null)
                {
                    // Gọi hàm cập nhật HP của Enemy_behaviour_idle
                    c.gameObject.GetComponent<Enemy_behaviour_idle>().UpdateHpEnemy(damage);
                }
                // Nếu là đối tượng BossBringerOfDeath
                else if (c.gameObject.GetComponent<BossBringerOfDeath>() != null)
                {
                    // Gọi hàm cập nhật HP của BossBringerOfDeath
                    c.gameObject.GetComponent<BossBringerOfDeath>().UpdateHpEnemy(damage);
                }
            }
        }
    }


    // Hàm thực hiện khi nhân vật cố gắng thực hiện Dash
    private void AttemptToDash()
    {
        // Đặt trạng thái isDashing là true, đặt thời gian còn lại của Dash và ghi lại thời điểm cuối cùng Dash
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
    }

    // Hàm kiểm tra và thực hiện Dash
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                // Nếu đang trong thời gian Dash, người chơi không thể di chuyển và tốc độ di chuyển theo hướng Dash
                canMove = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
            }

            if (dashTimeLeft <= 0)
            {
                // Khi Dash kết thúc, người chơi có thể di chuyển lại
                canMove = true;
                isDashing = false;
            }
        }
    }


    // Hàm cập nhật trạng thái và animation của nhân vật
    private void UpdateAmintions()
    {
        // Cập nhật trạng thái chạy, đang đứng trên mặt đất, và vận tốc theo trục y cho Animator
        _animatorController.SetBool("Run", move.x != 0);
        _animatorController.SetBool("isGrounded", isCheckGround);
        _animatorController.SetFloat("yVelocity", rb.velocity.y);
        // Đặt trạng thái Dash trong Animator dựa trên khả năng di chuyển
        _animatorController.SetBool("Dash", !canMove);
    }

    // Hàm kiểm tra khả năng nhảy của nhân vật
    private void CheckItCanJump()
    {
        if (isCheckGround && rb.velocity.y <= 0)
        {
            // Nếu đang ở trên mặt đất và vận tốc theo trục y không dương, đặt lại số lượng nhảy còn lại
            amountOfJumpsLeft = amountOfJumps;
        }

        // Kiểm tra xem còn khả năng nhảy không
        if (amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    // Hàm thực hiện khi nhân vật nhảy
    private void Jump()
    {
        // Nếu có khả năng nhảy, thiết lập vận tốc theo trục y để nhảy và giảm số lượng nhảy còn lại
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;
        }
    }

    // Hàm áp dụng vận tốc di chuyển cho nhân vật
    public void AppLyMovement()
    {
        // Nếu có khả năng di chuyển, đặt vận tốc di chuyển theo hướng người chơi nhập vào
        if (canMove)
        {
            rb.velocity = new Vector2(movementSpeed * move.x, rb.velocity.y);
        }
    }


    // Hàm chọn mục tiêu, đảo hướng nhân vật nếu cần
    private void SelectTarget()
    {
        // Nếu đang hướng về bên phải và người chơi di chuyển về bên phải, đảo hướng và thiết lập hướng đối tượng
        if (isFacingRight && move.x > 0)
        {
            Flip();
            facingDirection = 1;
        }
        // Nếu đang hướng về bên trái và người chơi di chuyển về bên trái, đảo hướng và thiết lập hướng đối tượng
        else if (!isFacingRight && move.x < 0)
        {
            Flip();
            facingDirection = -1;
        }
    }

    // Hàm cập nhật số lượng kim cương và hiển thị trên giao diện
    public void UpdateTextDiamondCount(int count)
    {
        // Tăng số lượng kim cương theo giá trị đầu vào
        diamondCount += count;
        // Cập nhật văn bản trên giao diện
        textDiamondCount.text = diamondCount.ToString();
        GameManager.instance.diamondCount = diamondCount;
        textDiamondCountYouWin.text = "Số kim cương: " + diamondCount;
    }

    // Hàm cập nhật số lượng viên đạn và hiển thị trên giao diện
    public void UpdateTextShooting(int count)
    {
        // Tăng số lượng viên đạn theo giá trị đầu vào
        bulletCount += count;
        // Cập nhật văn bản trên giao diện
        textBulletCount.text = bulletCount.ToString();
        
        GameManager.instance.bulletCount = bulletCount;
    }

    // Hàm đảo hướng nhân vật
    private void Flip()
    {
        // Đảo hướng nhân vật và quay giao diện đi 180 độ
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    // Hàm vẽ hình cầu trong Scene view để hiển thị vùng đất kiểm tra
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}