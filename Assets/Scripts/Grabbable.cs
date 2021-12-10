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
    double diffThresh = 0.0000001;

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
                this.transform.position = calculate();
            }
            // float dist = Vector3.Distance(headset.transform.position, transform.position);
            // if(dist > 0 && dist < 1){
            
            if(Mathf.Abs(transform.position.x - headset.transform.position.x) < 0.8 && 
            Mathf.Abs(transform.position.y - headset.transform.position.y) < 0.8 && 
            Mathf.Abs(transform.position.z - headset.transform.position.z) < 0.8 && 
            currentGrabber == null){
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
                // float m = (headZ - thisZ)/(headX - thisX);
                // float b = headZ - (m * headX);

                // if(thisZ > headZ){
                //     newZ = thisZ - speed;
                // } else {
                //     newZ = thisZ + speed;
                // }
                // newX = m * newZ + b;
                // return new Vector3 (newX, thisY, newZ);
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
