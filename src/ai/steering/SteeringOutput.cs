using Godot;

namespace Ai.Steering
{
    public class SteeringOutput
    {
        public Vector3 Linear { get; set; } = Vector3.Zero; // Linear acceleration vector
        public float Angular { get; set; } = 0;             // Angular acceleration
    }
}
