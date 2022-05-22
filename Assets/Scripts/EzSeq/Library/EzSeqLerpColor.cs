using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzSeqLerpColor : IEzSeq
{
    private SpriteRenderer _renderer;
    private Color _target;
    private float _time;

    private float _t;
    private Color _source;


    public static IEzSeq Create(SpriteRenderer renderer, Color color, float time)
    {
        return new EzSeqLerpColor() { _renderer = renderer, _target = color, _time = time};
    }

    private EzSeqLerpColor()
    {
}

    public void Begin()
    {
        _t = 0.0f;
        _source = _renderer.color;
    }

    public void Update(float deltaT)
    {
        _t = Mathf.Min(_t + deltaT, _time);

        _renderer.color = Color.Lerp(_source, _target, _t / _time);
    }

    public bool Done()
    {
        return _t >= _time;
    }
}
