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
    private Scrollbar musicVolumeScrollbar;
    [SerializeField]
    private Scrollbar effectsVolumeScrollbar;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("music")) PlayerPrefs.SetFloat("music", 1f);
        if (!PlayerPrefs.HasKey("effects")) PlayerPrefs.SetFloat("effects", 1f);
        musicVolumeScrollbar.value = PlayerPrefs.GetFloat("music");
        effectsVolumeScrollbar.value = PlayerPrefs.GetFloat("effects");
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
        PlayerPrefs.SetFloat("music", musicVolumeScrollbar.value);
    }

    public void SetEffectsVolume()
    {
        PlayerPrefs.SetFloat("effects", effectsVolumeScrollbar.value);
    }
}
