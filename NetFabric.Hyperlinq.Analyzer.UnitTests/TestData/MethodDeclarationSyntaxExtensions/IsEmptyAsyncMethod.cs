#pragma warning disable IDE0022 // Use expression body for methods
#pragma warning disable CS0219 // Variable is assigned but its value is never used

using System;
using System.Threading.Tasks;

class IsEmptyAsyncMethod
{
    public ValueTask EmptyArrowDefaultAsync() => default;

    public ValueTask EmptyArrowNewAsync() => new ValueTask();

    public ValueTask EmptyArrowThrowAsync() => throw new NotImplementedException();

    public ValueTask EmptyBlockDefaultAsync()
    {
        // comment
        return default;
    }

    public ValueTask EmptyBlockNewAsync()
    {
        // comment
        return new ValueTask();
    }

    public ValueTask EmptyBlockThrowAsync()
    {
        // comment
        throw new NotImplementedException();
    }

    public ValueTask NotEmptyBlockDefaultAsync()
    {
        var _ = 0;
        return default;
    }

    public ValueTask NotEmptyBlockNewAsync()
    {
        var _ = 0;
        return new ValueTask();
    }

    public ValueTask NotEmptyBlockThrowAsync()
    {
        var _ = 0;
        throw new NotImplementedException();
    }
}

#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning restore IDE0022 // Use expression body for methods
