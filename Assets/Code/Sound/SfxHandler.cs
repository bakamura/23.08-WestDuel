using UnityEngine;

public class SfxHandler : MonoBehaviour {

    [Header("UI SFX List")]

    [SerializeField] private AudioClip _uiClickSfx;
    private float _uiClickDuration;
    private float _uiClickCurrentCooldown = 0;

    [Header("Cache")]

    private AudioSource _as;

    protected void Awake() {
        _as = GetComponent<AudioSource>();

        _uiClickDuration = _uiClickSfx.length;
    }

    private void Update() {
        _uiClickCurrentCooldown -= Time.deltaTime;
    }

    private void PlaySfx(AudioClip sfx) {
        _as.PlayOneShot(sfx);
    }

    public void UiClickSfx() {
        if (_uiClickCurrentCooldown <= 0) {
            _uiClickCurrentCooldown = _uiClickDuration;
            PlaySfx(_uiClickSfx);
        }
    }

}
