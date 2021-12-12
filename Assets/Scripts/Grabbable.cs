using UnityEngine;

public class Grabbable : MonoBehaviour
{

    private Grabber currentGrabber;
    private Vector3 originalScale;
    private Vector3 Scale;
    public bool stored;
    // Start is called before the first frame update
    void Start()
    {
        stored = false;
        currentGrabber = null;

        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().Sleep();
        }

        originalScale = this.transform.localScale;
    }

    public void SetCurrentGrabber(Grabber grabber)
    {
        currentGrabber = grabber;
    }

    public Grabber GetCurrentGrabber()
    {
        return currentGrabber;
    }

    public Vector3 getScale()
    {
        return this.originalScale;
    }

    public void setScale(Vector3 scale)
    {
        this.Scale = scale;
    }
}
