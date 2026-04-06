using UnityEngine;

public class CielingScript : MonoBehaviour
{
    void Start()
    {
        setReset();
    }

    public void setReset() //moves back to initial pos
    {
        transform.position = new Vector3(transform.position.x, 17f, transform.position.z);
    }

    public void StepDown() // moves down when called
    {
        transform.position += Vector3.down * 0.8f;
    }
}
