using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWaves : MonoBehaviour
{
    public float sine1;
    private float sine1Speed = 2.5f;
    public float sine2;
    private float sine2Speed = 3.15f;

    public Shader shader;
    public GameObject sphere;

    private SizeModulation sizeModulation;

    // Start is called before the first frame update
    void Start()
    {
        sizeModulation = FindObjectOfType<SizeModulation>();
    }

    // Update is called once per frame
    void Update()
    {
        sine1 = (Mathf.Sin(Time.time / sine1Speed) + 1) / 2;
        sine2 = (Mathf.Sin(Time.time / sine2Speed) + 1) / 2;

        sphere.transform.localScale = new Vector3(0.5f + (sine1 / 5), 0.5f + (sine1 / 5), 0.5f + (sine1 / 5));
        sizeModulation.size = sine1;

        sphere.transform.Rotate(new Vector3((Time.time * sine2) / 15f, (Time.time * sine2) / 5, (Time.time * sine2) / 21f));
        sizeModulation.texture = sine2;

    }
}
