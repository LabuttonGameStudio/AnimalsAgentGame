using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class MenuPause : MonoBehaviour
{
    [Header("OPTIONS")]
    public GameObject Menu;
    public GameObject Background;
    public GameObject Configs;
    public GameObject Controls;
    public GameObject Exiit;

    [Header("CONFIGS")]
    public TMP_Dropdown resolution;
    public Toggle fullscreen;
    public Toggle window;
    public Toggle lowQualityToggle;
    public Toggle mediumQualityToggle;
    public Toggle highQualityToggle;
    public Toggle UltraQualityToggle;
    public Slider sensitivitySlider;

    private bool isMenuOpen;


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
    private const string QualityPrefKey = "GraphicsQuality";
    private const string SensitivityPrefKey = "MouseSensitivity";

    public ArmadilloVisualControl visualControl;
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

        #region Quality
        int savedQualityIndex = PlayerPrefs.GetInt(QualityPrefKey, 1);

        ApplyQualitySettings(savedQualityIndex);

        lowQualityToggle.isOn = savedQualityIndex == 0;
        mediumQualityToggle.isOn = savedQualityIndex == 1;
        highQualityToggle.isOn = savedQualityIndex == 2;
        UltraQualityToggle.isOn = savedQualityIndex == 3;

        lowQualityToggle.onValueChanged.AddListener(delegate { OnToggleChange(0, lowQualityToggle.isOn); });
        mediumQualityToggle.onValueChanged.AddListener(delegate { OnToggleChange(1, mediumQualityToggle.isOn); });
        highQualityToggle.onValueChanged.AddListener(delegate { OnToggleChange(2, highQualityToggle.isOn); });
        UltraQualityToggle.onValueChanged.AddListener(delegate { OnToggleChange(3, UltraQualityToggle.isOn); });
        #endregion

        #region Sensibility
        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey, 0.25f);

        sensitivitySlider.value = savedSensitivity;
        OnSensitivityChange(savedSensitivity);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChange);
        #endregion


        if (ArmadilloPlayerController.Instance != null &&
            ArmadilloPlayerController.Instance.inputControl != null &&
            ArmadilloPlayerController.Instance.inputControl.inputAction != null)
        {
            ArmadilloPlayerController.Instance.inputControl.inputAction.Pause.EnterPause.Enable();
            ArmadilloPlayerController.Instance.inputControl.inputAction.Pause.EnterPause.performed += MenuOpen;
        }

    }

    public void MenuOpen(InputAction.CallbackContext value)

    {
        isMenuOpen = !isMenuOpen;
        Cursor.visible = isMenuOpen;
        Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;

        Background.SetActive(isMenuOpen);
        Menu.SetActive(isMenuOpen);
        if (ArmadilloPlayerController.Instance.cameraControl != null)
        {
            if(isMenuOpen)
            {
                Time.timeScale = 0;
                ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(false);
                ArmadilloPlayerController.Instance.inputControl.ToggleDialogueControls(false);
            }
            else
            { 
                Time.timeScale = 1;
                ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(true);
                ArmadilloPlayerController.Instance.inputControl.ToggleDialogueControls(true);
            }
        }
        if (isMenuOpen)
        {
            if(visualControl != null)
            {
                visualControl.OnPause();
            }
                
        }
        else
        {
            if (visualControl != null)
            {
                visualControl.ReturnPause();
            }
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

    #region Quality
    private void OnToggleChange(int qualityIndex, bool isOn)
    {
        if (isOn)
        {
            ApplyQualitySettings(qualityIndex);
            SaveQualitySettings(qualityIndex);
            UpdateToggles(qualityIndex);
        }
    }

    private void ApplyQualitySettings(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        switch (qualityIndex)
        {
            case 0: //baixo
                QualitySettings.shadowResolution = ShadowResolution.Low; //Sombra
                QualitySettings.globalTextureMipmapLimit = 2; // Textura 
                QualitySettings.pixelLightCount = 0;// Iluminacao 
                break;
            case 1: //medio
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                QualitySettings.globalTextureMipmapLimit = 1;
                QualitySettings.pixelLightCount = 2;
                break;
            case 2: //alto
                QualitySettings.shadowResolution = ShadowResolution.High;
                QualitySettings.globalTextureMipmapLimit = 0;
                QualitySettings.pixelLightCount = 4;
                break;
            case 3: //Ultra
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                QualitySettings.globalTextureMipmapLimit = 0;
                QualitySettings.pixelLightCount = 8;

                break;
        }
    }

    private void SaveQualitySettings(int qualityIndex)
    {
        PlayerPrefs.SetInt(QualityPrefKey, qualityIndex);
        PlayerPrefs.Save();
    }

    private void UpdateToggles(int activeIndex)
    {
        lowQualityToggle.isOn = activeIndex == 0;
        mediumQualityToggle.isOn = activeIndex == 1;
        highQualityToggle.isOn = activeIndex == 2;
        UltraQualityToggle.isOn = activeIndex == 3;

    }

    #endregion

    #region Sensibility
    public void OnSensitivityChange(float newSensitivity)
    {
        ApplyMouseSensitivity(newSensitivity);
        SaveSensitivitySettings(newSensitivity);
    }

    private void ApplyMouseSensitivity(float sensitivity)
    {
        if(PlayerCamera.Instance != null)PlayerCamera.Instance.ChangeSensibility(new Vector2(sensitivity, sensitivity));
    }

    private void SaveSensitivitySettings(float sensitivity)
    {
        PlayerPrefs.SetFloat(SensitivityPrefKey, sensitivity);
        PlayerPrefs.Save();
    }


    #endregion


    #region Language

    public void OnLanguageSelected(int languageIndex)
    {
        if (LocalizationSettings.AvailableLocales.Locales.Count <= 0 || languageIndex> LocalizationSettings.AvailableLocales.Locales.Count) return;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)languageIndex];
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
