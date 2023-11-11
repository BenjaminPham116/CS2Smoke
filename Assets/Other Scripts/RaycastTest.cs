using UnityEngine;
public class RaycastTest : MonoBehaviour
{
    public bool doRaycasts = false;
    public int numRaycasts = 1000;
    public bool getHitInfo = true;

    private int numHits = 0;
    private float fps = 0.0f;

    void Update()
    {
        // Keep running average of frame rate to smooth out instantaneous jitters.
        float fpsinstant = 1.0f / Time.deltaTime;
        fps = Mathf.Lerp(fps, fpsinstant, Time.deltaTime);

        if (doRaycasts)
        {
            numHits = 0;
            for (int n = 0; n < numRaycasts; ++n)
            {
                Ray ray = new Ray(transform.position, Random.onUnitSphere);
                if (getHitInfo)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        ++numHits;
                    }
                }
                else
                {
                    if (Physics.Raycast(ray))
                    {
                        ++numHits;
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label(string.Format("{0} raycasts\n{1} hits\n{2} fps", numRaycasts, numHits, fps));
    }
}