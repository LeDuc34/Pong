using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PongAgent : Agent
{
    [Header("Références")]
    public Transform balle;
    public Transform raquette;
    public Transform murGauche;
    public Transform murDroit;
    public Transform murArriere;
    public Transform raquetteAdverse;
    
    [Header("Paramètres")]
    public float vitesseRaquette = 10f;
    public bool estJoueur = false;
    
    private Rigidbody rbRaquette;
    private Rigidbody rbBalle;
    private Vector3 positionInitialeRaquette;

    public override void Initialize()
    {
        rbRaquette = raquette.GetComponent<Rigidbody>();
        rbBalle = balle.GetComponent<Rigidbody>();
        positionInitialeRaquette = raquette.position;
    }

    // reset la position
    public override void OnEpisodeBegin()
    {
        raquette.position = positionInitialeRaquette;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

         if (balle == null) Debug.LogError($"[{gameObject.name}] balle est NULL !");
        if (raquette == null) Debug.LogError($"[{gameObject.name}] raquette est NULL !");
        if (murGauche == null) Debug.LogError($"[{gameObject.name}] murGauche est NULL !");
        if (murDroit == null) Debug.LogError($"[{gameObject.name}] murDroit est NULL !");
        if (murArriere == null) Debug.LogError($"[{gameObject.name}] murArriere est NULL !");
        if (raquetteAdverse == null) Debug.LogError($"[{gameObject.name}] raquetteAdverse est NULL !");
        if (rbBalle == null) Debug.LogError($"[{gameObject.name}] rbBalle est NULL !");

        float largeurTerrain = murDroit.position.x - murGauche.position.x;
        float posRaquetteX = (raquette.position.x - murGauche.position.x) / largeurTerrain;
        sensor.AddObservation(posRaquetteX);
        
        sensor.AddObservation(0f); // Vélocité placeholder
        
        float profondeurTerrain = Mathf.Abs(murArriere.position.z - raquetteAdverse.position.z);
        float posBalleX = (balle.position.x - murGauche.position.x) / largeurTerrain;
        float posBalleZ = (balle.position.z - raquetteAdverse.position.z) / profondeurTerrain;
        sensor.AddObservation(posBalleX);
        sensor.AddObservation(posBalleZ);
        
        sensor.AddObservation(rbBalle.linearVelocity.x / 10f);
        sensor.AddObservation(rbBalle.linearVelocity.z / 10f);
        
        float distanceX = Mathf.Abs(raquette.position.x - balle.position.x);
        sensor.AddObservation(distanceX / largeurTerrain);
        
        float directionVersIA = Mathf.Sign(rbBalle.linearVelocity.z) * 
            Mathf.Sign(raquette.position.z - raquetteAdverse.position.z);
        sensor.AddObservation(directionVersIA);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Ne pas bouger si la partie n'est pas en cours
        if (estJoueur || !GameManager.Instance.PartieEnCours) return;
        
        float mouvement = actions.DiscreteActions[0];
        DeplacerRaquette(mouvement);
    }

    private void DeplacerRaquette(float mouvement)
    {
        float deplacement = 0f;
        
        if (mouvement == 1)
            deplacement = -vitesseRaquette * Time.deltaTime;
        else if (mouvement == 2)
            deplacement = vitesseRaquette * Time.deltaTime;
        
        Vector3 nouvellePosition = raquette.position;
        nouvellePosition.x += deplacement;
        
        nouvellePosition.x = Mathf.Clamp(nouvellePosition.x, 
            murGauche.position.x + 0.5f, 
            murDroit.position.x - 0.5f);
        
        raquette.position = nouvellePosition;
    }

    private void Update()
    {
        // Le joueur peut bouger seulement si la partie est en cours
        if (estJoueur && GameManager.Instance != null && GameManager.Instance.PartieEnCours)
        {
            float mouvement = 0;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
                mouvement = 1;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                mouvement = 2;
            
            DeplacerRaquette(mouvement);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;
        
        if (!estJoueur)
        {
            // Heuristic pour adversaire automatique
            float diffX = balle.position.x - raquette.position.x;
            float seuil = 0.5f;
            
            if (diffX < -seuil)
                discreteActions[0] = 1;
            else if (diffX > seuil)
                discreteActions[0] = 2;
        }
        else
        {
            // Contrôle clavier
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
                discreteActions[0] = 1;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                discreteActions[0] = 2;
        }
    }

    // Récompenses
public void OnBalleTouchee()
{
        AddReward(0.1f);
}

public void OnPointMarque()
{
        AddReward(1f);
    EndEpisode(); 
}

public void OnPointPerdu()
{
        AddReward(-1f);
    EndEpisode(); 
}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == balle.gameObject)
        {
            OnBalleTouchee();
        }
    }
}