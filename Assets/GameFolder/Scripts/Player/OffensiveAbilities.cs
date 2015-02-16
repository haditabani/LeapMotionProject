﻿using UnityEngine;
using System.Collections;
using Leap;

public class OffensiveAbilities : MonoBehaviour
{
	// PLAYER OBJECTS
	public PlayerLogic playerLogic;
	public GameObject thisCamera;
	public HandController handController = null;
	// GAME LOGIC
	private GameLogic game;
	// INTERNAL VARIABLES
	private bool fireballCharged = false;
	private bool handWasFist = false;
	private float minVal = 0.5f;

	// Use this for initialization
	void Start ()
	{
			game = (GameLogic)GetComponent (typeof(GameLogic));
	}

	// Check for input once a frame
	public void controlCheck ()
	{
		HandModel[] hands = handController.GetAllGraphicsHands ();
		if (hands.Length == 1) {
			Vector3 direction0 = (hands [0].GetPalmPosition () - handController.transform.position).normalized;
			Vector3 normal0 = hands [0].GetPalmNormal ().normalized;
	
			//  Charge a fireball, -.6 or less means the palm is facing the camera
			if (Vector3.Dot (normal0, thisCamera.transform.forward) < -.6 && !fireballCharged) {
				fireballCharged = true;
			}
		
			// Fire a fireball, .6 or more means the palm is facing away from the camera
			if (Vector3.Dot (normal0, thisCamera.transform.forward) > .6 && fireballCharged) {
				fireballCharged = false;
		// First check if the player has enough energy
				if (playerLogic.getEnergy () > 10) {
					game.playerCastFireball ();
				}
			}

			bool handIsFist = checkFist (hands[0].GetLeapHand());
			if (handIsFist && !handWasFist)
			{
				game.fistProjectile();
				handWasFist = true;
			}

			handWasFist = false;

		} else if (hands.Length > 1) {
			Vector3 direction0 = (hands [0].GetPalmPosition () - handController.transform.position).normalized;
			Vector3 normal0 = hands [0].GetPalmNormal ().normalized;
	
			Vector3 direction1 = (hands [1].GetPalmPosition () - handController.transform.position).normalized;
			Vector3 normal1 = hands [1].GetPalmNormal ().normalized;
	
			//  Check for and perform a clap attack
			if (Vector3.Dot (normal0, normal1) < -.6) {
				Vector3 distance = hands [0].GetPalmPosition () - hands [1].GetPalmPosition ();
				if (distance.magnitude < .09) {
					game.clapAttack (playerLogic.transform.position + new Vector3 (0.0f, 0.7f, 0.0f));
				}
			}
		}
	}

	private bool checkFist(Hand hand){
		float sum = 0;
		for (int i = 0; i < hand.Fingers.Count; i++) {
			Finger f = hand.Fingers[i];
			Vector meta = f.Bone(Bone.BoneType.TYPE_METACARPAL).Direction;
			Vector proxi = f.Bone(Bone.BoneType.TYPE_PROXIMAL).Direction;
			Vector inter = f.Bone(Bone.BoneType.TYPE_INTERMEDIATE).Direction;
			float dMetaProxi = meta.Dot(proxi);
			float dProxiInter = proxi.Dot(inter);
			sum += dMetaProxi;
			sum += dProxiInter;
		}
		sum = sum/10;
		print("sum = " + sum);
		if(sum <= minVal && getExtendedFingers(hand)== 0){
			return true;
		}else{
			return false;
		}
	}

	private int getExtendedFingers(Hand h){
		int extendedFingers = 0;
		for(int i=0;i <h.Fingers.Count;i++){
			Finger finger = h.Fingers[i];
			if(finger.IsExtended) extendedFingers++;
		}
		return extendedFingers;
	}

}
