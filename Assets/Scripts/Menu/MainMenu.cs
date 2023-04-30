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

        public void OnSecretValueChanged(string s)
        {
            if (s == "5490")
            {
                SceneManager.LoadScene("Minigame");
            }
        }
    }
}
