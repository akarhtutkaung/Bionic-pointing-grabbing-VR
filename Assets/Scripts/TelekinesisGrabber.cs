
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

    Grabbable grabbedObjGrabble;

    Material lineRendererMaterial;
    GameObject selectObj;

    bool objSelected, objCanMove;
    Vector3 prevPos;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        prevPos = transform.position;

        objSelected = false;
        objCanMove = false;

        grabbedObject = null;
        currentObject = null;
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

        if(objSelected){
            // check the speed and movement
            Debug.Log(Vector3.Distance(transform.position, prevPos));
            if(Vector3.Distance(transform.position, prevPos) > 0.1f){
                // objCanMove = true;
                if(count == 0){
                grabbedObjGrabble.zeroGravity(true);
                count++;
                }
            }
        }
        
    }

    void Select(InputAction.CallbackContext context)
    {
        if(grabbedObject == null){
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<Grabbable>())
                {   
                    selectObj = hit.collider.gameObject;
                    laserPointer.enabled = false;

                    //if moving
                    objSelected = true;
                    prevPos = transform.position;
                    grabbedObjGrabble = hit.collider.GetComponent<Grabbable>();
                    // if(objCanMove){
                    //     hit.collider.GetComponent<Grabbable>().zeroGravity(true);
                    // }

                    // add the object to the select list
                    // first get the velocity > x
                    // second get the velocity reducing or to 0
                    // then make the object come toward the eye
                    // activate the floating stance of the object
                    // disable the gravity of the object
                    // enable the kinematic of the object
                }
            }
        } else {
            // grabbedObject.SetCurrentGrabber(null);
            // grabbedObject.transform.parent = null;
            // grabbedObject = null;
        }
    }

    void selectRelease(InputAction.CallbackContext context)
    {
        objSelected = false;
        count = 0;
        if(grabbedObject == null && grabbedObjGrabble.getCurrentGrabber() == null){
            selectObj.GetComponent<Grabbable>().zeroGravity(false);
        }
        // enable the gravity of the object
        // disable the kinematic of the object
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
            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
                grabbedObject.GetComponent<Rigidbody>().AddForce(transform.forward * 300);
            }

            grabbedObject.SetCurrentGrabber(null);
            grabbedObject.transform.parent = null;
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