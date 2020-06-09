using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Tooltip("The reload time taken to reload the scene in seconds")]
    /// <summary>
    /// The reload time taken to reload the scene in seconds
    /// </summary>
    public float reloadTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ReloadScene", reloadTime);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
