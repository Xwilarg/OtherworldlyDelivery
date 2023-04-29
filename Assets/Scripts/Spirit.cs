using UnityEngine;

namespace LudumDare53
{
    public class Spirit : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] _sprites;

        private SpriteRenderer _sr;

        private const float _refTimer = .25f;
        private float _timer;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _timer = _refTimer;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer < 0f)
            {
                _timer = _refTimer;
                _sr.sprite = _sprites[Random.Range(1, _sprites.Length)];
            }
        }
    }
}
