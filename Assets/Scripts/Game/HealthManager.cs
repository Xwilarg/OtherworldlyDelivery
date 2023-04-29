using LudumDare53.Effect;
using UnityEngine;

namespace LudumDare53.Game
{
    public class HealthManager : MonoBehaviour
    {
        public static HealthManager Instance { get; private set; }

        [SerializeField]
        private RectTransform _cursor;

        private HealthCursor _cursorScript;

        private int _currDamage;

        private void Awake()
        {
            Instance = this;
            _cursorScript = _cursor.GetComponent<HealthCursor>();
        }

        public void TakeDamage(int amount)
        {
            _currDamage += amount;
            var pos = _currDamage / 200f;
            _cursorScript.MoveTo(pos);
        }
    }
}
