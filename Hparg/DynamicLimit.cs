namespace Hparg
{
    internal class DynamicLimit
    {
        public DynamicLimit(float value, bool isDynamic)
             => (Value, IsDynamic) = (value, isDynamic);

        public float Value { set; get; }
        public bool IsDynamic { private init; get; }
    }
}
