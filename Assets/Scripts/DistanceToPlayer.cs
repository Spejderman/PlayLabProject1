using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToPlayer : MonoBehaviour
{

    Transform other;
    public float distance;

    void Start()
    {
        other = Camera.main.transform;
    }

    void Update()
    {
        distance = Vector3.Distance(other.position, transform.position);
        distance = Map(distance, 1, 0, 0, 1);
        distance = distance + 1;
        if (distance <= 0) { distance = 0; }
        if (distance >= 1) { distance = 1; }
    }

    float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
