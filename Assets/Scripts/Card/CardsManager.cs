using LudumDare53.SO;
using TMPro;
using UnityEngine;

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
        private CardInfo[] _cards;

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
        }

        public void SpawnCards()
        {
            for (int i = 0; i < 3; i++)
            {
                var go = Instantiate(_cardPrefab, _cardContainer);
                go.GetComponent<CardInstance>().Info = _cards[Random.Range(0, _cards.Length)];
            }
        }
    }
}
