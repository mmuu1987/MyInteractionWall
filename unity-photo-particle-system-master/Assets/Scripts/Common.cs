using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Common  {
    public static float GetCross(Vector2 p1, Vector2 p2, Vector2 p)
    {
        return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
    }
    //计算一个点是否在矩形里  2d
  public  static bool ContainsQuadrangle(Vector2 leftDownP2, Vector2 leftUpP1, Vector2 rightDownP3, Vector2 rightUpP4, Vector2 p)
    {

        float value1 = GetCross(leftUpP1, leftDownP2, p);

        float value2 = GetCross(rightDownP3, rightUpP4, p);

        if (value1 * value2 < 0) return false;

        float value3 = GetCross(leftDownP2, rightDownP3, p);

        float value4 = GetCross(rightUpP4, leftUpP1, p);

        if (value3 * value4 < 0) return false;

        return true;
    }

}
