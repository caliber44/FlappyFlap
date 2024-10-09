using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private static CameraShake s_instance;

    private CinemachineVirtualCamera m_virtualCamera;
    private CinemachineBasicMultiChannelPerlin m_perlinNoise;

    private WaitForEndOfFrame m_waitForEndOfFrame;

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
            m_virtualCamera = GetComponent<CinemachineVirtualCamera>();
            m_perlinNoise = m_virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            m_waitForEndOfFrame = new WaitForEndOfFrame();
        }
    }

    public static void Shake(float intensity, float duration)
    {
        s_instance.StartCoroutine(s_instance.ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float timer = duration;
        while (timer > 0)
        {
            // Reduce the intensity as time progresses (stronger at the start)
            float currentIntensity = Mathf.Lerp(0, intensity, timer / duration);
            m_perlinNoise.m_AmplitudeGain = currentIntensity;

            timer -= Time.deltaTime;
            yield return m_waitForEndOfFrame;
        }

        // Reset the shake when done
        m_perlinNoise.m_AmplitudeGain = 0;
    }
}