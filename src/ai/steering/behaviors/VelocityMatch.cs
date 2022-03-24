namespace Ai.Steering
{
    public class VelocityMatch
    {
        public AiInfo Character { get; set; }
        public AiInfo Target { get; set; }
        
        public int MaxAcceleration { get; set; }

        /// The time over which to achieve target speed.
        private float _timeToTarget = 0.1f; 
        
        public VelocityMatch(AiInfo character, AiInfo target, int maxAcceleration)
        {
            Character = character;
            Target = target;
            MaxAcceleration = maxAcceleration;
        }
        
        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();
            
            // Calculates the acceleration the character needs to reach the target velocity in
            // _timeToTarget seconds.
            result.Linear = (Target.Velocity - Character.Velocity) / _timeToTarget;
            
            // Check if the acceleration is too fast.
            if (result.Linear.Length() > MaxAcceleration)
                result.Linear = result.Linear.Normalized() * MaxAcceleration;

            result.Angular = 0;
            
            return true;
        }
    }
}
