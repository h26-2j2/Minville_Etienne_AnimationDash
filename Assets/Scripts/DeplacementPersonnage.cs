using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeplacementPersonnage : MonoBehaviour
{
    //Composants du perso
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    [Header("Actions du personnage")]
    public InputAction actionMarche;
    public InputAction actionSaut;
    public InputAction actionDash;
    public InputAction actionTir;


    [Header("Déplacement horizontal")]
    public float vitesse = 10f;
    public float inputMarche;

    [Header("Saut")]
    public float forceSaut = 5f;
    public bool inputSaut;

    public float forceDash = 25f;
    public bool estAuSol;

    public bool inputDash;
    public LayerMask masqueSol;

    [Header("Tir")]

    public GameObject projectilePrefab;

    public GameObject pointDeCreation;

    public float directionProjectile = 1;

    public float delaiTir = 0.25f;

    public float tempsEntreTirs = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Récupère les composants
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        //Sert à activer les écouteurs de touches
        actionMarche.Enable();
        actionSaut.Enable();
        actionTir.Enable();
        actionDash.Enable();
    }
    void OnDisable()
    {
        //Sert à désactiver les écouteurs de touches
        actionMarche.Disable();
        actionSaut.Disable();
        actionTir.Disable();
        actionDash.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        inputMarche = actionMarche.ReadValue<float>();
        //new Vector2(0,-1)
        estAuSol = Physics2D.Raycast(transform.position, Vector2.down, 1f, masqueSol);
        Debug.DrawRay(transform.position, Vector2.down * 1, Color.orange);
        if (actionSaut.WasPressedThisFrame() == true && estAuSol == true)
        {
            inputSaut = true;
        }
        else
        {
            inputSaut = false;
        }
       
       
       if (actionDash.WasPressedThisFrame()){
             inputDash = true;
            anim.SetTrigger("estDash");    
        }
        else
        {
            inputDash = false;
        }

        if (inputMarche < 0)
        {
            sr.flipX = true;
            Vector2 nouvellePosition = pointDeCreation.transform.localPosition;
            nouvellePosition.x = -1.5f;
            pointDeCreation.transform.localPosition = nouvellePosition;

            directionProjectile = -1;
        }
        else if (inputMarche > 0)
        {
            sr.flipX = false;
            Vector2 nouvellePosition = pointDeCreation.transform.localPosition;
            nouvellePosition.x = 1.5f;
            pointDeCreation.transform.localPosition = nouvellePosition;

            directionProjectile = 1;
        }
        if (tempsEntreTirs > 0)
        {
            tempsEntreTirs -= Time.deltaTime;
        }

        if (actionTir.WasPressedThisFrame() == true && tempsEntreTirs <= 0)
        {
            tempsEntreTirs = delaiTir;
            GameObject clone = Instantiate(projectilePrefab, pointDeCreation.transform.position, pointDeCreation.transform.rotation);
            clone.GetComponent<Projectile>().direction = directionProjectile;
        }

        float vitesseAbsolue = Mathf.Abs(rb.linearVelocityX);
        anim.SetFloat("vitesse", vitesseAbsolue);
        anim.SetBool("estDansLesAirs", estAuSol == false);
    }

    void FixedUpdate()
    {
        if (inputMarche != 0)
        {
            rb.linearVelocityX = vitesse * inputMarche;
        }

        if (inputSaut == true)
        {
            // rb.AddForce(new Vector2(0, 1) * forceSaut);
            rb.AddForce(Vector2.up * forceSaut, ForceMode2D.Impulse);
        }


        if (inputDash == true)
        {

            if (sr.flipX == true)
            {
                rb.AddForce(Vector2.left * forceDash, ForceMode2D.Impulse);
            }
            else if (sr.flipX == false)
            {
                rb.AddForce(Vector2.right * forceDash, ForceMode2D.Impulse);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ennemi")
        {
            anim.SetTrigger("estBlesse");
        }
    }
}
