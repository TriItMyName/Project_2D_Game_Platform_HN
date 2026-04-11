using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsMenu : MonoBehaviour
{
    [Header("UI Focus")]
    public GameObject backButton;

    void OnEnable()
    {
       
        if (backButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(backButton);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            GoBack();
        }
    }

    public void GoBack()
    {
        if (MenuSelection.instance != null)
        {
            MenuSelection.instance.FadeToMenu();
        }
        else
        {
            Debug.LogError("Không tìm thấy MenuSelection Instance trong Scene!");
        }
    }
}