using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneShader : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private RenderTexture renderTexture;

    void Start()
    {
        renderTexture = new RenderTexture(256, 256, 32);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("resolution", renderTexture.width);
        computeShader.Dispatch(0, renderTexture.width/8, renderTexture.width/8, 1);
        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", renderTexture);
    }
}
