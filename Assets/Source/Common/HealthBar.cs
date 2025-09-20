using R3;
using UnityEngine;
using UnityEngine.UI;

class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Health playerHealth;

    void Awake()
    {
        playerHealth.Max
            .Subscribe(maxHealth => slider.maxValue = maxHealth)
            .AddTo(this);

        playerHealth.Current
            .Subscribe(currentHealth => slider.value = currentHealth)
            .AddTo(this);
    }
}
