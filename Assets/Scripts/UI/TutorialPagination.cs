using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialPagination : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup canvasGroup;
    public GameObject levelSelectPanel;

    [Header("Pages")]
    public GameObject[] pages;
    public Button nextButton;

    [Header("Button Images")]
    public Image nextButtonImage;
    public Sprite nextSprite;
    public Sprite playSprite;

    private int _currentPageIndex = 0;

    // Флаг, который скажет нам, запустили ли мы туториал из настроек
    private bool _isReplayingFromSettings = false;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    // 1. СТАРЫЙ МЕТОД: Вызывается кнопкой PLAY при старте игры
    public void StartTutorialFlow()
    {
        // ВРЕМЕННО ОТКЛЮЧАЕМ ПРОВЕРКУ ДЛЯ ТЕСТОВ:
        // if (PlayerPrefs.GetInt("TutorialDone", 0) == 1)
        // {
        //     OpenLevelSelect();
        //     return;
        // }

        _currentPageIndex = 0;
        UpdatePagesVisibility();

        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
    }

    // 2. НОВЫЙ МЕТОД: Будем вызывать его кнопкой из Настроек
    public void ShowTutorialOnDemand()
    {
        _isReplayingFromSettings = true; // Запомнили, что это повтор из меню
        ShowTutorialPanel();
    }

    // Вынесли общую логику показа в отдельный блок, чтобы не дублировать код
    private void ShowTutorialPanel()
    {
        _currentPageIndex = 0;
        UpdatePagesVisibility();

        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.9f, 0.9f, 1f); // Легкий зум при появлении

        canvasGroup.DOFade(1f, 0.5f);
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnNextButtonClicked()
    {
        nextButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 5);
        _currentPageIndex++;

        if (_currentPageIndex >= pages.Length)
        {
            FinishTutorial();
            return;
        }

        UpdatePagesVisibility();
    }

    private void UpdatePagesVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == _currentPageIndex);
        }

        if (nextButtonImage != null)
        {
            if (_currentPageIndex == pages.Length - 1)
            {
                // Если мы открыли из настроек, можно написать "Закрыть", 
                // но пока оставим картинку playSprite (В бой / Ок)
                nextButtonImage.sprite = playSprite;
            }
            else
            {
                nextButtonImage.sprite = nextSprite;
            }
        }
    }

    private void FinishTutorial()
    {
        // Сохраняем прогресс ТОЛЬКО если это первый запуск
        if (!_isReplayingFromSettings)
        {
            PlayerPrefs.SetInt("TutorialDone", 1);
            PlayerPrefs.Save();

            // Заранее включаем Level Select для красивого перехода
            levelSelectPanel.SetActive(true);
            CanvasGroup levelGroup = levelSelectPanel.GetComponent<CanvasGroup>();
            if (levelGroup == null) levelGroup = levelSelectPanel.AddComponent<CanvasGroup>();

            levelGroup.alpha = 0f;
            levelSelectPanel.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            levelGroup.DOFade(1f, 0.6f).SetEase(Ease.OutQuad);
            levelSelectPanel.transform.DOScale(1f, 0.6f).SetEase(Ease.OutCubic);
        }

        // Плавное закрытие самого свитка
        transform.DOScale(0.9f, 0.4f).SetEase(Ease.InQuad);
        canvasGroup.DOFade(0f, 0.4f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;

            // Открываем уровни, ТОЛЬКО если мы не в режиме повтора
            if (!_isReplayingFromSettings)
            {
                OpenLevelSelect();
            }
        });
    }

    private void OpenLevelSelect()
    {
        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
            CanvasGroup levelGroup = levelSelectPanel.GetComponent<CanvasGroup>();
            if (levelGroup == null) levelGroup = levelSelectPanel.AddComponent<CanvasGroup>();

            levelGroup.alpha = 0f;
            levelGroup.DOFade(1f, 0.5f);
        }
    }
}