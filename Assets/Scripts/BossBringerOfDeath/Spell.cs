using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    // Hủy đối tượng spell/game
    public void DestroySpell()
    {
        Destroy(gameObject);
    }

    // Khi đối tượng spell va chạm với một collider khác
    void OnTriggerEnter2D(Collider2D trig)
    {
        // Kiểm tra nếu đối tượng va chạm có tag là "Player"
        if (trig.gameObject.CompareTag("Player"))
        {
            // Gọi phương thức TakeDamage trên thành phần PlayerHealth của đối tượng player và truyền giá trị 1
            trig.gameObject.GetComponent<PlayerHealth>().TakeDamage(1);
        }
    }

}
