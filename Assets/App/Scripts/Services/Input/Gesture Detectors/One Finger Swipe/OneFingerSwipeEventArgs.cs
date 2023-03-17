
using UnityEngine;

namespace App.Services.Input.GestureDetectors.OneFingerSwipe
{ 
    public class OneFingerSwipeEventArgs
    {
        public SelectedObject SelectedObject { get; set; }
        public Vector2 FingerPosition { get; set; }
        public float SwipeHorizontalValue { get; set; }
        public float SwipeVerticalValue { get; set; }
    }
}