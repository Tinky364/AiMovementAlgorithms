using Godot;

namespace Ai.Steering
{
    /// <summary>
    /// Used by AI behaviors to report the result of behaviors.
    /// </summary>
    public class SteeringOutput
    {
        /// Linear acceleration vector.
        public Vector3 LinearAcceleration { get; set; } = Vector3.Zero;
        
        /// Angular acceleration vector.
        public float AngularAcceleration { get; set; } = 0;             
    }
}
