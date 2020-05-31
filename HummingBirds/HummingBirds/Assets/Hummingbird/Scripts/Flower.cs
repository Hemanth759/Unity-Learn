using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a single flower with nectar
/// </summary>
public class Flower : MonoBehaviour
{
    [Tooltip("The color when the flower is full")]
    public Color fullFlowerColor = new Color(1f, 0f, .3f);

    [Tooltip("The color when the flower is empty")]
    public Color emptyFlowerColor = new Color(0.5f, 0f, 1f);

    /// <summary>
    /// The trigger collider representing the nector
    /// </summary>
    [HideInInspector]
    public Collider nectorCollider;

    // The solid collider representing the flower petals
    private Collider flowerCollider;

    // The flower's material
    private Material flowerMaterial;

    /// <summary>
    /// A vector pointing straight out of the flower
    /// </summary>
    /// <value></value>
    public Vector3 FlowerUpVector
    {
        get
        {
            return nectorCollider.transform.up;
        }
    }

    /// <summary>
    /// The center position of the nector collider
    /// </summary>
    /// <value></value>
    public Vector3 FlowerCenterPosition
    {
        get
        {
            return nectorCollider.transform.position;
        }
    }

    /// <summary>
    /// The amount of the nector in the flower
    /// </summary>
    /// <value></value>
    public float NectorAmount { get; private set; }

    /// <summary>
    /// Weather the flower has nector remaining
    /// </summary>
    /// <value></value>
    public bool HasNector
    {
        get
        {
            return NectorAmount > 0;
        }
    }

    /// <summary>
    /// Attempts to remove nectar from the flower
    /// </summary>
    /// <param name="amount">The amount of nector to remove</param>
    /// <returns>The actual amount successfully removed</returns>
    public float Feed(float amount){
        // Track how much nectar was successfully taken (cannot take more then the available)
        float nectarTaken = Mathf.Clamp(amount, 0f, NectorAmount);

        // Subtract the nectar
        NectorAmount -= nectarTaken;

        if (!HasNector) 
        {
            // Disable the flower and nectar colliders
            flowerCollider.gameObject.SetActive(false);
            nectorCollider.gameObject.SetActive(false);

            // Change the color of the flower
            flowerMaterial.SetColor("_BaseColor", emptyFlowerColor);
        }

        // Return the amount of nectar that was taken
        return nectarTaken;
    }

    /// <summary>
    /// Resets the flower
    /// </summary>
    public void ResetFlower()
    {
        // Refill the nectar amount
        NectorAmount = 1f;

        // Enable the flower and nectar colliders
        flowerCollider.gameObject.SetActive(true);
        nectorCollider.gameObject.SetActive(true);

        // Change the color of the flower to indicate that the flower is full
        flowerMaterial.SetColor("_BaseColor", fullFlowerColor);
    }

    /// <summary>
    /// Called whent he flower wakes up
    /// </summary>
    private void Awake() 
    {
        // Find the flower's mesh render and get the main material
        flowerMaterial = this.GetComponent<MeshRenderer>().material;

        // Find the flower and nectar colliders
        flowerCollider = this.transform.Find("FlowerCollider").GetComponent<Collider>();
        nectorCollider = this.transform.Find("FlowerNectarCollider").GetComponent<Collider>();
    }
}
