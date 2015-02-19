﻿using UnityEngine;
using System.Collections;

public class UIFollowPlayer : MonoBehaviour {

	public PlayerLogic player;

	public GameObject camera;

	// Have different offset positions for the two different types
	public Vector3 offensiveOffset;
	public Vector3 defensiveOffset;

	private Vector3 thisOffset = new Vector3(0.0f, 0.0f, 0.0f);

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		this.transform.position = camera.transform.position + thisOffset;
	}

	public void enableUI()
	{
		this.gameObject.SetActive (true);
		if (player.isDefensivePlayer)
		{
			thisOffset = camera.transform.rotation * defensiveOffset;
		}
		else
		{
			thisOffset = camera.transform.rotation * offensiveOffset;
		}
		this.transform.rotation = Quaternion.Euler (0.0f, camera.transform.rotation.eulerAngles.y, 0.0f);
	}

	public void disableUI()
	{
		this.gameObject.SetActive (false);
	}
}
