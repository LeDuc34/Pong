using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPrincipal;
    public GameObject panelJeu;
    public GameObject panelPause;
    public GameObject panelFin;

    [Header("UI Jeu")]
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("UI Fin")]
    public TMP_Text resultatText;
    public TMP_Text scoreFinText;
    public TMP_Text tempsFinText;

    private bool jeuEnPause = false;

    void Start()
    {
        // Au démarrage, afficher seulement le menu principal
        AfficherMenuPrincipal();
    }

    void Update()
    {
        // Pause avec Échap pendant le jeu
        if (Input.GetKeyDown(KeyCode.Escape) && panelJeu.activeSelf)
        {
            if (jeuEnPause)
                Reprendre();
            else
                Pause();
        }
    }

    // ========== MENU PRINCIPAL ==========
    public void AfficherMenuPrincipal()
    {
        menuPrincipal.SetActive(true);
        panelJeu.SetActive(false);
        panelPause.SetActive(false);
        panelFin.SetActive(false);
        Time.timeScale = 1f;
        jeuEnPause = false;
    }

    public void CommencerPartie()
    {
        menuPrincipal.SetActive(false);
        panelJeu.SetActive(true);
        panelPause.SetActive(false);
        panelFin.SetActive(false);
        Time.timeScale = 1f;
        jeuEnPause = false;

        // Notifier le GameManager
        GameManager.Instance.DemarrerPartie();
    }

    public void QuitterJeu()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


    // ========== PENDANT LE JEU ==========
    public void MettreAJourScore(int scoreJoueur, int scoreIA)
    {
        scoreText.text = $"JOUEUR {scoreJoueur} - {scoreIA} IA";
    }

    public void MettreAJourTimer(float temps)
    {
        int minutes = Mathf.FloorToInt(temps / 60f);
        int secondes = Mathf.FloorToInt(temps % 60f);
        timerText.text = $"{minutes:00}:{secondes:00}";
    }

    // ========== PAUSE ==========
    public void Pause()
    {
        jeuEnPause = true;
        panelPause.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reprendre()
    {
        jeuEnPause = false;
        panelPause.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RetourMenuDepuisPause()
    {
        Time.timeScale = 1f;
        GameManager.Instance.ResetPartie();
        AfficherMenuPrincipal();
    }

    // ========== FIN DE PARTIE ==========
    public void AfficherEcranFin(bool joueurGagne, int scoreJoueur, int scoreIA, float tempstotal)
    {
        panelJeu.SetActive(false);
        panelFin.SetActive(true);

        // Afficher le résultat
        if (joueurGagne)
        {
            resultatText.text = "VICTOIRE !";
            resultatText.color = Color.green;
        }
        else
        {
            resultatText.text = "DÉFAITE";
            resultatText.color = Color.red;
        }

        // Afficher le score final
        scoreFinText.text = $"Score final : {scoreJoueur} - {scoreIA}";

        // Afficher le temps de jeu
        int minutes = Mathf.FloorToInt(tempstotal / 60f);
        int secondes = Mathf.FloorToInt(tempstotal % 60f);
        tempsFinText.text = $"Temps de jeu : {minutes:00}:{secondes:00}";
    }

    public void Rejouer()
    {
        GameManager.Instance.ResetPartie();
        CommencerPartie();
    }

    public void RetourMenuDepuisFin()
    {
        GameManager.Instance.ResetPartie();
        AfficherMenuPrincipal();
    }
}