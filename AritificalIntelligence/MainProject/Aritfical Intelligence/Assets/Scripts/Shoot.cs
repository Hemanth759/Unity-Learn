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
    private bool canShoot;

    // Start is called before the first frame update is happened
    void Start()
    {
        parent = this.transform.parent.transform.parent.gameObject;
        canShoot = true;
    }

    // Update is called once per frame
    void Update()
    {   
        Vector3 direction = (target.position - parent.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, lookRotation, turningSpeed * Time.deltaTime);

        float? angle = RotateTurrent();

        if (Vector3.Angle(direction, parent.transform.forward) < 10)
            Fire();
    }

    private void CanShootAgain() {
        canShoot = true;
    }

    private void Fire() {
        if (canShoot) {
            GameObject shell = Instantiate(bulletPrefab, spawnPosition.position, spawnPosition.rotation);
            shell.GetComponent<Rigidbody>().velocity = speed * this.transform.forward;
            canShoot = false;
            Invoke("CanShootAgain", 0.2f);
        }
    }

    private float? RotateTurrent() {
        float? angle = CalculateAngle(true);

        if (angle != null) {
            this.transform.localEulerAngles = new Vector3(360f - (float)angle, 0f, 0f);
        }
        return angle;
    }

    private float? CalculateAngle(bool low) {
        Vector3 targetDir = target.position - this.transform.position;
        float y = targetDir.y;
        targetDir.y = 0f;
        float x = targetDir.magnitude;
        float gravity = 9.81f;
        float sSqr = speed * speed;
        float underTheSqrRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

        if (underTheSqrRoot >= 0f) {
            float root = Mathf.Sqrt(underTheSqrRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            if (low)
                return (Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg);
            else 
                return (Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg);
        }
        return null;
    }
}
