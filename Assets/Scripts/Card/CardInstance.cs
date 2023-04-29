using LudumDare53.Game;
using LudumDare53.SO;
using UnityEngine;

namespace LudumDare53.Card
{
    public class CardInstance : MonoBehaviour
    {
        public CardInfo Info { private get; set; }

        public void OnClick()
        {
            HealthManager.Instance.TakeDamage(Info.Damage);
            CardsManager.Instance.SpawnCards();
        }
    }
}
