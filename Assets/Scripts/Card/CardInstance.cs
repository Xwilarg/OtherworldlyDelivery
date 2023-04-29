using LudumDare53.Game;
using UnityEngine;

namespace LudumDare53.Card
{
    public class CardInstance : MonoBehaviour
    {
        public void OnClick()
        {
            HealthManager.Instance.TakeDamage(50);
        }
    }
}
