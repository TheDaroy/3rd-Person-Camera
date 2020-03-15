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


    const string mouseXAxis = "Mouse X";
    const string mouseYAxis = "Mouse Y";

   
    bool collidingWithTagYouCantCollideWith; // Look....its early ok.
     
    float relativeLastZPosition = 0;
    bool returnToPosition;



    float amopuntToScrollForward = 0.05f;
    float amopuntToScrollBackward = -0.05f;
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
        ForwardSphereCast();
        
        

        if (moveOutOfCollision)
        {
            relativeLastZPosition -= amopuntToScrollForward;
            Scroll(amopuntToScrollForward);
            Debug.Log("Moving Forward");
            returnToPosition = true;
        }
        ReturnToLastZPosition();
    }
   


    void RotationInput()
    {
        if (Input.GetAxis(mouseXAxis) != 0 || Input.GetAxis(mouseYAxis) !=0)
        {
            transform.LookAt(target);
            Rotate(Input.GetAxis(mouseYAxis) * rotationSpeed, Input.GetAxis(mouseXAxis) * rotationSpeed);
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
            if (CanScrollBack())
            {
                Scroll(Input.mouseScrollDelta.y * scrollSpeed);          
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                Scroll(Input.mouseScrollDelta.y * scrollSpeed);
            }
            
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
        
        transform.localPosition = new Vector3(0, 0, zScrollPosition);
        scroll = zScrollPosition;
        
    }

    void ReturnToLastZPosition()
    {
        if (returnToPosition )
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                returnToPosition = false;
                relativeLastZPosition = 0;
            }
            else if (CanScrollBack())
            {
                Debug.Log("Moving Back");
                Scroll(amopuntToScrollBackward);
                relativeLastZPosition -= amopuntToScrollBackward;
                if (relativeLastZPosition >= 0)
                {
                    relativeLastZPosition = 0;
                    returnToPosition = false;
                }
            }
        }    
    }

    void ForwardSphereCast()
    {
        Vector3 heading = target.position - transform.position;
        float distance = heading.magnitude;
        Vector3 diraction = heading / distance;

        Debug.DrawLine(transform.position, diraction);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, diraction, out hit,distance))
        {
            for (int i = 0; i < tagsYouCantCollideWith.Length; i++)
            {
                if (hit.transform.tag == tagsYouCantCollideWith[i])
                {
                   
                    returnToPosition = true;
                    relativeLastZPosition = transform.localPosition.z - hit.transform.position.z ;
                    Debug.Log("transform.position.z: " + transform.localPosition.z);
                    Debug.Log("hit.transform.position.z: " + hit.transform.position.z);
                    Debug.Log("relativeLastZPosition: " + relativeLastZPosition);
                    ScrollToPosition(hit.transform.position.z);
                    return;
                }
            }     
        }
    }


    bool BackwardsRayCast()
    {
        Vector3 heading = -transform.forward ;
        float distance = 2f;
        Vector3 diraction = heading / distance;
        RaycastHit hit;
        Vector3 origin = transform.position;
        origin.z -= 1;

        if (Physics.Raycast(origin,  diraction, out hit, distance))
        {
            for (int i = 0; i < tagsYouCantCollideWith.Length; i++)
            {
                if (hit.transform.tag == tagsYouCantCollideWith[i])
                {
                   
                    
                    return true;
                }

            }
            
        }
        return false;  
    }



    bool CanScrollBack()
    {
        if (collidingWithTagYouCantCollideWith || BackwardsRayCast())
        {
            Debug.Log("Can Not Scroll Back");
            return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {

        for (int i = 0; i < tagsYouCantCollideWith.Length; i++)
        {
            if (other.tag == tagsYouCantCollideWith[i])
            {
                collidingWithTagYouCantCollideWith = true;
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
                collidingWithTagYouCantCollideWith = false;               
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
