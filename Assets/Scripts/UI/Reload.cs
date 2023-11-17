using UnityEngine;
using UnityEngine.SceneManagement;

public class Reload : MonoBehaviour
{
   public void OnReload()
   {
      SceneManager.LoadScene("Map1");
   }
}
