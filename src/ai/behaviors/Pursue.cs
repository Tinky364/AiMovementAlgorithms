namespace Ai.Behavior
{
    /// <summary>
    /// The behavior that causes the character to move to the target`s future position and stop
    /// before reaching it.
    /// </summary>
    public class Pursue : Arrive
    {
        /// Pursue.Target is the AiInfo of what is targeted by the character.
        /// Arrive.Target is the AiInfo calculated by using Pursue.Target AiInfo.
        public new AiInfo Target { get; set; }
        
        /// The time that specifies how far the character can target to pursue from its current position.
        public float MaxPredictionTime { get; set; }

        /// <summary>
        /// The behavior that causes the character to move to the target`s future position and stop
        /// before reaching it. Properties can also be changed after construction.
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="target">The AiInfo of what is targeted by the character.</param>
        /// <param name="maxLinearSpeed">The maximum linear speed the character can reach.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        /// <param name="stopDistance">The distance that specifies how far from the target position the function can stop working.</param>
        /// <param name="slowDistance">The distance that specifies how far from the target position the function can slow the character down.</param>
        /// <param name="maxPredictionTime">The time that specifies how far the character can target to pursue from its current position.</param>
        public Pursue(
            AiInfo character, AiInfo target, int maxLinearSpeed, int maxLinearAcceleration,
            float stopDistance, float slowDistance, float maxPredictionTime
        ) : base(
            character, new AiInfo(), maxLinearSpeed, maxLinearAcceleration, stopDistance, 
            slowDistance
        )
        {
            Target = target;
            MaxPredictionTime = maxPredictionTime;
        }

        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public new bool GetSteering(out SteeringOutput result)
        {
            float distance = (Target.Position - Character.Position).Length();
            float speed = Character.Velocity.Length();

            // Checks if speed gives a reasonable prediction time.
            // The character tracks a position closer to the target when the distance is closer than
            // the distance derived from the MaxPrediction.
            float prediction = MaxPredictionTime;
            if (speed > distance / MaxPredictionTime) prediction = distance / speed;

            // Calculates the true target position.
            base.Target.Position = Target.Position + Target.Velocity * prediction;
            
            // Arrives to the target position calculated.
            return base.GetSteering(out result);
        }
    }
}
