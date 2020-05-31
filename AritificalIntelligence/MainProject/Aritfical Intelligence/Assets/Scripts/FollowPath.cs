using UnityEngine;

public class FollowPath : MonoBehaviour
{
    Transform goal;
    float speed = 5.0f;
    float accuracy = 1.0f;
    float rotSpeed = 2.0f;
    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;

    void Start () {
        wps = wpManager.GetComponent<WPManager> ().waypoints;
        g = wpManager.GetComponent<WPManager> ().graph;
        currentNode = wps[0];
    }

    public void gotToHeli() {
        g.AStar(currentNode, wps[9]);
        currentWP = 0;
    }

    public void goToRuin() {
        g.AStar(currentNode, wps[12]);
        currentWP = 0;
    }

    void LateUpdate () {
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
            return;

        // the node we are closest to at this moment
        currentNode = g.getPathPoint(currentWP);

        // if we are close enough to the current waypoint move to next
        if (Vector3.Distance(g.getPathPoint(currentWP).transform.position, this.transform.position) < accuracy) {
            currentWP++;
        }

        // if we are not at the end of the path
        if (currentWP < g.getPathLength()) {
            goal = g.getPathPoint(currentWP).transform;
            Vector3 lookAtGoal = new Vector3(goal.position.x, this.transform.position.y, goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }
}
