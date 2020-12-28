using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    Rigidbody2D rigidbody;
    Quaternion downRatation;
    Quaternion forwardRotation;

    GameManager game;

     void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRatation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameManager.Instance;
        //rigidbody.simulated = false; // comment cho nay` 
    }

    void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

     void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity; 
    }
    void Update()
    {
        if (game.GameOver) return;


        if (Input.GetMouseButtonDown(0))
        {
            transform.rotation = forwardRotation;
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRatation, tiltSmooth * Time.deltaTime);
    }
     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone"){
            //register a score event
            OnPlayerScored(); // event sent to GameNanager;
            //play a sound
        }
        if (collision.gameObject.tag == "DeadZone")
        {
            rigidbody.simulated = false;
            //register a dead event
            OnPlayerDied();  // event sent to GameManager;
            //play a sound
        }
    }
}
