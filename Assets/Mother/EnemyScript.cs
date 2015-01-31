using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	public AudioClip DeathSound;

	[HideInInspector]
	public bool facingRight = true;			// For determining which way the player is currently facing.
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	
	
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	
	
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.
	
	public float turnTime = 1.0f;
	private float nextTurn = 0.0f;
	private bool loaded = false;

	private int state = 0;
	private Transform shadow;	
	
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
		shadow = transform.Find("shadow");
	}
	
	
	void Update()
	{

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		
		//Debug.Log("grounded "+grounded);
		
		
	}
	
	
	void FixedUpdate ()
	{
		if (state < 0)
		{
			transform.Rotate(0,  0, 10.0f);
			shadow.transform.position = new Vector3(transform.position.x-100.0f, transform.position.y-100.0f, transform.position.z-100.0f);
			if (transform.position.y < -20) 
			{
				Destroy(this.gameObject);
			}
			return;
		}
		// Cache the horizontal input.
		float h = -1.0f;
		bool fire = false;

		if (Time.time > nextTurn) 
		{
			nextTurn = Time.time + turnTime;
			Flip ();
		}


		if (facingRight) h = -h;

		
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rigidbody2D.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody2D.AddForce(Vector2.right * h * moveForce);
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

		
		
		if (grounded)
		{
			if  (Mathf.Abs(h) <= 0.5f)
			{
				rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
			}
		}
		
		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight)
			// ... flip the player.
			Flip();
		
		RaycastHit2D li = Physics2D.Linecast(transform.position, new Vector3(transform.position.x, transform.position.y-10, 0), 1 << LayerMask.NameToLayer("Ground"));  
		if (li)
		{
			shadow.transform.position = new Vector3(transform.position.x, li.point.y+0.05f, 0.0f);
		}
		else
		{
			shadow.transform.position = new Vector3(transform.position.x-100.0f, transform.position.y-100.0f, transform.position.z-100.0f);
		}

		if(Mathf.Abs(rigidbody2D.velocity.x) > 0.1f)
		{
			anim.SetTrigger("Walk");
		}
		else
		{
			anim.SetTrigger("Stop");
		}
	}
	
	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	

	
	void OnCollisionEnter2D(Collision2D other) 
	{ 

		//Debug.Log("*** Enemy collision ");
		if(state == 0 && other.gameObject.tag=="Baby")
		{
			Rigidbody2D otherR = other.gameObject.GetComponent<Rigidbody2D>();

			if (Mathf.Abs(otherR.velocity.x) + Mathf.Abs(otherR.velocity.y) > 1.0f)
			{
				collider2D.enabled = false;
				rigidbody2D.AddForce(new Vector2(0f, 300.0f));
				state = -1;
				audio.PlayOneShot(DeathSound, 2.0f);
			}
			else
			{
				GameObject mum = GameObject.Find("Baby");
				mum.GetComponent<BabyScript>().Killed();
				//mum.GetComponent<PlayerControl2D>().ResetStage();
			}
		}


		if(state == 0 && other.gameObject.tag=="Player")
		{
			GameObject mum = GameObject.Find("Mother");
			mum.GetComponent<PlayerControl2D>().Killed();
		}

		//	this.enabled = false;
		//} 
		
		
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}
	
	void  OnTriggerEnter2D (Collider2D other) 
	{ 
		if(other.gameObject.tag=="Player")
		{
			Debug.Log("*** Enemy collision 2");
		}
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}

}
