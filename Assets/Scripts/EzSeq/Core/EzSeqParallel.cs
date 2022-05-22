
using System.Collections.Generic;

public class EzSeqParallel : IEzSeq
{
    private List<IEzSeq> _elements;

    public static IEzSeq Create(params IEzSeq[] elements)
    {
        EzSeqParallel self = new EzSeqParallel();

        self._elements.AddRange(elements);

        return self;
    }
    private EzSeqParallel()
    {
        _elements = new List<IEzSeq>();
    }

    public void Begin()
    {
        foreach (IEzSeq element in _elements)
        {
            element.Begin();
        }
    }

    public void Update(float deltaT)
    {
        foreach (IEzSeq element in _elements)
        {
            if (!element.Done())
            { 
                element.Update(deltaT);
            }
        }
    }

    public bool Done()
    {
        foreach (IEzSeq element in _elements)
        {
            if (!element.Done())
            {
                return false;
            }
        }

        return true;
    }
}
