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
    public int quantity;
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
            Destroy(gameObject);
        }
    }

}
