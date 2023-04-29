using UnityEngine;

namespace LudumDare53.Effect
{
    public class HealthCursor : MonoBehaviour
    {
        private float _baseX;
        private float _destX;
        private float _timer;

        private RectTransform _rTransform;

        [SerializeField]
        private RectTransform _healthBar;

        private void Awake()
        {
            _rTransform = (RectTransform)transform;
        }

        public void MoveTo(float x)
        {
            _baseX = _destX;
            _destX = x;
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime * 10f;
            _rTransform.anchoredPosition = new(Mathf.Lerp(_baseX, _destX, Mathf.Clamp01(_timer)) * _healthBar.rect.width, _rTransform.anchoredPosition.y);
        }
    }
}
