using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heal : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject imageHeal;
    public Sprite imageHpFull;
    public Sprite imageHpEmpty;
    private void Awake()
    {
        for (int i = 0; i < playerHealth.countHeal; i++)
        {
            var healClone = Instantiate(imageHeal,transform);
            healClone.GetComponent<Image>().sprite = imageHpFull;
        }
    }
}
