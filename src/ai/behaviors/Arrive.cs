using Godot;

namespace Ai.Behavior
{
    /// <summary>
    /// The behavior that causes the character to move to the target`s position and stop before
    /// reaching it.
    /// </summary>
    public class Arrive
    {
        /// The AiInfo of the character using the functionality.
        public AiInfo Character { get; set; }
        
        /// The AiInfo of what is targeted by the character.
        public AiInfo Target { get; set; }
        
        /// The maximum linear speed the character can reach.
        public int MaxLinearSpeed { get; set; }
        
        /// The maximum linear acceleration the character can reach.
        public int MaxLinearAcceleration { get; set; }
        
        /// The distance that specifies how far from the target position the function can stop working.
        public float StopDistance { get; set; }
        
        /// The distance that specifies how far from the target position the function can slow the character down.
        public float SlowDistance { get; set; }

        /// The time over which to achieve target speed.
        private const float TimeToTarget = 0.1f;

        /// <summary>
        /// The behavior that causes the character to move to the target`s position and stop before
        /// reaching it. Properties can also be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="target">The AiInfo of what is targeted by the character.</param>
        /// <param name="maxLinearSpeed">The maximum linear speed the character can reach.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        /// <param name="stopDistance">The distance that specifies how far from the target position the function can stop working.</param>
        /// <param name="slowDistance">The distance that specifies how far from the target position the function can slow the character down.</param>
        public Arrive(
            AiInfo character, AiInfo target, int maxLinearSpeed, int maxLinearAcceleration, 
            float stopDistance, float slowDistance
        )
        {
            Character = character;
            Target = target;
            MaxLinearSpeed = maxLinearSpeed;
            MaxLinearAcceleration = maxLinearAcceleration;
            StopDistance = stopDistance;
            SlowDistance = slowDistance;
        }
        
        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();

            Vector3 direction = Target.Position - Character.Position; // distance vector
            float distance = direction.Length();
            direction = direction.Normalized();
            
            // Stops when it reaches the target position.
            if (distance < StopDistance) return false;

            // Calculates the target velocity.
            // Stops slowly while inside the slow radius, else moves at max speed.
            // Slows it down using the ratio of distance.
            float targetSpeed = MaxLinearSpeed;
            if (distance < SlowDistance) targetSpeed = MaxLinearSpeed * distance / SlowDistance; 
            Vector3 targetVelocity = direction * targetSpeed;

            // Calculates the acceleration the character needs to reach the target velocity in
            // TimeToTarget seconds.
            result.LinearAcceleration = (targetVelocity - Character.Velocity) / TimeToTarget;
            
            // Checks if the acceleration is too fast.
            if (result.LinearAcceleration.Length() > MaxLinearAcceleration) 
                result.LinearAcceleration = result.LinearAcceleration.Normalized() * MaxLinearAcceleration;

            result.AngularAcceleration = 0;
            
            return true;
        }
    }
}
