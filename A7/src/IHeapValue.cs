using System;

namespace A9
{
    public interface IHeapValue<T>
    {
        int ComparePriority(T other);
    }
}