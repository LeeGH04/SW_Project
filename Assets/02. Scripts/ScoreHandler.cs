using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] ColliderHandler colliderHandler;

    private void Update()
    {
        scoreText.text = $"Score : {colliderHandler.Score}";
    }
}
