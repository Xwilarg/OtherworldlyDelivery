using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LudumDare53.Minigame
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField]
        private GameObject _prefab;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private float _reloadSpeed;

        private Rigidbody2D _current;

        private void Awake()
        {
            Spawn();
        }

        private void Spawn()
        {
            _current = Instantiate(_prefab, transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
            _current.angularVelocity = 200f;
        }

        private IEnumerator Reload()
        {
            yield return new WaitForSeconds(_reloadSpeed);
            Spawn();
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && _current != null && !ScoreManager.Instance.DidGameEnded)
            {
                _current.velocity = Vector2.up * _speed;
                _current.angularVelocity = 1000f;
                _current = null;
                StartCoroutine(Reload());
            }
        }
    }
}
