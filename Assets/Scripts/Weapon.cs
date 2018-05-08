using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public float fireRate = 20f;
	public int damage = 10;
	public LayerMask whatToHit;

	public Transform BulletTrailPrefab;
	public Transform MuzzleFlashPrefab;
	public Transform HitPrefab;
	private float timeToSpawnEffect = 0f;
	public float effectSpawnRate = 10;

	// Handle Camera Shaking
	public float camShakeAmt = 0.05f;
	public float camShakeLength = 0.1f;
	private CameraShake camShake;

	private float timeToFire = 0f;
	private Transform firePoint;

	public string weaponShootSound = "DefaultShot";

	// caching
	private AudioManager audioManager;

	void Awake () {
		firePoint = transform.Find ("FirePoint");
		// Null Check
		if (firePoint == null) {
			Debug.LogError ("firePoint Not Found!");
		}
	}

	void Start () {
		camShake = GameMaster.gm.GetComponent<CameraShake> ();
		if (camShake == null) {
			Debug.LogError ("No CameraShake Script Found on GM Object");
		}
		// caching
		audioManager = AudioManager.instance;
		if (audioManager == null) {
			Debug.LogError ("No AudioManager instance found in Main Scene!");
		}
	}

	// Update is called once per frame
	void Update () {
		// If gun is single-fire
		if (fireRate == 0) {
			if (Input.GetButtonDown ("Fire1")) {
				Shoot ();
			}
		}
		// If gun is not single-fire
		else {
			if (Input.GetButton ("Fire1") && Time.time > timeToFire) {
				timeToFire = Time.time + 1 / fireRate;
				Shoot ();
			}
		}
	}

	void Shoot () {
		Vector2 mousePosition = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x,
			                        		 Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);

		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, mousePosition - firePointPosition, 100, whatToHit);

		//Debug.DrawLine (firePointPosition, (mousePosition - firePointPosition) * 100, Color.cyan);

		if (hit.collider != null) {
			Debug.DrawLine (firePointPosition, hit.point, Color.red);

			Enemy enemy = hit.collider.GetComponent<Enemy> ();
			if (enemy != null) {
				enemy.DamageEnemy (damage);
				// Debug.Log ("We hit " + hit.collider.name + " and did " + damage + " damage");
			}
		}
		// Moving Effect() handler below collision handler
		// This is exactly the same technique as the 'Shoot()' mechanism in 'Update()'
		if (Time.time >= timeToSpawnEffect) {
			Vector3 hitPos;
			Vector3 hitNormal;

			if (hit.collider == null) { 
				// If we don't hit anything, just continue with the current direction (dir vector * multiplier)
				hitPos = (mousePosition - firePointPosition) * 100;
				hitNormal = new Vector3 (9999, 9999, 9999);
			} else {
				hitPos = hit.point;
				hitNormal = hit.normal;
			}
			
			Effect (hitPos, hitNormal);
			timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
		}
	}

	void Effect (Vector3 hitPos, Vector3 hitNormal) {
		Transform trail = Instantiate (BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform;
		LineRenderer lr = trail.GetComponent<LineRenderer> ();

		if (lr != null) {
			// Start position (element 1 out of 2)
			lr.SetPosition (0, firePoint.position);
			// End position (element 2 out of 2)
			lr.SetPosition (1, hitPos);
		}

		Destroy (trail.gameObject, 0.04f);

		if (hitNormal != new Vector3 (9999, 9999, 9999)) {
			Transform hitClone = Instantiate (HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.forward, hitNormal)) as Transform;
			Destroy (hitClone.gameObject, 0.1f);
		}

		Transform muzzleClone = Instantiate (MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform;
		muzzleClone.parent = firePoint;
		float size = Random.Range (0.6f, 0.9f);
		muzzleClone.localScale = new Vector3 (size, size, size);
		// NTS: muzzleClone is a Transform object NOT gameObject.
		Destroy (muzzleClone.gameObject, 0.02f);

		// Shake the camera
		camShake.Shake(camShakeAmt, camShakeLength);

		// Play Shoot Sound
		audioManager.PlaySound (weaponShootSound);
	}
}
