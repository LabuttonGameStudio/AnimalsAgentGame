using UnityEngine;

public static class MathFExtras 
{
    public static Vector3 NormalizeOnlyXAndZVector3(Vector3 vector)
    {
        Vector2 normalizedVector = new Vector2(vector.x, vector.z);
        normalizedVector.Normalize();
        return new Vector3(normalizedVector.x, vector.y, normalizedVector.y);
    }
}
