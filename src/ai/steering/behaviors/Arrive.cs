using Godot;

namespace Ai.Steering
{
    public class Arrive
    {
        public AiInfo Character { get; set; }
        public AiInfo Target { get; set; }
        
        public int MaxSpeed { get; set; }
        public int MaxAcceleration { get; set; }

        /// The time over which to achieve target speed.
        private const float TimeToTarget = 0.1f;
        
        protected Arrive() { }
        
        public Arrive(AiInfo character, AiInfo target, int maxSpeed, int maxAcceleration)
        {
            Character = character;
            Target = target;
            MaxSpeed = maxSpeed;
            MaxAcceleration = maxAcceleration;
        }
        
        public bool GetSteering(out SteeringOutput result, float targetRadius, float slowRadius)
        {
            result = new SteeringOutput();

            Vector3 direction = Target.Position - Character.Position; // distance vector
            float distance = direction.Length();
            direction = direction.Normalized();
            
            // Stops when it reaches the target position.
            if (distance < targetRadius) return false;

            // Calculates the target velocity.
            // Stops slowly while inside the slow radius, else moves at max speed.
            // Slows it down using the ratio of distance.
            float targetSpeed = MaxSpeed;
            if (distance < slowRadius) targetSpeed = MaxSpeed * distance / slowRadius; 
            Vector3 targetVelocity = direction * targetSpeed;

            // Calculates the acceleration the character needs to reach the target velocity in
            // TimeToTarget seconds.
            result.Linear = (targetVelocity - Character.Velocity) / TimeToTarget;
            
            // Checks if the acceleration is too fast.
            if (result.Linear.Length() > MaxAcceleration) 
                result.Linear = result.Linear.Normalized() * MaxAcceleration;

            result.Angular = 0;
            
            return true;
        }
    }
}
