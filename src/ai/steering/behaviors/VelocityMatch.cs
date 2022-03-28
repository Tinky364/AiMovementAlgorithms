namespace Ai.Steering
{
    /// <summary>
    /// The behavior that causes the character to match its velocity with the target`s velocity.
    /// </summary>
    public class VelocityMatch
    {
        /// The AiInfo of the character using the functionality.
        public AiInfo Character { get; set; }
        
        /// The AiInfo of what is targeted by the character.
        public AiInfo Target { get; set; }
        
        /// The maximum linear acceleration the character can reach.
        public int MaxLinearAcceleration { get; set; }
        
        /// The time over which to achieve target speed.
        private const float TimeToTarget = 0.1f; 
        
        /// <summary>
        /// The behavior that causes the character to match its velocity with the target`s velocity.
        /// Properties can also be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="target">The AiInfo of what is targeted by the character.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        public VelocityMatch(AiInfo character, AiInfo target, int maxLinearAcceleration)
        {
            Character = character;
            Target = target;
            MaxLinearAcceleration = maxLinearAcceleration;
        }
        
        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();
            
            // Calculates the acceleration the character needs to reach the target velocity in
            // TimeToTarget seconds.
            result.LinearAcceleration = (Target.Velocity - Character.Velocity) / TimeToTarget;
            
            // Check if the acceleration is too fast.
            if (result.LinearAcceleration.Length() > MaxLinearAcceleration)
                result.LinearAcceleration = result.LinearAcceleration.Normalized() * MaxLinearAcceleration;

            result.AngularAcceleration = 0;
            
            return true;
        }
    }
}
