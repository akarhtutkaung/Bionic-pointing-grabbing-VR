
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TelekinesisGrabber : Grabber
{
    public LineRenderer laserPointer;
    public Material grabbablePointerMaterial;
    public InputActionProperty touchAction;
    public InputActionProperty selectAction;
    public InputActionProperty grabAction;

    Grabbable currentObject;
    Grabbable grabbedObject;

    Grabbable selectedObj;

    GameObject headset;

    Material lineRendererMaterial;
    // GameObject selectObj;

    Vector3 prevPos;
    bool newMove = true;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;
        headset = GameObject.Find("Main Camera");

        grabbedObject = null;
        currentObject = null;
        selectedObj = null;
        laserPointer.enabled = false;
        lineRendererMaterial = laserPointer.material;

        selectAction.action.performed += Select;
        selectAction.action.canceled += selectRelease;

        touchAction.action.performed += TouchDown;
        touchAction.action.canceled += TouchUp;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;
    }

    private void OnDestroy()
    {
        selectAction.action.performed -= Select;
        selectAction.action.canceled -= selectRelease;

        touchAction.action.performed -= TouchDown;
        touchAction.action.canceled -= TouchUp;

        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;

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
                laserPointer.SetPosition(1, new Vector3(0, 0, 40));
                laserPointer.material = lineRendererMaterial;
            }
        }

        if(selectedObj && newMove){
            // check the speed and movement
            Vector3 direction = transform.position - prevPos;
            float angle = Vector2.Angle(new Vector2(headset.transform.forward.x, headset.transform.forward.z), new Vector2(direction.x, direction.z));
            
            if(Vector3.Distance(transform.position, prevPos) > 0.1f){
                if(Vector3.Dot(direction, headset.transform.forward) < 0){
                    if(count == 0){
                    selectedObj.zeroGravity(true);
                    count++;
                    }
                    selectedObj.GetComponent<Outline>().enabled = false;
                    newMove = false;
                } else {
                    selectedObj.GetComponent<Rigidbody>().AddForce(direction * 10000);
                    selectedObj.GetComponent<Outline>().enabled = false;
                    if(selectedObj.GetCurrentSelectedGrabber() == this){
                selectedObj.SetCurrentSelectedGrabber(null);
            }
                    selectedObj = null;
                }
            }
        }

        //     if((Vector3.Distance(transform.position, prevPos) > 0.1f) && 
        //         Vector3.Dot(direction, headset.transform.forward) < 0){
                
        //     } else if ((Vector3.Distance(transform.position, prevPos) > 0.1f)){
                
        //     }
        // }
    }

    void Select(InputAction.CallbackContext context)
    {
        if(grabbedObject == null){
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<Grabbable>())
                {   
                    if(hit.collider.GetComponent<Grabbable>().GetCurrentSelectedGrabber() == null){
                        selectedObj = hit.collider.GetComponent<Grabbable>();
                        laserPointer.enabled = false;
                        prevPos = transform.position;
                        selectedObj.GetComponent<Outline>().enabled = true;
                        selectedObj.SetCurrentSelectedGrabber(this);
                    }
                }
            }
        }
    }

    void selectRelease(InputAction.CallbackContext context)
    {
        count = 0;
        if(selectedObj){
            selectedObj.GetComponent<Outline>().enabled = false;
            if(grabbedObject == null){
                    if(selectedObj.GetCurrentGrabber() == null){
                        selectedObj.GetComponent<Grabbable>().zeroGravity(false);
                    }
                }
            
            if(selectedObj.GetCurrentSelectedGrabber() == this){
                selectedObj.SetCurrentSelectedGrabber(null);
            }
        }
        selectedObj = null;
        newMove = true;
    }

    void TouchDown(InputAction.CallbackContext context)
    {
        if(grabbedObject == null){
            laserPointer.enabled = true;
        }
    }

    void TouchUp(InputAction.CallbackContext context)
    {
        laserPointer.enabled = false;
    }

    public override void Grab(InputAction.CallbackContext context)
    {
        if (currentObject && grabbedObject == null)
        {
            if (currentObject.GetCurrentGrabber() != null)
            {
                currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
            }

            grabbedObject = currentObject;
            grabbedObject.SetCurrentGrabber(this);

            grabbedObject.GetComponent<Grabbable>().zeroGravity(false);

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }

            grabbedObject.transform.parent = this.transform;
        }
    }

    public override void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            grabbedObject.zeroGravity(false);
            grabbedObject.GetComponent<Rigidbody>().AddForce(transform.forward * 300);

            grabbedObject.SetCurrentGrabber(null);
            grabbedObject.transform.parent = null;

            if (grabbedObject.GetCurrentSelectedGrabber()) {
                grabbedObject.SetCurrentSelectedGrabber(null);
            }
            grabbedObject = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentObject == null && other.GetComponent<Grabbable>())
        {
            currentObject = other.gameObject.GetComponent<Grabbable>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentObject)
        {
            if (other.GetComponent<Grabbable>() && currentObject.GetInstanceID() == other.GetComponent<Grabbable>().GetInstanceID())
            {
                currentObject = null;
            }
        }
    }
}