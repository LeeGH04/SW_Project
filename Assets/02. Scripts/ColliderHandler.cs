using UnityEngine;

public class ColliderHandler : MonoBehaviour
{
    private int score;
    public int Score  { get { return score; }}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ScoreObj"))
        {
            Destroy(other.gameObject);
            score++;
        }
    }
}
