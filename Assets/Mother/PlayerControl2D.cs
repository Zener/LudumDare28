using UnityEngine;
using System.Collections;

public class PlayerControl2D : MonoBehaviour
{
	public AudioClip JumpSound;
	public AudioClip FireSound;
	public AudioClip DeathSound;
	public AudioClip EatSound;

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
	public GameObject baby;
	
	public float fireRate = 0.3f;
	private float nextFire = 0.0f;
	private bool loaded = false;

	private Transform shadow;	
	public int state = 0;
	
	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		anim = GetComponent<Animator>();
		shadow = transform.Find("shadow");
		//ResetStage();
		Transform t = GameObject.Find("StartingPos").transform;
		transform.position = new Vector3(t.position.x, t.position.y, t.position.z);
	}
	
	
	void Update()
	{
		if (state < 0)
		{
			anim.SetTrigger("Cry");
			//transform.Rotate(0,  0, 10.0f);
			shadow.transform.position = new Vector3(transform.position.x-100.0f, transform.position.y-100.0f, transform.position.z-100.0f);
			if (transform.position.y < -2) 
			{
				ResetStage();
			}
			return;
		}

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		
		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump") && grounded)
			jump = true;


		//Debug.Log("grounded "+grounded);
		
		
	}
	
	
	void FixedUpdate ()
	{
		if (state < 0)
		{
			return;
		}
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		bool fire = false;
		
		if (Input.GetButton ("Fire1") && Time.time > nextFire && loaded) 
		{
			nextFire = Time.time + fireRate;
			fire = true;
		}
		
		
		// The Speed animator parameter is set to the absolute value of the horizontal input.
		//anim.SetFloat("Speed", Mathf.Abs(h));
		
		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * rigidbody2D.velocity.x < maxSpeed)
			// ... add a force to the player.
			rigidbody2D.AddForce(Vector2.right * h * moveForce);
		
		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);

		if (fire)
		{
			//Launch baby
			FireBaby();
		}
		
		
		if (grounded)
		{
			if  (Mathf.Abs(h) <= 0.5f)
			{
				rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
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

		// If the input is moving the player right and the player is facing left...
		if(h > 0 && !facingRight)
			// ... flip the player.
			Flip();
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight)
			// ... flip the player.
			Flip();
		
		// If the player should jump...
		if(jump)
		{
			audio.PlayOneShot(JumpSound, 2.0f);
			anim.SetTrigger("Stop");
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			//rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpForce);
			
			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
		

		//Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		RaycastHit2D li = Physics2D.Linecast(new Vector3(transform.position.x, transform.position.y-0.1f, 0), new Vector3(transform.position.x, transform.position.y-10, 0), 1 << LayerMask.NameToLayer("Ground"));  
		if (li)
		{
			shadow.transform.position = new Vector3(transform.position.x, li.point.y+0.05f, 0.0f);
		}
		else
		{
			shadow.transform.position = new Vector3(transform.position.x, -100.0f, 0.0f);
		}

		if (baby.GetComponent<BabyScript>().state < 0)
		{
			anim.SetTrigger("Cry");
		}
			
		if (transform.position.y < -2) 
		{
			//ResetStage();
			state = -1;
		}



		
	}


	public void Killed() 
	{
		state = -1;
		collider2D.enabled = false;
		rigidbody2D.velocity = new Vector2(0,0);
		rigidbody2D.AddForce(new Vector2(0f, 1500.0f));

		anim.SetTrigger("Cry");
		audio.PlayOneShot(DeathSound, 2.0f);
	}

	public void ResetStage()
	{
		Transform t = GameObject.Find("StartingPos").transform;
		transform.position = new Vector3(t.position.x, t.position.y, t.position.z);
		anim.SetTrigger("Eat");
		baby.GetComponent<BabyScript>().onEated();
		loaded = true;
		state = 0;
		transform.eulerAngles = new Vector3(0,0,0);
		collider2D.enabled = true;

		baby.GetComponent<BabyScript>().Reset();
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


	void FireBaby()
	{
		audio.PlayOneShot(FireSound, 2.0f);
		baby.gameObject.SetActive(true);
		baby.transform.position = new Vector3(transform.position.x + (facingRight? 0.1f:-0.1f), transform.position.y, transform.position.z);
		baby.GetComponent<BabyScript>().onThrown(facingRight? 1:-1);
		anim.SetTrigger("Puke");
		loaded = false;
	}
	
	void OnCollisionEnter2D(Collision2D other) 
	{ 
		if(other.gameObject.tag=="Baby")
		{
			if (Time.time > nextFire)
			{
				audio.PlayOneShot(EatSound, 2.0f);
				Debug.Log("Collides with baby ");
				anim.SetTrigger("Eat");
				//BabyScript. other = other.gameObject.GetComponent<>
				baby.GetComponent<BabyScript>().onEated();
				loaded = true;
			}
		}

		if(other.gameObject.tag=="CheckPoint")
		{
			Debug.Log("************CHECKPOINT");
			Transform t = GameObject.Find("CheckPoint").transform;
			Transform t2 = GameObject.Find("StartingPos").transform;
			t2.position = new Vector3(t.position.x, t.position.y, t.position.z);
		}

		if(other.gameObject.tag=="Home")
		{
			Debug.Log("************HOME");
			if (loaded)
			{
				//End Game
				Application.LoadLevel("EndGame");
			}
		}
		
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}
	
	void  OnTriggerEnter2D (Collider2D other) 
	{ 
		/*if(other.gameObject.LayerMask =="Ground")
		{
			Debug.Log("collision2 ");
			shadow.transform.position = new Vector3(shadow.transform.position.x, other.gameObject.transform.position.y, shadow.transform.position.z);
		}*/
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}
	

}