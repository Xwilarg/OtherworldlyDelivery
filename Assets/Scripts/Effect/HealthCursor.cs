using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace LudumDare53.Effect
{
    public class HealthCursor : MonoBehaviour
    {
        private float _baseX;
        private float _destX;
        private float _timer;

        private RectTransform _rTransform;

        private void Awake()
        {
            _rTransform = (RectTransform)transform;
        }

        public void MoveTo(float x)
        {
            _baseX = _rTransform.anchoredPosition.x;
            _destX = x;
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime * 10f;
            _rTransform.anchoredPosition = new(Mathf.Lerp(_baseX, _destX, Mathf.Clamp01(_timer)), _rTransform.anchoredPosition.y);
        }
    }
}
