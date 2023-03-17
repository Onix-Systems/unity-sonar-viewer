
using UnityEngine;

namespace App.Services.Input
{
    public class SelectedObject
    {
        public GameObject GameObject { get; set; }
        public Vector2 TouchOffsetToObjectPivot { get; set; }
    }
}
