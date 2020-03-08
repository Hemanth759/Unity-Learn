using UnityEngine;

public class FollowWP : MonoBehaviour
{
    // public variables
    public GameObject[] waypoints;
    public float speed = 5f;
    public float turningSpeed = 5f;
    public float lookAHead = 10.0f;

    // private variables
    private int currentWP;
    private GameObject tracker;

    // Start is called before the first frame update
    void Start()
    {
        currentWP = 0;
        tracker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer> ().enabled = false;
        tracker.transform.position = this.transform.position;
        tracker.transform.rotation = this.transform.rotation;
    }

    void ProgressTracker() {
        if (Vector3.Distance(tracker.transform.position, this.transform.position) > lookAHead)
            return;
        if (Vector3.Distance(waypoints[currentWP].transform.position, tracker.transform.position) < 3)
            if (currentWP == (waypoints.Length-1))
                currentWP = 0;
            else 
                ++currentWP;

        tracker.transform.LookAt(waypoints[currentWP].transform.position);
        tracker.transform.Translate(0, 0, (speed + 20) * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        ProgressTracker();

        this.transform.Translate(0, 0, speed * Time.deltaTime);
        // this.transform.LookAt(waypoints[currentWP].transform.position);
        Quaternion lookAtWP = Quaternion.LookRotation(tracker.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtWP, turningSpeed * Time.deltaTime);
    }
}
