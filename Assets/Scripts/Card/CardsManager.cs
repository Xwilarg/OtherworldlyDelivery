using LudumDare53.Dialogue;
using LudumDare53.Game;
using LudumDare53.NPC;
using LudumDare53.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        private List<CardInfo> _playerCards;

        [SerializeField]
        private CardInfo _uselessMumble;

        [SerializeField]
        private RectTransform _cardAIPos;

        [SerializeField]
        private TMP_Text _rageText;

        private int _rage;

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
                go.GetComponent<CardInstance>().Info = _playerCards[Random.Range(0, _playerCards.Count)];
            }
        }

        public void SpawnAICard(CardInfo card)
        {
            var go = Instantiate(_cardPrefab, _cardAIPos);
            go.GetComponent<CardInstance>().Info = card;
            go.GetComponent<Button>().interactable = false;
        }

        public string GetDescription(CardInfo card)
        {
            return string.Join("\n", card.Effects.Select(x => x.Type switch
            {
                ActionType.DAMAGE => x.Value > 0 ?
                    $"Inflict {x.Value} damage" :
                    $"Take {-x.Value} damage",
                ActionType.RAGE => $"Increase rage by {x.Value}",
                ActionType.INTIMIDATE => $"Target gain a \"Useless Mumble\" card",
                ActionType.DESTROY_ON_DISCARD => "Destroyed when used",
                _ => throw new NotImplementedException()
            }));
        }

        public void DoAction(CardInfo card)
        {
            _isAITurn = !_isAITurn;
            foreach (var e in card.Effects)
            {
                switch (e.Type)
                {
                    case ActionType.DAMAGE:
                        HealthManager.Instance.TakeDamage(e.Value * (_isAITurn ? -(1 + _rage) : 1));
                        break;

                    case ActionType.RAGE:
                        _rage += e.Value;
                        if (_rage < 0)
                        {
                            _rage = 0;
                        }
                        _rageText.text = $"Rage: {_rage}";
                        break;

                    case ActionType.INTIMIDATE:
                        _playerCards.Add(_uselessMumble);
                        break;

                    case ActionType.DESTROY_ON_DISCARD:
                        var index = _playerCards.IndexOf(card);
                        _playerCards.RemoveAt(index);
                        break;

                    default:
                        throw new NotImplementedException();
                }
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
