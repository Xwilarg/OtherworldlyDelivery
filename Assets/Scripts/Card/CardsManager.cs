using LudumDare53.Dialogue;
using LudumDare53.Game;
using LudumDare53.NPC;
using LudumDare53.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [SerializeField]
        private TMP_Text _debuffDisplayPlayer, _debuffDisplayAI;

        private int _rage;
        public int Rage => _rage;

        private bool _isNotAITurn;

        private readonly Dictionary<bool, Dictionary<ActionType, Debuff>> _debuffs = new();

        private int _rageReduction = 7;

        public int TurnCount { set; get; }

        private void Awake()
        {
            Instance = this;
            _debuffs.Add(true, new());
            _debuffs.Add(false, new());
            _playerDeck = _playerCards.SelectMany(x => Enumerable.Repeat(x, 2)).ToList();
        }

        public bool CanAIPlayAttack => !GetDebuff(false, ActionType.CANT_ATTACK);

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
            TurnCount++;
            var tmpDeck = new List<CardInfo>(FilterCards(_playerDeck));
            for (int i = 0; i < 3; i++)
            {
                var index = Random.Range(0, tmpDeck.Count);
                var go = Instantiate(_cardPrefab, _cardContainer);
                go.GetComponent<CardInstance>().Info = tmpDeck[index];
                tmpDeck.RemoveAll(x => x.Title == tmpDeck[index].Title);
            }
        }

        public void SpawnAICard(CardInfo card)
        {
            var go = Instantiate(_cardPrefab, _cardAIPos);
            go.GetComponent<CardInstance>().Info = card;
            go.GetComponent<Button>().interactable = false;
        }

        string GetAttackText(int value, bool limit)
        {
            int modValue = value;
            if (_isNotAITurn)
            {
                modValue *= (1 + _rage / _rageReduction);
                if (GetDebuff(false, ActionType.DEFLECT_ON_RAGE))
                {
                    modValue /= 2;
                    if (limit && modValue > 100) modValue = 100;
                    return $"Inflict <color=green>{modValue}</color> damage and decrease rage by <color=green>{modValue}</color>";
                }
                else if (_rage > 0)
                {
                    if (limit && modValue > 100) modValue = 100;
                    return $"Inflict <color=red>{modValue}</color> damage";
                }
            }
            else
            {
                if (GetDebuff(true, ActionType.DAMAGE_BOOST))
                {
                    modValue *= 2;
                    if (limit && modValue > 100) modValue = 100;
                    return $"Inflict <color=green>{modValue}</color> damage";
                }
            }
            modValue = value;
            if (limit && modValue > 100) modValue = 100;
            return $"Inflict {modValue} damage";
        }

        public string GetDescription(CardInfo card)
        {
            return string.Join("\n", card.Effects.Select(x => x.Type switch
            {
                ActionType.DAMAGE =>
                x.Value > 0 ? // _isNotAITurn have the opposite value here, uh
                    GetAttackText(x.Value, false) :
                    $"Take {(!_isNotAITurn && GetDebuff(true, ActionType.NO_NEGATIVE_DAMAGE) ? "<color=grey>0</color>" : -x.Value)} damage",
                ActionType.DAMAGE_LIMIT => GetAttackText(x.Value, true),
                ActionType.RAGE => x.Value > 0 ?
                    $"Increase rage by {x.Value}" :
                    $"Decrease rage by {-x.Value}",
                ActionType.INTIMIDATE => $"Target gain a \"Useless Mumble\" card",
                ActionType.DESTROY_ON_DISCARD => "Destroyed when used",
                ActionType.CANT_ATTACK => $"Prevent target to play damage cards for {x.Value} turns",
                ActionType.MAX_HEALTH => $"Reduce target max health by {x.Value}",
                ActionType.NO_NEGATIVE_DAMAGE => $"Negative damage doesn't apply for the next {x.Value} turns",
                ActionType.DEFLECT_ON_RAGE => $"All damage taken for {x.Value} turns are halved and reduce the rage",
                ActionType.FORCE_ATTACK => $"Force target to play damage cards for {x.Value} turns",
                ActionType.DAMAGE_BOOST => $"All your attacks does twice the amount of damage for {x.Value} turns",
                _ => throw new NotImplementedException()
            }));
        }

        public CardInfo[] FilterCards(IEnumerable<CardInfo> cards)
        {
            return cards.Where(x =>
            {
                if (GetDebuff(!_isNotAITurn, ActionType.CANT_ATTACK))
                    return !x.Effects.Any(e => e.Type == ActionType.DAMAGE && e.Value > 0) && (!GetDebuff(_isNotAITurn, ActionType.CANT_ATTACK) || !x.Effects.Any(e => e.Type == ActionType.FORCE_ATTACK));
                if (GetDebuff(false, ActionType.FORCE_ATTACK) && _isNotAITurn)
                    return x.Effects.Any(e => e.Type == ActionType.DAMAGE && e.Value > 0);
                if (GetDebuff(_isNotAITurn, ActionType.CANT_ATTACK)) // If enemy can't attack, don't let player use a card that force him to attack
                    return !x.Effects.Any(e => e.Type == ActionType.FORCE_ATTACK);
                return true;
            }).ToArray();
        }

        private void AddDebuff(bool isPlayer, ActionType action, int value)
        {
            var targetD = _debuffs[isPlayer];
            if (targetD.ContainsKey(action))
            {
                targetD[action].Value = value;
            }
            else
            {
                targetD.Add(action, new() { Value = value });
            }
            if (_isNotAITurn && isPlayer)
            {
                targetD[action].BypassFirst = true;
            }
            UpdateUI();
        }

        public void DoAction(CardInfo card)
        {
            _isNotAITurn = !_isNotAITurn;
            foreach (var e in card.Effects)
            {
                if (_isNotAITurn && GetDebuff(true, ActionType.NO_NEGATIVE_DAMAGE) && e.Type == ActionType.DAMAGE && e.Value < 0)
                    continue;
                switch (e.Type)
                {
                    case ActionType.DAMAGE_LIMIT:
                    case ActionType.DAMAGE:
                        var value = e.Value;
                        if (!_isNotAITurn)
                        {
                            value *= 1 + _rage / _rageReduction;
                            if (GetDebuff(false, ActionType.DEFLECT_ON_RAGE))
                            {
                                value /= 2;
                                _rage -= value;
                                if (_rage < 0)
                                {
                                    _rage = 0;
                                }
                            }
                            if (value > 100 && e.Type == ActionType.DAMAGE_LIMIT)
                            {
                                value = 100;
                            }
                        }
                        else
                        {
                            value *= -1;
                            if (GetDebuff(true, ActionType.DAMAGE_BOOST) && value < 0)
                            {
                                value *= 2;
                            }
                        }
#if UNITY_EDITOR
                        if (!_isNotAITurn)
                        {
                            Debug.Log($"[AI] Attacking with {value} damage ({e.Value} with a rage of {_rage})");
                        }
#endif
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
                        else { } // Done automatically in AIManager
                        break;

                    case ActionType.CANT_ATTACK: // Debuff that will be applied to the player need to have their counter increased by 1 because of how the debuff system is handled
                        AddDebuff(!_isNotAITurn, e.Type, e.Value);
                        break;

                    case ActionType.FORCE_ATTACK:
                        if (_isNotAITurn) AddDebuff(false, e.Type, e.Value);
                        else throw new NotImplementedException();
                        break;

                    case ActionType.MAX_HEALTH:
                        if (_isNotAITurn) HealthManager.Instance.ReduceAIMaxHealth(e.Value);
                        else throw new NotImplementedException();
                        break;

                    case ActionType.NO_NEGATIVE_DAMAGE:
                        if (_isNotAITurn) AddDebuff(true, e.Type, e.Value);
                        else throw new NotImplementedException();
                        break;

                    case ActionType.DEFLECT_ON_RAGE:
                        if (_isNotAITurn) AddDebuff(false, e.Type, e.Value);
                        else throw new NotImplementedException();
                        break;

                    case ActionType.DAMAGE_BOOST:
                        if (_isNotAITurn) AddDebuff(true, e.Type, e.Value);
                        else throw new NotImplementedException();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            HealthManager.Instance.ApplyDamages();
            if (_isNotAITurn)
            {
                foreach (var d2 in _debuffs[true])
                {
                    if (d2.Value.BypassFirst)
                    {
                        d2.Value.BypassFirst = false;
                        continue;
                    }
                    if (d2.Value.Value > 0)
                    {
                        d2.Value.Value--;
                    }
                }
                UpdateUI();
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
                        AIManager.Instance.Play();
                    }
                    else
                    {
                        foreach (var d2 in _debuffs[false])
                        {
                            if (d2.Value.Value > 0)
                            {
                                d2.Value.Value--;
                            }
                        }
                        UpdateUI();
                        SpawnCards();
                    }
                });
            }
        }

        private bool GetDebuff(bool isPlayer, ActionType type)
            => _debuffs[isPlayer].ContainsKey(type) ? _debuffs[isPlayer][type].Value > 0 : false;

        private string DebuffKeyToString(ActionType type, int value)
        {
            return type switch
            {
                ActionType.CANT_ATTACK => $"No Attack: {value}",
                ActionType.FORCE_ATTACK => $"Force Attack: {value}",
                ActionType.NO_NEGATIVE_DAMAGE => $"No Negative Damage: {value}",
                ActionType.DEFLECT_ON_RAGE => $"Damage Halved: {value}\nDamage Reduce Rage: {value}",
                ActionType.DAMAGE_BOOST => $"Damage Doubled: {value}",
                _ => throw new NotImplementedException()
            };
        }

        private void UpdateUI()
        {
            StringBuilder str = new();
            foreach (var d2 in _debuffs[true])
            {
                if (GetDebuff(true, d2.Key))
                {
                    str.AppendLine(DebuffKeyToString(d2.Key, d2.Value.Value));
                }
            }
            _debuffDisplayPlayer.text = str.ToString();
            str = new();
            foreach (var d2 in _debuffs[false])
            {
                if (GetDebuff(false, d2.Key))
                {
                    str.AppendLine(DebuffKeyToString(d2.Key, d2.Value.Value));
                }
            }
            _debuffDisplayAI.text = str.ToString();
        }
    }
}
