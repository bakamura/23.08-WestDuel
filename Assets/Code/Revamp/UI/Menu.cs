using System.Collections;
using UnityEngine;

public abstract class Menu : MonoBehaviour {

    [Header("Fade Transition")]

    [Tooltip("Duration of the fade in and out")]
    [SerializeField] private float _fadeDuration;
    [Tooltip("The time between ending fade out and starting fade in")]
    [SerializeField] private float _fadeOpenDelay;
    protected CanvasGroup _currentUi;

    [Header("Move Transition")]

    [Tooltip("Duration of the move in and out")]
    [SerializeField] private float _moveDuration;
    [Tooltip("The time between ending move out and starting move in")]
    [SerializeField] private float _moveOpenDelay;

    [Header("Cache")]

    private float _floatC;
    private WaitForSeconds _fadeDelayWait;
    private WaitForSeconds _moveDelayWait;

    protected virtual void Awake() {
        _fadeDelayWait = new WaitForSeconds(_fadeOpenDelay);
        _moveDelayWait = new WaitForSeconds(_moveOpenDelay);
    }

    protected void OpenUIFade(CanvasGroup fadeIn, CanvasGroup newActiveCanvas = null) {
        StartCoroutine(FadeUITransition(_currentUi, fadeIn, newActiveCanvas));
    }

    private IEnumerator FadeUITransition(CanvasGroup fadeOut, CanvasGroup fadeIn, CanvasGroup newActiveCanvas) {
        fadeOut.interactable = false;
        fadeOut.blocksRaycasts = false;
        while (fadeOut.alpha > 0) {
            fadeOut.alpha -= Time.deltaTime / _fadeDuration;

            yield return null;
        }
        fadeOut.alpha = 0;

        yield return _fadeDelayWait;

        while (fadeIn.alpha < 1) {
            fadeIn.alpha += Time.deltaTime / _fadeDuration;

            yield return null;
        }
        fadeIn.alpha = 1;
        fadeIn.interactable = true;
        fadeIn.blocksRaycasts = true;

        _currentUi = newActiveCanvas == null ? fadeIn : newActiveCanvas;
    }

    protected void OpenUIMove(RectTransform moveOut, RectTransform moveIn, Vector2 moveOutActivePos, Vector2 moveOutDeactivePos, Vector2 moveInActivePos, Vector2 moveInDeactivePos) {
        StartCoroutine(MoveUITransition(moveOut, moveIn, moveOutActivePos, moveOutDeactivePos, moveInActivePos, moveInDeactivePos));
    }



    private IEnumerator MoveUITransition(RectTransform moveOut, RectTransform moveIn, Vector2 moveOutActivePos, Vector2 moveOutDeactivePos, Vector2 moveInActivePos, Vector2 moveInDeactivePos) {
        if (moveOut != null) {
            _floatC = 1;
            while (_floatC > 0) {
                moveOut.anchoredPosition = Vector2.Lerp(moveOutDeactivePos, moveInActivePos, _floatC);
                _floatC -= Time.deltaTime / _moveDuration;

                yield return null;
            }
            moveOut.anchoredPosition = moveOutDeactivePos;
        }


        if (moveIn != null) {
            yield return _moveDelayWait;

            _floatC = 0;
            while (_floatC < 1) {
                moveIn.anchoredPosition = Vector2.Lerp(moveInDeactivePos, moveInActivePos, _floatC);
                _floatC += Time.deltaTime / _moveDuration;

                yield return null;
            }
            moveIn.anchoredPosition = moveInActivePos;
        }
    }

}
