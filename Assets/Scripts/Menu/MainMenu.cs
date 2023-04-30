using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare53.Menu
{
    public class MainMenu : MonoBehaviour
    {
        private SHA256 _sha256;

        private void Awake()
        {
            _sha256 = SHA256.Create();
        }

        public void Play()
        {
            SceneManager.LoadScene("Main");
        }

        public void OnSecretValueChanged(string s)
        {
            var hash = string.Concat(_sha256.ComputeHash(Encoding.UTF8.GetBytes(s)).Select(item => $"{item:x2}"));
            if (hash == "3900e2a1012f330abc08d90dff351d447847cabaed0149185e8269a350a079ae")
            {
                SceneManager.LoadScene("Minigame");
            }
        }
    }
}
