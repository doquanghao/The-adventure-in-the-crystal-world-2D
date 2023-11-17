using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int countHeal; // Số lượng máu tối đa
    private int currentHealth; // Mức máu hiện tại
    public Transform groundHp; // Đối tượng chứa hình ảnh biểu tượng máu
    public Sprite imageHpFull; // Hình ảnh biểu tượng máu đầy
    public Sprite imageHpEmpty; // Hình ảnh biểu tượng máu rỗng
    public GameObject uiDie; // Giao diện hiển thị khi nhân vật chết
    private Animator _animatorController; // Điều khiển Animator của nhân vật

    private void Awake()
    {
        // Tắt giao diện chết khi khởi tạo
        uiDie.SetActive(false);

        // Lấy Animator của nhân vật
        _animatorController = GetComponent<Animator>();

        // Đặt máu hiện tại bằng máu tối đa
        if (GameManager.instance.currentHealth == 0)
        {
            currentHealth = countHeal;
        }
        else
        {
            currentHealth = GameManager.instance.currentHealth;
        }
    }

    // Hàm trừ máu
    public void TakeDamage(int damageAmount)
    {
        // Trừ lượng máu theo lượng damageAmount
        currentHealth -= damageAmount;
        GameManager.instance.currentHealth=currentHealth;
        // Kiểm tra nếu máu hiện tại ít hơn hoặc bằng 0
        if (currentHealth <= 0)
        {
            // Gọi hàm Die khi nhân vật hết máu
            _animatorController.SetTrigger("death");
        }
        else
        {
            // Chơi animation "hurt" khi nhân vật bị thương
            _animatorController.SetTrigger("hurt");

            // Cập nhật hình ảnh biểu tượng máu theo máu hiện tại
            groundHp.GetChild(currentHealth).GetComponent<Image>().sprite = imageHpEmpty;
        }
    }

    // Hàm cập nhật lượng máu khi nhận thêm máu
    public void UpdateHealth(int count)
    {
        // Kiểm tra xem máu hiện tại có bé hơn máu tối đa không
        if (currentHealth < countHeal)
        {
            // Cộng thêm lượng máu vào máu hiện tại
            currentHealth += count;
            // Cập nhật hình ảnh biểu tượng máu theo máu hiện tại
            groundHp.GetChild(currentHealth - 1).GetComponent<Image>().sprite = imageHpFull;
            GameManager.instance.currentHealth=currentHealth;
        }
    }

    // Hàm xử lý khi nhân vật chết
    public void Die()
    {
        // Dừng thời gian trong trò chơi
        Time.timeScale = 0;

        // Hiển thị giao diện chết
        uiDie.SetActive(true);
    }
}