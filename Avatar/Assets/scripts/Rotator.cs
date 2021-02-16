using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator
{

    private static Dictionary<Transform, Quaternion> allRotations = new Dictionary<Transform, Quaternion>();

    public static Quaternion Rotate(Transform t, Vector3 rotation)
    {
        if (!allRotations.ContainsKey(t))
        {
            allRotations.Add(t, t.rotation);
        }
        Vector3 newAngle = allRotations[t].eulerAngles + rotation;
        return SetRotation(t, newAngle);
    }

    public static Quaternion SetRotation(Transform t, Vector3 rotation)
    {
        if (!allRotations.ContainsKey(t))
        {
            allRotations.Add(t, t.rotation);
        }
        Quaternion slerpRot = Quaternion.Slerp(allRotations[t], Quaternion.Euler(rotation), Time.deltaTime);
        t.rotation = allRotations[t] = slerpRot;
        return slerpRot;
    }
}
