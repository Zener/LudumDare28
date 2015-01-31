using UnityEngine;
using System.Collections;


public class CoverScript : MonoBehaviour 
{
	
	private float StartTime;
	
	// Use this for initialization
	void Start () {
		StartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time-StartTime >= 2)
		{
			if (Input.GetButton ("Fire1"))
		    {
				Application.LoadLevel("Level001");
			}
		}
	}
}
