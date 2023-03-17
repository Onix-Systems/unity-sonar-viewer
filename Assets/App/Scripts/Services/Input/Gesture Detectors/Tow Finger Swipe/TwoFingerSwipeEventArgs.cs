
using UnityEngine;

namespace App.Services.Input.GestureDetectors.TowFingerSwipe
{
    public class TwoFingerSwipeEventArgs
    {
        public Vector2 Finger1Position { get; set; }
        public Vector2 Finger2Position { get; set; }
        public float SwipeHorizontalValue { get; set; }
        public float SwipeVerticalValue { get; set; }
    }
}
