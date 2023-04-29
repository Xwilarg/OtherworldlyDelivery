using LudumDare53.Dialogue;
using LudumDare53.Game;
using LudumDare53.NPC;
using LudumDare53.SO;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        private CardInfo[] _playerCards;

        [SerializeField]
        private RectTransform _cardAIPos;

        private bool _isAITurn;

        private void Awake()
        {
            Instance = this;
        }

        public void RemoveCards()
        {
            for (var i = 0; i < _cardContainer.childCount; i++)
            {
                Destroy(_cardContainer.GetChild(i).gameObject);
            }
            for (var i = 0; i < _cardAIPos.childCount; i++)
            {
                Destroy(_cardAIPos.GetChild(i).gameObject);
            }
        }

        public void SpawnCards()
        {
            for (int i = 0; i < 3; i++)
            {
                var go = Instantiate(_cardPrefab, _cardContainer);
                go.GetComponent<CardInstance>().Info = _playerCards[Random.Range(0, _playerCards.Length)];
            }
        }

        public void SpawnAICard(CardInfo card)
        {
            var go = Instantiate(_cardPrefab, _cardAIPos);
            go.GetComponent<CardInstance>().Info = card;
            go.GetComponent<Button>().interactable = false;
        }

        public void DoAction(CardInfo card)
        {
            _isAITurn = !_isAITurn;
            switch (card.Type)
            {
                case ActionType.DAMAGE:
                    HealthManager.Instance.TakeDamage(card.Value);
                    break;

                default:
                    throw new NotImplementedException();
            }
            DialogueManager.Instance.ShowText(string.Empty, "NONE", card.Sentence, () =>
            {
                RemoveCards();
                if (_isAITurn)
                {
                    AIManager.Instance.Play();
                }
                else
                {
                    SpawnCards();
                }
            });
        }
    }
}
