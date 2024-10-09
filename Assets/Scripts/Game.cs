using UnityEngine;

public class Game : MonoBehaviour
{
    private const float GAME_STARTING_SPEED = 0.025f;
    private const float PLAYER_STARTING_Y = 0.2f;
    private const float PLAYER_STARTING_X = -0.9f;
    private const float PLAYER_UP_FORCE = 3f;
    private const float PLAYER_UP_MAXFORCE = 7f;
    private const float PLAYER_MAX_ROTATION = 65f;
    private const float PLAYER_ROTATION_SPEED = 7f;
    private const float PLAYER_HURTBOX_RADIUS = 0.2f;
    private const float PLAYER_GROUND_HIT_OFFSET = 1.35f;
    private const float BACKGROUND_STARTING_X = 0f;
    private const float BACKGROUND_LAYER_SPEED_OFFSET = 0.3f;
    private const float GRAVITY = -13f;
    private const float STARTING_GRAVITY = -0.1f;
    private const string PLAYER_TAG = "Player";
    private const string START_BUTTON_TAG = "StartButton";
    private const string BACKGROUND_TAG = "ScrollingBackground";
    private const string BACKGROUND_LAYER_TAG = "ScrollingBackgroundLayer";
    private const string BACKGROUND_XPOS_SHADER_ID = "_Xpos";
    private const string LAYER_NAME_HURT_PLAYER = "HurtLayer";

    private Material m_ScrollingBackgroundMaterial;
    private Material m_ScrollingLayerMaterial;
    private int m_ScrollingMaterialScrollSpeedID;
    private float m_currentBackgroundPosX;
    private float m_currentGameSpeed;
    private Transform m_playerTransform;
    private float m_playersVelocity_Y;
    private bool m_isPause = true;
    private LayerMask m_hurtPlayerLayer;
    private GameObject m_startButton;

    void Start()
    {
        SetupGame();
        SetupPlayer();
        SetupBackground();
    }

    private void SetupGame()
    {
        m_startButton = GameObject.FindGameObjectWithTag(START_BUTTON_TAG);
        m_hurtPlayerLayer = LayerMask.GetMask(LAYER_NAME_HURT_PLAYER);
    }

    public void StartGame()
    {
        PauseGame(false);

        m_startButton.SetActive(false);

        m_playerTransform.gameObject.SetActive(true);

        m_playersVelocity_Y = STARTING_GRAVITY;

        m_playerTransform.position = (Vector3.right * PLAYER_STARTING_X) + (Vector3.up * PLAYER_STARTING_Y);

    }
    public void GameOver()
    {
        PauseGame(true);
        m_startButton.SetActive(true);
    }
    public void PauseGame(bool value)
    {
        m_isPause = value;
    }
    private void Update()
    {
        UpdatePlayerMovement();
    }

    private void FixedUpdate()
    {
        ScrollBackground();
        PlayerHitDectection();
    }

    private void PlayerHitDectection()
    {
        if (m_isPause) return;

        Collider2D hit = Physics2D.OverlapCircle(m_playerTransform.position + (Vector3.up * (m_playersVelocity_Y * Time.deltaTime)), PLAYER_HURTBOX_RADIUS, m_hurtPlayerLayer);

        if (hit != null) 
        {
            Vector2 closestPoint = hit.ClosestPoint(m_playerTransform.position);

            Vector2 hitToPlayerDirection = ((m_playerTransform.position - (Vector3)closestPoint).normalized);

            m_playerTransform.position = closestPoint + (hitToPlayerDirection * (PLAYER_HURTBOX_RADIUS * PLAYER_GROUND_HIT_OFFSET));

            GameOver();
        }
    }

    private void SetupPlayer()
    {
        m_playerTransform = GameObject.FindGameObjectWithTag(PLAYER_TAG).GetComponent<Transform>();

        m_playerTransform.gameObject.SetActive(false);
    }
    private void UpdatePlayerMovement()
    {
        if (m_isPause) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_playersVelocity_Y <= 0)
            {
                m_playersVelocity_Y = PLAYER_UP_FORCE;
            }
            else
            {
                m_playersVelocity_Y += PLAYER_UP_FORCE;
            }
            if(m_playersVelocity_Y > PLAYER_UP_MAXFORCE)
            {
                m_playersVelocity_Y = PLAYER_UP_MAXFORCE;
            }
        }

        m_playerTransform.position += Vector3.up * (m_playersVelocity_Y * Time.deltaTime);

        m_playersVelocity_Y += GRAVITY * Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0, 0, Mathf.Clamp(m_playersVelocity_Y, -PLAYER_UP_MAXFORCE, PLAYER_UP_MAXFORCE) / PLAYER_UP_MAXFORCE * PLAYER_MAX_ROTATION);

        m_playerTransform.rotation = Quaternion.Lerp(m_playerTransform.rotation, targetRotation, Time.deltaTime * PLAYER_ROTATION_SPEED);

    }

    private void SetupBackground()
    {
        m_ScrollingBackgroundMaterial = GameObject.FindGameObjectWithTag(BACKGROUND_TAG).GetComponent<SpriteRenderer>().material;
        m_ScrollingLayerMaterial = GameObject.FindGameObjectWithTag(BACKGROUND_LAYER_TAG).GetComponent<SpriteRenderer>().material;
        m_ScrollingMaterialScrollSpeedID = Shader.PropertyToID(BACKGROUND_XPOS_SHADER_ID);
        m_currentGameSpeed = GAME_STARTING_SPEED;
        m_currentBackgroundPosX = BACKGROUND_STARTING_X;
    }
    private void ScrollBackground()
    {
        if (m_isPause) return;

        m_currentBackgroundPosX += m_currentGameSpeed * Time.fixedDeltaTime;
        m_ScrollingBackgroundMaterial.SetFloat(m_ScrollingMaterialScrollSpeedID, m_currentBackgroundPosX);
        m_ScrollingLayerMaterial.SetFloat(m_ScrollingMaterialScrollSpeedID, m_currentBackgroundPosX * BACKGROUND_LAYER_SPEED_OFFSET);
    }
}
