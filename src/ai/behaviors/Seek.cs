namespace Ai.Behavior
{
    /// <summary>
    /// The behavior that causes the character to move to the target`s position.
    /// </summary>
    public class Seek
    {
        /// The AiInfo of the character using the functionality.
        public AiInfo Character { get; set; }
        
        /// The AiInfo of what is targeted by the character.
        public AiInfo Target { get; set; }
        
        /// The maximum linear acceleration the character can reach.
        public int MaxLinearAcceleration { get; set; }

        /// <summary>
        /// The behavior that causes the character to move to the target`s position. Properties can
        /// also be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="target">The AiInfo of what is targeted by the character.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        public Seek(AiInfo character, AiInfo target, int maxLinearAcceleration)
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

            // Calculates the distance vector to the target.
            result.LinearAcceleration = Target.Position - Character.Position;
            
            // Calculates the linear acceleration.
            result.LinearAcceleration = result.LinearAcceleration.Normalized() * MaxLinearAcceleration; // direction * acceleration
            
            result.AngularAcceleration = 0;
            
            return true;
        }
    }
}
