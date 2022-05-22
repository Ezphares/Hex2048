using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzSeqLerpPosition : IEzSeq
{
    private Transform _transform;
    private Vector3 _target;
    private float _time;
    private bool _relative;

    private float _t;
    private Vector3 _offset;


    public static IEzSeq CreateRelative(Transform transform, Vector3 movement, float time)
    {
        return new EzSeqLerpPosition() { _transform = transform, _target = movement, _time = time, _relative = true };
    }

    private EzSeqLerpPosition()
    { }

    public void Begin()
    {
        _t = _time;
        _offset = _relative ? (_target) : (_target - _transform.localPosition);
    }

    public void Update(float deltaT)
    {
        float dt = Mathf.Min(deltaT, _t);
        _t -= dt;

        float step = dt / _time;

        _transform.localPosition += _offset * step;
    }

    public bool Done()
    {
        return _t < float.Epsilon;
    }
}
