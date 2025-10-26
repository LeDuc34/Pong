using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Paramètres")]
    public float vitesseConstante = 8f;
    public float effetMaximal = 3f;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Juste ajouter l'effet sur les raquettes
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            Vector3 vitesse = rb.linearVelocity;
            
            // Ajouter effet selon position d'impact
            float decalage = collision.contacts[0].point.x - collision.transform.position.x;
            float effet = Mathf.Clamp(decalage / (collision.collider.bounds.size.x / 2f), -1f, 1f);
            vitesse.x += effet * effetMaximal;
            
            rb.linearVelocity = vitesse;
        }
        
    }

    void FixedUpdate()
    {
        // maintenir vitesse constante et Y = 0
        Vector3 vitesse = rb.linearVelocity;
        vitesse.y = 0;
        
        float magnitude = vitesse.magnitude;
        if (magnitude > 0.5f)
        {
            vitesse = vitesse.normalized * vitesseConstante;
        }
        
        rb.linearVelocity = vitesse;
        
        // Maintenir position Y
        Vector3 pos = transform.position;
        pos.y = 0.1f;
        transform.position = pos;
    }
}