using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class VRVideoPlayer : MonoBehaviour
{
    [Header("Core")]
    public VideoPlayer videoPlayer;
    public Renderer screenRenderer;
    public RawImage uiScreen; // opsional kalau pakai canvas screen

    [Header("Controls")]
    public Button playPauseButton;
    public GameObject playIcon;
    public GameObject pauseIcon;

    private bool isPlaying = false;

    void Start()
    {
        if (videoPlayer.targetTexture != null && screenRenderer != null)
            screenRenderer.material.mainTexture = videoPlayer.targetTexture;

        if (uiScreen != null && videoPlayer.targetTexture != null)
            uiScreen.texture = videoPlayer.targetTexture;

        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(TogglePlayPause);

        UpdateIcons();
    }

    public void TogglePlayPause()
    {
        if (!isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
        }
        else
        {
            videoPlayer.Pause();
            isPlaying = false;
        }
        UpdateIcons();
    }

    public void LoadVideo(VideoClip clip)
    {
        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Play();
        isPlaying = true;
        UpdateIcons();
    }

    void UpdateIcons()
    {
        if (playIcon) playIcon.SetActive(!isPlaying);
        if (pauseIcon) pauseIcon.SetActive(isPlaying);
    }
}
