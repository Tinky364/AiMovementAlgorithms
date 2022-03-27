using Godot;

namespace Ai.Steering
{
    public class AiInfo
    {
        public Vector3 Position { get; set; } // Global position
        public float Orientation { get; set; } // Angle in Y axis
        public Vector3 Velocity { get; set; } // Linear speed vector
        public float Rotation { get; set; } // Angular speed

        /// <summary>
        /// Call this function every frame to calculate the position, the orientation, the velocity
        /// and the rotation of the owner. 
        /// </summary>
        /// <param name="steeringOutput">Calculated result of the called behaviour.</param>
        /// <param name="delta">Frame delta time.</param>
        public void Process(SteeringOutput steeringOutput, float delta)
        {
            Position += Velocity * delta;
            Orientation += Rotation * delta;
            Velocity += steeringOutput.Linear * delta;
            Rotation += steeringOutput.Angular * delta;
        }
        
        /// <summary>
        /// Call this function every frame if you use the physics engine to update the ai info.
        /// </summary>
        /// <param name="position">Current global position of the owner.</param>
        /// <param name="orientation">Current angle in Vector.Up axis of the owner.</param>
        /// <param name="velocity">Current linear speed vector of the owner.</param>
        /// <param name="rotation">Current angular speed of the owner.</param>
        public void Equalize(Vector3 position, float orientation, Vector3 velocity, float rotation)
        {
            Position = position;
            Orientation = orientation;
            Velocity = velocity;
            Rotation = rotation;
        }
    }
}
