using Godot;

namespace Ai.Steering
{
    public class SteeringOutput
    {
        public Vector3 Linear { get; set; } // Linear acceleration vector
        public float Angular { get; set; } // Angular acceleration
    }
}
