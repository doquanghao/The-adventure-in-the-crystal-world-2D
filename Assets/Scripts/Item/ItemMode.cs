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
    public TypeItem typeItem;
    public Animator animatorItem;
    public void InitItem()
    {
        animatorItem.SetInteger("item", (int)typeItem);
    }
    void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            switch (typeItem)
            {
                case TypeItem.dagger:
                    trig.GetComponent<PlayerController>().UpdateTextShooting(1);
                    break;
                case TypeItem.diamond_big:
                    trig.GetComponent<PlayerController>().UpdateTextDiamondCount(2000);
                    break;
                case TypeItem.diamond_small:
                    trig.GetComponent<PlayerController>().UpdateTextDiamondCount(200);
                    break;
                case TypeItem.heart:
                    trig.GetComponent<PlayerHealth>().UpdateHealth(1);
                    break;
            }
            Destroy(gameObject);
        }
    }

}
