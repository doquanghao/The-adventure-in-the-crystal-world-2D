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
    public GameObject uiDie;
    private Animator _animatorController;
    private void Awake()
    {
         uiDie.SetActive(false);
        _animatorController = GetComponent<Animator>();
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
            _animatorController.SetTrigger("hurt");
            groundHp.GetChild(currentHealth).GetComponent<Image>().sprite = imageHpEmpty;
        }
    }

    public void UpdateHealth(int count)
    {
        if (currentHealth <= countHeal)
        {
            currentHealth += count;
            groundHp.GetChild(currentHealth).GetComponent<Image>().sprite = imageHpEmpty;
        }
    }

    // Hàm xử lý khi nhân vật chết
    private void Die()
    {
        uiDie.SetActive(true);
    }

}
