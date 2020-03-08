using UnityEngine;

public class Shell : MonoBehaviour
{
    public GameObject explosion;
    // public float mass = 10f;
    // public float force = 200;
    // public float speedZ;
    // public float speedY;

    // // private varaibles
    // private float gravity = -9.8f;


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "tank")
        {
            GameObject exp = Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(exp, 0.5f);
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // speedZ = force / mass;
        // speedY = 0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // speedY += gravity / mass;
        // this.transform.Translate(0f, speedY * Time.deltaTime, speedZ * Time.deltaTime);
    }
}
