using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class BandScript : MonoBehaviour
{
    [Range(1, 100)] public float bandSpeed = 10f;
    public Vector3 bandDirection = -Vector3.forward;
    //public ForceMode bandForceMode;//For Testing

    void Start()
    {

    }
    void OnCollisionStay(Collision collision)
    {
        Rigidbody objectRb = collision.rigidbody;
        if (objectRb != null)
        {
            Vector3 targerVelocity = bandDirection.normalized * bandSpeed;
            objectRb.linearVelocity= new Vector3(targerVelocity.x, targerVelocity.y, targerVelocity.z);
            //objectRb.AddForce(bandDirection * bandSpeed, bandForceMode); Deactived for optimzation
        }
    }
}
