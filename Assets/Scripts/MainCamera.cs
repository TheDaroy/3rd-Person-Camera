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
        startPosition.x -= 5;
        transform.position = startPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(masterObject.transform);
        transform.RotateAround(masterObject.transform.position, new Vector3 (0f,1f,0f), 5 * Input.GetAxis("Mouse X"));
        transform.RotateAround(masterObject.transform.position, new Vector3(1f, 0f, 0f), 5 * Input.GetAxis("Mouse Y"));
        //transform.RotateAround(masterObject.transform.localPosition, Vector3.up, 30 * Input.GetAxis("Mouse Y"));
        //transform.Rotate(30 * Input.GetAxis("Mouse X") * Time.deltaTime, 0, 0);
        //transform.Rotate(0, 30 * Input.GetAxis("Mouse Y")* Time.deltaTime, 0);
    }
}
