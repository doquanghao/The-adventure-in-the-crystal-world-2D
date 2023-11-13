using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public void DestroySpell()
    {
        Destroy(gameObject);
    }
}
