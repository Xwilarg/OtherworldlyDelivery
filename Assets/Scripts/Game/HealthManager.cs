using LudumDare53.Audio;
using LudumDare53.Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare53.Game
{
    public class HealthManager : MonoBehaviour
    {
        public static HealthManager Instance { get; private set; }

        [SerializeField]
        private RectTransform _cursor;

        [SerializeField]
        private AudioClip _baseBGM;

        [SerializeField]
        private RectTransform _aiHealthBar;

        private HealthCursor _cursorScript;

        private int _currDamage;
        private int _prevMaxHealth = 100;
        private float _maxHealthTimer;
        private int _aiMaxHealth = 100;

        public bool HasLost { private set; get; }

        private void Awake()
        {
            Instance = this;
            _cursorScript = _cursor.GetComponent<HealthCursor>();
        }

        public void ReduceAIMaxHealth(int amount)
        {
            _prevMaxHealth = _aiMaxHealth;
            _aiMaxHealth -= amount;
            _maxHealthTimer = 0f;
            TakeDamage(0); // Used to do victory check and stuff
        }

        public void TakeDamage(int amount)
        {
            _currDamage -= amount;
            if (_currDamage < -100)
            {
                _currDamage = -100;
            }
            else if (_currDamage > _aiMaxHealth)
            {
                _currDamage = _aiMaxHealth;
            }
            var pos = _currDamage / 200f;
            _cursorScript.MoveTo(pos);
            if (_currDamage <= -100)
            {
                HasLost = true;
                DialogueManager.Instance.ShowText("Divyansh", "BLUE", "See, I told you it's not mine, so now just go! Leave the package here tho, as a compensation, so I can... burn it", () =>
                {
                    SceneManager.LoadScene("MainMenu");
                });
                BGMManager.Instance.SetBGM(_baseBGM);
            }
            else if (_currDamage >= _aiMaxHealth)
            {
                HasLost = true;
                DialogueManager.Instance.ShowText("Divyansh", "BLUE", "Fine, you won! Here I'm taking it, grab your money for the livraison fees and just leave now!", () =>
                {
                    SceneManager.LoadScene("MainMenu");
                });
                BGMManager.Instance.SetBGM(_baseBGM);
            }
        }

#if UNITY_EDITOR
        public int Health => _currDamage;
#endif

        public bool IsAILoosing => _currDamage > _aiMaxHealth / 2;

        private void Update()
        {
            _maxHealthTimer += Time.deltaTime * 10f;
            _aiHealthBar.localScale = new(Mathf.Lerp(_prevMaxHealth, _aiMaxHealth, Mathf.Clamp01(_maxHealthTimer)) / 100f, _aiHealthBar.localScale.y, _aiHealthBar.localScale.z);
        }
    }
}
