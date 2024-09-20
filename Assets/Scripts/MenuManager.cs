using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField seedInputField;  
    public Button playButton;              
    public Button settingsButton;         
    public Button quitButton;              

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    void OnPlayButtonClicked()
    {
        // Z�sk�n� textu z TMP input field (seed)
        string seedText = seedInputField.text;
        int seed;

        // Pokus�me se p�ev�st vstupn� text na ��slo
        if (int.TryParse(seedText, out seed))
        {
            // Pokud je seed platn� ��slo, pou�ijeme ho
        }
        else
        {
            // Pokud je seed pr�zdn� nebo neplatn�, vygeneruj n�hodn� seed
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        // Ulo� seed do PlayerPrefs
        PlayerPrefs.SetInt("WorldSeed", seed);

        // Na�ti hern� sc�nu (zm�� "GameWorld" na n�zev sv� sc�ny)
        SceneManager.LoadScene("WorldLevel");
    }

    void OnSettingsButtonClicked()
    {
        Debug.Log("Otev�eno nastaven� (zat�m bez funkce)");
    }

    void OnQuitButtonClicked()
    {
        Debug.Log("Hra je ukon�ena");
        Application.Quit();
    }
}
