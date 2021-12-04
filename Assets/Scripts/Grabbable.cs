using UnityEngine;

public class Grabbable : MonoBehaviour
{

    private Grabber currentGrabber;
    GameObject headset;
    bool move;

    // Start is called before the first frame update
    void Start()
    {
        move = false;
        headset = GameObject.Find("Main Camera");
        currentGrabber = null;

        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().Sleep();
        }
    }

    void Update() 
    {
        Vector3 pos = this.transform.position;
        if(move && pos.x > headset.transform.position.x){
            this.transform.position = new Vector3 (this.transform.position.x - 0.1f, this.transform.position.y, this.transform.position.z);
        }
        if(move && pos.y > headset.transform.position.y){
            this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y - 0.1f, this.transform.position.z);
        }
        if(move && pos.z > headset.transform.position.z){
            this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z - 0.1f);
        }
    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        currentGrabber = grabber;
    }

    public Grabber GetCurrentGrabber()
    {
        return currentGrabber;
    }



    public void zeroGravity(bool choice){
        if(choice){
            move = true;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            // this.transform.position = headset.transform.position;
        } else {
            
        }
    }
}
