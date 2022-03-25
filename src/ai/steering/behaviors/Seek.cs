using Godot;

namespace Ai.Steering
{
    public class Seek
    {
        public AiInfo Character { get; set; }
        public AiInfo Target { get; set; }
        
        public int MaxAcceleration { get; set; }

        protected Seek() { }

        public Seek(AiInfo character, AiInfo target, int maxAcceleration)
        {
            Character = character;
            Target = target;
            MaxAcceleration = maxAcceleration;
        }
        
        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();

            // Calculates the distance vector to the target.
            result.Linear = Target.Position - Character.Position;
            
            // Calculates the linear acceleration.
            result.Linear = result.Linear.Normalized() * MaxAcceleration; // direction * acceleration
            
            result.Angular = 0;
            
            return true;
        }
    }
}
