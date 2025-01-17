// Spaceship C# Script (version: 1.02)
// SPACE UNITY - Space Scene Construction Kit
// http://www.spaceunity.com
// (c) 2013 Stefan Persson

// DESCRIPTION:
// Thruster, steering and weapon control script for Spaceship prefab.

// INSTRUCTIONS:
// This script is attached to the Spaceship demo prefab. Configure parameters to suit your needs.

// PARAMETERS:
//  thrusters			(Thruster array containing reference to thrusters prefabs attached to the ship for propulsion)
//  rollRate			(multiplier for rolling the ship when steering left/right)	
//  yawRate				(multiplier for rudder/steering the ship when steering left/right)
//  pitchRate			(multiplier for pitch when steering up/down)
//  weaponMountPoints	(Vector3 array for mount points relative to ship where weapons will fire from)
//  laserShotPrefab		(reference to Laser Shot prefab, i.e. the laser bullet prefab to be instanitated)
//  soundEffectFire		(sound effect audio clip to be played when firing weapon)

// HINTS:
// The propulsion force of the thruster is configured for each attached thruster in the Thruster script.

// Version History
// 1.02 - Prefixed with SU_Spaceship to avoid naming conflicts.
// 1.01 - Initial Release.

using UnityEngine;
using System.Collections;

public class SU_Spaceship_2 : MonoBehaviour {
	
	// Array of thrusters attached to the spaceship
	public SU_Thruster[] thrusters;
	// Specify the roll rate (multiplier for rolling the ship when steering left/right)	
	public float rollRate = 100.0f;
	// Specify the yaw rate (multiplier for rudder/steering the ship when steering left/right)
	public float yawRate = 30.0f;
	// Specify the pitch rate (multiplier for pitch when steering up/down)
	public float pitchRate = 100.0f;
	// Weapon mount points on ship (this is where lasers will be fired from)
	public Vector3[] weaponMountPoints;	
	// Laser shot prefab
	public Transform laserShotPrefab;
	// Laser shot sound effect
	public AudioClip soundEffectFire;
	
	// Private variables
	private Rigidbody _cacheRigidbody;
	
	
	void Start () {		
		// Ensure that the thrusters in the array have been linked properly
		foreach (SU_Thruster _thruster in thrusters) {
			if (_thruster == null) {
				Debug.LogError("Thruster array not properly configured. Attach thrusters to the game object and link them to the Thrusters array.");
			}			
		}
		
		// Cache reference to rigidbody to improve performance
		_cacheRigidbody = GetComponent<Rigidbody>();
		if (_cacheRigidbody == null) {
			Debug.LogError("Spaceship has no rigidbody - the thruster scripts will fail. Add rigidbody component to the spaceship.");
		}
	}
	
	void Update () {
		// Start all thrusters when pressing Fire 1
		if (Input.GetButtonDown("Fire1")) {		
			foreach (SU_Thruster _thruster in thrusters) {
				_thruster.StartThruster();
			}
		}
		// Stop all thrusters when releasing Fire 1
		if (Input.GetButtonUp("Fire1")) {		
			foreach (SU_Thruster _thruster in thrusters) {
				_thruster.StopThruster();
			}
		}
		
		if (Input.GetButtonDown("Fire2")) {
			// Itereate through each weapon mount point Vector3 in array
			foreach (Vector3 _wmp in weaponMountPoints) {
				// Calculate where the position is in world space for the mount point
				Vector3 _pos = transform.position + transform.right * _wmp.x + transform.up * _wmp.y + transform.forward * _wmp.z;
				// Instantiate the laser prefab at position with the spaceships rotation
				Transform _laserShot = (Transform) Instantiate(laserShotPrefab, _pos, transform.rotation);
				// Specify which transform it was that fired this round so we can ignore it for collision/hit
				_laserShot.GetComponent<SU_LaserShot>().firedBy = transform;
				
			}
			// Play sound effect when firing
			if (soundEffectFire != null) {
				GetComponent<AudioSource>().PlayOneShot(soundEffectFire);
			}
		}		
	}

	void FixedUpdate () {
		// In the physics update...
		// Add relative rotational roll torque when steering left/right
		_cacheRigidbody.AddRelativeTorque(new Vector3(0,0,-Input.GetAxis("Horizontal")*rollRate*_cacheRigidbody.mass));
		// Add rudder yaw torque when steering left/right
		_cacheRigidbody.AddRelativeTorque(new Vector3(0,Input.GetAxis("Horizontal")*yawRate*_cacheRigidbody.mass,0));
		// Add pitch torque when steering up/down
		_cacheRigidbody.AddRelativeTorque(new Vector3(-Input.GetAxis("Vertical")*pitchRate*_cacheRigidbody.mass,0,0));	
	}
}
