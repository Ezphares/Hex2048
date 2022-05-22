using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EzHexMap))]
public class EzHexMapHexagonal : MonoBehaviour
{
    public EzHexMap map;
    // Start is called before the first frame update

    public int radius = 1;

    private void Awake()
    {
        map = GetComponent<EzHexMap>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<EzHexResident> Fill(EzHexResident prefab)
    {
        List<EzHexResident> tiles = new List<EzHexResident>();

        for (int i = 0; i < radius; ++i)
        {
            Vector3Int pos = new Vector3Int(0, -i, i);

            if (i == 0)
            {
                EzHexResident obj = Instantiate(prefab, map.CellToLocal(pos), Quaternion.identity, transform);
                obj.position = pos;
                tiles.Add(obj);
            }

            for (int j = 0; j < 6; ++j)
            {
                for (int k = 0; k < i; ++k)
                {
                    pos += EzHexMap.directions[j];
                    EzHexResident obj = Instantiate(prefab, map.CellToLocal(pos), Quaternion.identity, transform);
                    obj.position = pos;
                    tiles.Add(obj);

                }
            }
        }

        return tiles;
    }
}
