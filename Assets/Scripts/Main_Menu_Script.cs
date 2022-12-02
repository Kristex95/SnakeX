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
    private AudioSource musicAudio;
    [SerializeField]
    private Scrollbar musicVolumeScrollbar;
    [SerializeField]
    private Scrollbar effectsVolumeScrollbar;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("effects")) PlayerPrefs.SetFloat("effects", 1f);
        effectsVolumeScrollbar.value = PlayerPrefs.GetFloat("effects");
        DontDestroyOnLoad(musicAudio);
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
        musicAudio.volume = musicVolumeScrollbar.value;
    }

    public void SetEffectsVolume()
    {
        PlayerPrefs.SetFloat("effects", effectsVolumeScrollbar.value);
    }
}
