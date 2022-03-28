using Godot;

namespace Ai.Behavior
{
    /// <summary>
    /// The behavior that causes the character to look at the angle the target is looking at.
    /// </summary>
    public class Align
    {
        /// The AiInfo of the character using the functionality.
        public AiInfo Character { get; set; }
        
        /// The AiInfo of what is targeted by the character.
        public AiInfo Target { get; set; }
        
        /// The maximum angular speed the character can reach.
        public int MaxAngularSpeed { get; set; }
        
        /// The maximum angular acceleration the character can reach.
        public int MaxAngularAcceleration { get; set; }
        
        /// The angle that specifies how far from the target angle the function can stop working.
        public float StopAngle { get; set; }
        
        /// The angle that specifies how far from the target angle the function can slow the character down.
        public float SlowAngle { get; set; }

        /// The time over which to achieve target speed.
        private const float TimeToTarget = 0.1f;

        /// <summary>
        /// The behavior that causes the character to look at the angle the target is looking at.
        /// Properties can also be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="target">The AiInfo of what is targeted by the character.</param>
        /// <param name="maxAngularSpeed">The maximum angular speed the character can reach.</param>
        /// <param name="maxAngularAcceleration">The maximum angular acceleration the character can reach.</param>
        /// <param name="stopAngle">The angle that specifies how far from the target angle the function can stop working.</param>
        /// <param name="slowAngle">The angle that specifies how far from the target angle the function can slow the character down.</param>
        public Align(
            AiInfo character, AiInfo target, int maxAngularSpeed, int maxAngularAcceleration,
            float stopAngle, float slowAngle
        )
        {
            Character = character;
            Target = target;
            MaxAngularSpeed = maxAngularSpeed;
            MaxAngularAcceleration = maxAngularAcceleration;
            StopAngle = stopAngle;
            SlowAngle = slowAngle;
        }
        
        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();

            // Calculates the angle needed to rotate to the target orientation.
            float rotation = Mathff.DeltaAngle(Character.Orientation, Target.Orientation);
            float rotationSize = Mathf.Abs(rotation);
            
            // Stops when it reaches the target position.
            if (rotationSize <= StopAngle) return false;

            // Calculates the target rotation.
            // Stops slowly while inside the slow radius, else rotates at max rotation.
            // Slows it down using the ratio of distance.
            float targetRotation = MaxAngularSpeed;
            if (rotationSize <= SlowAngle) targetRotation = MaxAngularSpeed * rotationSize / SlowAngle;

            // The final target rotation combines speed (already in the variable) and direction.
            targetRotation *= rotation / rotationSize;

            // Calculates the acceleration the character needs to reach the target rotation in
            // TimeToTarget seconds.
            result.AngularAcceleration = (targetRotation - Character.Rotation) / TimeToTarget;
            
            // Checks if the acceleration is too fast.
            float angularAcceleration = Mathf.Abs(result.AngularAcceleration);
            if (angularAcceleration > MaxAngularAcceleration)
                result.AngularAcceleration = result.AngularAcceleration / angularAcceleration * MaxAngularAcceleration;

            result.LinearAcceleration = Vector3.Zero;
            
            return true;
        }
    }
}
