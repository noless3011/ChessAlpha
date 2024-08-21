using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntUtilities
{
    public static bool IsAlign(Vector2Int v1, Vector2Int v2)
    {
        if(AreFloatsEqual((float)v1.x / (float)v2.x , (float)v1.y / (float)v2.y))
        {
            return true;
        }
        return false;
    }
    static bool AreFloatsEqual(float a, float b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }
}
