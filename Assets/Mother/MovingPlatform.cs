using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour 
{

	public float platformSpeed = 0.3f;		
	
	public bool facingRight = true;			// For determining which way the player is currently facing.
	private float startY;

	// Use this for initialization
	void Start () {

		startY = transform.position.y;

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void FixedUpdate () 
	{
		if (facingRight)	
		{
			rigidbody2D.velocity = new Vector2(-platformSpeed, 0);
			//transform.position = new Vector2(transform.position.x+0.005f, transform.position.y);
		}
		else
		{
			rigidbody2D.velocity = new Vector2(+platformSpeed, 0);
			//transform.position = new Vector2(transform.position.x-0.005f, transform.position.y);
		}

		transform.position = new Vector2(transform.position.x, startY);
		//transform.position = new Vector2(transform.position.x, startY);
		/*if (facingRight)	
		{
		
			transform.position = new Vector2(transform.position.x-0.005f, startY);
		}
		else
		{
			//rigidbody2D.velocity = new Vector2(+0.25f, 0);
			transform.position = new Vector2(transform.position.x+0.005f, startY);
		}*/
	}



	void OnCollisionEnter2D(Collision2D collision) 
	{ 
		Debug.Log("Platform collision1 ");

	}
	
	void  OnTriggerEnter2D (Collider2D other) 
	{ 
		Debug.Log("Platform collision2 ");
		//if(other.gameObject.tag=="ground")
		if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("TransparentFX"))
		{ 
			Debug.Log("Ground  Platform collision2 ");
			facingRight = !facingRight;
			if (facingRight)	
			{
				rigidbody2D.velocity = new Vector2(-platformSpeed, 0);
				//transform.position = new Vector2(transform.position.x+0.005f, transform.position.y);
			}
			else
			{
				rigidbody2D.velocity = new Vector2(+platformSpeed, 0);
				//transform.position = new Vector2(transform.position.x-0.005f, transform.position.y);
			}
		} 
		//Debug.Log("bcollision2 ");
		//if(collision.gameObject.tag=="Player"){ 
		//	this.enabled = false;
		//} 
	}
}
