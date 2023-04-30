﻿using LudumDare53.Card;
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

        private bool _canUseUlt = true;

        private void Awake()
        {
            Instance = this;
            _deck = _cards.ToList();
        }

        public void Play()
        {
            CardInfo[] deck;
            if (_canUseUlt && HealthManager.Instance.IsAILoosing)
            {
                _canUseUlt = false;
                deck = new[] { _ult };
            }
            else
            {
                deck = CardsManager.Instance.FilterCards(_deck);
            }
            var card = deck[Random.Range(0, deck.Length)];
            CardsManager.Instance.SpawnAICard(card);
            CardsManager.Instance.DoAction(card);
        }
    }
}
