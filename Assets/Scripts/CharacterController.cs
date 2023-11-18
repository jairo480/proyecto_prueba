using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float velocidadDash;
    [SerializeField] private float tiempoDash;
    private float gravedadInicial;
    private bool puedeHacerDash = true;



    public float velocidadFinal;
    public float velocidad;
    public float fuerzaSalto;
    public float fuerzaGolpe;
    public float saltosMaximos;

    public LayerMask capaSuelo;
    public AudioClip sonidoSalto;
     public AudioClip sonidoDash;

    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    private bool mirandoDerecha = true;
    private float saltosRestantes;
    private Animator animator;
    private bool puedeMoverse = true;

    private GameObject target;
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        saltosRestantes = saltosMaximos;
        animator = GetComponent<Animator>();
        gravedadInicial = rigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        ProcesarMovimiento();
        ProcesarSalto();
        Golpe();
        StartCoroutine(doDash());
        moveFastOnSide();
        

        
    }

void Golpe(){
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetBool("isAttack", true);
        }else
        {
            animator.SetBool("isAttack", false);
        }
    }
    bool EstaEnSuelo()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y), 0f, Vector2.down, 0.2f, capaSuelo);
        return raycastHit.collider != null;
    }

    void ProcesarSalto()
    {
        if(EstaEnSuelo())
        {
            saltosRestantes = saltosMaximos;
        }

        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            saltosRestantes--;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
            rigidBody.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
//            AudioManager.Instance.ReproducirSonido(sonidoSalto);
            
        }
    }

      IEnumerator doDash() {
        if (Input.GetKeyDown(KeyCode.B) && puedeHacerDash)
        {
            
         puedeMoverse = false;
        puedeHacerDash = false;
        rigidBody.gravityScale = 0;
        rigidBody.velocity = new Vector2(velocidadDash * transform.localScale.x, 0);   
        
        animator.SetTrigger("Dash");
        yield return new WaitForSeconds(tiempoDash);

        puedeMoverse = true;
        puedeHacerDash = true;
        rigidBody.gravityScale = gravedadInicial;

       
       
}

    }
    
 
    void moveFastOnSide()
{
    if (Input.GetKeyDown(KeyCode.F) && puedeMoverse)
    {
        velocidadFinal = velocidadFinal + velocidad;
        if (velocidadFinal >= 100)
        {
            velocidadFinal = velocidad;
        }

        // Lógica de movimiento
        float inputMovimiento = Input.GetAxis("Horizontal");

        rigidBody.velocity = new Vector2(inputMovimiento * velocidadFinal, rigidBody.velocity.y);

        GestionarOrientacion(inputMovimiento);

        // Activa el trigger "Dash"
        animator.SetTrigger("isDash");
    }
}


    void ProcesarMovimiento()
    {
        // Si no puede moverse, salimos de la funcion
        if(!puedeMoverse) return;

        // Lógica de movimiento
        float inputMovimiento = Input.GetAxis("Horizontal");

        if(inputMovimiento != 0f)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        rigidBody.velocity = new Vector2(inputMovimiento * velocidad, rigidBody.velocity.y);
        
        GestionarOrientacion(inputMovimiento);
    }

    void GestionarOrientacion(float inputMovimiento)
    {
        // Si se cumple condición
        if( (mirandoDerecha == true && inputMovimiento < 0) || (mirandoDerecha == false && inputMovimiento > 0) )
        {
            // Ejecutar código de volteado
            mirandoDerecha = !mirandoDerecha;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    public void AplicarGolpe() {

        puedeMoverse = false;

        Vector2 direccionGolpe;

        if(rigidBody.velocity.x > 0)
        {
            direccionGolpe = new Vector2(-1, 1);
        } else {
            direccionGolpe = new Vector2(1, 1);
        }

        rigidBody.AddForce(direccionGolpe * fuerzaGolpe);

        StartCoroutine(EsperarYActivarMovimiento());
    }

    IEnumerator EsperarYActivarMovimiento() {
        // Esperamos antes de comprobar si esta en el suelo.
        yield return new WaitForSeconds(0.1f);

        while(!EstaEnSuelo()) {
            // Esperamos al siguiente frame.
            yield return null;
        }

        // Si ya está en suelo activamos el movimiento.
        puedeMoverse = true;
    }
}