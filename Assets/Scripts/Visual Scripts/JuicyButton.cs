using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening; // НЕ ЗАБУДЬ ИМПОРТИРОВАТЬ DOTWEEN!

[RequireComponent(typeof(Image))]
// RequireComponent автоматически добавит AudioSource, если его нет
[RequireComponent(typeof(AudioSource))]
public class JuicyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float shrinkScale = 0.9f; // На сколько сжимается (1.0 = нет сжатия)
    [SerializeField] private float shrinkDuration = 0.1f; // Время сжатия
    [SerializeField] private float springDuration = 0.4f; // Время возврата (пружины)
    // Ease.OutBack дает тот самый эффект "перелета" и пружинистого возврата
    [SerializeField] private Ease springEase = Ease.OutBack;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip clickSound; // Перетащи сюда звук "клик деревом"
    [SerializeField] private float volume = 1f;
    [SerializeField] private float pitchRandomness = 0.1f; // Случайность высоты тона для естественности

    private RectTransform rectTransform;
    private AudioSource audioSource;
    private Vector3 originalScale;
    private Sequence animationSequence;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
        originalScale = rectTransform.localScale;

        // Настройки AudioSource, чтобы звук не перекрывал сам себя
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    // Срабатывает РОВНО в момент нажатия пальцем/мышкой
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayClickSound();
        AnimatePress();
    }

    // Срабатывает в момент отпускания
    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateRelease();
    }

    private void AnimatePress()
    {
        // Убиваем старую анимацию, если игрок спамит кликами
        animationSequence?.Kill();

        // Создаем новую цепочку анимации
        animationSequence = DOTween.Sequence()
            .Append(rectTransform.DOScale(originalScale * shrinkScale, shrinkDuration).SetEase(Ease.OutQuad))
            // Делаем цепочку не зависящей от Time.timeScale (работает даже на паузе)
            .SetUpdate(true);
    }

    private void AnimateRelease()
    {
        animationSequence?.Kill();

        animationSequence = DOTween.Sequence()
            // Возвращаемся к исходному размеру с эффектом пружины (OutBack)
            .Append(rectTransform.DOScale(originalScale, springDuration).SetEase(springEase))
            .SetUpdate(true);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            // Устанавливаем базовую громкость
            audioSource.volume = volume;
            // Добавляем легкую случайность в Pitch, чтобы звук дерева каждый раз был уникальным
            audioSource.pitch = 1f + Random.Range(-pitchRandomness, pitchRandomness);
            // Воспроизводим звук
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Гарантируем, что шкала вернется в норму, если объект выключится во время анимации
    void OnDisable()
    {
        animationSequence?.Kill();
        rectTransform.localScale = originalScale;
    }
}