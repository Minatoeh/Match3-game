using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

[RequireComponent(typeof(Button))]
public class PlayButtonAnimator : MonoBehaviour
{
    // Ссылка на менеджер туториала
    public TutorialPagination tutorialPagination;

    private RectTransform _rectTransform;
    private Button _button;
    private Vector2 _startPos;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();

        _startPos = _rectTransform.anchoredPosition;

        _button.onClick.AddListener(OnPlayClicked);
    }

    void Start()
    {
        StartIdleAnimation();
    }

    private void StartIdleAnimation()
    {
        _rectTransform.DOAnchorPosY(_startPos.y + 10f, 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(this);

        StartCoroutine(ShiverRoutine());
    }

    private IEnumerator ShiverRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));

            _rectTransform.DOShakeRotation(0.4f, new Vector3(0, 0, 4f), vibrato: 10, randomness: 90f)
                .SetId(this);
        }
    }

    // Оставили только ОДИН метод OnPlayClicked
    private void OnPlayClicked()
    {
        _button.interactable = false;
        DOTween.Kill(this);
        StopAllCoroutines();
        _rectTransform.localRotation = Quaternion.identity;

        _rectTransform.DOPunchScale(new Vector3(0.15f, 0.15f, 0f), 0.3f, 10, 1f)
            .OnComplete(() =>
            {
                if (tutorialPagination != null)
                {
                    // 1. Сначала запускаем туториал (или переход к уровням)
                    tutorialPagination.StartTutorialFlow();

                    // 2. Выключаем стартовый экран с задержкой 0.5 сек (пока идет анимация Fade)
                    // Используем удобную фичу DOTween для задержек:
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        if (transform != null && transform.parent != null)
                        {
                            transform.parent.gameObject.SetActive(false);
                        }
                    });
                }
                else
                {
                    Debug.LogError("Не назначена ссылка на TutorialPagination в инспекторе!");
                }
            });
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}