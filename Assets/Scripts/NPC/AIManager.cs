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
                    deck = deck.Where(x => x.Effects.Any(e => e.Type == ActionType.DAMAGE)).ToArray();
                }
            }
            var card = deck[Random.Range(0, deck.Length)];
            if (card.Effects.Any(x => x.Type == ActionType.DAMAGE || x.Type == ActionType.DAMAGE_LIMIT))
            {
                _waitBeforeAttack = 2;
            }
            else
            {
                _waitBeforeAttack = 0;
            }
            _lastCard = card.Title;
            CardsManager.Instance.SpawnAICard(card);
            CardsManager.Instance.DoAction(card);
        }
    }
}
