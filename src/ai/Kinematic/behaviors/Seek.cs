using Godot;

namespace Ai.Kinematic
{
    public class Seek
    {
        public AiInfo Character { get; set; }
        public AiInfo Target { get; set; }
        
        public int MaxSpeed { get; set; }

        public Seek(AiInfo character, AiInfo target, int maxSpeed)
        {
            Character = character;
            Target = target;
            MaxSpeed = maxSpeed;
        }

        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();
            
            // Calculate the distance vector to the target.
            result.Velocity = Target.Position - Character.Position; 
            
            // The velocity along this direction, at max speed.
            result.Velocity = result.Velocity.Normalized() * MaxSpeed;
            
            // Face the character in the direction the character wants to move.
            Character.Orientation = NewOrientation(Character.Orientation, result.Velocity); 
            
            result.Rotation = 0;
            
            return true;
        }

        private float NewOrientation(float current, Vector3 direction)
        {
            if (direction == Vector3.Zero) return current;
            float targetOrientation = Mathff.DirectionToOrientation(direction);
            float diff = (targetOrientation - current + 180) % 360 - 180;
            diff = diff < -180 ? diff + 360 : diff;
            if (Mathf.Abs(diff) <= 0.1f) return current;
            current += diff;
            return current;
        }
    }
}
