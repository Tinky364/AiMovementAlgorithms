using Godot;

namespace Ai.Kinematic
{
    public struct SteeringOutput
    {
        public Vector3 Velocity { get; set; } // Linear speed vector
        public float Rotation { get; set; } // Angular speed
    }
}
