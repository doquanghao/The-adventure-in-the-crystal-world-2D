using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDie : MonoBehaviour
{
   public void OnReload()
   {
      SceneManager.LoadScene("Map1");
   }
}
