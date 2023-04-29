using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare53.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
