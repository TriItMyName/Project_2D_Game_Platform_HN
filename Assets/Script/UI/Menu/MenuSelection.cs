using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSelection : MonoBehaviour
{
    //Biến toàn cục
    public static MenuSelection instance;
    public RectTransform selectionBorder; // Border hoặc Icon
    public Button[] menuButtons;
    public CanvasGroup[] Menu;
    public GameObject leftBar, rightBar;  // Tham chiếu đến GameObject của Select Bar bên trái , phải
    public GameObject StartFirstOption, OptionMenuFirst, ClosedOptionMenu, ClosedCreditMenu, NotificationButton, LoadGameButton;
    public GameObject NotificationPanel;
    public float moveDistance = 10f; // Khoảng cách di chuyển
    public float speed = 1f; // Tốc độ di chuyển
    //public int MenuState = 0;
    public int currentIndex = 0;
    //Biến cục bộ
    private float inputDelay = 0.2f; // Thời gian chờ giữa các lần nhận input từ analog stick
    private float lastInputTime;
    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Animator left, right;

    void Start()
    {
        left = leftBar.gameObject.GetComponent<Animator>();
        right = rightBar.gameObject.GetComponent<Animator>();
        // Gán sự kiện hover cho mỗi nút
        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i; // Lưu chỉ số hiện tại vào biến local để sử dụng trong lambda
            EventTrigger trigger = menuButtons[i].gameObject.AddComponent<EventTrigger>();
            
            // Tạo sự kiện khi con trỏ chuột vào
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnButtonHover(index); });
            trigger.triggers.Add(entryEnter);
        }

        MoveSelectionBorder();

        leftStartPos = leftBar.transform.localPosition;
        rightStartPos = rightBar.transform.localPosition;

        //StartCoroutine(MoveBars());
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

    }
    void Update()
    {
        HandleInput();
    }

    // Safe helper to read CanvasGroup.alpha without throwing if destroyed or missing
    private bool TryGetAlpha(int index, out float alpha)
    {
        alpha = 0f;
        if (Menu == null) return false;
        if (index < 0 || index >= Menu.Length) return false;
        var cg = Menu[index];
        if (cg == null) return false; // Unity's == checks for destroyed too
        alpha = cg.alpha;
        return true;
    }

    public void HandleInput()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float dPagInput = Input.GetAxis("7th axis");
        bool moved = false;

        // Use safe check for Menu[0]
        if (TryGetAlpha(0, out float alpha0) && alpha0 >= 0.99f)
        {
            if(Input.anyKeyDown)
            {
                Debug.Log("AnyKeyPressed");
                StartCoroutine(ExecuteFadeOutInSequence());
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(StartFirstOption);
            }
        }
        else
        {
            // Check Menu[1]
            if (TryGetAlpha(1, out float alpha1) && alpha1 > 0.99f)
            {
                if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || ((dPagInput < -0.5f) && Time.time - lastInputTime > inputDelay) || verticalInput < -0.5f) && Time.time - lastInputTime > inputDelay)
                {
                    currentIndex = (currentIndex + 1) % menuButtons.Length;
                    MoveSelectionBorder();
                    lastInputTime = Time.time;
                    moved = true;
                }
                else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || ((dPagInput > 0.5f) && Time.time - lastInputTime > inputDelay) || verticalInput > 0.5f) && Time.time - lastInputTime > inputDelay)
                {
                    currentIndex = (currentIndex - 1 + menuButtons.Length) % menuButtons.Length;
                    MoveSelectionBorder();
                    lastInputTime = Time.time;
                    moved = true;
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    StartCoroutine(Click());
                }

                if (moved)
                {
                    lastInputTime = Time.time;
                }
            }
            else if (TryGetAlpha(3, out float alpha3) && alpha3 > 0.99f)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    FadeToMenu();
                }
            }
        }

    }

    public void FadeOutPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;
        StartCoroutine(FadeOut(canvasGroup, 0.5f));
    }
    public void FadeInPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;
        StartCoroutine(FadeIn(canvasGroup, 0.5f));
    }

    IEnumerator Click()
    {
        StartCoroutine(MoveBars());
        left.SetBool("Click",true);
        right.SetBool("Click", true);
        yield return new WaitForSeconds(1.3f);
        menuButtons[currentIndex].onClick.Invoke();
    }

    void OnButtonHover(int index)
    {
        currentIndex = index;
        MoveSelectionBorder();
    }

    void MoveSelectionBorder()
    {
        selectionBorder.position = menuButtons[currentIndex].transform.position;
        selectionBorder.sizeDelta = menuButtons[currentIndex].GetComponent<RectTransform>().sizeDelta;
        left.SetTrigger("Move");
        right.SetTrigger("Move");
    }
    IEnumerator MoveBars()
    {
            yield return MoveBarsToPosition(moveDistance);
            yield return MoveBarsToPosition(-moveDistance);
    }

    IEnumerator MoveBarsToPosition(float targetOffset)
    {
        float elapsedTime = 0f;
        Vector3 leftTargetPos = leftStartPos + new Vector3(targetOffset, 0, 0);
        Vector3 rightTargetPos = rightStartPos + new Vector3(-targetOffset, 0, 0);

        while (elapsedTime < speed)
        {
            leftBar.transform.localPosition = Vector3.Lerp(leftBar.transform.localPosition, leftTargetPos, elapsedTime / speed);
            rightBar.transform.localPosition = Vector3.Lerp(rightBar.transform.localPosition, rightTargetPos, elapsedTime / speed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leftBar.transform.localPosition = leftTargetPos;
        rightBar.transform.localPosition = rightTargetPos;
    }

    IEnumerator ExecuteFadeOutInSequence()
    {
        // Guard against destroyed entries
        if (TryGetAlpha(0, out _)) yield return StartCoroutine(FadeOut(Menu[0], 0.5f));
        if (TryGetAlpha(1, out _)) yield return StartCoroutine(FadeIn(Menu[1], 0.5f));
    }

    IEnumerator FadeOut(CanvasGroup canvasGroup, float _seconds)
    {
        if (canvasGroup == null) yield break;

        if (currentIndex == 2)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(OptionMenuFirst);
        }
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        left.SetBool("Click", false);
        right.SetBool("Click", false);
        yield return null;
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup, float _seconds)
    {
        if (canvasGroup == null) yield break;
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return null;
    }
    public void FadeToMenu()
    {
        if (TryGetAlpha(2, out float alpha2) && alpha2 == 1f)
        {
            StartCoroutine(FadeOut(Menu[2], 0.5f));
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(ClosedOptionMenu);
        }
        else if (TryGetAlpha(3, out float alpha3) && alpha3 == 1f)
        {
            StartCoroutine(FadeOut(Menu[3], 0.5f));
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(menuButtons[currentIndex].gameObject);
        }
        StartCoroutine(FadeIn(Menu[1], 0.5f));
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void LoadGame()
    {
        string playerDataPath = Application.persistentDataPath + "/save.player.data";
        string benchDataPath = Application.persistentDataPath + "/save.bench.data";
        if (File.Exists(playerDataPath) && File.Exists(benchDataPath))
        { 
            SaveData.Instance.LoadPlayerData(); 
        }
        else
        {
            NotificationPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(NotificationButton);
        }
    }
    public void NotificationPanelOff()
    {
        NotificationPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(LoadGameButton);
    }
}
