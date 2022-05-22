using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzHexMap : MonoBehaviour
{
    public static Vector3Int right { get; } = Vector3Int.right + Vector3Int.back;
    public static Vector3Int upLeft { get; } = Vector3Int.up + Vector3Int.left;
    public static Vector3Int downLeft { get; } = Vector3Int.down + Vector3Int.forward;
    public static Vector3Int left { get; } = -right;
    public static Vector3Int upRight { get; } = -downLeft;
    public static Vector3Int downRight { get; } = -upLeft;

    public static Vector3Int[] directions = { right, upRight, upLeft, left, downLeft, downRight };

    public static Quaternion[] orientations = {
        Quaternion.identity,
        Quaternion.AngleAxis( 60, Vector3.forward) ,
        Quaternion.AngleAxis( 120, Vector3.forward) ,
        Quaternion.AngleAxis( 180, Vector3.forward) ,
        Quaternion.AngleAxis( 240, Vector3.forward) ,
        Quaternion.AngleAxis( 300, Vector3.forward) };

    private static Vector3 _Y = Vector3.up * 0.5f;
    private static Vector3 _X = new Vector3(Mathf.Sin(Mathf.Deg2Rad * 60.0f), -0.5f, 0.0f) * 0.5f;
    private static Vector3 _Z = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * 60.0f), -0.5f, 0.0f) * 0.5f;

    public static Vector3Int RotateClockwise(Vector3Int direction)
    {
        for (int i = 0; i < 6; ++i)
        {
            if (direction == directions[i])
            {
                return i > 0 ? directions[i - 1] : directions[5];
            }

        }
        return right;
    }

    public static Vector3Int RotateCounterClockwise(Vector3Int direction)
    {
        for (int i = 0; i < 6; ++i)
        {
            if (direction == directions[i])
            {
                return i < 5 ? directions[i + 1] : directions[0];
            }

        }
        return right;
    }

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Vector3 CellToLocal(Vector3Int cell)
    {
        return _X * cell.x + _Y * cell.y + _Z * cell.z;
    }

    public Vector3Int LocalToCell(Vector3 local)
    {
        int z = Mathf.RoundToInt(Vector3.Dot(local, _Z) / (Vector3.Dot(_Z, _Z)) * 0.75f);
        int x = Mathf.RoundToInt(Vector3.Dot(local, _X) / (Vector3.Dot(_X, _X)) * 0.75f);

        return new Vector3Int(x, -x - z, z);
    }
}
