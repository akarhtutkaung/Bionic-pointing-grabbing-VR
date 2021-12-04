
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

    Material lineRendererMaterial;
    // Start is called before the first frame update
    void Start()
    {

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
    }

    void Select(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.GetComponent<Grabbable>())
            {
                laserPointer.enabled = false;
                hit.collider.GetComponent<Grabbable>().zeroGravity(true);

                // add the object to the select list
                // first get the velocity > x
                // second get the velocity reducing or to 0
                // then make the object come toward the eye
                // activate the floating stance of the object
                // disable the gravity of the object
                // enable the kinematic of the object
            }
        }
    }

    void selectRelease(InputAction.CallbackContext context)
    {
        // enable the gravity of the object
        // disable the kinematic of the object
    }

    void TouchDown(InputAction.CallbackContext context)
    {
        laserPointer.enabled = true;
    }

    void TouchUp(InputAction.CallbackContext context)
    {
        laserPointer.enabled = false;
    }

    public override void Grab(InputAction.CallbackContext context){
        // grab the item
    }

    public override void Release(InputAction.CallbackContext context){
        // release the item
    }
}