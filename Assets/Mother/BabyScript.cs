using UnityEngine;
using System.Collections;

public class BabyScript : MonoBehaviour {

	public AudioClip CrySound;
	public AudioClip KillSound;
	public AudioClip DeathSound;

	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private GameObject cryText;	
	public int state = 0;
	private Animator anim;		

	// Use this for initialization
	void Start () 
	{
		groundCheck = transform.Find("groundCheck");
		cryText = GameObject.Find("TextGetMe");
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//groundCheck.transform.eulerAngles = new Vector3(0, 0, -transform.rotation.z);
		cryText.SetActive(false);


		if (state < 0)
		{
			transform.Rotate(0,  0, 10.0f);
			//shadow.transform.position = new Vector3(transform.position.x-100.0f, transform.position.y-100.0f, transform.position.z-100.0f);
			if (transform.position.y < -20) 
			{
				BabyDead();
			}
			return;
		}

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));  
		
		if (transform.eulerAngles.z < 330 && transform.eulerAngles.z > 30 && rigidbody2D.angularVelocity < 0.1f && Mathf.Abs(rigidbody2D.velocity.x) + Mathf.Abs(rigidbody2D.velocity.y) < 0.01f)
		{
			
			//rigidbody2D.AddForce(new Vector2(0f, 10.0f));
			rigidbody2D.AddTorque((transform.eulerAngles.z > 180) ? 2.5f : -2.5f);
			Debug.Log("Baby Down 2");
		}
		else
		{
			if (rigidbody2D.angularVelocity < 0.01f && Mathf.Abs(rigidbody2D.velocity.x) + Mathf.Abs(rigidbody2D.velocity.y) < 0.01f)
			{
				cryText.SetActive(true);
				cryText.transform.position = new Vector3(cryText.transform.position.x, transform.position.y+0.15f + 0.1f*Mathf.Abs(Mathf.Sin(3.0f*Time.time)), 0 );
				anim.SetTrigger("Cry");	
				audio.mute = false;
				//audio.PlayOneShot(CrySound, 2.0f);
			}
		}

		if (transform.position.y < -2) 
		{
			Killed();//BabyDead();
		}
	}

	public void Killed() 
	{
		audio.mute = false;
		audio.PlayOneShot(DeathSound, 2.0f);
		state = -1;
		collider2D.enabled = false;
		rigidbody2D.AddForce(new Vector2(0f, 300.0f));
		anim.SetTrigger("Cry");
	}

	public void Reset() 
	{
		state = 0;
		collider2D.enabled = true;
		anim.SetTrigger("DontCry");
	}


	void BabyDead() 
	{
		GameObject mum = GameObject.Find("Mother");
		mum.GetComponent<PlayerControl2D>().ResetStage();
		Reset();
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{ 
		//Debug.Log("bcollision1 ");
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}
	
	void  OnTriggerEnter2D (Collider2D other) 
	{ 
		//Debug.Log("bcollision2 ");
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}
	
	public void onEated()
	{
		Debug.Log("Baby Eated");
		gameObject.SetActive(false);
		anim.SetTrigger("DontCry");
		audio.mute = true;
	}

	public void onThrown(float dir)
	{
		audio.mute = true;
		Debug.Log("Baby Thrown");
		rigidbody2D.velocity = new Vector2(dir * 4.5f, 0);
		//rigidbody2D.angularVelocity = dir*100.0f;
		rigidbody2D.AddTorque(dir*20);
	}
}
