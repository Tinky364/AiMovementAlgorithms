using Godot;

namespace Ai.Steering
{
    public class Align
    {
        public AiInfo Character { get; set; }
        public AiInfo Target { get; set; }
        
        public int MaxRotation { get; set; }
        public int MaxAngularAcceleration { get; set; }

        /// The time over which to achieve target speed.
        private const float TimeToTarget = 0.1f;
        
        public Align(AiInfo character, AiInfo target, int maxRotation, int maxAngularAcceleration)
        {
            Character = character;
            Target = target;
            MaxRotation = maxRotation;
            MaxAngularAcceleration = maxAngularAcceleration;
        }
        
        public bool GetSteering(out SteeringOutput result, float targetRadius, float slowRadius)
        {
            result = new SteeringOutput();

            float rotation = Target.Orientation - Character.Orientation;
            float rotationSize = Mathf.Abs(rotation);

            // Stops when it reaches the target position.
            if (rotationSize < targetRadius) return false;

            // Calculates the target rotation.
            // Stops slowly while inside the slow radius, else rotates at max rotation.
            // Slows it down using the ratio of distance.
            float targetRotation = MaxRotation;
            if (rotationSize < slowRadius) targetRotation = MaxRotation * rotationSize / slowRadius;

            // The final target rotation combines speed (already in the variable) and direction.
            targetRotation *= rotation / rotationSize;

            // Calculates the acceleration the character needs to reach the target rotation in
            // TimeToTarget seconds.
            result.Angular = (targetRotation - Character.Rotation) / TimeToTarget;

            // Checks if the acceleration is too fast.
            float angularAcceleration = Mathf.Abs(result.Angular);
            if (angularAcceleration > MaxAngularAcceleration)
                result.Angular = result.Angular / angularAcceleration * MaxAngularAcceleration;

            result.Linear = Vector3.Zero;
            
            return true;
        }
    }
}
