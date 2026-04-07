using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialPagination : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup canvasGroup;
    public GameObject levelSelectPanel;

    [Header("Pages")]
    public GameObject[] pages;
    public Button nextButton;
    public Button prevButton; // ДОБАВИЛИ: Ссылка на кнопку "Назад"

    [Header("Button Images")]
    public Image nextButtonImage;
    public Sprite nextSprite;
    public Sprite playSprite;

    private int _currentPageIndex = 0;
    private bool _isReplayingFromSettings = false;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        nextButton.onClick.AddListener(OnNextButtonClicked);

        // Подключаем кнопку "Назад", если она назначена в инспекторе
        if (prevButton != null)
        {
            prevButton.onClick.AddListener(OnPrevButtonClicked);
        }
    }

    public void StartTutorialFlow()
    {
        _currentPageIndex = 0;
        UpdatePagesVisibility();

        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
    }

    public void ShowTutorialOnDemand()
    {
        _isReplayingFromSettings = true;
        ShowTutorialPanel();
    }

    private void ShowTutorialPanel()
    {
        _currentPageIndex = 0;
        UpdatePagesVisibility();

        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        canvasGroup.DOFade(1f, 0.5f);
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnNextButtonClicked()
    {
        nextButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 5);
        _currentPageIndex++;

        // Если дошли до конца — закрываем туториал
        if (_currentPageIndex >= pages.Length)
        {
            FinishTutorial();
            return;
        }

        UpdatePagesVisibility();
    }

    // НОВЫЙ МЕТОД: Обработка клика "Назад"
    private void OnPrevButtonClicked()
    {
        prevButton.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.2f, 5);

        _currentPageIndex--;
        if (_currentPageIndex < 0) _currentPageIndex = 0;

        UpdatePagesVisibility();
    }

    private void UpdatePagesVisibility()
    {
        // 1. Включаем нужную страницу, выключаем остальные
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == _currentPageIndex);
        }

        // 2. Логика отображения кнопки "Назад"
        if (prevButton != null)
        {
            // Показываем кнопку только если мы НЕ на первой странице
            prevButton.gameObject.SetActive(_currentPageIndex > 0);
        }

        // 3. Логика смены иконки на кнопке "Далее / Старт"
        if (nextButtonImage != null)
        {
            if (_currentPageIndex == pages.Length - 1)
            {
                nextButtonImage.sprite = playSprite; // Последняя страница
            }
            else
            {
                nextButtonImage.sprite = nextSprite; // Все остальные страницы
            }
        }
    }

    private void FinishTutorial()
    {
        if (!_isReplayingFromSettings)
        {
            levelSelectPanel.SetActive(true);
            CanvasGroup levelGroup = levelSelectPanel.GetComponent<CanvasGroup>();
            if (levelGroup == null) levelGroup = levelSelectPanel.AddComponent<CanvasGroup>();

            levelGroup.alpha = 0f;
            levelSelectPanel.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            levelGroup.DOFade(1f, 0.6f).SetEase(Ease.OutQuad);
            levelSelectPanel.transform.DOScale(1f, 0.6f).SetEase(Ease.OutCubic);
        }

        transform.DOScale(0.9f, 0.4f).SetEase(Ease.InQuad);
        canvasGroup.DOFade(0f, 0.4f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;

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