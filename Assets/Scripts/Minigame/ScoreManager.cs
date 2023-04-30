using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LudumDare53.Minigame
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [SerializeField]
        private TMP_Text _scoreText, _timerText, _finalScoreText;

        [SerializeField]
        private GameObject _endContainer;

        private float _timer = 60f;
        private int _score;

        public bool DidGameEnded { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (!DidGameEnded)
            {
                _timer -= Time.deltaTime;
                _timerText.text = $"{Mathf.CeilToInt(_timer)}";
                if (_timer <= 0f && !DidGameEnded)
                {
                    DidGameEnded = true;
                    _endContainer.SetActive(true);
                    _finalScoreText.text = $"Final Score:\n{_score}";
                    foreach (var go in GameObject.FindGameObjectsWithTag("Mag"))
                    {
                        Destroy(go);
                    }
                }
            }
        }

        public void IncreaseScore(int value)
        {
            _score += value;
            _scoreText.text = $"{_score}";
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
