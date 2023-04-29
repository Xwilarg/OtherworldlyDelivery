using UnityEngine;

namespace LudumDare53.Game
{
    public class HealthManager : MonoBehaviour
    {
        public static HealthManager Instance { get; private set; }

        [SerializeField]
        private RectTransform _cursor;

        [SerializeField]
        private RectTransform _healthBar;

        private int _currDamage;

        private void Awake()
        {
            Instance = this;
        }

        public void TakeDamage(int amount)
        {
            _currDamage += amount;
            var pos = _currDamage * _healthBar.rect.width / 200f;
            _cursor.anchoredPosition = new(pos, _cursor.anchoredPosition.y);
        }
    }
}
