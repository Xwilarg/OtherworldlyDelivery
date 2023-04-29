using UnityEngine;

namespace LudumDare53.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/CardInfo", fileName = "CardInfo")]
    public class CardInfo : ScriptableObject
    {
        public string Title;
        public string Sentence;
        public CardEffect[] Effects;
    }
}