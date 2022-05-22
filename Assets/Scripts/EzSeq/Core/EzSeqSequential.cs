
using System.Collections.Generic;

public class EzSequence : IEzSeq
{
    private int _current;
    private List<IEzSeq> _elements;

    public static IEzSeq Create(params IEzSeq[] elements)
    {
        EzSequence self = new EzSequence();

        self._elements.AddRange(elements);

        return self;
    }

    private EzSequence()
    {
        _elements = new List<IEzSeq>();
        _current = 0;
    }

    public void Begin()
    {
        if (_elements.Count > 0)
        {
            _elements[0].Begin();
        }
    }

    public void Update(float deltaT)
    {
        while (_current < _elements.Count)
        {
            if (_elements[_current].Done())
            {
                if (++_current < _elements.Count)
                {
                    _elements[_current].Begin();
                }
                continue;
            }

            _elements[_current].Update(deltaT);
            break;
        }
    }

    public bool Done()
    {
        return _current >= _elements.Count;
    }
}
