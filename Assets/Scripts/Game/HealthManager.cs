﻿using LudumDare53.Audio;
using LudumDare53.Dialogue;
using LudumDare53.Effect;
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

        private HealthCursor _cursorScript;

        private int _currDamage;

        public bool HasLost { private set; get; }

        private void Awake()
        {
            Instance = this;
            _cursorScript = _cursor.GetComponent<HealthCursor>();
        }

        public void TakeDamage(int amount)
        {
            _currDamage -= amount;
            var pos = _currDamage / 200f;
            _cursorScript.MoveTo(pos);
            if (_currDamage < -1)
            {
                HasLost = true;
                DialogueManager.Instance.ShowText("Divyansh", "BLUE", "See, I told you it's not mine, so now just go! Leave the package here tho, as a compensation, so I can... burn it", () =>
                {
                    SceneManager.LoadScene("MainMenu");
                });
                BGMManager.Instance.SetBGM(_baseBGM);
            }
            else if (_currDamage > 100)
            {
                HasLost = true;
                DialogueManager.Instance.ShowText("Divyansh", "BLUE", "Fine, you won! Here I'm taking it, grab your money for the livraison fees and just leave now!", () =>
                {
                    SceneManager.LoadScene("MainMenu");
                });
                BGMManager.Instance.SetBGM(_baseBGM);
            }
        }
    }
}
