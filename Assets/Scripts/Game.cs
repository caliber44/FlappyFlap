using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Constants for game parameters
    private const float GAME_STARTING_SPEED = 0.1f; // The initial speed at which the game starts moving. A small value to ensure smooth acceleration.
    private const float PIPE_SCROLL_SPEED = 1f; // The inital speed at which the pipes will be scrolling througt the game.
    private const float PIPE_MIN_POSITION_X = -2f; // The left most position a pipe can scroll to before being reset to the pipe reset position. 
    private const float PIPE_MIN_POSITION_Y = -0.85f; // The lowest vertical position a pipe can spawn after it resets.
    private const float PIPE_MAX_POSITION_Y = 0.60f;// The highest vertical position a pipe can spawn after it resets.
    private const float PIPE_RESET_POSITION_X = 4f; // Once the pipe reaches the min allowed x coord, the pipe will be reset to this x position.
    private const float PIPE_PLAYER_REWARD_POSITION_X = -0.8f; // The x position a pipe must scroll to in order for the player to be rewarded with a point;
    private const float PLAYER_STARTING_Y = 0.1f; // The starting vertical (Y) position for the player. This helps position the player above the ground.
    private const float PLAYER_STARTING_X = -0.75f; // The starting horizontal (X) position for the player. This helps position the player in the game world.
    private const float PLAYER_IDLE_BOB_INTENSITIY = 0.2f; // The intensity of the player's idle bobbing motion when they are not moving. Affects how much they sway up and down.
    private const float PLAYER_IDLE_BOB_SPEED = 4f; // The speed at which the player bobs up and down while idle. A higher value means faster bobbing.
    private const float PLAYER_UP_FORCE = 4.3f; // The upward force applied when the player jumps. This value determines how high the player can jump.
    private const float PLAYER_UP_MAXFORCE = 7f; // The maximum upward force the player can apply. This helps limit the jump height to prevent excessive jumps.
    private const float PLAYER_MAX_UP_ROTATION = 29f; // The maximum rotation angle (in degrees) the player can tilt upwards. Helps create a natural jump arc.
    private const float PLAYER_MAX_DOWN_ROTATION = 90f; // The maximum rotation angle (in degrees) the player can tilt downwards. Helps create a natural landing arc.
    private const float PLAYER_POSITION_MIN_Y = -3f; // The minimum Y position the player can reach. This prevents the player from falling indefinitely.
    private const float PLAYER_POSITION_MAX_Y = 2.8f; // The maximum Y position the player can reach. This defines the upper limit of the player's movement.
    private const float PLAYER_ROTATION_UP_SPEED = 80f; // The speed at which the player rotates upwards during a jump. Affects how quickly they tilt upwards.
    private const float PLAYER_ROTATION_DOWN_SPEED = 9f; // The speed at which the player rotates downwards. Affects how quickly they tilt downwards upon landing.
    private const float PLAYER_DOWN_ROTATION_VELOCITY_Y_THRESHOLD = -4.3f; // The threshold for downward rotation velocity. This helps manage the rotation effect when falling quickly.
    private const float PLAYER_HURTBOX_RADIUS = 0.05f; // The radius of the player's hurtbox, which defines the area where the player can take damage. A small radius to ensure precision.
    private const float PLAYER_GROUND_HIT_OFFSET = 2.9f; // The offset distance when the player collides with the ground. This prevents the player from getting stuck on the ground.
    private const float PLAYER_ANIMATION_WAIT = 0.13f; // The wait time (in seconds) between player animations. Controls the timing of animation transitions.
    private const float BACKGROUND_STARTING_X = 0f; // The starting X position for the background. This helps position the background at the beginning of the game.
    private const float BACKGROUND_LAYER_SPEED_OFFSET = 0.3f; // The speed offset for scrolling background layers. This creates a parallax effect, making the game visually dynamic.
    private const float CAMERA_SHAKE_TIME = 0.1f; // The duration (in seconds) of the camera shake effect. This adds excitement during certain events in the game.
    private const float CAMERA_SHAKE_INTENSITIY = 0.2f; // The intensity of the camera shake effect. Affects how much the camera will move during the shake.
    private const float GRAVITY = -13f; // The force of gravity affecting the player. A negative value pulls the player down towards the ground.
    private const float STARTING_FORCE = 0f; // The initial force applied to the player. This can be used to set the starting momentum for the player.
    private const string PLAYER_TAG = "Player"; // A string tag used to identify the player object in the game. This helps in finding and referencing the player.
    private const string START_BUTTON_TAG = "StartButton"; // A string tag used to identify the start button in the game. Useful for interactions with the button.
    private const string BACKGROUND_TAG = "ScrollingBackground"; // A string tag used to identify the scrolling background. This helps manage background-related functionalities.
    private const string PIPE_TAG = "Pipes"; // A string tag used to identify the parent of all the pipes.This helps in finding all the pipes that we will scroll.
    private const string BACKGROUND_LAYER_TAG = "ScrollingBackgroundLayer"; // A string tag used to identify scrolling background layers. Useful for organizing multiple layers in the background.
    private const string BACKGROUND_XPOS_SHADER_ID = "_Xpos"; // The shader ID for the X position of the background in the rendering process. Helps control the position of the background in shaders.
    private const string LAYER_NAME_HURT_PLAYER = "HurtLayer"; // The name of the layer where the player can be hurt. Helps organize collision layers for gameplay mechanics.

    // Variables to hold materials and game state
    private Material m_ScrollingBackgroundMaterial; // Material used for the scrolling background. This defines the appearance and texture of the background.
    private Material m_ScrollingLayerMaterial; // Material used for the scrolling layer. This defines the appearance of individual layers in the background.
    private int m_ScrollingMaterialScrollSpeedID; // The ID for the property that controls the speed of the scrolling material. Used to update the material in the game.
    private float m_currentBackgroundPosX; // The current X position of the background in the game world. This is updated to create the scrolling effect.
    private float m_currentGameSpeed; // The current speed of the game. This can change based on gameplay events (like speed boosts).
    private int m_playerScore; // the current score of the player just a single game loop.
    private int m_playerHighScore; //the recorded hightscore this game session. It will be set if the current score is larger.
    private Transform m_playerTransform; // The Transform component of the player. This holds the position, rotation, and scale of the player in the game.
    private Transform m_pipeParentTransform; // The transform of the pipe parent. Used for moving all the pipe children.
    private Vector3[] m_pipeStartingPositions; //An array of vector3s, The position of all the pipes are stored so we can reset them once the game is restared.
    private bool[] m_pipeRewards; //An array to hold if we collected a reward from a pipe that passed us by. The value is reset when the pipe is reset, meaning it just counts once.  
    private float m_playersVelocity_Y; // The current vertical velocity of the player. This affects how fast the player is moving up or down.
    private bool m_isPause = true; // A boolean variable that indicates if the game is currently paused. Useful for managing game states.
    private bool m_isWaitingForInput = true; // A boolean variable that indicates if the game is waiting for player input to start. Helps control game flow.
    private LayerMask m_hurtPlayerLayer; // Layer mask used for detecting hits on the player. This helps identify when the player takes damage.
    private GameObject m_startButton; // Reference to the start button game object in the game. Used to manage interactions with the start button.
    private WaitForSecondsRealtime m_waitForSecondsRealtime; // The wait time for animation frames, allowing for smoother transitions between frames.

    // Public variable to set a degub flag
    public bool IsDegugMode = false; // Setting to TRUE stops the player checking for physics interactions that would end the game  

    void Start()
    {
        SetupGame(); // Initialize game settings
        SetupPlayer(); // Setup player properties
        SetupBackground(); // Setup background properties
    }
    private void SetupGame()
    {
        // Find and setup the start button
        m_startButton = GameObject.FindGameObjectWithTag(START_BUTTON_TAG);

        //Find and setup the pipe parent transform
        m_pipeParentTransform = GameObject.FindGameObjectWithTag(PIPE_TAG).transform;
        //Create the array of vector3 positions to store the pipe staring positions in.
        //The length of the array in the amount of children the pipe parent has
        m_pipeStartingPositions = new Vector3[m_pipeParentTransform.childCount];
        //An array that holds data about pipes that have had the reward collected or not for the player passing them.
        m_pipeRewards = new bool[m_pipeParentTransform.childCount];

        // Loop through each child (pipe) of the pipe parent transform.
        // Store the current position of the pipe in an array, so we can loop through it at game reset and 
        // Reset all the pipes back to their starting position.
        Vector3 startingPipePosition;
        for (int i = 0; i < m_pipeParentTransform.childCount; i++)
        {
            //stores the current position of the tranforms child at that index into the array.
            startingPipePosition = m_pipeParentTransform.GetChild(i).position;
            //Randomise the Y of the pipe by first setting a different y value in startingPipPosition vector3
            //Range of random float is defined by consts
            startingPipePosition.y = Random.Range(PIPE_MIN_POSITION_Y, PIPE_MAX_POSITION_Y);
            //copy value of starting position.
            m_pipeStartingPositions[i] = startingPipePosition;
        }

        //Players hightscore and score are set to zero
        m_playerScore = 0;
        m_playerHighScore = 0;

        // Set the layer mask for the hurt player layer
        m_hurtPlayerLayer = LayerMask.GetMask(LAYER_NAME_HURT_PLAYER);

        // Initialize wait time for player animation
        m_waitForSecondsRealtime = new WaitForSecondsRealtime(PLAYER_ANIMATION_WAIT);
    }
    public void StartGame()
    {
        // Start the game, unpausing it after one frame of delay
        PauseGame(false, true);

        // Play the start game sound
        Audio.PlayClip(AudioClipID.START);

        // Hide the start button
        m_startButton.SetActive(false);

        // Activate the player game object
        m_playerTransform.gameObject.SetActive(true);

        // Initialize player velocity and position
        m_playersVelocity_Y = STARTING_FORCE;
        m_playerTransform.position = (Vector3.right * PLAYER_STARTING_X) + (Vector3.up * PLAYER_STARTING_Y);
        m_playerTransform.rotation = Quaternion.identity; // Reset player rotation

        // Reset Pipe Positions
        for (int i = 0; i < m_pipeParentTransform.childCount; i++)
        {
            //gets the position stored in our pipe starting array and sets the position of the tranforms child at that index.
            m_pipeParentTransform.GetChild(i).position = m_pipeStartingPositions[i];
            //set the reward for each pipe to false, as we havent collected the reward yet.
            m_pipeRewards[i] = false;
        }
        //Hide the highscore
        ScoreTextUpdate.HideHighScore();
        // Set the player score and UI score text to zero
        m_playerScore = 0;
        ScoreTextUpdate.SetScore(m_playerScore);
    }
    public void GameOver()
    {
        // Handle game over scenario, pausing the game
        PauseGame(true);
        m_startButton.SetActive(true); // Show the start button again
        //Check to see if the highscore was beaten
        if(m_playerScore > m_playerHighScore)
        {
            //set the highscore to the current score as its the new highest
            m_playerHighScore = m_playerScore;
            //plays the sound effect for beating the highscore
            Audio.PlayClip(AudioClipID.HIGHSCORE);
        }
        //Hide the current score and show the highscore
        ScoreTextUpdate.HideCurrentScore();
        ScoreTextUpdate.ShowHighScore(m_playerHighScore);
    }
    public void PauseGame(bool value, bool delay = false)
    {
        // Pause or unpause the game based on the value
        m_isWaitingForInput = true; // Prevent input while pausing

        if (!delay)
        {
            m_isPause = value; // Set pause state immediately
            return;
        }

        StartCoroutine(DelayStart(value)); // Start delay coroutine if specified
    }
    public void Mute(bool value)
    {
        // Mute or unmute audio
        Audio.Mute(value);
    }

    IEnumerator DelayStart(bool value)
    {
        yield return null; // Wait for one frame
        m_isPause = value; // Set pause state after delay
    }
    private void Update()
    {
        // Update player movement every frame
        UpdatePlayerMovement();
        // Update the pipes
        UpdatePipeMovement();
    }
    private void FixedUpdate()
    {
        // Update background scrolling and check for player hits
        ScrollBackground();
        PlayerHitDectection();
    }
    private void PlayerHitDectection()
    {
        // Exit the method if the game is paused or waiting for player input
        if (m_isPause || m_isWaitingForInput || IsDegugMode) return;

        // Check for collisions within a circular area around the player's position, offset by the player's upward velocity
        Collider2D hit = Physics2D.OverlapCircle(m_playerTransform.position + (Vector3.up * (m_playersVelocity_Y * Time.deltaTime)), PLAYER_HURTBOX_RADIUS, m_hurtPlayerLayer);

        // If a hit is detected (i.e., player collided with an object)
        if (hit != null)
        {
            // Get the closest point on the collider to the player
            Vector2 closestPoint = hit.ClosestPoint(m_playerTransform.position);

            // Calculate the direction from the closest point to the player and normalize it
            Vector2 hitToPlayerDirection = ((m_playerTransform.position - (Vector3)closestPoint).normalized);

            // Move the player to the closest point plus an offset based on the hurtbox radius
            m_playerTransform.position = closestPoint + (hitToPlayerDirection * (PLAYER_HURTBOX_RADIUS * PLAYER_GROUND_HIT_OFFSET));

            // Play a sound effect for the hit
            Audio.PlayClip(AudioClipID.THUD);

            // Reset the player's sprite animation
            PlayerSpriteAnimator.ResetSprite();

            // Apply camera shake effect based on the player's velocity
            CameraShake.Shake(Mathf.Abs(m_playersVelocity_Y * CAMERA_SHAKE_INTENSITIY), CAMERA_SHAKE_TIME);

            // Trigger game over sequence
            GameOver();
        }
    }
    private void SetupPlayer()
    {
        // Find the player object by its tag and get its Transform component
        m_playerTransform = GameObject.FindGameObjectWithTag(PLAYER_TAG).GetComponent<Transform>();

        // Initially set the player object to inactive until the game starts
        m_playerTransform.gameObject.SetActive(false);
    }
    private IEnumerator SpriteAnimationLoop()
    {
        // Continuously animate the player's sprite while the game is not paused or waiting for input
        while (!m_isPause || !m_isWaitingForInput)
        {
            // Move to the next sprite in the animation sequence
            PlayerSpriteAnimator.NextSprite();

            // Wait for the specified time before proceeding to the next frame
            yield return m_waitForSecondsRealtime;
        }
    }
    private void UpdatePlayerMovement()
    {
        // Exit the method if the game is paused
        if (m_isPause) return;

        // Check for player input (space bar or mouse buttons)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // Start the sprite animation loop on the first input if waiting for input
            if (m_isWaitingForInput)
            {
                StartCoroutine(SpriteAnimationLoop());
                m_isWaitingForInput = false; // Update state to indicate input has been received
            }

            // Play the flap sound effect
            Audio.PlayClip(AudioClipID.FLAP);

            // Apply upward force to the player's vertical velocity
            if (m_playersVelocity_Y <= 0)
            {
                m_playersVelocity_Y = PLAYER_UP_FORCE; // Set to initial upward force if falling
            }
            else
            {
                m_playersVelocity_Y += PLAYER_UP_FORCE; // Increase upward velocity
            }

            // Clamp the upward velocity to the maximum force
            if (m_playersVelocity_Y > PLAYER_UP_MAXFORCE)
            {
                m_playersVelocity_Y = PLAYER_UP_MAXFORCE; // Ensure it does not exceed max limit
            }
        }

        // If waiting for input, perform idle bobbing motion instead of applying gravity
        if (m_isWaitingForInput)
        {
            // Calculate new Y position based on a sine wave for bobbing effect
            float newY = PLAYER_STARTING_Y + Mathf.Sin(Time.time * PLAYER_IDLE_BOB_SPEED) * PLAYER_IDLE_BOB_INTENSITIY;
            m_playerTransform.position = new Vector3(m_playerTransform.position.x, newY, m_playerTransform.position.z);
            return; // Exit early to prevent further movement logic
        }

        // Calculate new position based on player's current vertical velocity
        Vector3 newPosition = m_playerTransform.position + Vector3.up * (m_playersVelocity_Y * Time.deltaTime);

        // Clamp the new Y position within defined limits
        newPosition.y = Mathf.Clamp(newPosition.y, PLAYER_POSITION_MIN_Y, PLAYER_POSITION_MAX_Y);

        // Update the player's position
        m_playerTransform.position = newPosition;

        // Apply gravity to the player's vertical velocity
        m_playersVelocity_Y += GRAVITY * Time.deltaTime;

        // Determine the player's rotation based on vertical velocity
        if (m_playersVelocity_Y > PLAYER_DOWN_ROTATION_VELOCITY_Y_THRESHOLD)
        {
            // Set target rotation for upward movement
            Quaternion targetRotation = Quaternion.Euler(0, 0, PLAYER_MAX_UP_ROTATION);
            m_playerTransform.rotation = Quaternion.Lerp(m_playerTransform.rotation, targetRotation, Time.deltaTime * PLAYER_ROTATION_UP_SPEED);
        }
        else
        {
            // Set target rotation for downward movement based on velocity
            Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Clamp(m_playersVelocity_Y, -PLAYER_UP_MAXFORCE, 0) / PLAYER_UP_MAXFORCE * PLAYER_MAX_DOWN_ROTATION);
            m_playerTransform.rotation = Quaternion.Lerp(m_playerTransform.rotation, targetRotation, Time.deltaTime * PLAYER_ROTATION_DOWN_SPEED);
        }
    }
    private void UpdatePipeMovement()
    {
        // Exit the method if the game is paused
        if (m_isPause || m_isWaitingForInput) return;

        // Define the left direction vector to move the pipes to the left.
        Vector3 left = Vector3.left;
        Vector3 newPos; // Variable to hold the new position of each pipe.

        // Loop through each child (pipe) of the pipe parent transform.
        for (int i = 0; i < m_pipeParentTransform.childCount; i++)
        {
            // Calculate the new position for the current pipe by moving it left
            // based on the defined scroll speed and the time since the last frame.
            newPos = m_pipeParentTransform.GetChild(i).position + (left * (PIPE_SCROLL_SPEED * Time.deltaTime));

            //Check if the pipe has moved far enough along the x coord to trigger the reward for the player
            //and that the player hasnt collected it yet.
            if(newPos.x <= PIPE_PLAYER_REWARD_POSITION_X && !m_pipeRewards[i]) 
            {
                //set that we have collected the reward from this pipe. 
                m_pipeRewards[i] = true;
                //finish rewarding the player
                PlayerRewarded();
            }

            // Check if the new position is beyond the minimum allowed X position.
            if (newPos.x <= PIPE_MIN_POSITION_X)
            {
                // If the pipe has moved past the minimum position, reset its position
                // to the specified reset position, creating a looping effect.
                newPos.x = PIPE_RESET_POSITION_X;
                // Give the pipe a random height by setting the y value to a range between the values defined in the consts
                newPos.y = Random.Range(PIPE_MIN_POSITION_Y, PIPE_MAX_POSITION_Y);
                //the pipe has been reset, so make it available to have its reward collected again
                m_pipeRewards[i] = false;
            }

            // Update the position of the current pipe to its new calculated position.
            m_pipeParentTransform.GetChild(i).position = newPos;
        }
    }
    private void PlayerRewarded()
    {
        //increase the players score by 1
        m_playerScore++;
        //play the sound effect for scoring a point
        Audio.PlayClip(AudioClipID.POINT);
        // Set the UI score text to zero
        ScoreTextUpdate.SetScore(m_playerScore);
    }
    private void SetupBackground()
    {
        // Retrieve the material for the scrolling background using its tag and assign it to the variable
        m_ScrollingBackgroundMaterial = GameObject.FindGameObjectWithTag(BACKGROUND_TAG).GetComponent<SpriteRenderer>().material;

        // Retrieve the material for the scrolling layer using its tag and assign it to the variable
        m_ScrollingLayerMaterial = GameObject.FindGameObjectWithTag(BACKGROUND_LAYER_TAG).GetComponent<SpriteRenderer>().material;

        // Get the property ID for the background X position shader parameter
        m_ScrollingMaterialScrollSpeedID = Shader.PropertyToID(BACKGROUND_XPOS_SHADER_ID);

        // Initialize the current game speed with the starting speed constant
        m_currentGameSpeed = GAME_STARTING_SPEED;

        // Set the initial X position of the background to the starting position constant
        m_currentBackgroundPosX = BACKGROUND_STARTING_X;
    }

    private void ScrollBackground()
    {
        // Exit the method if the game is paused to prevent background scrolling
        if (m_isPause) return;

        // Update the current X position of the background based on the game speed and fixed delta time
        m_currentBackgroundPosX += m_currentGameSpeed * Time.fixedDeltaTime;

        // Set the updated X position value on the scrolling background material
        m_ScrollingBackgroundMaterial.SetFloat(m_ScrollingMaterialScrollSpeedID, m_currentBackgroundPosX);

        // Set the updated X position value for the scrolling layer material, adjusted by the layer speed offset
        m_ScrollingLayerMaterial.SetFloat(m_ScrollingMaterialScrollSpeedID, m_currentBackgroundPosX * BACKGROUND_LAYER_SPEED_OFFSET);
    }
}
