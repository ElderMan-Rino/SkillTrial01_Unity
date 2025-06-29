namespace Elder.Framework.Input.Domain.Values
{
    public readonly struct AxisInputData 
    {
        public readonly InputVector2 Move;
        public readonly InputVector2 Look;

        public AxisInputData(InputVector2 move, InputVector2 look)
        {
            Move = move;
            Look = look;
        }
    }
}