using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    [Header("Scroll Settings")]  
    [SerializeField] float scrollSpeed = 1.0f;
    [SerializeField] float maxScrollDistance = -10.0f;
    [SerializeField] float minScrollDistance = 0.0f;


    [Header("Rotation Settings")]
    [SerializeField] float rotationSpeed = 5.0f;
    [SerializeField] float cameraXRotationMax = 0.0f;
    [SerializeField] float cameraXRotationMin = 0.0f;
    [SerializeField] float cameraYRotationMax = 0.0f;
    [SerializeField] float cameraYRotationMin = 0.0f;

    [Header("Arrays")]
    public string[] tagToNotRenderOnCollision;
    public string[] tagsYouCantCollideWith;
    float mouseX;
    float mouseY;
    float scroll;

    bool moveOutOfCollision = false;
   
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
        SphereCast();

        if (moveOutOfCollision)
        {
            Scroll(0.05f);
        }
    }

    void RotationInput()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") !=0)
        {
            transform.LookAt(target);
            Rotate(Input.GetAxis("Mouse Y") * rotationSpeed, Input.GetAxis("Mouse X") * rotationSpeed);
        }
        
    }
    void Rotate (float yAxis, float xAxis)
    {
        mouseY -= yAxis;
        mouseX += xAxis;

        if (cameraXRotationMax != 0 && cameraXRotationMin != 0) { mouseX = Mathf.Clamp(mouseX, cameraXRotationMin, cameraXRotationMax); }
        if (cameraYRotationMax != 0 && cameraYRotationMin != 0) { mouseY = Mathf.Clamp(mouseY, cameraYRotationMin, cameraYRotationMax); }
        target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
    }



    void ScrollInput()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Scroll(Input.mouseScrollDelta.y * scrollSpeed);
        }
    }
    void Scroll(float amountToScroll)
    {
        scroll += amountToScroll;
        scroll = Mathf.Clamp(scroll, maxScrollDistance, minScrollDistance);
        transform.localPosition = new Vector3(0, 0, scroll);
    }
    void ScrollToPosition(float zScrollPosition)
    {
        if (zScrollPosition > 0) { zScrollPosition *= -1; }
        Debug.Log("zScrollPosition: " + zScrollPosition);
        transform.localPosition = new Vector3(0, 0, zScrollPosition);
        scroll = zScrollPosition;
    }



    void SphereCast()
    {
        Vector3 heading = target.position - transform.position;
        float distance = heading.magnitude;
        Vector3 diraction = heading / distance;

        Debug.DrawLine(transform.position, diraction);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position,1, diraction, out hit,distance))
        {
            for (int i = 0; i < tagsYouCantCollideWith.Length; i++)
            {
                if (hit.transform.tag == tagsYouCantCollideWith[i])
                {
                    Debug.Log(hit.transform);
                    ScrollToPosition(hit.transform.position.z);
                    return;
                }
            }     
        }

    }





    private void OnTriggerEnter(Collider other)
    {

        for (int i = 0; i < tagsYouCantCollideWith.Length; i++)
        {
            if (other.tag == tagsYouCantCollideWith[i])
            {
                moveOutOfCollision = true;
                return;
            }
        }
        for (int i = 0; i < tagToNotRenderOnCollision.Length; i++)
        {
            if (other.tag == "Enemy")
            {
                other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                return;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {

        for (int i = 0; i < tagsYouCantCollideWith.Length; i++)
        {
            if (other.tag == tagsYouCantCollideWith[i])
            {
                moveOutOfCollision = false;
                return;
            }
        }

        for (int i = 0; i < tagToNotRenderOnCollision.Length; i++)
        {
            if (other.tag == "Enemy")
            {
                other.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                return;
            }
        }
    
    }
}
