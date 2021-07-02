using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class PacmanScript : MonoBehaviour
{
	[Header("Objects")]
	[SerializeField] GameObject sprite; 
	[SerializeField] Transform leftGate;
	[SerializeField] Transform rightGate;
	Rigidbody2D rb;

	[Header("Settings")]
	[SerializeField] float movingSpeed;
	[SerializeField] float powerUpTime;
	[SerializeField] float flashingSpeed;
	[HideInInspector] public float score;

	[Header("Game States")]
	[SerializeField] bool right;
	[SerializeField] bool left;
	[SerializeField] bool up;
	[SerializeField] bool down;
	[SerializeField] public bool powerUp;
	[SerializeField] bool hasTeleported;
	[SerializeField] public bool died;

	[Header("Sound Effects")]
	[SerializeField] AudioSource eatingAudio;

    // Start is called before the first frame update
    void Start()
    {
    	died = false; // when game starts we're not dead anymore
        rb = gameObject.GetComponent<Rigidbody2D> (); // getting the rigidbody component
        score += PlayerPrefs.GetFloat("Score", score); // adding score from saved data
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasTeleported) // if we have just teleported then we can't move till the teleporting is done
        {
        	// getting input
	        if(Input.GetAxis("Horizontal") > 0) // if we're moving right
	        {
	        	right = true; // set right to true and everything else to false
	        	left = false;
	        	up = false;
	        	down = false;
	        }

	        if(Input.GetAxis("Horizontal") < 0) // if we're moving left
	        {
	        	left = true; // set left to true and everything else to false
	        	right = false;
	        	up = false;
	        	down = false;
	        }

	        if(Input.GetAxis("Vertical") > 0) // if we're moving up
	        {
	        	up = true; // set up to true and everything else to false
	        	right = false;
	        	left = false;
	        	down = false;
	        }

	        if(Input.GetAxis("Vertical") < 0) // if we're moving down
	        {
	        	down = true; // set down to true and everything else to false
	        	right = false;
	        	left = false;
	        	up = false;
	        }
        }
        // executing the movement
        if(right)
        {
        	Move(1, 0); // moving right
        	AdjustSprite(1, 0); // facing right
        }

        if(left)
        {
        	Move(-1, 0); // moving left
        	AdjustSprite(-1, 0); // facing left
        }

        if(up)
        {
        	Move(0, 1); // moving up
        	AdjustSprite(1, 90); // facing up
        }

        if(down)
        {
        	Move(0, -1); // moving down
        	AdjustSprite(1, -90); // facing down
        }

        if(powerUp) // if we're in power up mode
        {
        	StartCoroutine("SpriteFlashing"); // flash the sprite
        }

        if(died) // if we died
        {
        	sprite.gameObject.SetActive(false); // deactivate the sprite
        }

        PlayerPrefs.SetFloat("Score", score); // save the score to saved data
    }

    // moving method
    void Move(float horizontalDirection, float verticalDirection)
    {
    	rb.velocity = new Vector3(horizontalDirection * movingSpeed, verticalDirection * movingSpeed, 0); // setting up the velocity
    }

    // adjusting the sprite to face the direction where we're moving 
    void AdjustSprite(float direction, float rotation)
    {
    	sprite.gameObject.transform.localScale = new Vector2(direction, 1);
        sprite.gameObject.transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    // teleporting
    void Teleport(Transform location)
    {
    	transform.position = new Vector2(location.position.x, location.position.y); // these positions depend on the gate we teleport to
    }

    // flashing the sprite so the player knows they're in power up mode
    IEnumerator SpriteFlashing()
    {
    	sprite.SetActive(false);
        yield return new WaitForSeconds(flashingSpeed);
        sprite.SetActive(true);
        yield return new WaitForSeconds(flashingSpeed);
        sprite.SetActive(false);
        yield return new WaitForSeconds(flashingSpeed);
        sprite.SetActive(true);
    }

    // power up mode
    IEnumerator PowerUp()
    {
    	powerUp = true;
    	yield return new WaitForSeconds(powerUpTime);
    	powerUp = false;
    }

    // teleporting cooldown s we cant move WHILE teleporting 
    IEnumerator TeleportCooldown()
    {
    	hasTeleported = true;
    	yield return new WaitForSeconds(0.5f);
    	hasTeleported = false;
    }


    // on collisions
    void OnCollisionEnter2D(Collision2D other)
    {
    	if(other.gameObject.tag == "cherry") // if we hit a cherry
    	{
    		score += 50;
    		eatingAudio.Play();
    		Destroy(other.gameObject);
    	}
    	if(other.gameObject.tag == "banana") // if we hit a banana
    	{
    		score += 100;
    		eatingAudio.Play();
    		Destroy(other.gameObject);
    	}
    	if(other.gameObject.tag == "enemy") // if we hit an enemy
    	{
    		if(powerUp) // if we're on powerup meaning we will eat the ghosts and they cant eat us
    		{
    			score += 250;
    			eatingAudio.Play();
    			other.gameObject.GetComponent<EnemyScript> ().dead = true; // make the enemy in dying state
    		} else // if we're not on powerup meaning the ghosts ate us
    		{
    			score = 0;
    			PlayerPrefs.SetFloat("Score", score);
    			died = true;
    		}
    	}
    } // CollisionEnter End 

    // on collisions
    void OnTriggerEnter2D(Collider2D other)
    {
    	if(other.gameObject.tag == "rightTeleport") // if we hit right teleporting gate
    	{
    		if(!hasTeleported) // if we hit it while playing and NOT while teleporting
    		{
    			Teleport(leftGate); // teleport to left gate
    			StartCoroutine("TeleportCooldown"); // start cooldown
    		}
    	}
    	if(other.gameObject.tag == "leftTeleport") // if we hit left teleporting gate
    	{
    		if(!hasTeleported) // if we hit it while playing and NOT while teleporting
    		{
    			Teleport(rightGate); // teleport to right gate
    			StartCoroutine("TeleportCooldown"); // start cooldown
    		}
    	}
    	if(other.gameObject.tag == "coin") // if we hit a coin
    	{
    		eatingAudio.Play();
    		score += 10;
    		Destroy(other.gameObject);
    	}
    	if(other.gameObject.tag == "bigCoin") // if we hit a bigCoin
    	{
    		eatingAudio.Play();
    		StartCoroutine("PowerUp");
    		score += 25;
    		Destroy(other.gameObject);
    	}
    } // TriggerEnter End
}
