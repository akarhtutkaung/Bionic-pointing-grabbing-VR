using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;


public class GraspGrabber : Grabber
{
    public GameObject storageParent;
    public InputActionProperty grabAction;
    public InputActionProperty Activation;
    public InputActionProperty Activation_other;

    public GameObject righthandController;

    List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    Grabbable currentObject;
    Grabbable grabbedObject;

    UnityEngine.XR.InputDevice head;
    UnityEngine.XR.InputDevice LeftHand;
    UnityEngine.XR.InputDevice RightHand;

    Vector3 headPosition;
    Vector3 LeftHandPosition;
    Vector3 RightHandPosition;

    Vector3 storageScale = new Vector3(0.03f,0.03f,0.03f);

    int selectedIndex = -1;
    bool selectingStorage = false;  

    // Start is called before the first frame update
    void Start()
    {

        storageParent.SetActive(true);
        storageParent.transform.position = this.transform.position;

        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        grabbedObject = null;
        currentObject = null;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;


        foreach (var device in inputDevices)
        {
            if (device.isValid)
            {
                if(device.role.ToString() == "Generic")
                {
                    head = device;
                    Debug.Log(head.name + " activated");

                }
                else if (device.role.ToString() == "LeftHanded")
                {
                    LeftHand = device;
                    Debug.Log(LeftHand.name + " activated");

                }
                else if (device.role.ToString() == "RightHanded")
                {
                    RightHand = device;
                    Debug.Log(RightHand.name + " activated");
                }
            }
        }

        storageParent.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        head.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out headPosition);
        LeftHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out LeftHandPosition);
        RightHand.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out RightHandPosition);

        this.closestStorageScale();
/*        if (activated && this.name == "controller_r")
        {
            this.transform.localPosition = (new Vector3(0,0,0.04f)) * growFunction((headPosition - RightHandPosition).magnitude);

        }
        else if (activated && this.name == "controller_l")
        {
            this.transform.localPosition = (new Vector3(0, 0, 0.04f)) * growFunction((headPosition - LeftHandPosition).magnitude);

        }else if (grabbedObject && !activated)
        {
            Vector3 dir = (theOtherController.transform.position - this.transform.position);
            if (this.name == "controller_r")
            {
                dir = (this.transform.position - theOtherController.transform.position);
            }
            float currentDis = dir.magnitude;
            grabbedObject.transform.position = storageParent.transform.position;


            Vector3 orthogonalVec = Vector3.Cross(this.transform.parent.forward, dir.normalized).normalized;
            storageParent.transform.right = dir.normalized;
            grabbedObject.transform.localScale = grabbedObject.getScale() * (currentDis / scaleDivider);

        }*/
        Debug.Log("selectedIndex: " + selectedIndex);

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

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }

            grabbedObject.transform.parent = this.transform;
            grabbedObject.transform.localScale = grabbedObject.getScale();
            grabbedObject.stored = false;
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
            grabbedObject.setScale(grabbedObject.transform.localScale);
            grabbedObject.SetCurrentGrabber(null);
            grabbedObject.transform.parent = null;


            if (selectedIndex != -1 && storageParent.transform.GetChild(selectedIndex).childCount == 0)
            {
                
                
                grabbedObject.transform.localScale = (storageParent.transform.GetChild(selectedIndex).GetComponent<Renderer>().bounds.size.magnitude /(1.5f *
                                                        grabbedObject.transform.GetComponent<Renderer>().bounds.size.magnitude)) * grabbedObject.transform.localScale;
                grabbedObject.transform.parent = storageParent.transform.GetChild(selectedIndex).transform;
                grabbedObject.transform.rotation = storageParent.transform.GetChild(selectedIndex).transform.rotation;
                grabbedObject.transform.localPosition = new Vector3(0.0f,-0.5f,0.0f);


                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                grabbedObject.stored = true;
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

    void closestStorageScale()
    {
           
        float closestDis = 0.2f;
        int closestIndex = -1;

        for (int i = 0; i < storageParent.transform.childCount; i++)
        {
            if (Vector3.Distance(righthandController.transform.position, storageParent.transform.GetChild(i).position) < closestDis)
            {
                closestDis = Vector3.Distance(righthandController.transform.position, storageParent.transform.GetChild(i).position);
                closestIndex = i;
            }
            else
            {
                storageParent.transform.GetChild(i).transform.localScale = storageScale;
            }
        }

        if (closestIndex != -1)
        {
            storageParent.transform.GetChild(closestIndex).transform.localScale = storageScale * 2.0f;
            selectingStorage = true;
            if (selectedIndex != closestIndex && selectedIndex != -1)
            {
                storageParent.transform.GetChild(selectedIndex).transform.localScale = storageScale;
            }
            selectedIndex = closestIndex;
        }
        else
        {
            selectedIndex = -1;
            selectingStorage = false;


        }
            //Debug.Log("selectedIndex: " + selectedIndex);
        


    }
}
