using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// A Humming bird Machine Learning Agent
/// </summary>
public class HummingBirdAgent : Agent
{
    [Tooltip("Force to apply when moving")]
    public float moveForce = 2f;

    [Tooltip("Pitch to move up and down")]
    public float pitchSpeed = 100f;

    [Tooltip("Speed to rotate around the up axis")]
    public float yawSpeed = 100f;

    [Tooltip("Transform at the tip of bird beak")]
    public Transform beakTip;

    [Tooltip("The agent's camera")]
    public Camera agentCamera;

    [Tooltip("Whether this is training mode or gameplay mode")]
    public bool trainingMode;

    // The Rigidbody of the agent
    new private Rigidbody rigidbody;

    // The flower area the bird is in
    private FlowerArea flowerArea;

    // The nearest flower to the agent
    private Flower nearestFlower;

    // Allows for smoother pitch changes
    private float smoothPitchChange = 0f;

    // Allow for smoother yaw changes
    private float smoothYawChange = 0f;

    // Maximum angle that the bird can pitch up or down
    private const float MaxPitchAngle = 80f;

    // Maximum distance from the beak tip to accept nectar collision
    private const float BeakTipRadius = 0.008f;

    // Whether the agent is frozen (Intentionally not flying)
    private bool frozen = false;

    /// <summary>
    /// The amount of nectar the agent has obtained this episode
    /// </summary>
    /// <value></value>
    public float NectarObtained { get; private set; }

    /// <summary>
    /// Initializes the agent
    /// </summary>
    public override void Initialize() 
    {
        rigidbody = this.GetComponent<Rigidbody>();
        flowerArea = this.GetComponentInParent<FlowerArea>();

        // If not training mode, no max step, play forever
        if (!trainingMode) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        if (trainingMode) 
        {
            // Only reset the flowers in training when there is one agent per area
            flowerArea.ResetFlowers();
        }

        // Reset nectar obtained
        NectarObtained = 0f;

        // Zero out velocities so that the movement stops before a new episode begins
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        // Default to spawning in front of the flower
        bool inFrontOfFlower = true;
        if (trainingMode)
        {
            // Spawn in front of the flower 50% of the time during training
            inFrontOfFlower = UnityEngine.Random.value > 0.5f;
        }

        // Move the agent to a new rangom position
        MoveToSageRandomPosition(inFrontOfFlower);

        // Recalculate the nearest flower now that the agent has moved to a new random position
        UpdateNearestFlower();
    }

    /// <summary>
    /// Called when the action is received from either input from the player or from the neural network
    /// 
    /// vectorAction[i] represents:
    /// Index 0: move vector x (+1 = right, -1 = left)
    /// Index 1: move vector y (+1 = up, -1 = dowm)
    /// Index 2: move vector z (+1 = forward, -1 = backward)
    /// Index 3: pitch angle (+1 = pitch up, -1 = pitch down)
    /// Index 4: yaw angle (+1 = turn right, -1 = turn left)
    /// </summary>
    /// <param name="vectorAction">The actions to take</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        // Don't take any actions if frozen
        if (frozen) return;

        // Calculate the movement vector
        Vector3 move = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]);

        // Add force in the direction of the move vector
        rigidbody.AddForce(move * moveForce);

        // Get the current rotation
        Vector3 rotationVector = this.transform.rotation.eulerAngles;

        // Calculate pitch and yaw rotation
        float pitchChange = vectorAction[3];
        float yawChange = vectorAction[4];

        // Calculate smooth rotation changes
        smoothPitchChange = Mathf.MoveTowards(smoothPitchChange, pitchChange, 2f * Time.fixedDeltaTime);
        smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);

        // Calculate new pitch and yaw values based on smooth values
        // Clamp pitch to avoid flipping upside down
        float pitch = rotationVector.x + smoothPitchChange * Time.fixedDeltaTime * pitchSpeed;
        if (pitch > 180f) pitch -= 360;
        pitch = Mathf.Clamp(pitch, -MaxPitchAngle, MaxPitchAngle);

        float yaw = rotationVector.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

        // Apply the new rotation
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    /// <summary>
    /// Collect vector observations from the environment
    /// </summary>
    /// <param name="sensor">The vector sersor</param>
    public override void CollectObservations(VectorSensor sensor) 
    {
        // If the nearest flower is null, Observe an empty array and return early
        if (nearestFlower == null) 
        {
            sensor.AddObservation(new float[10]);
            return;
        }

        // Observe the agent's local rotation (4 Observaions)
        sensor.AddObservation(transform.localRotation.normalized);

        // Get a vector from the beakTip to the nearest flower
        Vector3 toFlower = nearestFlower.FlowerCenterPosition - beakTip.position;

        // Observe a normalized vector pointing to the nearest flower (3 observations)
        sensor.AddObservation(toFlower.normalized);

        // Observe a dot product that indicates whether the beaktip is in front of the flower or behind the flower (1 observation)
        // (+1 indicates that the beakTip is infront of the flower, -1 indicates that the beakTip is behind the flower)
        sensor.AddObservation(Vector3.Dot(toFlower.normalized, -nearestFlower.FlowerUpVector.normalized));

        // Observe a dot product that indicates whether the beaktip is pointing towards the flower (1 observation)
        // (+1 means that the beaktip is pointing directly at the flower, -1 means directly away)
        sensor.AddObservation(Vector3.Dot(beakTip.forward.normalized, -nearestFlower.FlowerUpVector.normalized));

        // Observe the realative distance from the beaktip to the flower (1 observation)
        sensor.AddObservation(toFlower.magnitude / FlowerArea.areaDiameter);

        // 10 Total observations
    }

    /// <summary>
    /// When behaviour type is set to "heuristic only" on the agent's Behaviour parameters,
    /// this function will be called. Its return values will be fed into 
    /// <see cref="OnActionReceived(float[])"> instead of using the neural networks
    /// </summary>
    /// <param name="actionsOut">An output action array</param>
    public override void Heuristic(float[] actionsOut)
    {
        // Create place holders for all movements and turnings
        Vector3 forward = Vector3.zero;
        Vector3 left = Vector3.zero;
        Vector3 up = Vector3.zero;
        float pitch = 0f;
        float yaw = 0f;

        // Convert keyboard inputs to movements and turning 
        // All values should be between -1 and +1

        // Forward/Backwards
        if (Input.GetKey(KeyCode.W)) forward = transform.forward;
        else if(Input.GetKey(KeyCode.S)) forward = -transform.forward;

        // Left/Right
        if (Input.GetKey(KeyCode.A)) left = -transform.right;
        else if (Input.GetKey(KeyCode.D)) left = transform.right;

        // Up/Down
        if (Input.GetKey(KeyCode.E)) up = transform.up;
        else if (Input.GetKey(KeyCode.C)) up = -transform.up;

        // Pitch up/down
        if (Input.GetKey(KeyCode.UpArrow)) pitch = 1f;
        else if (Input.GetKey(KeyCode.DownArrow)) pitch = -1f;

        // yaw left/right
        if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
        else if (Input.GetKey(KeyCode.RightArrow)) yaw = 1f;

        // Combine the movement vectors and normalize
        Vector3 combined = (forward + left + up).normalized;

        // Add the 3 movement values, pitch, yaw to the actions out array
        actionsOut[0] = combined.x;
        actionsOut[1] = combined.y;
        actionsOut[2] = combined.z;
        actionsOut[3] = pitch;
        actionsOut[4] = yaw;
    }

    /// <summary>
    /// Prevent the agent from moving and taking actions
    /// </summary>
    public void FreezeAgent() 
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = true;
        rigidbody.Sleep();
    }

    /// <summary>
    /// Resume the agent moving and take actions
    /// </summary>
    public void UnfreezeAgent() 
    {
        Debug.Assert(trainingMode == false, "Freeze/Unfreeze not supported in training");
        frozen = false;
        rigidbody.WakeUp();
    }

    /// <summary>
    /// Update the nearest flower to the agent
    /// </summary>
    private void UpdateNearestFlower()
    {
        foreach (Flower flower in flowerArea.Flowers)
        {
            if (nearestFlower == null && flower.HasNector) 
            {
                // No current nearest flower and this flower has nectar, so set to this flower
                nearestFlower = flower;
            }
            else if (flower.HasNector) 
            {
                // Calculate distance to this flower and distance to the current nearest flower
                float distanceToflower = Vector3.Distance(beakTip.position, flower.transform.position);
                float distanceToCurrentNearestFlower = Vector3.Distance(beakTip.position, nearestFlower.transform.position);

                // If current nearest flower is empty OR this flower is closer, update the nearest flower]
                if (!nearestFlower.HasNector || distanceToflower < distanceToCurrentNearestFlower)
                {
                    nearestFlower = flower;
                }
            }
        }
    }

    /// <summary>
    /// Move the agent to a safe random position (i.e. does not collide with anything)
    /// If in front of flower, also point the beak at the flower
    /// </summary>
    /// <param name="inFrontOfFlower">Whether to choose a spot in front of flower</param>
    private void MoveToSageRandomPosition(bool inFrontOfFlower)
    {
        bool sagePositionFound = false;
        int attemptsRemaining = 100; // Prevents the infinite loop
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        // Loop until safe place is found or we run out of attempts
        while(!sagePositionFound && attemptsRemaining > 0)
        {
            --attemptsRemaining;
            if (inFrontOfFlower) 
            {
                // Pick a random flower
                Flower randomFlower = flowerArea.Flowers[UnityEngine.Random.Range(0, flowerArea.Flowers.Count)];

                // Position 10 to 20 cm in front of the flower
                float distanceFromFlower = UnityEngine.Random.Range(0.1f, 0.2f);
                potentialPosition = randomFlower.transform.position + randomFlower.FlowerUpVector * distanceFromFlower;

                // Point beak at flower(bird's head is center of tranform)
                Vector3 toFlower = randomFlower.FlowerCenterPosition - potentialPosition;
                potentialRotation = Quaternion.LookRotation(toFlower, Vector3.up);
            }
            else 
            {
                // Pick a random height from the ground 
                float height = UnityEngine.Random.Range(1.2f, 2.5f);

                // Pick a random radius from the center of the area
                float radius = UnityEngine.Random.Range(2f, 7f);

                // Pick a random rotation rotated around the Y axis
                Quaternion direction = Quaternion.Euler(0f, UnityEngine.Random.Range(-180f,180f),0f);

                // Combine height, radius and direction to pick a potential direction
                potentialPosition = flowerArea.transform.position + Vector3.up * height + Vector3.forward * radius;

                // Choose and set random starting pitch and yaw
                float pitch = UnityEngine.Random.Range(-60f, 60f);
                float yaw = UnityEngine.Random.Range(-180f, 180f);
                potentialRotation = Quaternion.Euler(pitch, yaw, 0f); 
            }

            // Check if the potential position and rotation are no colliding with any game object
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 0.05f);

            // Safe posiion found if no colliders are found
            sagePositionFound = colliders.Length == 0;
        }

        Debug.Assert(sagePositionFound, "Cound not find a sage position to spawn");

        // Set the position and rotation
        transform.position = potentialPosition;
        transform.rotation = potentialRotation;
    }

    /// <summary>
    /// Called when agent's collider enters a other trigger collider
    /// </summary>
    /// <param name="other">The Other trigger gaemeobject</param>
    private void OnTriggerEnter(Collider other) 
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// Called when agent's collider stays in a other trigger collider
    /// </summary>
    /// <param name="other">The Other trigger gaemeobject</param>
    
    private void OnTriggerStay(Collider other) {
        TriggerEnterOrStay(other);    
    }

    /// <summary>
    /// Handles when a agent's collider enters or stays in other trigger collider
    /// </summary>
    /// <param name="collider">The Trigger collider</param>
    private void TriggerEnterOrStay(Collider collider)
    {
        // Check if agent is colliding with nectar
        if (collider.CompareTag("nectar"))
        {
            Vector3 closestPointToBeakTip = collider.ClosestPoint(beakTip.position); 

            // Check if the closest collision point is close to the beaktip
            // Note: a collision with anything but beaktip should not count
            if (Vector3.Distance(beakTip.position, closestPointToBeakTip) < BeakTipRadius)
            {
                // Look up the flower for this nectar collider
                Flower flower = flowerArea.GetFlowerFromNectar(collider);

                // Attempt to take 0.01 nectar
                // Note: this is per fixed timestep, meaning it happens every 0.02 seconds, or 50x per second
                float nectarReceived = flower.Feed(0.01f);

                // Keep track of nectar obtained
                NectarObtained += nectarReceived;

                if (trainingMode)
                {
                    // Calculate reward for getting nectar
                    float bonus = 0.02f * Mathf.Clamp01(Vector3.Dot(transform.forward.normalized, -nearestFlower.FlowerUpVector.normalized));
                    AddReward(0.01f + bonus);
                } 

                // If the flower is empty update the nearest flower
                if (!flower.HasNector) 
                {
                    UpdateNearestFlower();
                }
            }
        }
    }

    /// <summary>
    /// Called when the agent collides with something solid
    /// </summary>
    /// <param name="collision">The collision info</param>
    private void OnCollisionEnter(Collision collision) 
    {
        if (trainingMode && collision.collider.CompareTag("boundary"))
        {
            // Collided with the area boundary, give a negative reward
            AddReward(-0.5f);
        }
    }

    /// <summary>
    /// Called every fram
    /// </summary>
    private void Update()
    {
        // Draw a line from the beatip to the nearest flower center
        if (nearestFlower != null)
            Debug.DrawLine(beakTip.position, nearestFlower.FlowerCenterPosition, Color.green);    
    }

    /// <summary>
    /// Called every 0.02 seconds
    /// </summary>
    private void FixedUpdate() 
    {
        // Avoids the csae where the nectar of the nearest flower is stolen by other bird and not getting updated in the nearest flower
        if (nearestFlower != null && !nearestFlower.HasNector) 
            UpdateNearestFlower();
    }
}
