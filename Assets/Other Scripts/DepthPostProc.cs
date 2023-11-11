using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class DepthPostProc : MonoBehaviour
{
    public Material postProccessingMat;
    public RenderTexture rt;
    private Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        cam.targetTexture = rt;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
        Graphics.Blit(source, destination, postProccessingMat);
    }
}
