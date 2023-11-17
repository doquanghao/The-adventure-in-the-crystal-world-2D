using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScreen : MonoBehaviour
{
   public string nameScreen;
   void OnTriggerEnter2D(Collider2D trig)
   {
      if (trig.gameObject.CompareTag("Player"))
      {
         SceneManager.LoadScene(nameScreen);
      }
   }
}
