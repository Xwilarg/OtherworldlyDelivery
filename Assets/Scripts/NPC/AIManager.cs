using LudumDare53.Card;
using LudumDare53.Game;
using LudumDare53.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LudumDare53.NPC
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance { get; private set; }

        [SerializeField]
        private CardInfo[] _cards;
        private List<CardInfo> _deck;

        [SerializeField]
        private CardInfo _ult;

        private string _lastCard;

        private bool _canUseUlt = true;

        private int _waitBeforeAttack = 2;
        private int _waitBeforeRage = 1;

        private int _rageLowerLimit = 10;

        private void Awake()
        {
            Instance = this;
            _deck = _cards.ToList();
        }

        public void Play()
        {
            CardInfo[] deck;
            if (_canUseUlt && HealthManager.Instance.IsAILoosing && CardsManager.Instance.CanAIPlayAttack)
            {
                _canUseUlt = false;
                deck = new[] { _ult };
            }
            else
            {
                deck = CardsManager.Instance.FilterCards(_deck.Where(x => x.Title != _lastCard));
                if (_waitBeforeAttack <= 0 && deck.Any(x => x.Effects.Any(e => e.Type == ActionType.DAMAGE))) // Force enemy to play a damage card at least once every 3 turns
                {
#if UNITY_EDITOR
                    Debug.Log("[AI] Force use of attack card");
#endif
                    deck = deck.Where(x => x.Effects.Any(e => e.Type == ActionType.DAMAGE)).ToArray();
                }
                else if (CardsManager.Instance.Rage <= _rageLowerLimit && _waitBeforeRage <= 0 && deck.Any(x => x.Effects.Any(e => e.Type == ActionType.RAGE && e.Value > 0))) // Force enemy to play a rage cards
                {
#if UNITY_EDITOR
                    Debug.Log("[AI] Force use of rage card");
#endif
                    deck = deck.Where(x => x.Effects.Any(e => e.Type == ActionType.RAGE && e.Value > 0)).ToArray();
                }
            }
            var card = deck[Random.Range(0, deck.Length)];
            if (card.Effects.Any(x => x.Type == ActionType.DAMAGE || x.Type == ActionType.DAMAGE_LIMIT))
            {
                _waitBeforeAttack = 2;
            }
            else
            {
                _waitBeforeAttack--;
            }
            if (card.Effects.Any(x => x.Type == ActionType.RAGE && x.Value > 0))
            {
                _waitBeforeAttack = 1;
            }
            else if (CardsManager.Instance.Rage <= _rageLowerLimit)
            {
                _waitBeforeAttack--;
            }
            _lastCard = card.Title;
            CardsManager.Instance.SpawnAICard(card);
            CardsManager.Instance.DoAction(card);
        }
    }
}
