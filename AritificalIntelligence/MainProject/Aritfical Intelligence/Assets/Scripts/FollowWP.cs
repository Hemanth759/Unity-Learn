using UnityEngine;

public class FollowWP : MonoBehaviour
{
    // public variables
    public GameObject[] waypoints;
    public float speed = 5f;
    public float turningSpeed = 5f;

    // private variables
    int currentWP;

    // Start is called before the first frame update
    void Start()
    {
        currentWP = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(waypoints[currentWP].transform.position, this.transform.position) < 3)
            if (currentWP == (waypoints.Length-1))
                currentWP = 0;
            else 
                ++currentWP;

        this.transform.Translate(0, 0, speed * Time.deltaTime);
        // this.transform.LookAt(waypoints[currentWP].transform.position);
        Quaternion lookAtWP = Quaternion.LookRotation(waypoints[currentWP].transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtWP, turningSpeed * Time.deltaTime);
    }
}
