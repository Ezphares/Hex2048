
public class EzSeqDelay : IEzSeq
{
    private float _t;

    public static IEzSeq Create(float time)
    {
        return new EzSeqDelay() { _t = time };
    }

    private EzSeqDelay()
    {
    }

    public void Begin()
    {
    }

    public void Update(float deltaT)
    {
        _t -= deltaT;
    }

    public bool Done()
    {
        return _t <= 0.0f;
    }
}
