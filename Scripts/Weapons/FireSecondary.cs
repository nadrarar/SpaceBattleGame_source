using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FireSecondary : MonoBehaviour {
	
	public GameObject MisslePrefab;
    public GameObject MisslePrefabGreen;
    public GameObject MisslePrefabWhite;
	public bool hasMissiles = false;
	public bool hasScatter = false;
    public bool hasBurst = false;
	public bool hasSonic = false;
	private float lastShootTime = 0;
    // Used to fix Sonic Wave's reload time issue when switching weapons
    private bool sonicWaveReloading = false;
    // Used to prevent missile tracking on other secondary weapons
    public bool missileTracking = false;

    //Maximum weapons available
    public static int MAX_WEAPON = 3;

    // Stores secondary weapon characteristics
	private int [] ammunitionForWeapons = new int [MAX_WEAPON + 1];
    private float [] velocity = new float [MAX_WEAPON + 1];
    private float [] reloadTime = new float [MAX_WEAPON + 1];
	
	//Switching weapon variables
	public int currentWeapon = 0;

	//Minimum weapon number
	const int MIN_WEAPON = 0;
	
	private const int MISSILE_INDEX = 0;
	private const int SCATTERSHOT_INDEX = 1;
    private const int BURSTGUN_INDEX = 2;
	private const int SONICWAVE_INDEX = 3;

    //Configures weapon sounds
    public AudioClip missileSound;
    public AudioClip scatterShotSound;
    public AudioClip burstGunSound;
    public AudioClip sonicWaveSound;
	
	private ParticleSystem sonicWaveAnimation;
	private float sonicWaveTimeout;
	
	// Use this for initialization
	void Start () {
		ammunitionForWeapons = new int [MAX_WEAPON + 1];
        // Set Missile Characteristics
        ammunitionForWeapons[0] = 20;
        velocity[0] = 60;
        reloadTime[0] = 0.6f;

        // Set ScatterShot Characteristics
        ammunitionForWeapons[1] = 30;
        velocity[1] = 140;
        reloadTime[1] = 0.4f;

        // Set BurstGun Characteristics
        ammunitionForWeapons[2] = 60;
        velocity[2] = 180;
        reloadTime[2] = 0.6f;
		
		//Set SonicWave Characteristics
		ammunitionForWeapons[3] = 5;
		velocity[3] = 500;
		reloadTime[3] = 5f;
		
		//Start all weapons with 10 ammunition
		//for (int i = 0; i < ammunitionForWeapons.Length; i ++) {
		//	ammunitionForWeapons[i] = 10;
		//}
        if (hasMissiles == false && hasScatter == true)
        {
            currentWeapon = 1;
        }
        else if (hasMissiles == false && hasBurst == true)
        {
            currentWeapon = 2;
        } else if(hasMissiles == false && hasSonic == true) 
		{
			currentWeapon = 3;
		}
		sonicWaveAnimation = (ParticleSystem)(gameObject.GetComponent("ParticleSystem"));
		if(sonicWaveAnimation) sonicWaveAnimation.enableEmission = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - sonicWaveTimeout > .05){
			if(sonicWaveAnimation){ 
				//Debug.Log ("error, trying to make sonic wave"); 
				sonicWaveAnimation.enableEmission = false;}
		}
		/*
		if(Input.GetMouseButtonDown(0)== true){
			var bullet = (GameObject) Instantiate (BulletPrefab, transform.position, transform.rotation);
			bullet.rigidbody.velocity = (transform.root.GetComponent<Rigidbody>()).velocity;
			bullet.rigidbody.AddForce(transform.root.forward*force);
		}*/
	}
	
	public void switchWeaponUp() {
		bool [] weaponsAvailable = {hasMissiles, hasScatter, hasBurst, hasSonic};
		for (int i = currentWeapon + 1; i <= MAX_WEAPON; i ++) {
			if (weaponsAvailable[i]) {
				currentWeapon = i;
				return;
			}
		}
	}
	
	public void switchWeaponDown() {
		bool [] weaponsAvailable = {hasMissiles, hasScatter, hasBurst, hasSonic};
		for (int i = currentWeapon - 1; i >= 0; i --) {
			if (weaponsAvailable[i]) {
				currentWeapon = i;
				return;
			}
		}
	}
	
	public void shoot () {
		switch (currentWeapon) {
		case 0:
			if(hasMissiles){
				fireMissile();
			}
			break;
		case 1:
			if(hasScatter){
				fireScattershot();
			}
            break;
        case 2:
            if(hasBurst){
                fireBurstGun();
            }
            break;
		case 3:
            if(hasSonic){
                fireSonicWave();
            }
        break;
        }
	}
	
	public void fireMissile () {
		if( (Time.time > lastShootTime + reloadTime[0]) && ammunitionForWeapons[MISSILE_INDEX] > 0 ){
            missileTracking = true;
			Transform ship = transform.root;
			Transform spawn = transform.FindChild("ProjectileSpawner");
			GameObject bullet = Instantiate(MisslePrefab, spawn.position, spawn.rotation) as GameObject;
			bullet.rigidbody.velocity = (ship.GetComponent<Rigidbody>()).velocity;
			bullet.rigidbody.AddForce(ship.forward*velocity[0],ForceMode.VelocityChange);
			bullet.GetComponent<MissileController>().missileSpawner = transform.root.gameObject;
			//ignore collisions between the bullet and the ship and weapon, so that the 
			//ship doesn't hit itself
			
			Physics.IgnoreCollision(bullet.collider,collider);
			if(ship.GetComponent<Collider>())
				Physics.IgnoreCollision(bullet.collider,ship.collider);
			//this loop is added because ships could have multiple colliders, and those are stored
			//in the children of the ship
			foreach(Collider childCollider in ship.GetComponentsInChildren<Collider>()){
				Physics.IgnoreCollision(bullet.collider,childCollider.collider);
			}
			ammunitionForWeapons[MISSILE_INDEX] --;
			lastShootTime = Time.time;

            audio.clip = missileSound;
			audio.Play();
		}
	}
	
	public void fireScattershot () {
		if( (Time.time > lastShootTime + reloadTime[1]) && ammunitionForWeapons[SCATTERSHOT_INDEX] > 0 ){
            missileTracking = false;
			Transform ship = transform.root;
			Transform spawn = transform.FindChild("ProjectileSpawner");
			GameObject [] bullets = new GameObject[5];
			for (int i = 0; i < 5; i ++) {
				bullets[i] = Instantiate(MisslePrefabWhite, spawn.position, spawn.rotation) as GameObject;

                bullets[i].GetComponent<MissileController>().missileSpawner = transform.root.gameObject;
				
				bullets[i].rigidbody.velocity = (ship.GetComponent<Rigidbody>()).velocity;
				//ignore collisions between the bullet and the ship and weapon, so that the 
				//ship doesn't hit itself
				Physics.IgnoreCollision(bullets[i].collider,collider);
				if(ship.GetComponent<Collider>())
					Physics.IgnoreCollision(bullets[i].collider,ship.collider);
				
				//this loop is added because ships could have multiple colliders, and those are stored
				//in the children of the ship
				foreach(Collider childCollider in ship.GetComponentsInChildren<Collider>()){
					Physics.IgnoreCollision(bullets[i].collider,childCollider.collider);
				}
			}
			
			//Ignore collisions between bullets
			for (int i = 0; i < 4; i ++) {
				for (int j = i + 1; j < 5; j++) {
					Physics.IgnoreCollision(bullets[i].collider, bullets[j].collider);
				}
			}

            //Adds force to the new scatterbullets
            Vector3 up = new Vector3(ship.forward.x, ship.forward.y + 0.1f, ship.forward.z);
            Vector3 down = new Vector3(ship.forward.x, ship.forward.y - 0.1f, ship.forward.z);
            Vector3 left = new Vector3(ship.forward.x - 0.1f, ship.forward.y, ship.forward.z);
            Vector3 right = new Vector3(ship.forward.x + 0.1f, ship.forward.y, ship.forward.z);
            Vector3 center = ship.forward;
            bullets[0].rigidbody.AddForce(up * velocity[1], ForceMode.VelocityChange);
            bullets[1].rigidbody.AddForce(down * velocity[1], ForceMode.VelocityChange);
            bullets[2].rigidbody.AddForce(left * velocity[1], ForceMode.VelocityChange);
            bullets[3].rigidbody.AddForce(right * velocity[1], ForceMode.VelocityChange);
            bullets[4].rigidbody.AddForce(center * velocity[1], ForceMode.VelocityChange);

            ammunitionForWeapons[SCATTERSHOT_INDEX]--;
            lastShootTime = Time.time;

            audio.clip = scatterShotSound;
            audio.Play();
		}
	}
	
	public void fireSonicWave () {
        if ((Time.time > lastShootTime + reloadTime[SONICWAVE_INDEX] || sonicWaveReloading == false) 
            && ammunitionForWeapons[SONICWAVE_INDEX] > 0)
        {
			if(sonicWaveAnimation) sonicWaveAnimation.enableEmission = true;
			sonicWaveTimeout = Time.time;
            StartCoroutine(sonicWaveTimer());
            Transform ship = transform.root;
			Destructible[] GameObjects = FindObjectsOfType(typeof(Destructible)) as Destructible[];
			foreach (Destructible thisGameObject in GameObjects)
			{
 				if (Vector3.Distance(thisGameObject.transform.position, ship.transform.position) <= 10.0f)
 				{
					if(thisGameObject.tag != "Player") 
					{
						thisGameObject.AdjustCurrentShield(-100);
					}
 				}

			}
			ammunitionForWeapons[SONICWAVE_INDEX] --;
			lastShootTime = Time.time;

            audio.clip = sonicWaveSound;
			audio.Play();
		}
	}

    // Used to prevent the long reload time from applying when switching from other weapons
    IEnumerator sonicWaveTimer()
    {
        sonicWaveReloading = true;
        yield return new WaitForSeconds(reloadTime[SONICWAVE_INDEX]);
        sonicWaveReloading = false;
    }
	
    public void fireBurstGun()
    {
        if ((Time.time > lastShootTime + reloadTime[2]) && ammunitionForWeapons[BURSTGUN_INDEX] > 0)
        {
            StartCoroutine(burstGunTimer());
        }
    }

    IEnumerator burstGunTimer()
    {
        if ((Time.time > lastShootTime + reloadTime[2]) && ammunitionForWeapons[BURSTGUN_INDEX] > 0)
        {
            missileTracking = false;
            lastShootTime = Time.time;
            for (int i = 0; i < 3; i++)
                {
                    Transform ship = transform.root;
                    Transform spawn = transform.FindChild("ProjectileSpawner");
                    GameObject bullet = Instantiate(MisslePrefabGreen, spawn.position, spawn.rotation) as GameObject;
                    bullet.rigidbody.velocity = (ship.GetComponent<Rigidbody>()).velocity;
                    bullet.rigidbody.AddForce(ship.forward * velocity[2], ForceMode.VelocityChange);
                    bullet.GetComponent<MissileController>().missileSpawner = transform.root.gameObject;
                    //ignore collisions between the bullet and the ship and weapon, so that the 
                    //ship doesn't hit itself

                    Physics.IgnoreCollision(bullet.collider, collider);
                    if (ship.GetComponent<Collider>())
                        Physics.IgnoreCollision(bullet.collider, ship.collider);
                    //this loop is added because ships could have multiple colliders, and those are stored
                    //in the children of the ship
                    foreach (Collider childCollider in ship.GetComponentsInChildren<Collider>())
                    {
                        Physics.IgnoreCollision(bullet.collider, childCollider.collider);
                    }
                    ammunitionForWeapons[BURSTGUN_INDEX]--;
                    yield return new WaitForSeconds(0.1f);

                    audio.clip = burstGunSound;
                    audio.Play();
                }
        }
    }
	
	public int ammo() {
		return ammunitionForWeapons[currentWeapon];
	}
	
	public int currentWeaponEquipped() {
		if(hasMissiles == false && hasScatter == false && hasBurst == false && hasSonic == false){
			return -1;	
		}
		return currentWeapon;
	}
}