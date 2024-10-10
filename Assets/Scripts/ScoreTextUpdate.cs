using UnityEngine;
using UnityEngine.UI;

public class ScoreTextUpdate : MonoBehaviour
{
    private static ScoreTextUpdate s_instance;

    [SerializeField] private Sprite[] m_sprites;
    [SerializeField] private Image m_HighScoreRendererUnits;
    [SerializeField] private Image m_highScoreRendererTens;
    [SerializeField] private Image m_currentScoreRendererUnits;
    [SerializeField] private Image m_currentScoreRendererTens;
    [SerializeField] private GameObject m_highscoreGameObject;


    private void Awake()
    {
        // Ensure we have only one instance of this class (singleton pattern)
        if (s_instance != null)
        {
            Destroy(s_instance);
        }
        s_instance = this;

        gameObject.SetActive(false);
    }

    // Public method to set the score that other scripts can call
    public static void SetScore(int score)
    {
        // Use the instance of this class to update the sprites
        s_instance.SetSprites(score,s_instance.m_currentScoreRendererTens, s_instance.m_currentScoreRendererUnits);
    }
    public static void HideCurrentScore()
    {
        s_instance.m_currentScoreRendererUnits.gameObject.SetActive(false);
        s_instance.m_currentScoreRendererTens.gameObject.SetActive(false);
    }
    public static void ShowHighScore(int score) 
    {
        s_instance.m_highscoreGameObject.SetActive(true);
        s_instance.SetSprites(score, s_instance.m_highScoreRendererTens, s_instance.m_HighScoreRendererUnits);
    }
    public static void HideHighScore()
    {
        s_instance.m_highscoreGameObject.SetActive(false);
    }

    // Private method to update the sprites based on the score
    private void SetSprites(int score, Image tensImageRenderer, Image unitsImageRenderer)
    {
        unitsImageRenderer.gameObject.SetActive(true);

        // The score is split into tens and units.
        int tens = score / 10;   // Extract the tens digit (e.g., for 45, tens would be 4)
        int units = score % 10;  // Extract the units digit (e.g., for 45, units would be 5)

        // Make sure tens is a valid index for the array, only update the tens renderer if needed
        if (tens > 0)
        {
            tensImageRenderer.sprite = m_sprites[tens];  // Set the sprite for the tens place
            tensImageRenderer.gameObject.SetActive(true); // Ensure the tens sprite is visible
        }
        else
        {
            tensImageRenderer.gameObject.SetActive(false); // Hide the tens place if score is less than 10
        }

        // Set the sprite for the units place
        unitsImageRenderer.sprite = m_sprites[units];
    }
}