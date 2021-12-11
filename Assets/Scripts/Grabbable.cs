using UnityEngine;

public class Grabbable : MonoBehaviour
{

    private Grabber currentGrabber;
    GameObject headset;
    bool move;
    bool grabbed;
    bool alreadyGrabbed;
    bool hasMoved;
    bool canFloat;
    float speed = 0.05f;
    float count = 0;
    double diffThresh = 0.0000001;

    // Start is called before the first frame update
    void Start()
    {
        hasMoved = false;
        move = false;
        canFloat = false;
        headset = GameObject.Find("Main Camera");
        currentGrabber = null;
        grabbed = false;
    }

    void Update() 
    {
        if(grabbed == false){
            Vector3 pos = this.transform.position;
            if(move){
                if(!(Mathf.Abs(transform.position.x - headset.transform.position.x) < 0.5 && 
                Mathf.Abs(transform.position.z - headset.transform.position.z) < 0.5)) {
                    this.transform.position = calculate();
                    hasMoved = true;
                }
            }
            if(Mathf.Abs(transform.position.x - headset.transform.position.x) < 0.5 && 
            Mathf.Abs(transform.position.y - headset.transform.position.y) < 0.2 && 
            Mathf.Abs(transform.position.z - headset.transform.position.z) < 0.5){
                canFloat = true;
            }
            
            if(canFloat && currentGrabber == null && alreadyGrabbed == false && hasMoved){
                move = false;
                floatInAir();
            }
        }
    }

    void floatInAir(){
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
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

    Vector3 calculate(){
        
        float thisX = this.transform.position.x;
        float thisY = this.transform.position.y;
        float thisZ = this.transform.position.z;
        float headX = headset.transform.position.x;
        float headY = headset.transform.position.y;
        float headZ = headset.transform.position.z;
        float newX, newY, newZ;

        if(Mathf.Abs(thisX - headX) < diffThresh){
            if(Mathf.Abs(thisY - headY) < diffThresh){
                Debug.Log("00a");
                if(thisZ > headZ){
                    return new Vector3 (thisX, thisY, thisZ - speed);
                } else {
                    return new Vector3 (thisX, thisY, thisZ + speed);
                }
            } else if (Mathf.Abs(thisZ - headZ) < diffThresh){
                Debug.Log("0a0");
                if(thisY > headY){
                    return new Vector3 (thisX, thisY - speed, thisZ);
                } else {
                    return new Vector3 (thisX, thisY + speed, thisZ);
                }
            } else {
                Debug.Log("0aa");
                float m = (headY - thisY)/(headZ - thisZ);
                float b = headY - (m * headZ);
                
                if(thisZ > headZ){
                    newZ = thisZ - speed;
                } else {
                    newZ = thisZ + speed;
                }
                newY = m * newZ + b;
                return new Vector3 (thisX, newY, newZ);                
            }
        } else if(Mathf.Abs(thisY - headY) < diffThresh){
            if (Mathf.Abs(thisZ - headZ) < diffThresh){
                Debug.Log("a00");
                if(thisX > headX){
                    return new Vector3 (thisX - speed, thisY, thisZ);
                } else {
                    return new Vector3 (thisX + speed, thisY, thisZ);
                }
            } else {
                Debug.Log("a0a");
                float m = (headX - thisX)/(headZ - thisZ);
                float b = headX - (m * headZ);

                if(thisX > headX){
                    newX = thisX - speed;
                } else {
                    newX = thisX + speed;
                }
                newZ = m * newX + b;
                return new Vector3 (newX, thisY, newZ);
            }
        } else if(Mathf.Abs(thisZ - headZ) < diffThresh){
            Debug.Log("aa0");
            float m = (headY - thisY)/(headX - thisX);
            float b = headY - (m * headX);

            if(thisX > headX){
                newX = thisX - speed;
            } else {
                newX = thisX + speed;
            }
            newY = m * newX + b;
            return new Vector3 (newX, newY, thisZ);
        } else {
            Debug.Log("aaa");
            // linear equation for XZ plane
            float mXZ = (headZ - thisZ)/(headX - thisX);
            float bXZ = headZ - (mXZ * headX);

            if(thisX > headX){
                newX = thisX - speed;
            } else {
                newX = thisX + speed;
            }
            newZ = mXZ * newX + bXZ;

            // linear equation for YX plane
            float mYZ = (headY - thisY)/(headZ - thisZ);
            float bYZ = headY - (mYZ * headZ);
            newY = mYZ * newZ + bYZ;

            return new Vector3 (newX, newY, newZ);
        }
    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        alreadyGrabbed = true;
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
            alreadyGrabbed = false;
            hasMoved = false;
            move = false;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public Grabber getCurrentGrabber(){
        return currentGrabber;
    }
}
