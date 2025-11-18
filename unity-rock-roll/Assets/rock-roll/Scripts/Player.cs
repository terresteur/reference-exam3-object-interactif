using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class Player : MonoBehaviour
{
    public float torqueForce = 1f;
    public float jumpForce = 5f;

    public float checkRadius = 0.1f;
    public LayerMask groundLayer;
    public extOSC.OSCReceiver oscReceiver;

    private Rigidbody2D rb;
    private int etatEnMemoire = 1;
    private int etatEnMemoireB = 1;

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

        int nouveauEtat = message.Values[0].IntValue; // REMPLACER ici les ... par le code qui permet de récuérer la nouvelle donnée du flux
        if (etatEnMemoire != nouveauEtat)
        { // Le code compare le nouvel etat avec l'etat en mémoire
            etatEnMemoire = nouveauEtat; // Le code met à jour l'état mémorisé
            if (nouveauEtat > 0)
            {
                rb.AddTorque(-torqueForce * -nouveauEtat);


            }
            else if (nouveauEtat < 0)
            {
                rb.AddTorque(torqueForce * nouveauEtat);// METTRE ici le code pour lorsque le bouton est relaché

            }
        }

    }
    
    void TraiterOscBouton(OSCMessage messageB)
    {
        // Si le message n'a pas d'argument ou l'argument n'est pas un Int on l'ignore
        if (messageB.Values.Count == 0)
        {
            Debug.Log("No value in OSC message");
            return;
        }

        if (messageB.Values[0].Type != OSCValueType.Int)
        {
            Debug.Log("Value in message is not an Int");
            return;
        }

        int nouveauEtatB = messageB.Values[0].IntValue;
        if (etatEnMemoireB != nouveauEtatB)
        { // Le code compare le nouvel etat avec l'etat en mémoire
            etatEnMemoireB = nouveauEtatB; // Le code met à jour l'état mémorisé
            if (nouveauEtatB == 0)
            {
                if (IsGrounded())
                {
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
            else 
            {
                
            }
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        oscReceiver.Bind("/angle", TraiterOscAngle);
        oscReceiver.Bind("/bouton", TraiterOscBouton);
    }

    void FixedUpdate()
    {
        // Roll left/right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddTorque(-torqueForce); // clockwise
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddTorque(torqueForce); // counter-clockwise
        }

       
    }

    void Update()
    {
         // Jump
         // GetKeyDown() does not work in FixedUpdate()
        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            if (IsGrounded())
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    public bool IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            GetComponent<CircleCollider2D>().radius + extraHeight,
            groundLayer
        );
        return hit.collider != null;
    }
}
