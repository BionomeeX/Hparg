namespace Hparg
{
    internal class DynamicBoundary
    {
        public DynamicBoundary(float value, bool isDynamic)
             => (Value, IsDynamic) = (value, isDynamic);

        public float Value { set; get; }
        public bool IsDynamic { private init; get; }
    }
}
