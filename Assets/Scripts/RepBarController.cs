using UnityEngine;
using UnityEngine.UI;

public class ReputationBar : MonoBehaviour
{
    public Image reputationBar; // Reference to the UI Image for the reputation bar
    public float maxReputation = 100f; // Maximum reputation value
    public float currentReputation; // Current reputation value

    private float baseDecayRate = 1f; // Base rate at which reputation decreases
    private float decayAcceleration = 0.01f; // How much the decay rate increases over time
    private float currentDecayRate; // Current decay rate, which increases over time

    void Start()
    {
        // Initialize the reputation bar
        currentReputation = maxReputation; // Start with full reputation

        // Initialize the decay rate
        currentDecayRate = baseDecayRate;
    }

    void Update()
    {
        // Decrease reputation over time, with the rate increasing slowly
        currentReputation -= Time.deltaTime * currentDecayRate;

        // Increase the decay rate over time
        currentDecayRate += decayAcceleration * Time.deltaTime;

        // Clamp the reputation value to stay within bounds
        currentReputation = Mathf.Clamp(currentReputation, 0f, maxReputation);

        // Update the reputation bar's fill amount
        reputationBar.fillAmount = currentReputation / maxReputation;
    }

    // Method to increase reputation
    public void IncreaseReputation(float amount)
    {
        currentReputation += amount;
        currentReputation = Mathf.Clamp(currentReputation, 0f, maxReputation); // Ensure it doesn't exceed max
        Debug.Log($"Reputation increased by {amount}. Current reputation: {currentReputation}");
    }
}