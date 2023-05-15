using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    #region Variables

    // Input Manager:
    Controls control;
    Controls.JugadorActions jugador;

    //Interfaz:
    [SerializeField] TextMeshProUGUI TextSalud;
    [SerializeField] TextMeshProUGUI TextVidas;
    [SerializeField] TextMeshProUGUI TextPuntuacion;
    [SerializeField] TextMeshProUGUI TextMonedas;

    // Movimiento horizontal:
    Vector2 horizontalInput;
    [Tooltip("Controlador del jugador")]
    [SerializeField] CharacterController controller;
    [Tooltip("Velocidad del jugador")]
    [Range(1.0f, 30.0f)]
    [SerializeField] float velocidad = 11.0f;

    // Movimiento vertical:
    [Tooltip("Gravedad del jugador")]
    [SerializeField] float gravedad = -30.0f;
    Vector3 verticalVel = Vector3.zero;
    [Tooltip("Layer que el script detecta como suelo para poder saltar")]
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    // Salto:
    [Tooltip("Altura del salto que realiza el jugador")]
    [Range(1.0f, 10.0f)]
    [SerializeField] float alturaSalto = 3.5f;
    bool jump;

    // Camara que sigue al jugador:
    [Tooltip("Camara asignada al jugador")]
    [SerializeField] Camera cam;

    // Salud del jugador, 2 al inicio:
    int salud = 2;
    // Vidas del jugador, no es lo mismo que salud, si la salud llega a 0, se reduce una vida y salud = 2:
    int vidas = 3;
    // Booleano para saber si el jugador esta vivo o no, muerto = salud && vidas == 0:
    bool estaVivo = true;

    // Puntuación del jugador:
    int puntuacion = 0;

    // Monedas recogidas por el jugador:
    int monedas = 0;

    [Tooltip("Posición donde se moverá el jugador al perder vida/caer")]
    [SerializeField] Transform spawn;

    [Tooltip("Posición donde el jugador será movido a spawn al caer debajo de la posicion Y de puntoCaida")]
    [SerializeField] Transform puntoCaida;

    //TODO Empujar el jugador al recibir daño, ahora mismo no funciona
    [SerializeField] float fuerzaEmpuje = 10f;
    [SerializeField] float duracionEmpuje = 0.2f;
    [SerializeField] float tiempoEspera = 0.5f;
    bool empujando = false;

    // Nombre de la siguiente escena a cargar al ganar el nivel:
    [Tooltip("Nombre de la siguiente escena a cargar cuando el jugador entra en la tuberia al final del mapa")]
    [SerializeField] string escena;

    // Sonidos:
    AudioSource audioSource;
    [SerializeField] AudioClip saltoSound;
    [SerializeField] AudioClip dañoSound;
    [SerializeField] AudioClip crecerSound;
    [SerializeField] AudioClip derrotaSound;
    [SerializeField] AudioClip cambiarNivelSound;

    #endregion

    // Propiedad para modificar/devolver atributos privados:
    public int Salud
    {
        get { return salud; }
        set { salud = value; }
    }

    public int Vidas
    {
        get { return vidas; }
        set { vidas = value; }
    }

    public int Puntuacion
    {
        get { return puntuacion; }
        set { puntuacion = value; }
    }

    public int Monedas
    {
        get{ return monedas; }
        set{ monedas = value; }
    }

    //* ######## Variables ########

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //? El jugador ha de mirar hacia donde se mueve, transform.lookat?
        //? Quizas hacer la esfera dependiendo del modelo, no del jugador entero? hijo de jugador

        //* #### Movimiento ####

        // Creamos una esfera invisible que compruebe si toca suelo con layer = Suelo, si toca suelo, isGrounded = True:
        float halfHeight = controller.height * 0.5f;
        Vector3 bottomPoint = transform.TransformPoint(controller.center - Vector3.up * halfHeight);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.1f, groundMask);
        // Si esta en el suelo, deja de ganar velocidad en y:
        if (isGrounded)
        {
            verticalVel.y = 0.0f;
        }

        // Mover el jugador horizontalmente:
        if (estaVivo)
        {
            Vector3 horizontalVel = (cam.transform.right * horizontalInput.x + cam.transform.forward * horizontalInput.y) * velocidad;
            controller.Move(horizontalVel * Time.deltaTime);
        }

        // Si se pulsa saltar, si esta en el suelo, salta, sino, jump = false:
        if (jump && estaVivo)
        {
            if (isGrounded)
            {
                verticalVel.y = Mathf.Sqrt(-2f * alturaSalto * gravedad);

                // Sonido salto:
                audioSource.PlayOneShot(saltoSound);
            }
            jump = false;
        }

        // Mover el jugador verticalmente:
        if (estaVivo)
        {
            verticalVel.y += gravedad * Time.deltaTime;
            controller.Move(verticalVel * Time.deltaTime);
        }

        //* #### Fin movimiento ####

        // Salud y vidas:
        if (salud == 0 && vidas > 0)
        {
            // Mover el jugador a la posicion de spawn
            //! No funciona por algun motivo, pero con caida si funciona...
            transform.position = spawn.position; 

            salud = 2;
            vidas--;

            // Aumentar tamaño
            CambiarTamaño(true);
        }
        else if (salud == 0 && vidas == 0 && estaVivo)
        {
            estaVivo = false;

            // Sonido derrota:
            audioSource.PlayOneShot(derrotaSound);

            Esperar(2.0f);

            //TODO Has perdido, cargar escena de derrota
        }

        //TODO Cambiar modelo de fuego a normal al perder salud, o de normal a fuego al ganar flor, usar funcion publica

        //TODO Flor de Fuego, solo cuando salud == 3, el jugador puede disparar bolas de fuego
        /*if (salud == 3)
        {
            // Código para comprobar si se ha pulsado keybinding para disparar
            // Disparar la bola de fuego, que se borre al cabo de x tiempo
            // Si la bola de fuego da a un enemigo, eliminarlo o hacerle daño, script en la bola que compruebe el tag
        }*/

        // Arreglar la puntuación para que no sea menor que 0, para evitar mostrar puntuacion: -10
        if (puntuacion < 0)
        {
            puntuacion = 0;
        }

        // Actualizar textos
        TextPuntuacion.text = " " + puntuacion;
        TextSalud.text = "Salud: " + salud;
        TextVidas.text = "Vidas: " + vidas;
        TextMonedas.text = "x" + monedas;
    }

    void FixedUpdate()
    {
        // Si se cae del nivel (debajo de Y de puntoCaida), mover a spawn y perder salud
        if (transform.position.y < puntoCaida.position.y)
        {
            transform.position = spawn.position;
            salud--;

            if (salud == 1)
            {
                CambiarTamaño(false);
            }

            // Sonido daño:
            audioSource.PlayOneShot(dañoSound);
        }
    }

    void Awake()
    {
        // Obtener los controles e inputs:
        control = new Controls();
        jugador = control.Jugador;

        //* Input:
        // Jugador.[accion].performed += ctx => cosas a hacer
        // Movimiento horizontal:
        jugador.Movimiento.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        // Saltar, llama a la funcion OnJumpPressed, que asigna jump a true:
        jugador.Saltar.performed += _ => OnJumpPressed();
        //TODO! Disparar bola de fuego:
    }

    void OnEnable()
    {
        control.Enable();
    }

    void OnDestroy()
    {
        control.Disable();
    }

    void OnJumpPressed()
    {
        jump = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si tocamos el lado de un enemigo, perdemos vida, si tocamos su cabeza, le matamos
        if (other.gameObject.tag == "EnemigoLado")
        {
            salud -= 1;
            puntuacion -= 50;

            //TODO Al chocarse con un enemigo de lado, empujar al jugador
            /*
            if (!empujando)
            {
                StartCoroutine(Empujar(other.gameObject));
            }
            
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direccionEmpuje = (transform.position - other.transform.position).normalized;
                rb.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode.Impulse);
            }
            */

            //TODO si salud = 3 antes de chocarse, cambiar modelo, si salud = 2, reducir tamaño
            /*
            if (salud == 2)
            {
                /// Cambiar modelo, funcion publica
            }
            else */if (salud == 1)
            {
                // Reducir tamaño
                CambiarTamaño(false);
            }
            

            // Sonido daño:
            audioSource.PlayOneShot(dañoSound);
        }
        else if (other.gameObject.tag == "EnemigoTop")
        {
            puntuacion += 25;
            Destroy(other.transform.parent.gameObject);
        }
        else if (other.gameObject.tag == "BloqueDestructible")
        {
            Destroy(other.gameObject);

            //TODO animacion o particulas de romper bloque?
        }
        else if (other.gameObject.tag == "CambiarNivel")
        {
            puntuacion += 100;
            
            // Sonido cambiar de nivel:
            audioSource.PlayOneShot(cambiarNivelSound);

            Esperar(1.0f);
            
            //TODO se ha de pasar puntuacion, tamaño, salud y vidas al siguiente nivel, singleton?

            SceneManager.LoadScene(escena);
        }
    }

    IEnumerator Esperar(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
    }

    IEnumerator Empujar(GameObject enemigo)
    {
        empujando = true;
        Vector3 direccionEmpuje = (enemigo.transform.position - transform.position).normalized;
        Vector3 fuerza = direccionEmpuje * fuerzaEmpuje;
        float tiempoInicio = Time.time;

        while (Time.time < tiempoInicio + duracionEmpuje)
        {
            enemigo.transform.Translate(fuerza * Time.deltaTime, Space.World);
            Debug.Log("El jugador ha sido empujado.");
            yield return null;
        }
        yield return new WaitForSeconds(tiempoEspera);
        empujando = false;
    }

    public void CambiarTamaño(bool aumentar)
    {
        // Velocidad de disminución de la escala
        float velocidadDisminucion = 1f;

        // Nueva escala del jugador
        Vector3 nuevaEscala;

        // Si aumentar es true, aumentamos el tamaño del jugador, sino, reducimos el tamaño
        if (aumentar)
        {
            // Multiplicar la escala actual del jugador por dos
            nuevaEscala = transform.localScale * 2.0f;

            // Sonido crecer:
            audioSource.PlayOneShot(crecerSound);
        }
        else
        {
            // Divide la escala actual del jugador por dos
            nuevaEscala = transform.localScale / 2.0f;
        }

        // Disminuye o aumenta la escala del jugador suavemente con una velocidad definida
        StartCoroutine(DisminuirEscalaSuavemente(transform.localScale, nuevaEscala, velocidadDisminucion));
    }

    // Función que disminuye la escala del jugador suavemente con una velocidad definida
    private IEnumerator DisminuirEscalaSuavemente(Vector3 escalaInicial, Vector3 escalaFinal, float velocidad)
    {
        float tiempo = 0f;
        while (tiempo < 1f)
        {
            // Interpola entre la escala inicial y final del jugador
            tiempo += Time.deltaTime * velocidad;
            transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tiempo);
            yield return null;
        }
    }
}
