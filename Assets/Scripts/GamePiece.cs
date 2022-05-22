using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(EzHexResident))]
public class GamePiece : MonoBehaviour
{
    public int level;
    public GUISkin skin;
    public TMPro.TMP_Text text;
    public EzHexResident resident;
    public Color c0;
    public Color c1;
    public SpriteRenderer colorSprite;

    private void Awake()
    {
        resident = GetComponent<EzHexResident>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        text.text = (1 << level).ToString();

        colorSprite.color = Color.Lerp(c0, c1, (float)level / 11.0f);
    }


}
