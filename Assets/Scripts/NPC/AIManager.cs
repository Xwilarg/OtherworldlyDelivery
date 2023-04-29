using LudumDare53.Card;
using LudumDare53.SO;
using UnityEngine;

namespace LudumDare53.NPC
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance { get; private set; }

        [SerializeField]
        private CardInfo[] _cards;

        private void Awake()
        {
            Instance = this;
        }

        public void Play()
        {
            var deck = CardsManager.Instance.FilterCards(_cards);
            var card = deck[Random.Range(0, deck.Length)];
            CardsManager.Instance.SpawnAICard(card);
            CardsManager.Instance.DoAction(card);
        }
    }
}
