using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class ParameterModulation : MonoBehaviour
{

    FMOD.Studio.System system;

    string parameterName = "Size";
    public string distanceParameterName;
    public float value;

    public float zoneDistance;

    [EventRef]
    public string eventPath;
    FMOD.Studio.EventInstance eventInstance;
    

    private void Start()
    {
        system = FMODUnity.RuntimeManager.StudioSystem;
        eventInstance = RuntimeManager.CreateInstance(eventPath);
        eventInstance.start();

        zoneDistance = GetComponentInChildren<DistanceToPlayer>().distance;
    }


    void Update()
    {
        zoneDistance = GetComponentInChildren<DistanceToPlayer>().distance;

        system.setParameterByName(parameterName, value);
        system.setParameterByName(distanceParameterName, zoneDistance);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject.transform, gameObject.GetComponent<Rigidbody>());

        }
}
