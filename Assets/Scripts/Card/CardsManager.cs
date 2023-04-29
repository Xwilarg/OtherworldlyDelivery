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
        private CardInfo[] _playerCards;
        private List<CardInfo> _playerDeck;

        [SerializeField]
        private CardInfo _uselessMumble;

        [SerializeField]
        private RectTransform _cardAIPos;

        [SerializeField]
        private TMP_Text _rageText;

        private int _rage;

        private bool _isNotAITurn;

        private int _attackCooldownPlayer, _attackCooldownAI, _attackForceCooldownAI;
        private int _noDrawbackCooldownPlayer;
        private int _damageToRageCooldown;

        private void Awake()
        {
            Instance = this;
            _playerDeck = _playerCards.SelectMany(x => Enumerable.Repeat(x, 2)).ToList();
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
            var tmpDeck = new List<CardInfo>(FilterCards(_playerDeck));
            for (int i = 0; i < 3; i++)
            {
                var index = Random.Range(0, tmpDeck.Count);
                var go = Instantiate(_cardPrefab, _cardContainer);
                go.GetComponent<CardInstance>().Info = tmpDeck[index];
                tmpDeck.RemoveAt(index);
            }
        }

        public void SpawnAICard(CardInfo card)
        {
            var go = Instantiate(_cardPrefab, _cardAIPos);
            go.GetComponent<CardInstance>().Info = card;
            go.GetComponent<Button>().interactable = false;
        }

        string GetAttackText(int value)
        {
            if (_isNotAITurn)
            {
                var modValue = value * (1 + _rage / 10);
                if (_damageToRageCooldown > 0)
                    return $"Inflict <color=green>{modValue / 2}</color> damage and decrease rage by <color=green>{modValue / 2}</color>";
                else if (_rage > 0)
                    return $"Inflict <color=red>{modValue}</color> damage";
            }
            return $"Inflict {value} damage";
        }

        public string GetDescription(CardInfo card)
        {
            return string.Join("\n", card.Effects.Select(x => x.Type switch
            {
                ActionType.DAMAGE => 
                x.Value > 0 ? // _isNotAITurn have the opposite value here, uh
                    GetAttackText(x.Value) :
                    $"Take {(!_isNotAITurn && _noDrawbackCooldownPlayer > 0 ? "<color=grey>0</color>" : -x.Value)} damage",
                ActionType.RAGE => x.Value > 0 ?
                    $"Increase rage by {x.Value}" :
                    $"Decreate rage by {-x.Value}",
                ActionType.INTIMIDATE => $"Target gain a \"Useless Mumble\" card",
                ActionType.DESTROY_ON_DISCARD => "Destroyed when used",
                ActionType.CANT_ATTACK => $"Prevent target to play damage cards for {x.Value} turns",
                ActionType.MAX_HEALTH => $"Reduce target max health by {x.Value}",
                ActionType.NO_NEGATIVE_DAMAGE => $"Negative damage doesn't apply for the next {x.Value} turns",
                ActionType.DEFLECT_ON_RAGE => $"All damage taken for {x.Value} are halved and reduce the rage",
                ActionType.FORCE_ATTACK => $"Force target to play damage cards for {x.Value} turns",
                _ => throw new NotImplementedException()
            }));
        }

        public CardInfo[] FilterCards(IEnumerable<CardInfo> cards)
            => cards.Where(x =>
            {

                var targetCounter = _isNotAITurn ? _attackCooldownAI : _attackCooldownPlayer;
                if (targetCounter > 0)
                    return !x.Effects.Any(e => e.Type == ActionType.DAMAGE && e.Value > 0) && !x.Effects.Any(x => x.Type == ActionType.CANT_ATTACK);
                if (_attackForceCooldownAI > 0 && _isNotAITurn)
                    return x.Effects.Any(e => e.Type == ActionType.DAMAGE && e.Value > 0);
                return true;
            }).ToArray();

        public void DoAction(CardInfo card)
        {
            _isNotAITurn = !_isNotAITurn;
            foreach (var e in card.Effects)
            {
                if (_isNotAITurn && _noDrawbackCooldownPlayer > 0 && e.Type == ActionType.DAMAGE && e.Value < 0)
                    continue;
                switch (e.Type)
                {
                    case ActionType.DAMAGE:
                        var value = e.Value;
                        if (!_isNotAITurn)
                        {
                            value += 1 + _rage / 10;
                            if (_damageToRageCooldown > 0)
                            {
                                value /= 2;
                                _rage -= value;
                            }
                        }
                        else
                        {
                            value *= -1;
                        }
                        HealthManager.Instance.TakeDamage(value);
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
                        _playerDeck.Add(_uselessMumble);
                        break;

                    case ActionType.DESTROY_ON_DISCARD:
                        if (_isNotAITurn)
                        {
                            var index = _playerDeck.IndexOf(card);
                            _playerDeck.RemoveAt(index);
                        }
                        else AIManager.Instance.UseSpe();
                        break;

                    case ActionType.CANT_ATTACK:
                        if (_isNotAITurn) _attackCooldownAI = e.Value;
                        else _attackCooldownPlayer = e.Value;
                        break;

                    case ActionType.FORCE_ATTACK:
                        if (_isNotAITurn) _attackForceCooldownAI = e.Value;
                        else throw new NotImplementedException();
                        break;

                    case ActionType.MAX_HEALTH:
                        if (_isNotAITurn) HealthManager.Instance.ReduceAIMaxHealth(e.Value);
                        else throw new NotImplementedException();
                        break;

                    case ActionType.NO_NEGATIVE_DAMAGE:
                        if (_isNotAITurn) _noDrawbackCooldownPlayer = e.Value;
                        else throw new NotImplementedException();
                        break;

                    case ActionType.DEFLECT_ON_RAGE:
                        if (_isNotAITurn) _damageToRageCooldown = e.Value;
                        else throw new NotImplementedException();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            if (!HealthManager.Instance.HasLost)
            {
                DialogueManager.Instance.ShowText(_isNotAITurn ? string.Empty : "Divyansh", "BLUE", card.Sentence, () =>
                {
                    if (HealthManager.Instance.HasLost)
                    {
                        return;
                    }
                    RemoveCards();
                    if (_isNotAITurn)
                    {
                        if (_attackCooldownPlayer > 0)
                            _attackCooldownPlayer--;
                        AIManager.Instance.Play();
                    }
                    else
                    {
                        if (_attackCooldownAI > 0)
                            _attackCooldownAI--;
                        if (_attackForceCooldownAI > 0)
                            _attackForceCooldownAI--;
                        if (_noDrawbackCooldownPlayer > 0)
                            _noDrawbackCooldownPlayer--;
                        if (_damageToRageCooldown > 0)
                            _damageToRageCooldown--;
                        SpawnCards();
                    }
                });
            }
        }
    }
}
