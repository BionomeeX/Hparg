namespace Hparg.Drawable
{
    internal enum Zone
    {
        LeftMargin = -4,
        RightMargin,

        UpperMarginFull,
        LowerMarginFull,

        // Sub zones, each zone is an increment of 3 starting at 0
        UpperMargin,
        Main,
        LowerMargin
    }
}
