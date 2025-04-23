using UnityEngine;
using UnityEngine.UI;

public class ReputationBar : MonoBehaviour
{
    public Image reputationBar;
    public float maxReputation = 100f;
    public float currentReputation;

    private float baseDecayRate = 1f;
    private float decayAcceleration = 0.01f;
    private float currentDecayRate;

    void Start()
    {
        currentReputation = maxReputation;
        currentDecayRate = baseDecayRate;
    }

    void Update()
    {
        currentReputation -= Time.deltaTime * currentDecayRate;
        currentDecayRate += decayAcceleration * Time.deltaTime;
        currentReputation = Mathf.Clamp(currentReputation, 0f, maxReputation);
        reputationBar.fillAmount = currentReputation / maxReputation;
    }

    public void IncreaseReputation(float amount)
    {
        currentReputation += amount;
        currentReputation = Mathf.Clamp(currentReputation, 0f, maxReputation);
        Debug.Log($"Reputation increased by {amount}. Current reputation: {currentReputation}");
    }
}