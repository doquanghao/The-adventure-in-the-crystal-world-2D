using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoss : MonoBehaviour
{
    public GameObject victoryScreen; // Màn hình chiến thắng

    void OnTriggerEnter2D(Collider2D trig)
    {
        // Kiểm tra xem vật phẩm va chạm với đối tượng có tag "Player" không
        if (trig.gameObject.CompareTag("Player") && trig != null)
        {
            // Gọi hàm cập nhật số lượng kim cương lớn khi nhận vật phẩm Diamond Big
            trig.GetComponent<PlayerController>().UpdateTextDiamondCount(20000);
            // Hiển thị màn hình chiến thắng
            victoryScreen.SetActive(true);
            Time.timeScale = 0;
            // Hủy đối tượng vật phẩm sau khi xử lý
            Destroy(gameObject);
        }
    }
}
