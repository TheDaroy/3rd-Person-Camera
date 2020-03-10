using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;

    [SerializeField] float rotationSpeed = 5.0f;
    [SerializeField] float scrollSpeed = 1.0f;

    [SerializeField] float maxScrollDistance = -10.0f;
    [SerializeField] float minScrollDistance = 0.0f;

    float mouseX;
    float mouseY;
    float scroll;
   
    void Start()
    {
        scroll = transform.localPosition.z;
        mouseX = target.rotation.eulerAngles.y;
        mouseY = target.rotation.eulerAngles.x;

        if (maxScrollDistance > 0) { maxScrollDistance *= -1; }
        
    }
    
    void Update()
    {
        RotationInput();
        ScrollInput();
        CapsuleCast();
    }

    void RotationInput()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
       

        transform.LookAt(target);
        Rotate(mouseY, mouseX); 
    }
    void Rotate (float yAxis, float xAxis)
    {
        target.rotation = Quaternion.Euler(yAxis, xAxis, 0);
    }

    void ScrollInput()
    {
        scroll += Input.mouseScrollDelta.y * scrollSpeed;          
        scroll = Mathf.Clamp(scroll, maxScrollDistance, minScrollDistance);

        Scroll(scroll);
    }

    void Scroll(float amountToScroll)
    {
        transform.localPosition = new Vector3(0, 0, amountToScroll);
    }

    void ScrollToPosition(float zScrollPosition)
    {
        transform.localPosition = new Vector3(0, 0, zScrollPosition);
        scroll = zScrollPosition;
    }
    void CapsuleCast()
    {
        Debug.DrawLine(transform.position, target.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position,target.position, out hit))
        {
            Debug.DrawLine(transform.position, target.position);
            Debug.Log(hit.transform);
            ScrollToPosition(hit.transform.position.z);
        }

    }
}
