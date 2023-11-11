using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public float timeToDisappear = .1f;
    private float timeExisted;
    private float initSize;
    // Start is called before the first frame update
    void Start()
    {
        initSize = transform.localScale.x;
        timeExisted = timeToDisappear;    
    }

    // Update is called once per frame
    void Update()
    {
        float newSize = Mathf.SmoothStep(initSize, 0, timeExisted / timeToDisappear);
        transform.localScale = new Vector3(newSize, transform.localScale.y, newSize);
        if (timeExisted < 0)
        {
            Destroy(gameObject);
        }
        timeExisted -= Time.deltaTime;
        
    }
}
