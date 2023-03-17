
using UnityEngine;

namespace App.Services.Input.GestureDetectors.Pinch
{
    public class PinchEventArgs
    {
        public Vector2 Finger1Position { get; set; }
        public Vector2 Finger2Position { get; set; }
        public float PinchValue { get; set; }
    }
}
