using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundUI : MonoBehaviour
{
    public float input;
    public GameObject indicator;
    private Image img;
    private float defaultSize = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        img = indicator.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(input <= 0) { input = 0; }
        if(input >= 1) { input = 1; }

        img.color = new Color(img.color.r, img.color.g, img.color.b, input);
        indicator.transform.localScale = new Vector3(defaultSize * input, defaultSize * input, defaultSize * input);
        input = Camera.main.GetComponent<MicProcessing>().inputLevel * 10;
    }
}
