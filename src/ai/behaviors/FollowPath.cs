using Godot;

namespace Ai.Behavior
{
    /// <summary>
    /// The behavior that causes the character to move on the path.
    /// </summary>
    public class FollowPath : Seek
    {
        /// The path the character follows.
        public Path Path { get; set; } 
        
        /// The distance that specifies how far the character can seek from its current position.
        public float PathDistance { get; set; }

        /// <summary>
        /// The behavior that causes the character to move on the path. Properties can also be
        /// changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        /// <param name="path">The path the character follows.</param>
        /// <param name="pathDistance">The distance that specifies how far the character can seek from its current position.</param>
        public FollowPath(
            AiInfo character, int maxLinearAcceleration, Path path, float pathDistance
        ) : base(character, new AiInfo(), maxLinearAcceleration)
        {
            Path = path;
            PathDistance = pathDistance;
        }

        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public new bool GetSteering(out SteeringOutput result)
        {
            // Finds the closest offset value of the current position of the character on the path.
            float currentOffset = Path.Curve.GetClosestOffset(Path.ToLocal(Character.Position));
            
            // Offsets that closest offset value.
            float targetOffset = currentOffset + PathDistance;

            // Gets the global position of the target offset calculated.
            Target.Position = Path.Curve.InterpolateBaked(targetOffset);
            
            // Moves to the global position of the calculated target offset.
            return base.GetSteering(out result);
        }
    }
}
