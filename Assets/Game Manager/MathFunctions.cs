using UnityEngine;
using System.Collections;

namespace MathFunctions
{
    public class MathFunctions
    {
        public MathFunctions(){}
        public float Angle(Vector3 from, Vector3 to, Vector3 axis)
        {
            axis = axis.normalized;
            if (Vector3.Cross(from, to).normalized == axis)
                return Vector3.Angle(from, to);
            else
                return -Vector3.Angle(from, to);
        }
        public Vector3 RandomNormal(Vector3 normal)
        {
            if (normal.x != 0 && normal.y != 0 && normal.z != 0)
            {
                float x = Random.Range(0.1f, 1f);
                float y = Random.Range(0.1f, 1f);
                float z = (normal.x * x + normal.y * y) / (-1 * normal.z);
                return new Vector3(x, y, z);
            }
            else
            {
                float x, y, z;
                if (normal.x == 0)
                    x = Random.Range(0.1f, 1f);
                else x = 0;
                if (normal.y == 0)
                    y = Random.Range(0.1f, 1f);
                else y = 0;
                if (normal.z == 0)
                    z = Random.Range(0.1f, 1f);
                else z = 0;
                return new Vector3(x, y, z);
            }
        }
    }

}