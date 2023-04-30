using System.Collections;
using UnityEngine;

namespace LudumDare53.Minigame
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private float _min, _max;

        [SerializeField]
        private float _speed;

        [SerializeField]
        private bool _goRight;

        [SerializeField]
        private GameObject _door;

        [SerializeField]
        private int _score;

        private void Awake()
        {
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            yield return new WaitForSeconds(Random.Range(_min, _max));
            var go = Instantiate(_door, transform.position, Quaternion.identity);
            go.GetComponent<DoorController>().Init(_goRight, _speed, () =>
            {
                ScoreManager.Instance.IncreaseScore(_score);
            });
            yield return Spawn();
        }
    }
}
