using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastClick : MonoBehaviour
{
    Camera _camera;
    // Use this for initialization
    void Start()
    {
        _camera = GetComponent<Camera>();
    }
    Ray ray;
    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);
           
            if (Physics.Raycast(ray, out hit))
            {
                 //Debug.LogError("hit:" + hit.collider.gameObject.name);
                hit.collider.gameObject.GetComponent<SelfStateControl>().StateChange();
            }

        }
    }
}

