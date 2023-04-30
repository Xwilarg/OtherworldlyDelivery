using System;
using UnityEngine;

namespace LudumDare53.Minigame
{
    public class DoorController : MonoBehaviour
    {
        private bool _goRight;
        private Action _onDestroy;
        public void Init(bool goRight, float speed, Action onDestroy)
        {
            _onDestroy = onDestroy;
            GetComponent<Rigidbody2D>().velocity = Vector2.right * (goRight ? 1 : -1) * speed;
            _goRight = goRight;
        }

        private void FixedUpdate()
        {
            if ((_goRight && transform.position.x > 12) || (!_goRight && transform.position.x < -12))
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Mag"))
            {
                _onDestroy?.Invoke();
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
