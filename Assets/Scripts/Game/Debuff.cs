namespace LudumDare53.Game
{
    public class Debuff
    {
        private int _value;
        public bool BypassFirst;
        public int Value
        {
            set
            {
                _value = value;
            }
            get => _value;
        }
    }
}
