using LudumDare53.Game;
using LudumDare53.SO;
using TMPro;
using UnityEngine;

namespace LudumDare53.Card
{
    public class CardInstance : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _title, _content;

        private CardInfo _info;
        public CardInfo Info
        {
            set
            {
                _info = value;
                _title.text = value.Title;
                _content.text = CardsManager.Instance.GetDescription(value);
            }
            get => _info;
        }

        public void OnClick()
        {
            if (HealthManager.Instance.HasLost)
            {
                return;
            }
            CardsManager.Instance.RemoveCards();
            CardsManager.Instance.DoAction(Info);
        }
    }
}
