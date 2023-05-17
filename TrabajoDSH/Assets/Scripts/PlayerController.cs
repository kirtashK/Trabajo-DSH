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
    bool dañado = false;
    float tiempoUltimoDaño = 0.0f;

    // Puntuación del jugador:
    int puntuacion = 0;

    // Monedas recogidas por el jugador:
    int monedas = 0;

    [Tooltip("Posición donde se moverá el jugador al perder vida/caer")]
    [SerializeField] Transform spawn;

    [Tooltip("Posición donde el jugador será movido a spawn al caer debajo de la posicion Y de puntoCaida")]
    [SerializeField] Transform puntoCaida;

    // Nombre de la siguiente escena a cargar al ganar el nivel:
    [Tooltip("Nombre de la siguiente escena a cargar cuando el jugador entra en la tuberia al final del mapa")]
    [SerializeField] string escena;

    // Sonidos:
    AudioSource audioSource;
    [SerializeField] AudioClip saltoSound;
    [SerializeField] AudioClip dañoSound;
    [SerializeField] AudioClip crecerSound;
    [SerializeField] AudioClip ladrilloSound;
    [SerializeField] AudioClip derrotaSound;
    [SerializeField] AudioClip cambiarNivelSound;

    // Modelos del jugador, normal y fuego:
    public GameObject[] playerModels;

    // Disparar bola de fuego:
    bool disparo = false;
    [Tooltip("Prefab a disparar con la flor de fuego")]
    [SerializeField] GameObject bolaFuegoPrefab;
    [SerializeField] float bolaFuegoSpeed = 10f;
    float tiempoUltimoDisparo = 0.0f;

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

    //* ######## Fin Variables ########

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //? El jugador ha de mirar hacia donde se mueve, transform.lookat?

        //* #### Movimiento ####
        #region Movimiento

        // Creamos una esfera invisible que compruebe si toca suelo con layer = Suelo, si toca suelo, isGrounded = True:
        /*float halfHeight = controller.height * 0.5f;
        Vector3 bottomPoint = transform.TransformPoint(controller.center - Vector3.up * halfHeight);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.3f, groundMask);
        */
        RaycastHit hit;
        float height = transform.localScale.y * 1.5f;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, height, groundMask);

        // Si esta en el suelo, deja de ganar velocidad en y:
        if (isGrounded)
        {
            verticalVel.y = 0.0f;
        }


        // Mover el jugador horizontalmente:
        if (estaVivo)
        {
            Vector3 horizontalVel = (cam.transform.right * horizontalInput.x + cam.transform.forward * horizontalInput.y) * velocidad;
            horizontalVel.y = 0;
            controller.Move(horizontalVel * Time.deltaTime);
        }

        // Si se pulsa saltar, si esta en el suelo, salta, sino, jump = false:
        if (jump && estaVivo)
        {
            if (isGrounded)
            {
                verticalVel.y = Mathf.Sqrt(-2f * alturaSalto * gravedad);

                // Sonido salto:
                Sonido(saltoSound);
                //audioSource.PlayOneShot(saltoSound);
            }
            jump = false;
        }

        // Mover el jugador verticalmente:
        if (estaVivo)
        {
            verticalVel.y += gravedad * Time.deltaTime;
            controller.Move(verticalVel * Time.deltaTime);
        }

        #endregion
        //* #### Fin movimiento ####

        // Salud y vidas:
        if (salud == 0 && vidas > 0 && estaVivo)
        {
            estaVivo = false;

            // Mover el jugador a la posicion de spawn
            transform.position = spawn.position;

            // Asegurar que el jugador no se puede mover mientras respawnea, sino, a veces no se teletransporta bien.
            StartCoroutine(RespawnWait(0.2f));

            salud = 2;
            vidas--;

            // Aumentar tamaño
            CambiarTamaño(true);
        }
        else if (salud == 0 && vidas == 0 && estaVivo)
        {
            estaVivo = false;

            // Sonido derrota:
            Sonido(derrotaSound);
            //audioSource.PlayOneShot(derrotaSound);

            // Has perdido, cargar escena de derrota
            SceneManager.LoadScene("Derrota");
        }

        //Flor de Fuego, solo cuando salud == 3, el jugador puede disparar bolas de fuego y su modelo cambia:
        if (salud == 3)
        {
            // Cambiar modelo del jugador a fuego:
            // Desactivar el modelo normal
            playerModels[0].SetActive(false);

            // Activar el modelo de fuego
            playerModels[1].SetActive(true);

            tiempoUltimoDisparo += Time.deltaTime;
            // Código para comprobar si se ha pulsado keybinding para disparar y ha pasado mas de 0.5f desde el ultimo disparo:
            if (disparo && tiempoUltimoDisparo >= 1f)
            {
                disparo = false;
                tiempoUltimoDisparo = 0.0f;

                // Disparar la bola de fuego y aplicarle movimiento:
                GameObject fireball = Instantiate(bolaFuegoPrefab, transform.position, transform.rotation);
                Rigidbody rb = fireball.GetComponent<Rigidbody>();
                rb.velocity = transform.forward * bolaFuegoSpeed;
            }
        }

        tiempoUltimoDaño += Time.deltaTime;
        if (dañado && tiempoUltimoDaño >= 1.0f)
        {
            dañado = false;
        }

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
            // Si teniamos flor de fuego:
            else if (salud == 2)
            {
                // Desactivar el modelo normal
                playerModels[1].SetActive(false);

                // Activar el modelo de fuego
                playerModels[0].SetActive(true);
            }

            // Sonido daño:
            Sonido(dañoSound);
            //audioSource.PlayOneShot(dañoSound);
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
        // Disparar bola de fuego:
        jugador.Disparar.performed += _ => OnDisparoPressed();

        //* Recoger valores entre escenas
        // Comprobamos si el valor existe en PlayerPrefs, si existe, lo recogemos, sino, lo ponemos a su valor por defecto:
        if (PlayerPrefs.HasKey("vidas"))
        {
            vidas = PlayerPrefs.GetInt("vidas");
        }
        else
        {
            vidas = 3;
            PlayerPrefs.SetInt("vidas", vidas);
        }

        if (PlayerPrefs.HasKey("salud"))
        {
            salud = PlayerPrefs.GetInt("salud");

            if (salud == 3)
            {
                // Desactivar el modelo normal
                playerModels[0].SetActive(false);

                // Activar el modelo de fuego
                playerModels[1].SetActive(true);
            }
        }
        else
        {
            salud = 2;
            PlayerPrefs.SetInt("salud", salud);
        }

        if (PlayerPrefs.HasKey("puntuacion"))
        {
            puntuacion = PlayerPrefs.GetInt("puntuacion");
        }
        else
        {
            puntuacion = 0;
            PlayerPrefs.SetInt("puntuacion", puntuacion);
        }

        if (PlayerPrefs.HasKey("monedas"))
        {
            monedas = PlayerPrefs.GetInt("monedas");
        }
        else
        {
            monedas = 0;
            PlayerPrefs.SetInt("monedas", monedas);
        }
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

    void OnDisparoPressed()
    {
        disparo = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si tocamos el lado de un enemigo, perdemos vida, si tocamos su cabeza, le matamos
        if (other.gameObject.tag == "EnemigoLado" && !dañado)
        {
            // Si recibe daño, no puede dañar ni recibir mas daño durante x tiempo
            dañado = true;
            tiempoUltimoDaño = 0.0f;

            salud -= 1;
            puntuacion -= 50;

            //TODO Al chocarse con un enemigo de lado, empujar al jugador o al enemigo

            // Si salud = 2, cambiar modelo, si salud = 1, reducir tamaño
            if (salud == 2)
            {
                // Desactivar el modelo de fuego
                playerModels[1].SetActive(false);

                // Activar el modelo normal
                playerModels[0].SetActive(true);
            }
            else if (salud == 1)
            {
                // Reducir tamaño
                CambiarTamaño(false);
            }
            
            // Sonido daño:
            Sonido(dañoSound);
            //audioSource.PlayOneShot(dañoSound);
        }
        else if (other.gameObject.tag == "EnemigoTop" && !dañado)
        {
            puntuacion += 25;
            Destroy(other.transform.parent.gameObject);
        }
        else if (other.gameObject.tag == "BloqueDestructible")
        {
            Destroy(other.gameObject);

            //TODO animacion o particulas de romper bloque?

            // Sonido al romper el bloque:
            //! Suena bastante bajo
            Sonido(ladrilloSound);
            //audioSource.PlayOneShot(ladrilloSound);
        }
        else if (other.gameObject.tag == "CambiarNivel")
        {
            puntuacion += 100;
            
            // Sonido cambiar de nivel:
            Sonido(cambiarNivelSound);
            //audioSource.PlayOneShot(cambiarNivelSound);

            // Se ha de pasar puntuacion, monedas, tamaño (o hacer if en start), salud y vidas al siguiente nivel
            PlayerPrefs.SetInt("vidas", vidas);
            PlayerPrefs.SetInt("salud", salud);
            PlayerPrefs.SetInt("puntuacion", puntuacion);
            PlayerPrefs.SetInt("monedas", monedas);
            PlayerPrefs.Save();

            //Esperar(2.0f);

            SceneManager.LoadScene(escena);
        }
    }

    public void Sonido(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // El jugador no se puede mover hasta que acabe la corutina
    IEnumerator RespawnWait(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        estaVivo = true;
        yield return null;
    }

    public void CambiarTamaño(bool aumentar)
    {
        //! A veces cambia de tamaño mas o menos de la cuenta...
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
