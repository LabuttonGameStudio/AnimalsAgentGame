using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPause : MonoBehaviour
{
    [Header("OPTIONS")]
    public GameObject Menu;
    public GameObject Configs;
    public GameObject Controls;
    public GameObject Exiit;

    [Header("CONFIGS")]
    public TMP_Dropdown resolution;
    public Toggle fullscreen;
    public Toggle window;
    
    private Resolution[] resolutions = new Resolution[]
     {
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 1366, height = 768 }
     };

    [Header("AUDIO")]
    public Slider Volume;

    //playerprefs - salvar
    private const string VolumePrefKey = "MasterVolume";
    private const string ResolutionPrefKey = "ScreenResolution";
    private const string ScreenModePrefKey = "ScreenMode";

    void Start()
    {
        #region Sound

        // Carrega o volume salvo, se existir, caso contrario usa 1.0
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1.0f);

        // volume inicial do AudioListener e o valor do slider
        AudioListener.volume = savedVolume;
        Volume.value = savedVolume;

       Volume.onValueChanged.AddListener(SetVolume);

        #endregion

        #region Resolution

        // carrega a resolucao salva, caso contrario usa a resolucao padrao (1920x1080)
        int savedResolutionIndex = PlayerPrefs.GetInt(ResolutionPrefKey, 1);
        resolution.value = savedResolutionIndex;

        // Aplica a resolucao salva
        ApplyResolution(savedResolutionIndex);

        resolution.onValueChanged.AddListener(OnResolutionChange);

        #endregion

        #region WindowMode
        
        int savedScreenMode = PlayerPrefs.GetInt(ScreenModePrefKey, 1);

        ApplyScreenMode(savedScreenMode);

        fullscreen.isOn = savedScreenMode == 1;
        window.isOn = savedScreenMode == 0;

        // adiciona listener aos toggles para chamar as funcoes quando os valores mudar
        fullscreen.onValueChanged.AddListener(delegate { OnFullscreenToggleChange(fullscreen.isOn); });
        window.onValueChanged.AddListener(delegate { OnWindowedToggleChange(window.isOn); });

        #endregion

    }


    void Update()
    {
        //trocdar para playerinput, test
        if (Input.GetKeyDown(KeyCode.M))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;

            Menu.SetActive(Cursor.visible);
        }
    }

    public void ClickConfigs()
    {
        Configs.SetActive(true);
        Controls.SetActive(false);
    }

    public void ClickControls()
    {
        Configs.SetActive(false);
        Controls.SetActive(true);
    }

    public void ClickExit()
    {
        Exiit.SetActive(true);
    }
    public void ClickReturn()
    {
        Exiit.SetActive(false);
    }

    #region Sound

    // ajusta o volume do AudioListener e salva a configuraca
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        // salva o valor usando PlayerPrefs
        PlayerPrefs.SetFloat(VolumePrefKey, volume);
        PlayerPrefs.Save();
    }

    #endregion

    #region Resolution
    private void OnResolutionChange(int index)
    {
        // aplica a resolucao selecionada
        ApplyResolution(index);

        // salva a resoluaco selecionada
        PlayerPrefs.SetInt(ResolutionPrefKey, index);
        PlayerPrefs.Save();
    }
    private void ApplyResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.FullScreenWindow);
    }

    #endregion

    #region WindowMode
    private void OnFullscreenToggleChange(bool isOn)
    {
        if (isOn)
        {
            ApplyScreenMode(1);
            SaveScreenMode(1);
            window.isOn = false; //desativar
        }
    }

    private void OnWindowedToggleChange(bool isOn)
    {
        if (isOn)
        {
            ApplyScreenMode(0);
            SaveScreenMode(0);
            fullscreen.isOn = false; // desativar
        }
    }

    private void ApplyScreenMode(int mode)
    {
        if (mode == 1)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    private void SaveScreenMode(int mode)
    {
        PlayerPrefs.SetInt(ScreenModePrefKey, mode);
        PlayerPrefs.Save();
    }

    #endregion

    public void Exit()
    {
        Application.Quit();
    }

    public void ReturnInicialScreen()
    {
        SceneManager.LoadScene("TelaInicialScene");
    }
}
