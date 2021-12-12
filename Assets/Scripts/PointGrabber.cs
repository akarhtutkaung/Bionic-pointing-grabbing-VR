using UnityEngine;
using UnityEngine.InputSystem;

public class PointGrabber : Grabber
{
    public float fishSpeed = 2.5f;
    public float deadZone = 0.25f;
    public LineRenderer laserPointer;
    public Material grabbablePointerMaterial;

    public InputActionProperty touchAction;
    public InputActionProperty grabAction;
    public InputActionProperty fishReelAction;

    Material lineRendererMaterial;
    Transform grabPoint;
    Grabbable grabbedObject;
    Transform initialParent;


    private bool grabbing;

    

    
    // Start is called before the first frame update
    void Start()
    {
        

        laserPointer.enabled = false;
        lineRendererMaterial = laserPointer.material;

        grabPoint = new GameObject().transform;
        grabPoint.name = "Grab Point";
        grabPoint.parent = this.transform;
        grabbedObject = null;
        initialParent = null;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;

        touchAction.action.performed += TouchDown;
        touchAction.action.canceled += TouchUp;

        fishReelAction.action.performed += Move;
        grabbing = false;
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;

        touchAction.action.performed -= TouchDown;
        touchAction.action.canceled -= TouchUp;

        fishReelAction.action.performed -= Move;
        
    }

    // Update is called once per frame
    void Update()
    {



        if (laserPointer.enabled)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, hit.distance));

                if (hit.collider.GetComponent<Grabbable>())
                {
                    laserPointer.material = grabbablePointerMaterial;
                }
                else
                {
                    laserPointer.material = lineRendererMaterial;
                }
            }
            else
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, 100));
                laserPointer.material = lineRendererMaterial;
            }
        }
    }

    public override void Grab(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.GetComponent<Grabbable>() && !hit.collider.GetComponent<Grabbable>().stored)
            {
                
                grabPoint.localPosition = new Vector3(0, 0, hit.distance);

                if (hit.collider.GetComponent<Grabbable>().GetCurrentGrabber() != null)
                {
                    hit.collider.GetComponent<Grabbable>().GetCurrentGrabber().Release(new InputAction.CallbackContext());
                }

                grabbedObject = hit.collider.GetComponent<Grabbable>();
                
                grabbedObject.SetCurrentGrabber(this);

                if (grabbedObject.GetComponent<Rigidbody>())
                {
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                    grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                }

                initialParent = grabbedObject.transform.parent;
                grabbedObject.transform.parent = grabPoint;

                grabbing = true;

                

            }
        }
    }

    public override void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            }

            grabbedObject.transform.parent = initialParent;
            grabbedObject = null;
            grabbing = false;
        }
    }

    void TouchDown(InputAction.CallbackContext context)
    {
        laserPointer.enabled = true;
    }

    void TouchUp(InputAction.CallbackContext context)
    {
        laserPointer.enabled = false;
    }

    void Move(InputAction.CallbackContext context)
    {
        if (grabbing)
        {   
            var rightHandValue = fishReelAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            float dir_r = rightHandValue.y > 0 ? 1.0f : -1.0f;
            if(rightHandValue.y >= deadZone || rightHandValue.y <= -deadZone)
            {
                grabbedObject.transform.position += dir_r * this.laserPointer.transform.forward * fishSpeed * Time.deltaTime;
            }

        }
    }
}
