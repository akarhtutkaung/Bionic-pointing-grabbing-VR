using UnityEngine;

public class Grabbable : MonoBehaviour
{

    Grabber currentGrabber;
    Grabber currentSelectedGrabber;
    GameObject headset;
    bool move;
    bool grabbed;
    bool canFloat;
    bool headsetTriggered;
    float speed = 0.05f;
    float count = 0;

    float m_XZ, m_YZ, m_XY, b_XZ, b_YZ, b_XY;
    bool firstMovement;

    // Start is called before the first frame update
    void Start()
    {

        this.GetComponent<Outline>().setColor(new Color(0/255f, 180/255f, 3/255f, 255/255f));
        this.GetComponent<Outline>().setOutlineWidth(3);
        this.GetComponent<Outline>().setMode(Outline.Mode.OutlineVisible);
        this.GetComponent<Outline>().enabled = false;
        
        firstMovement = true;

        headsetTriggered = false;
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
                if(firstMovement){
                    formula();
                    firstMovement = false;
                }
                if(headsetTriggered == false) {
                    this.transform.position = calculate();
                } else {
                    canFloat = true;
                }
            }
            if(currentGrabber == null && canFloat){
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

        if(Mathf.Abs(thisX - headX) > Mathf.Abs(thisY - headY) && Mathf.Abs(thisX - headX) > Mathf.Abs(thisZ - headZ)){
            if(thisX > headX){
                newX = thisX - speed;
            } else {
                newX = thisX + speed;
            }
            newZ = m_XZ * newX + b_XZ;
            newY = m_XY * newX + b_XY;
        } else if (Mathf.Abs(thisY - headY) > Mathf.Abs(thisX - headX) && Mathf.Abs(thisY - headY) > Mathf.Abs(thisZ - headZ)){
            if(thisY > headY){
                newY = thisY - speed;
            } else {
                newY = thisY + speed;
            }
            newX = (newY - b_XY) / m_XY;
            newZ = (newY - b_YZ) / m_YZ;
        } else {
            if(thisZ > headZ){
                newZ = thisZ - speed;
            } else {
                newZ = thisZ + speed;
            }
            newX = (newZ - b_XZ) / m_XZ;
            newY = m_YZ * newZ + b_YZ;
        }
        return new Vector3 (newX, newY, newZ);
    }

    void formula() {
        float thisX = this.transform.position.x;
        float thisY = this.transform.position.y;
        float thisZ = this.transform.position.z;
        float headX = headset.transform.position.x;
        float headY = headset.transform.position.y;
        float headZ = headset.transform.position.z;
        
        m_XZ = (headZ - thisZ)/(headX - thisX);
        b_XZ = headZ - (m_XZ * headX);

        m_YZ = (headY - thisY)/(headZ - thisZ);
        b_YZ = headY - (m_YZ * headZ);

        m_XY = (headY - thisY)/(headX - thisX);
        b_XY = headY - (m_XY * headX);
    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        currentGrabber = grabber;
    }

    public void SetCurrentSelectedGrabber(Grabber grabber){
        currentSelectedGrabber = grabber;
    }

    public void setHeadsetTriggered(bool choice){
        headsetTriggered = choice;
    }

    public Grabber GetCurrentGrabber()
    {
        return currentGrabber;
    }

    public Grabber GetCurrentSelectedGrabber()
    {
        return currentSelectedGrabber;
    }

    public void zeroGravity(bool choice){
        if(choice){
            move = true;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        } else {
            move = false;
            firstMovement = true;
            canFloat = false;
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
