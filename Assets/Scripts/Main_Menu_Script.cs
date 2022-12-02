using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Menu_Script : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;
    [SerializeField]
    private GameObject musicAudioPrefab;
    [SerializeField]
    private Scrollbar musicVolumeScrollbar;
    [SerializeField]
    private Scrollbar effectsVolumeScrollbar;

    private AudioSource musicAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("effects")) PlayerPrefs.SetFloat("effects", 1f);
        if (!PlayerPrefs.HasKey("music")) PlayerPrefs.SetFloat("music", 1f);

        if (GameObject.FindGameObjectWithTag("GameMusic") == null)
        {
            GameObject musicObj = Instantiate(musicAudioPrefab);
            musicAudioSource = musicObj.GetComponent<AudioSource>();
            DontDestroyOnLoad(musicObj);
        }
        else
        {
            musicAudioSource = GameObject.FindGameObjectWithTag("GameMusic").GetComponent<AudioSource>();
            musicAudioSource.volume = PlayerPrefs.GetFloat("music");
        }

        effectsVolumeScrollbar.value = PlayerPrefs.GetFloat("effects");
        musicVolumeScrollbar.value = PlayerPrefs.GetFloat("music");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
    }

    public void SetMusicVolume()
    {
        if (PlayerPrefs.HasKey("music"))
        {
            PlayerPrefs.SetFloat("music", musicVolumeScrollbar.value);
            musicAudioSource.volume = musicVolumeScrollbar.value;
        }

    }

    public void SetEffectsVolume()
    {
        if (PlayerPrefs.HasKey("effects"))
        {
            PlayerPrefs.SetFloat("effects", effectsVolumeScrollbar.value);
        }
    }
}
