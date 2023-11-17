using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentHealth = 0; // Mức máu hiện tại
    public int diamondCount = 0; // Số lượng kim cương nhân vật có
    public int bulletCount = 0; // Số lượng đạn ban đầu

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}