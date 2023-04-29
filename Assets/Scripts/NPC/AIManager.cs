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
            CardsManager.Instance.DoAction(_cards[Random.Range(0, _cards.Length)]);
        }
    }
}
