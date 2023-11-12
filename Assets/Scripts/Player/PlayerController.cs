using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
   public float movementSpeed = 10f;
   public float jumpForce = 16f;
   public float groundCheckRadius;
   public float dashTime;
   public float dashSpeed;
   public float dashCoolDown;
   public int damage = 20;
   public int bulletCount = 100;


   public int amountOfJumps = 1;
   public LayerMask whatIsGround;
   public Transform groundCheck;
   public Transform positionShooting;
   public GameObject shooting;
   public GameObject boxAttack;
   public GameObject boxAttack1;
   public ContactFilter2D enemyContactFilter;

   public TMP_Text textBulletCount;

   private Rigidbody2D rb;
   private Vector2 move;
   private Animator _animatorController;
   private bool isFacingRight;
   private bool isCheckGround;
   private bool canJump;
   private bool isDashing;
   private bool canMove = true;


   // Biến lưu trữ chỉ số của cuộc tấn công hiện tại
   [Range(0, 1)]
   public float TimeAttack = .25f;
   public float TimeShooting = .25f;
   // Biến lưu trữ thời gian kể từ lần tấn công gần nhất
   private float timeSinceAttack = 0.0f;
   private float timeSkillShooting = 0.0f;

   private float dashTimeLeft;
   private float lastDash = -100f;
   private float facingDirection = 1;

   private int amountOfJumpsLeft;
   private void Awake()
   {
      rb = GetComponent<Rigidbody2D>();
      _animatorController = GetComponent<Animator>();
      amountOfJumpsLeft = amountOfJumps;
      textBulletCount.text = bulletCount.ToString();
   }

   private void Update()
   {
      GatherInput();
      SelectTarget();
      CheckItCanJump();
      UpdateAmintions();
      CheckDash();
   }

   private void FixedUpdate()
   {
      timeSinceAttack += Time.deltaTime;
      timeSkillShooting += Time.deltaTime;
      AppLyMovement();
      CheckSurroundings();
   }


   public void CheckSurroundings()
   {
      isCheckGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
   }

   // Thu thập thông tin đầu vào từ người chơi
   private void GatherInput()
   {
      // Lấy vector di chuyển từ trục ngang và dọc
      move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

      if (Input.GetButtonDown("Jump"))
      {
         Jump();
      }
      if (Input.GetButtonDown("Dash"))
      {
         if (Time.time >= (lastDash + dashCoolDown))
            AttemptToDash();
      }

      if (Input.GetMouseButtonDown(1))
      {
         AttackShooting();
      }
      if (Input.GetMouseButtonDown(0))
      {
         Attack();
      }
   }

   private void AttackShooting()
   {
      if (timeSkillShooting > TimeShooting && bulletCount > 0)
      {
         _animatorController.SetTrigger("AttackFireball");
         timeSkillShooting = 0.0f;
      }
   }

   public void InstantiateShooting()
   {
      bulletCount--;
      textBulletCount.text = bulletCount.ToString();
      var shootingClone = Instantiate(shooting);
      shootingClone.transform.position = positionShooting.position;
      shootingClone.GetComponent<SkillShooting>().SetDirection(facingDirection);
   }
   private void Attack()
   {
      // Nếu người chơi nhấp chuột trái và thời gian kể từ lần tấn công gần nhất lớn hơn 0.25s và nhân vật không đang cuộn
      if (timeSinceAttack > TimeAttack)
      {
         // Gọi một trong ba hoạt cảnh tấn công "Attack"
         _animatorController.SetTrigger("Attack");
         // Đặt lại bộ hẹn giờ
         timeSinceAttack = 0.0f;
      }
   }

   public void DamageEnemy()
   {
      List<Collider2D> colliders = new List<Collider2D>();
      Physics2D.OverlapCollider(boxAttack.GetComponent<Collider2D>(), enemyContactFilter, colliders);
      foreach (Collider2D c in colliders)
      {
         if (c.gameObject.layer == LayerMask.NameToLayer("Enemy"))
         {
            if (c.gameObject.GetComponent<Enemy_behaviour>() != null)
            {
               c.gameObject.GetComponent<Enemy_behaviour>().UpdateHpEnemy(damage, gameObject);
            }
            else if (c.gameObject.GetComponent<Enemy_behaviour_idle>() != null)
            {
               c.gameObject.GetComponent<Enemy_behaviour_idle>().UpdateHpEnemy(damage);
            }
         }
      }
   }

   private void AttemptToDash()
   {
      isDashing = true;
      dashTimeLeft = dashTime;
      lastDash = Time.time;
   }
   private void CheckDash()
   {
      if (isDashing)
      {
         if (dashTimeLeft > 0)
         {
            canMove = false;
            rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
            dashTimeLeft -= Time.deltaTime;
         }
         if (dashTimeLeft <= 0)
         {
            canMove = true;
         }
      }
   }

   private void UpdateAmintions()
   {
      _animatorController.SetBool("Run", rb.velocity.x != 0);
      _animatorController.SetBool("isGrounded", isCheckGround);
      _animatorController.SetFloat("yVelocity", rb.velocity.y);
      _animatorController.SetBool("Dash", !canMove);
   }
   private void CheckItCanJump()
   {
      if (isCheckGround && rb.velocity.y <= 0)
      {
         amountOfJumpsLeft = amountOfJumps;
      }
      if (amountOfJumpsLeft <= 0)
      {
         canJump = false;
      }
      else
      {
         canJump = true;
      }
   }
   private void Jump()
   {
      if (canJump)
      {
         rb.velocity = new Vector2(rb.velocity.y, jumpForce);
         amountOfJumpsLeft--;
      }
   }

   public void AppLyMovement()
   {
      if (canMove)
      {
         rb.velocity = new Vector2(movementSpeed * move.x, rb.velocity.y);
      }
   }

   private void SelectTarget()
   {

      if (isFacingRight && move.x > 0)
      {
         Flip();
         facingDirection = 1;
      }
      else if (!isFacingRight && move.x < 0)
      {
         Flip();
         facingDirection = -1;
      }

   }
   private void Flip()
   {
      isFacingRight = !isFacingRight;
      transform.Rotate(0, 180, 0);
   }

   private void OnDrawGizmos()
   {
      Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
   }
}
