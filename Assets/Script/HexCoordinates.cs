// HexCoordinates.cs
// Basic hex math

using UnityEngine;

public static class HexCoordinates
{
    public static Vector3Int OffsetToCube(int col, int row)
    {
        int x = col - (row - (row & 1)) / 2;
        int z = row;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    public static Vector2Int CubeToOffset(Vector3Int cube)
    {
        int col = cube.x + (cube.z - (cube.z & 1)) / 2;
        int row = cube.z;
        return new Vector2Int(col, row);
    }
}