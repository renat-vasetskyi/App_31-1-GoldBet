#region

using System;
using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class PlayerHealth
    {
        public event Action<int> OnHealthChanged;
        public event Action OnDeath;

        public int CurrentHealth { get; private set; }

        public PlayerHealth(int maxHealth = 3)
        {
            CurrentHealth = maxHealth;
        }
        public void TakeDamage()
        {
            if (CurrentHealth <= 0)
            {
                return;
            }
            CurrentHealth--;
            OnHealthChanged?.Invoke(CurrentHealth);
            if (CurrentHealth == 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}
