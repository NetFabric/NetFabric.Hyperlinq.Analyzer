#pragma warning disable IDE0022 // Use expression body for methods
#pragma warning disable CS0219 // Variable is assigned but its value is never used

using System;

class IsEmptyMethod
{
    public void EmptyArrowThrow() => throw new NotImplementedException();

    public void EmptyBlock()
    {
        // comment
    }

    public void EmptyBlockThrow()
    {
        // comment
        throw new NotImplementedException();
    }

    public int NotEmptyBlock()
    {
        return default;
    }

    public void NotEmptyBlockThrow()
    {
        var _ = 0;
        throw new NotImplementedException();
    }
}

#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning restore IDE0022 // Use expression body for methods
