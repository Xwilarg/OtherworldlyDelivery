using TMPro;
using UnityEngine;

namespace LudumDare53.Minigame
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [SerializeField]
        private TMP_Text _scoreText, _timerText;

        private float _timer = 120f;
        private int _score;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            _timerText.text = $"{Mathf.CeilToInt(_timer)}";
        }

        public void IncreaseScore(int value)
        {
            _score += value;
            _scoreText.text = $"{_score}";
        }
    }
}
