using LudumDare53.SO;
using TMPro;
using UnityEngine;

namespace LudumDare53.Card
{
    public class CardsManager : MonoBehaviour
    {
        public static CardsManager Instance { get; private set; }

        [SerializeField]
        private Transform _cardContainer;

        [SerializeField]
        private GameObject _cardPrefab;

        [SerializeField]
        private CardInfo[] _cards;

        [SerializeField]
        private TMP_Text _dialogueText;

        private float _timerText;

        private void Awake()
        {
            Instance = this;
        }

        public void ShowText(string text)
        {
            _dialogueText.text = text;
            _timerText = 2f;
        }

        private void Update()
        {
            if (_timerText > 0f)
            {
                _timerText -= Time.deltaTime;
                if (_timerText <= 0f)
                {
                    _dialogueText.text = string.Empty;
                }
            }
        }

        public void SpawnCards()
        {
            for (var i = 0; i < _cardContainer.childCount; i++)
            {
                Destroy(_cardContainer.GetChild(i).gameObject);
            }
            for (int i = 0; i < 3; i++)
            {
                var go = Instantiate(_cardPrefab, _cardContainer);
                go.GetComponent<CardInstance>().Info = _cards[Random.Range(0, _cards.Length)];
            }
        }
    }
}
