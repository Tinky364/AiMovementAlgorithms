using Godot;

namespace Ai.Steering
{
    /// <summary>
    /// The behavior that causes the character to look at the target`s position.
    /// </summary>
    public class Face : Align
    {
        /// Face.Target is the AiInfo of what is targeted by the character.
        /// Align.Target is the AiInfo calculated by using Pursue.Target AiInfo.
        public new AiInfo Target { get; set; }

        /// <summary>
        /// The behavior that causes the character to look at the target`s position. Properties can
        /// also be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="target">The AiInfo of what is targeted by the character.</param>
        /// <param name="maxAngularSpeed">The maximum angular speed the character can reach.</param>
        /// <param name="maxAngularAcceleration">The maximum angular acceleration the character can reach.</param>
        /// <param name="stopAngle">The angle that specifies how far from the target angle the function can stop working.</param>
        /// <param name="slowAngle">The angle that specifies how far from the target angle the function can slow the character down.</param>
        public Face(
            AiInfo character, AiInfo target, int maxAngularSpeed, int maxAngularAcceleration,
            float stopAngle, float slowAngle
        ) : base(
            character, new AiInfo(), maxAngularSpeed, maxAngularAcceleration, stopAngle, slowAngle
        )
        {
            Target = target;
        }

        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public new bool GetSteering(out SteeringOutput result)
        {
            Vector3 direction = Target.Position - Character.Position;

            // If the direction is a zero direction, does not rotates. 
            if (direction.Length() == 0)
            {
                result = null;
                return false;
            }

            // Calculates the target orientation from the direction.
            base.Target.Orientation = Mathff.DirectionToOrientation(direction);
            
            // Aligns with the target orientation.
            return base.GetSteering(out result);
        }
    }
}
