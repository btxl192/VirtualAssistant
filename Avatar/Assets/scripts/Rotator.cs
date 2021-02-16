using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator
{

    private static Dictionary<Transform, Quaternion> allRotations = new Dictionary<Transform, Quaternion>();

    public static void Rotate(Transform t, Vector3 rotation)
    {
        if (!allRotations.ContainsKey(t))
        {
            allRotations.Add(t, t.rotation);
        }
        Vector3 newAngle = allRotations[t].eulerAngles + rotation;
        SetRotation(t, newAngle);
    }

    public static void SetRotation(Transform t, Vector3 rotation)
    {
        if (!allRotations.ContainsKey(t))
        {
            allRotations.Add(t, t.rotation);
        }
        t.rotation = allRotations[t] = Quaternion.Slerp(allRotations[t], Quaternion.Euler(rotation), Time.deltaTime);
    }
}
