using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class VRVideoListItem : MonoBehaviour
{
    public VRVideoPlayer videoPlayer;
    public VideoClip clip;
    public Button button;
    public TextMeshProUGUI label;

    void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnClick);

        if (label != null && clip != null)
            label.text = clip.name;
    }

    void OnClick()
    {
        if (videoPlayer != null && clip != null)
        {
            videoPlayer.LoadVideo(clip);
        }
    }
}
