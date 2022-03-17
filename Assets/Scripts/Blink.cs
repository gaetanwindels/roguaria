using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blink : MonoBehaviour
{

    [SerializeField] private Color blinkColor;
    private float blinkTimer = 0f;
    private float blinkDuration = 0f;
    private Material[] materials;
    private Color[] originColors;

    // Start is called before the first frame update
    void Start()
    {
        materials = GetComponentInChildren<Renderer>().materials;
        originColors = materials.Select(material => material.color).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= blinkDuration)
        {
            blinkTimer = 0f;
            blinkDuration = 0f;

            for (var i = 0; i < materials.Length; i++)
            {
                materials[i].SetColor("_BaseColor", originColors[i]);
            }

        }
    }

    public void doBlink(float duration)
    {
        blinkDuration = duration;

        foreach (var material in materials)
        {
            material.SetColor("_BaseColor", blinkColor);
        }
    }
}
