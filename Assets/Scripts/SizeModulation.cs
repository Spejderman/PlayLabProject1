using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SizeModulation : MonoBehaviour
{
    public float size;
    public float texture;
    private float speed = 0.5f;

    FMOD.Studio.System system;
    private string parameterName = "ObjectSize";

    SoundEmission childSound;

    MicProcessing mic;

    void Start()
    {
        system = FMODUnity.RuntimeManager.StudioSystem;
        mic = FindObjectOfType<MicProcessing>();
        Debug.Assert(mic != null,"Fandt ikke mic");

        childSound = GetComponentInChildren<SoundEmission>();
    }

    void Update()
    {
        system.setParameterByName(parameterName, size);
        if (size < 0) { size = 0; }
        if (size > 1) { size = 1; }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            size += Time.deltaTime * speed;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            size -= Time.deltaTime * speed;
        }

        childSound.localTexture = texture;
        if (texture < 0) { texture = 0; }
        if (texture > 1) { texture = 1; }

        if (Input.GetKey(KeyCode.RightArrow)) {
            texture += Time.deltaTime * speed;
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            texture -= Time.deltaTime * speed;
        }

        size = mic.inputLevel * 10;
    }
}
