#region

using UnityEngine;

#endregion

namespace RabbitGame.Gameplay
{
    public class CarrotController : MonoBehaviour, ICollectable
    {
        [SerializeField] private GameObject carrotObject;
        [SerializeField] private GameObject holeObject;

        public bool IsCollected { get; private set; }

        private void Awake()
        {
            carrotObject.SetActive(true);
            holeObject.SetActive(true);
        }

        public void Collect()
        {
            if (IsCollected) return;

            IsCollected = true;
            carrotObject.SetActive(false);
        }

        public void ResetCarrot()
        {
            IsCollected = false;
            carrotObject.SetActive(true);
        }
    }
}
