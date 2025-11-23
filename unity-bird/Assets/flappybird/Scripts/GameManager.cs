using UnityEngine;
using UnityEngine.UI;
using extOSC;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public extOSC.OSCReceiver oscReceiver;
    public extOSC.OSCTransmitter oscTransmitter;
    private int etatEnMemoire = 1;

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Parallax ground;
    [SerializeField] private Text scoreText;
   // [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;

    public int score { get; private set; } = 0;

    private bool isPlaying = false;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    public static float Proportion(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return Mathf.Clamp(((value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin), outputMin, outputMax);
    }

    void TraiterOscAngle(OSCMessage message)
    {
        // Si le message n'a pas d'argument ou l'argument n'est pas un Int on l'ignore
        if (message.Values.Count == 0)
        {
            Debug.Log("No value in OSC message");
            return;
        }

        if (message.Values[0].Type != OSCValueType.Int)
        {
            Debug.Log("Value in message is not an Int");
            return;
        }

        // Récupérer la valeur de l’angle depuis le message OSC

        // EXEMPLE : utiliser la valeur pour appliquer une rotation
        // Adapter proportionnellement la valeur reçue
        //float angle = Proportion(value, 0, 1, 0, 1);
        // Appliquer la rotation à l’objet
        //transform.rotation = Quaternion.Euler(0, angle, 0);
        int nouveauEtat = message.Values[0].IntValue; // REMPLACER ici les ... par le code qui permet de récuérer la nouvelle donnée du flux
        if (etatEnMemoire != nouveauEtat)
        { // Le code compare le nouvel etat avec l'etat en mémoire
            etatEnMemoire = nouveauEtat; // Le code met à jour l'état mémorisé
            if (!isPlaying && nouveauEtat == 0)
            {
                Play();
            }
            else
            {
                // METTRE ici le code pour lorsque le bouton est relaché
            }
        }

    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    public bool IsPlaying() {
        return isPlaying;
    }

     void Start()
    {
        Stop();
        oscReceiver.Bind("/bouton", TraiterOscAngle);
    }

    public void Stop()
    {
        var oSCMessage = new OSCMessage("/pixel");  // CHANGER l'adresse /pixel pour l'adresse désirée

        // AJOUTER autant d'arguments que désiré
        // Dans cet exemple, trois arguments de type entiers (int) sont ajoutés au message
        oSCMessage.AddValue(OSCValue.Int(255)); // Ajoute l'entier 255
        oSCMessage.AddValue(OSCValue.Int(0)); // Ajoute un autre 255
        oSCMessage.AddValue(OSCValue.Int(0)); // Ajoute un troisième 255

        // Envoyer le message 
        oscTransmitter.Send(oSCMessage);
        //Time.timeScale = 0f;
        player.enabled = false;
        spawner.enabled = false;
        ground.enabled = false;
        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++) {
            pipes[i].enabled = false;
        }

        isPlaying = false;
    }

    public void Play()
    {
        var oSCMessage = new OSCMessage("/pixel");  // CHANGER l'adresse /pixel pour l'adresse désirée

        // AJOUTER autant d'arguments que désiré
        // Dans cet exemple, trois arguments de type entiers (int) sont ajoutés au message
        oSCMessage.AddValue(OSCValue.Int(0)); // Ajoute l'entier 255
        oSCMessage.AddValue(OSCValue.Int(255)); // Ajoute un autre 255
        oSCMessage.AddValue(OSCValue.Int(0)); // Ajoute un troisième 255

        // Envoyer le message 
        oscTransmitter.Send(oSCMessage);

        score = 0;
        scoreText.text = score.ToString();

        //playButton.SetActive(false);
        gameOver.SetActive(false);

        //Time.timeScale = 1f;
        player.enabled = true;
        spawner.enabled = true;
        ground.enabled = true;

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }

        isPlaying = true;
    }

    public void GameOver()
    {
        //.SetActive(true);
        gameOver.SetActive(true);

        Stop();
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

    public void Update()
    {
        if (!isPlaying && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))) {
            Play();
        }
    }

}
