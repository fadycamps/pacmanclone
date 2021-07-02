using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
	[SerializeField] float speed;
	[SerializeField] float healingSpeed;
	[SerializeField] GameObject target;
	[SerializeField] GameObject sprite;
	[SerializeField] Transform healingArea;
	[SerializeField] Sprite ghostSprite;
	[SerializeField] Sprite eyesSprite;
	SpriteRenderer spriteRen;
	NavMeshAgent agent;
	public bool dead;
    // Start is called before the first frame update
    void Start()
    {
    	target = GameObject.FindGameObjectWithTag("Player"); // get the player's object
        spriteRen = sprite.gameObject.GetComponent<SpriteRenderer> ();
        agent = gameObject.GetComponent<NavMeshAgent> ();

        // making sure the AI doesn't go out of bounds or make random weird rotations..etc
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
    	if(!dead) // if the enemy isn't dead 
    	{
    		spriteRen.sprite = ghostSprite; // render the ghost sprite
    		agent.speed = speed;
    		if(target.gameObject.GetComponent<PacmanScript> ().powerUp) // pacman is on powerup mode
	        {
	        	spriteRen.color = Color.red; // sprite becomes red (terrified)
	        	FleeFromPlayer(); // start running away from player
	        } else // if pacman isnt on powerup mode
	        {
	        	spriteRen.color = Color.white; // sprite becomes white (aka the normal color thats set on the sprite)
	        	FollowPlayer(); // chase player
	        }
    	} else // if enemy is dead
    	{
    		spriteRen.sprite = eyesSprite; // the enemy's sprite becomes just eyes
    		agent.SetDestination(healingArea.position); // goes to the middle base
    		agent.speed = healingSpeed;
    		if(Vector2.Distance(transform.position, healingArea.position) < 0.5) // if the enemy is inside the middle base
    		{
    			dead = false; // alive again!
    		}
    	}

    }

    // chasing player
    void FollowPlayer()
    {
    	agent.SetDestination(target.transform.position);
    }

    // running away from player
    void FleeFromPlayer()
    {
    	Vector2 moveDirection = transform.position - target.transform.position; // going the opposite direction of the player
    	agent.SetDestination(moveDirection);
    }
}
