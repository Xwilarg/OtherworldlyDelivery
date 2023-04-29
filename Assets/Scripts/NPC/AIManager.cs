using LudumDare53.Card;
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

        private int _turnBeforeUlt = 10;
        private bool _canUseUlt;

        private void Awake()
        {
            Instance = this;
            _deck = _cards.ToList();
        }

        public void UseSpe()
        {
            _canUseUlt = false;
        }

        public void NextTurn()
        {
            if (_turnBeforeUlt == 0)
                return;
            _turnBeforeUlt--;
            if (_turnBeforeUlt == 0)
                _canUseUlt = true;
        }

        public void Play()
        {
            var deck = CardsManager.Instance.FilterCards(_deck).Where(x => _canUseUlt || !x.Effects.Any(e => e.Type == ActionType.DESTROY_ON_DISCARD)).ToArray();
            var card = deck[Random.Range(0, deck.Length)];
            CardsManager.Instance.SpawnAICard(card);
            CardsManager.Instance.DoAction(card);
        }
    }
}
