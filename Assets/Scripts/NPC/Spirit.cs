using UnityEngine;

namespace LudumDare53.NPC
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

        public void ChangeSprite()
        {
            _timer = _refTimer;
            _sr.sprite = _sprites[Random.Range(0, _sprites.Length)];
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer < 0f)
            {
                ChangeSprite();
            }
        }
    }
}
