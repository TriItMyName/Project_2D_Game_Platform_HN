//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PauseMenuUI : MonoBehaviour
//{
//    public static PauseMenuUI Instance;

//    public bool GameIsPaused = false;

//    [SerializeField] FadeUI pauseMenu,OptionMenu;
//    [SerializeField] private CanvasGroup pausePanel, optionPanel;
//    [SerializeField] float fadeTime;
//    void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//        if (pauseMenu != null)
//        {
//            DontDestroyOnLoad(pauseMenu);
//        }
//        else
//        {
//            Destroy(pauseMenu);
//        }
//    }

//    public void Pause()
//    {
//        pauseMenu.FadeUIIn(pausePanel, fadeTime);
//        Time.timeScale = 0f;
//        GameIsPaused = true;
//    }
//    public void SaveGame()
//    {
//        SaveData.Instance.SavePlayerData();
//    }
//    public void Resume()
//    {
//        pauseMenu.FadeUIOut(pausePanel, fadeTime);
//        Time.timeScale = 1f;
//        GameIsPaused = false;
//    }
//    public void Option()
//    {
//        pauseMenu.FadeUIOut(pausePanel, fadeTime);
//        OptionMenu.FadeUIIn(optionPanel,fadeTime);
//    }
//    public void Back()
//    {
//        OptionMenu.FadeUIOut(optionPanel, fadeTime);
//        pauseMenu.FadeUIIn(pausePanel, fadeTime);
//    }
//    public void Quit()
//    {
//        Application.Quit();

//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
//    }
//}
using System.Collections;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    public bool GameIsPaused = false;

    [Header("UI References")]
    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private FadeUI optionMenu;

    [SerializeField] private CanvasGroup pausePanel;
    [SerializeField] private CanvasGroup optionPanel;

    [Header("Fade Settings")]
    [SerializeField] private float fadeTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (pauseMenu != null)
            DontDestroyOnLoad(pauseMenu.gameObject);
    }

    // Gọi khi pause game (mở menu)
    public void Pause()
    {
        if (pausePanel != null)
            StartCoroutine(FadeInPanel(pausePanel));

        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    // Gọi khi resume game (đóng menu)
    public void Resume()
    {
        if (pausePanel != null)
            StartCoroutine(FadeOutPanel(pausePanel));

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    // Gọi khi bấm nút Option
    public void Option()
    {
        StartCoroutine(SwitchMenu(pausePanel, optionPanel));
    }

    // Gọi khi bấm Back trong Option
    public void Back()
    {
        StartCoroutine(SwitchMenu(optionPanel, pausePanel));
    }

    // Lưu game (nếu bạn có hệ thống SaveData)
    public void SaveGame()
    {
        SaveData.Instance.SavePlayerData();
    }

    // Thoát game
    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator FadeInPanel(CanvasGroup panel)
    {
        panel.gameObject.SetActive(true);
        panel.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            yield return null;
        }
        panel.alpha = 1f;
    }

    private IEnumerator FadeOutPanel(CanvasGroup panel)
    {
        panel.blocksRaycasts = false;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            yield return null;
        }
        panel.alpha = 0f;
        panel.gameObject.SetActive(false);
    }

    // Chuyển từ panel A sang panel B (Option <-> Pause)
    private IEnumerator SwitchMenu(CanvasGroup from, CanvasGroup to)
    {
        yield return StartCoroutine(FadeOutPanel(from));
        yield return StartCoroutine(FadeInPanel(to));
    }
}
