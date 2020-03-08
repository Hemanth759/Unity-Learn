using UnityEngine;

public class Shoot : MonoBehaviour
{
    // public varaibles
    public GameObject bulletPrefab;
    public Transform spawnPosition;
    public Transform target;
    public float turningSpeed = 2f;
    public float speed = 15f;

    // private variables
    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Fire();
        
        Vector3 direction = (target.position - parent.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, lookRotation, turningSpeed * Time.deltaTime);

        float? angle = RotateTurrent();
    }

    void Fire() {
        GameObject shell = Instantiate(bulletPrefab, spawnPosition.position, spawnPosition.rotation);
    }

    float? RotateTurrent() {
        float? angle = CalculateAngle(true);

        if (angle != null) {
            this.transform.localEulerAngles = new Vector3(360 - (float)angle, 0f, 0f);
        }
        return angle;
    }

    float? CalculateAngle(bool low) {
        Vector3 targetDir = target.transform.position - this.transform.position;
        float y = targetDir.y;
        targetDir.y = 0f;
        float x = targetDir.magnitude;
        float gravity = 9.8f;
        float sSqr = speed * speed;
        float underSquareRoot = (sSqr * sSqr) - gravity * (gravity * x * x - 2 * y * sSqr);

        if (underSquareRoot >= 0) {
            float root = Mathf.Sqrt(underSquareRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            if (low)
                return Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg;
            else 
                return Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg;
        }
        return null;
    }
}
