using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideButton : MonoBehaviour
{
    public delegate void ButtonCallback(Vector3Int direction);

    public ButtonCallback onClick;
    public ButtonCallback onHover;
    public ButtonCallback onLeave;
    public Vector3Int direction;
    public TMPro.TMP_Text text;
    public Transform icon;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseUpAsButton()
    {
        onClick(direction);
    }

    private void OnMouseEnter()
    {
        onHover(direction);
    }

    private void OnMouseExit()
    {
        onLeave(direction);
    }
}
