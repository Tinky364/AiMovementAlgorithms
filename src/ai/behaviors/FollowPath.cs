using Godot;

namespace Ai.Behavior
{
    /// <summary>
    /// The behavior that causes the character to move on the path.
    /// </summary>
    public class FollowPath : Seek
    {
        public enum Type { NearestPoint, PredictedPoint }
        
        /// The nearest type finds the nearest point on path. The predicted type finds the closest
        /// point to the character's future position on the path using PredictionTime.
        public Type CurrentType { get; set; }
            
        /// The path the character follows.
        public Path Path { get; set; } 
        
        /// The distance that specifies how far the character can seek from its current position.
        public float PathDistance { get; set; }
        
        /// The time that specifies how far the character can target a point on the path from its current position.
        public float PredictionTime { get; set; }

        /// <summary>
        /// The behavior that causes the character to move on the path. Properties can also be
        /// changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        /// <param name="type">The type of the follow path.</param>
        /// <param name="path">The path the character follows.</param>
        /// <param name="pathDistance">The distance that specifies how far the character can seek from its current position.</param>
        /// <param name="predictionTime">The time that specifies how far the character can target a point on the path from its current position.</param>
        public FollowPath(
            AiInfo character, int maxLinearAcceleration, Type type, Path path, float pathDistance,
            float predictionTime
        ) : base(character, new AiInfo(), maxLinearAcceleration)
        {
            CurrentType = type;
            Path = path;
            PathDistance = pathDistance;
            PredictionTime = predictionTime;
        }

        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public new bool GetSteering(out SteeringOutput result)
        {
            Vector3 currentPosition;
            switch (CurrentType)
            {
                case Type.NearestPoint:
                    currentPosition = Path.ToLocal(Character.Position);
                    break;
                case Type.PredictedPoint:
                    currentPosition = Path.ToLocal(
                        Character.Position + Character.Velocity * PredictionTime
                    );
                    break;
                default: 
                    currentPosition = Path.ToLocal(Character.Position);
                    break;
            }
            
            // Finds the closest offset value of the current position of the character on the path.
            float currentOffset = Path.Curve.GetClosestOffset(currentPosition);
            
            // Offsets that closest offset value.
            float targetOffset = currentOffset + PathDistance;

            // Gets the global position of the target offset calculated.
            Target.Position = Path.Curve.InterpolateBaked(targetOffset);
            
            // Moves to the global position of the calculated target offset.
            return base.GetSteering(out result);
        }
    }
}
