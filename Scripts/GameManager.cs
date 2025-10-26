using UnityEngine;  

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Références")]
    public Transform balle;
    public PongAgent raquetteIA;
    public PongAgent raquetteJoueur;
    public Transform murGauche;
    public Transform murDroit;
    public Transform murHaut;
    public Transform murBas;
    public UIManager uiManager;
    
    [Header("Paramètres")]
    public float vitesseBalle = 7f;
    public float vitesseMax = 15f;
    public int pointsMaximum = 7;
    
    [Header("ML-Agents")]
    public bool modeEntrainement = false; 
    
    private Rigidbody rbBalle;
    private Vector3 positionInitialeBalle;
    private int scoreIA = 0;
    private int scoreJoueur = 0;
    private float tempsDeJeu = 0f;
    private bool partieEnCours = false;
    private bool butDetecte = false; 
    public bool PartieEnCours => partieEnCours;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        rbBalle = balle.GetComponent<Rigidbody>();
        positionInitialeBalle = balle.position;
        
        // Mode entraînement : démarrage automatique
        if (modeEntrainement)
        {
            Debug.Log("MODE ENTRAINEMENT ACTIVÉ - Partie infinie");

            if (uiManager != null)
                uiManager.gameObject.SetActive(false);
            DemarrerPartie();
        }
        else
        {
            Debug.Log(" MODE NORMAL - Menu actif");
            balle.position = positionInitialeBalle;
            rbBalle.linearVelocity = Vector3.zero;
            rbBalle.angularVelocity = Vector3.zero;
            rbBalle.isKinematic = true;
        }
    }
    
    void Update()
    {
        if (!partieEnCours) return;
        
        // Mise à jour du timer (seulement en mode normal)
        if (!modeEntrainement && uiManager != null)
        {
            tempsDeJeu += Time.deltaTime;
            uiManager.MettreAJourTimer(tempsDeJeu);
        }
        
        // Limiter la vitesse maximale
        if (rbBalle.linearVelocity.magnitude > vitesseMax)
        {
            rbBalle.linearVelocity = rbBalle.linearVelocity.normalized * vitesseMax;
        }
        
        // Détection des buts
        if (balle.position.z <= murBas.position.z && !butDetecte)
        {
            butDetecte = true;
            MarquerPoint(false); // IA marque
        }
        else if (balle.position.z >= murHaut.position.z && !butDetecte)
        {
            butDetecte = true;
            MarquerPoint(true); // Joueur marque
        }
    }
    
    public void DemarrerPartie()
    {
        partieEnCours = true;
        scoreIA = 0;
        scoreJoueur = 0;
        tempsDeJeu = 0f;
        butDetecte = false;
        rbBalle.isKinematic = false;
        
        if (!modeEntrainement && uiManager != null)
        {
            uiManager.MettreAJourScore(scoreJoueur, scoreIA);
            uiManager.MettreAJourTimer(tempsDeJeu);
        }
        
        ReinitialiserBalle();
    }
    
    public void ResetPartie()
    {
        partieEnCours = false;
        scoreIA = 0;
        scoreJoueur = 0;
        tempsDeJeu = 0f;
        butDetecte = false;
        
        balle.position = positionInitialeBalle;
        rbBalle.linearVelocity = Vector3.zero;
        rbBalle.angularVelocity = Vector3.zero;
        rbBalle.isKinematic = true;
    }
    
    void MarquerPoint(bool joueurMarque)
    {
        rbBalle.linearVelocity = new Vector3(0, 0, 0);
        balle.position = positionInitialeBalle;

        if (joueurMarque)
        {
            scoreJoueur++;
            raquetteJoueur.OnPointMarque();
            raquetteIA.OnPointPerdu();
        }
        else
        {
            scoreIA++;
            raquetteIA.OnPointMarque();
            raquetteJoueur.OnPointPerdu();
        }
        
        // EN MODE ENTRAINEMENT : JEU INFINI
        if (modeEntrainement)
        {
            Invoke("ReinitialiserBalle", 0.5f);
        }
        else
        {
            // Mode normal : UI + vérifier fin de partie
            if (uiManager != null)
                uiManager.MettreAJourScore(scoreJoueur, scoreIA);
            
            if (scoreJoueur >= pointsMaximum || scoreIA >= pointsMaximum)
            {
                TerminerPartie();
            }
            else
            {
                Invoke("ReinitialiserBalle", 1f);
            }
        }
    }
    
    void TerminerPartie()
    {
        partieEnCours = false;
        bool joueurGagne = scoreJoueur > scoreIA;
        if (uiManager != null)
            uiManager.AfficherEcranFin(joueurGagne, scoreJoueur, scoreIA, tempsDeJeu);
    }
    
    public void ReinitialiserBalle()
    {
        butDetecte = false;
        rbBalle.linearVelocity = new Vector3(0, 0, -1) * vitesseBalle;
    }
}