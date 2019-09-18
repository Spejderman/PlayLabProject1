using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SoundEmission : MonoBehaviour
{

    FMOD.Studio.System system;

    //Event variables
    public string eventName;
    private string eventPath;
    FMOD.Studio.EventInstance eventInstance;

    //Distance (occlusion) variables
    private float distance;
    private DistanceToPlayer dtp;

    private Rigidbody rb;



    void Start()
    {
        system = FMODUnity.RuntimeManager.StudioSystem;

        rb = GetComponent<Rigidbody>();

        //Creating event instance and playing sound
        eventPath = "event:/" + eventName;
        eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        eventInstance.start();

        //Finding distance component in child object
        dtp = GetComponentInChildren<DistanceToPlayer>();
    }

    void Update()
    {
        //Attaching sound to gameobject so 3d sound works
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject.transform, rb);

        //Updating correct distance + setting local parameter
        distance = dtp.distance;
        eventInstance.setParameterByName("Occlusion", distance);
    }
}
