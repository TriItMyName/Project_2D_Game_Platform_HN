using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyCamera : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int[] destroyScenes = { 1, 3, 9 };
        if (System.Array.Exists(destroyScenes, s => s == scene.buildIndex))
        {
            Destroy(gameObject);
        }
    }
}
