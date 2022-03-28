using Godot;

namespace Ai.Steering
{
    /// <summary>
    /// The behavior that causes the character to look at where it is going.
    /// </summary>
    public class LookWhereGo : Align
    {
        /// <summary>
        /// The behavior that causes the character to look at where it is going. Properties can also
        /// be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="maxAngularSpeed">The maximum angular speed the character can reach.</param>
        /// <param name="maxAngularAcceleration">The maximum angular acceleration the character can reach.</param>
        /// <param name="stopAngle">The angle that specifies how far from the target angle the function can stop working.</param>
        /// <param name="slowAngle">The angle that specifies how far from the target angle the function can slow the character down.</param>
        public LookWhereGo(
            AiInfo character, int maxAngularSpeed, int maxAngularAcceleration, float stopAngle,
            float slowAngle
        ) : base(
            character, new AiInfo(), maxAngularSpeed, maxAngularAcceleration, stopAngle, slowAngle
        )
        {
            
        }

        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public new bool GetSteering(out SteeringOutput result)
        {
            Vector3 direction = Character.Velocity;
            
            // If the direction is a zero direction, does not rotates. 
            if (direction.Length() == 0)
            {
                result = null;
                return false;
            }

            // Calculates the target orientation from the direction.
            Target.Orientation = Mathff.DirectionToOrientation(direction);

            // Aligns with the target orientation calculated.
            return base.GetSteering(out result);
        }
    }
}
