using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    IEzSeq testSeq;

    // Start is called before the first frame update
    void Start()
    {
        testSeq = EzSequence.Create(EzSeqDelay.Create(2.0f), EzSeqLerpPosition.CreateRelative(transform, Vector3.right * 5.0f, 2.0f), EzSeqLerpColor.Create(GetComponent<SpriteRenderer>(), Color.clear, 3.0f));
    }

    // Update is called once per frame
    void Update()
    {
        if (!testSeq.Done())
        {
            testSeq.Update(Time.deltaTime);
        }
    }
}
