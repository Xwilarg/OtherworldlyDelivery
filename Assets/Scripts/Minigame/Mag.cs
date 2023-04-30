using UnityEngine;

namespace LudumDare53.Minigame
{
    public class Mag : MonoBehaviour
    {
        private void Update()
        {
            if (transform.position.y > 6f)
            {
                ScoreManager.Instance.IncreaseScore(-1);
                Destroy(gameObject);
            }
        }
    }
}
