using Godot;

namespace Ai
{
    /// <summary>
    /// Enables the processing of motion information of entities from AI behaviors. Keep motion
    /// information of the entities, who use AI behaviors, in this class instances.
    /// </summary>
    public class AiInfo
    {
        /// The current global position of the entity.
        public Vector3 Position { get; set; } 
        
        /// The current angle degree in Y axis of the entity.
        public float Orientation { get; set; } 
        
        /// The current linear speed vector of the entity.
        public Vector3 Velocity { get; set; } 
        
        /// The current angular speed vector of the entity.
        public float Rotation { get; set; }

        /// <summary>
        /// Calculates the position, the orientation, the velocity and the rotation of the AiInfo by
        /// using acceleration values of the steeringOutput.
        /// </summary>
        /// <param name="steeringOutput">The result of the called behaviour.</param>
        /// <param name="delta">The frame delta time.</param>
        public void Process(SteeringOutput steeringOutput, float delta)
        {
            Position += Velocity * delta;
            Orientation += Rotation * delta;
            Velocity += steeringOutput.LinearAcceleration * delta;
            Rotation += steeringOutput.AngularAcceleration * delta;
        }
        
        /// <summary>
        /// Syncs the values of the AiInfo instance with the values of the entity.
        /// </summary>
        /// <param name="position">The current global position of the entity.</param>
        /// <param name="orientation">The current angle degree in Y axis of the entity.</param>
        /// <param name="velocity">The current linear speed vector of the entity.</param>
        /// <param name="rotation">The current angular speed vector of the entity.</param>
        public void Sync(Vector3 position, float orientation, Vector3 velocity, float rotation)
        {
            Position = position;
            Orientation = orientation;
            Velocity = velocity;
            Rotation = rotation;
        }
    }
}
