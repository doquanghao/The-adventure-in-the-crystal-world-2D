using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heal : MonoBehaviour
{
    public PlayerHealth playerHealth;// Tham chiếu đến script PlayerHealth
    public GameObject imageHeal;// Prefab của hình ảnh cho mỗi điểm máu
    public Sprite imageHpFull; // Sprite cho sức khỏe đầy đủ
    public Sprite imageHpEmpty;       // Sprite cho sức khỏe hết

    private void Awake()
    {
        // Lặp qua số lượng điểm máu ban đầu của người chơi
        for (int i = 0; i < playerHealth.countHeal; i++)
        {
            // Instantiate một hình ảnh đại diện cho mỗi điểm máu
            var healClone = Instantiate(imageHeal, transform);
            // Đặt sprite của hình ảnh là hình ảnh cho sức khỏe đầy đủ
            healClone.GetComponent<Image>().sprite = imageHpFull;
        }
    }
}
