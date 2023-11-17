using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TypeItem
{
    dagger = 1,
    diamond_big = 2,
    diamond_small = 3,
    heart = 4,
}
public class ItemMode : MonoBehaviour
{
    // Loại của vật phẩm
    public TypeItem typeItem;

    // Animator của vật phẩm
    public Animator animatorItem;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource=GetComponent<AudioSource>();
        // Tạo một số ngẫu nhiên để chọn TypeItem mới
        System.Random random = new System.Random();
        TypeItem randomTypeItem = (TypeItem)random.Next(1, Enum.GetValues(typeof(TypeItem)).Length + 1);
        // Gán giá trị TypeItem mới cho itemMode và khởi tạo item
        typeItem = randomTypeItem;
        InitItem();
    }
    // Hàm khởi tạo vật phẩm
    public void InitItem()
    {
        // Đặt trạng thái của Animator dựa trên loại vật phẩm
        animatorItem.SetInteger("item", (int)typeItem);
    }

    // Hàm xử lý khi vật phẩm va chạm với Collider2D khác
    void OnTriggerEnter2D(Collider2D trig)
    {
        // Kiểm tra xem vật phẩm va chạm với đối tượng có tag "Player" không
        if (trig.gameObject.CompareTag("Player") && trig != null)
        {
            // Sử dụng cấp độ vật phẩm để thực hiện hành động tương ứng
            switch (typeItem)
            {
                case TypeItem.dagger:
                    // Gọi hàm cập nhật số lượng viên đạn khi nhận vật phẩm Dagger
                    trig.GetComponent<PlayerController>().UpdateTextShooting(1);
                    break;
                case TypeItem.diamond_big:
                    // Gọi hàm cập nhật số lượng kim cương lớn khi nhận vật phẩm Diamond Big
                    trig.GetComponent<PlayerController>().UpdateTextDiamondCount(2000);
                    break;
                case TypeItem.diamond_small:
                    // Gọi hàm cập nhật số lượng kim cương nhỏ khi nhận vật phẩm Diamond Small
                    trig.GetComponent<PlayerController>().UpdateTextDiamondCount(200);
                    break;
                case TypeItem.heart:
                    // Gọi hàm cập nhật sức khỏe khi nhận vật phẩm Heart
                    trig.GetComponent<PlayerHealth>().UpdateHealth(1);
                    break;
            }
            audioSource.Play();

            // Hủy đối tượng vật phẩm sau khi xử lý
            Destroy(gameObject);
        }
    }

}
