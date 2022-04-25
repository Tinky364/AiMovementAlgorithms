using Godot;

namespace Ai
{
    /// <summary>
    /// Used by AI behaviors to report the result of behaviors.
    /// </summary>
    public struct SteeringOutput
    {
        /// Linear acceleration vector.
        public Vector3 LinearAcceleration { get; set; }
        
        /// Angular acceleration vector.
        public float AngularAcceleration { get; set; }             
    }
}
