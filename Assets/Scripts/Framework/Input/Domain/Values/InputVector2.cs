namespace Elder.Framework.Input.Domain.Values
{
    public readonly struct InputVector2 
    {
        public readonly float X;
        public readonly float Y;

        public InputVector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        
        public static InputVector2 Zero = new InputVector2(0, 0);
    }
}