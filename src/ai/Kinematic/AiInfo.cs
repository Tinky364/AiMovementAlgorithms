using Godot;

namespace Ai.Kinematic
{
    public class AiInfo
    {
        public Vector3 Position { get; set; } // Global position
        public float Orientation { get; set; } // Angle in Vector.Up axis

        /// <summary>
        /// Call this function every frame to calculate the position and the orientation of the
        /// owner.
        /// </summary>
        /// <param name="steeringOutput">Calculated result of the called behaviour.</param>
        /// <param name="delta">Frame delta time.</param>
        public void Process(SteeringOutput steeringOutput, float delta)
        {
            Position += steeringOutput.Velocity * delta;
            Orientation += steeringOutput.Rotation * delta;
        }

        /// <summary>
        /// Call this function every frame if you use the physics engine to update the ai info.
        /// </summary>
        /// <param name="position">Current global position of the owner.</param>
        /// <param name="orientation">Current angle in Vector.Up axis of the owner.</param>
        public void Equalize(Vector3 position, float orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}
