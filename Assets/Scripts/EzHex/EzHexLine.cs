
using UnityEngine;

public class EzHexLine
{
    public Vector3Int start, direction;
    public int length;


    public Vector3Int GetTile(int i)
    {
        return start + direction * i;
    }

    public bool Contains(Vector3Int position)
    {
        for (int i = 0; i < length; ++i)
        {
            if (start + direction * i == position) return true;
        }

        return false;
    }

}
