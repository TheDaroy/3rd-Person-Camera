using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] GameObject masterObject;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();       
        if (masterObject) { this.transform.SetParent(masterObject.transform);}
        else { Debug.LogError(this + " No MasterObject Found"); }
    }
    void Start()
    {
        Vector3 startPosition = masterObject.transform.position;
        startPosition.x -= 30;
        transform.position = startPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(masterObject.transform.localPosition, Vector3.left, 30 * Input.GetAxis("Mouse X"));
        //transform.RotateAround(masterObject.transform.localPosition, Vector3.up, 30 * Input.GetAxis("Mouse Y"));
        
    }
}
