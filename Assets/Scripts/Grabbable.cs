using UnityEngine;

public class Grabbable : MonoBehaviour
{

    private Grabber currentGrabber;
    GameObject headset;
    bool move;
    bool floatInAir;
    bool grabbed;
    float speed = 0.05f;
    float count = 0;

    // Start is called before the first frame update
    void Start()
    {
        floatInAir = false;
        move = false;
        headset = GameObject.Find("Main Camera");
        currentGrabber = null;
        grabbed = false;
        // if (this.GetComponent<Rigidbody>())
        // {
        //     this.GetComponent<Rigidbody>().Sleep();
        // }
    }

    void Update() 
    {
        if(grabbed == false){
            Vector3 pos = this.transform.position;
            if(move){
                if(pos.x >= headset.transform.position.x){
                    this.transform.position = calculate(1);
                }
                else if (pos.x < headset.transform.position.x){
                    this.transform.position = calculate(2);
                }
            }
            // float dist = Vector3.Distance(headset.transform.position, transform.position);
            // if(dist > 0 && dist < 1){
            if(Mathf.Abs(transform.position.y - headset.transform.position.y) < 0.2 && Mathf.Abs(transform.position.z - headset.transform.position.z) < 0.6 && currentGrabber == null){
                float x = transform.position.x;
                float y = transform.position.y;
                float z = transform.position.z;

                move = false;
                floatInAir = true;
                if(count < 100){
                    if(count > 30 && count < 70){
                        transform.position = new Vector3(x, y+0.0005f, z);
                    } else {
                        transform.position = new Vector3(x, y+0.0003f, z);
                    }
                    count++;
                } else {
                    if(count > 130 && count < 170){
                        transform.position = new Vector3(x, y-0.0005f, z);
                    } else {
                        
                    transform.position = new Vector3(x, y-0.0003f, z);
                    }
                    if(count < 200){
                        count++;
                    } else {
                        count = 0;
                    }
                }
            }
        }
        
        //  else if (floatInAir){
        //     move = true;
        //     floatInAir = false;
        // }
    }

    Vector3 calculate(int code){
        // linear equation for XZ plane
        
        float mXZ = ((headset.transform.position.z + 0.5f) - this.transform.position.z)/(headset.transform.position.x - this.transform.position.x);
        float bXZ = (headset.transform.position.z + 0.5f) - (mXZ * headset.transform.position.x);
        float z;
        if(code == 1){
            z = mXZ * (this.transform.position.x - speed) + bXZ;
        } else {
            z = mXZ * (this.transform.position.x + speed) + bXZ;
        }

        // linear equation for YX plane
        float mYZ = (headset.transform.position.y - this.transform.position.y)/((headset.transform.position.z + 0.5f) - this.transform.position.z);
        float bYZ = headset.transform.position.y - (mYZ * (headset.transform.position.z + 0.5f));
        float y = mYZ * z + bYZ;

        if(code == 1){
            return new Vector3 (this.transform.position.x - speed, y, z);
        } else if (code == 2){
            return new Vector3 (this.transform.position.x + speed, y, z);
        }

        // error
        return new Vector3 (0,0,0);


        // // linear equation for XZ plane
        // float mXZ = (headset.transform.position.z - this.transform.position.z)/(headset.transform.position.x - this.transform.position.x);
        // float bXZ = headset.transform.position.z - (mXZ * headset.transform.position.x);
        // float z;
        // if(code == 1){
        //     z = mXZ * (this.transform.position.x - speed) + bXZ;
        // } else {
        //     z = mXZ * (this.transform.position.x + speed) + bXZ;
        // }

        // // linear equation for YX plane
        // float mYZ = (headset.transform.position.y - this.transform.position.y)/(headset.transform.position.z - this.transform.position.z);
        // float bYZ = headset.transform.position.y - (mYZ * headset.transform.position.z);
        // float y = mYZ * z + bYZ;

        // if(code == 1){
        //     return new Vector3 (this.transform.position.x - speed, y, z);
        // } else if (code == 2){
        //     return new Vector3 (this.transform.position.x + speed, y, z);
        // }

        // // error
        // return new Vector3 (0,0,0);
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
        } else {
            move = false;
            // floatInAir = false;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
