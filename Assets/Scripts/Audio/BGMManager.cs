using UnityEngine;

namespace LudumDare53.Audio
{
    public class BGMManager : MonoBehaviour
    {
        public static BGMManager Instance { private set; get; }

        private AudioSource _bgm;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _bgm = GetComponent<AudioSource>();
            }
            else if (Instance.GetInstanceID() != Instance.GetInstanceID())
            {
                Destroy(gameObject);
            }
        }

        public void SetBGM(AudioClip clip)
        {
            var pos = _bgm.time;
            _bgm.clip = clip;
            _bgm.Play();
            _bgm.time = pos;
        }
    }
}
