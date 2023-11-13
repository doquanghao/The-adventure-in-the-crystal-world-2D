using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int countHeal;
    private int currentHealth;
    public Transform groundHp;
    public Sprite imageHpFull;
    public Sprite imageHpEmpty;

    private void Awake()
    {
        currentHealth = countHeal;
    }

    // Hàm trừ HP
    public void TakeDamage(int damageAmount)
    {
        // Trừ HP
        currentHealth -= damageAmount;
        // Kiểm tra xem nhân vật còn HP không
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            groundHp.GetChild(currentHealth).GetComponent<Image>().sprite = imageHpEmpty;
        }
    }
    // Hàm xử lý khi nhân vật chết
    private void Die()
    {
        // Xử lý khi nhân vật chết, ví dụ như kết thúc trò chơi, chơi animation chết, v.v.
        Debug.Log("Nhân vật đã chết!");
        // Đặt HP về lại giá trị ban đầu sau khi chết (nếu cần)
    }

}
