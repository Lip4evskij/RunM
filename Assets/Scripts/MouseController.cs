using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
    public GameObject rest;
    public Texture2D coinIconTexture;
    private uint coins = 0;
    public Transform groundCheckTransform;
    private bool grounded;
    public LayerMask groundCheckLayerMask;
    Animator animator;
    public ParticleSystem jetpack;
    private bool dead = false;

    public float jetpackForce = 75.0f;
    public float forwardMovementSpeed = 3.0f;
    public AudioClip coinCollectSound;
	// Use this for initialization
    void DisplayCoinsCount()
    {
        Rect coinIconRect = new Rect(10, 10, 32, 32);
        GUI.DrawTexture(coinIconRect, coinIconTexture);

        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;

        Rect labelRect = new Rect(coinIconRect.xMax, coinIconRect.y, 60, 32);
        GUI.Label(labelRect, coins.ToString(), style);
    }
    void OnGUI()
    {
        DisplayCoinsCount();
        RestartLevel();
    }
    void RestartLevel()
    {
        if (dead && grounded)
        {
            Rect buttonRect = new Rect(Screen.width * 2f, Screen.height * 2f, Screen.width * 1f, Screen.height * 1f);
            if (GUI.Button(buttonRect, "Tap to restart!"))
            {
                Application.LoadLevel(0);
            };
        }
    }
	void Start () {
        animator = GetComponent<Animator>();
	}
    void CollectCoin(Collider2D coinCollider)
    {
        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
        coins++;

        Destroy(coinCollider.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1");

        if (jetpackActive)
        {
          
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jetpackForce));
            
        }
        if (!dead)
        {
            Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;
            newVelocity.x = forwardMovementSpeed;
            GetComponent<Rigidbody2D>().velocity = newVelocity;
        }
        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
    }
    void UpdateGroundedStatus()
    {
        //1
        grounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);

        //2
        animator.SetBool("grounded", grounded);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Coins"))
            CollectCoin(collider);
        else
            HitByLaser(collider);
    }

    void HitByLaser(Collider2D laserCollider)
    {
        if (!dead)
            laserCollider.gameObject.GetComponent<AudioSource>().Play();
        dead = true;
        animator.SetBool("dead", true);
    }
    void AdjustJetpack(bool jetpackActive)
    {
        jetpack.enableEmission = !grounded;
        jetpack.emissionRate = jetpackActive ? 300.0f : 75.0f;
    }
}
