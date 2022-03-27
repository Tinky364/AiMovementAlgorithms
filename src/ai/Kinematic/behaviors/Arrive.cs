using Godot;

namespace Ai.Kinematic
{
    public class Arrive
    {
        public AiInfo Character { get; set; }
        public AiInfo Target { get; set; }
        
        public int MaxSpeed { get; set; }
        public float Radius { get; set; }

        private const float TimeToTarget = 0.25f;

        public Arrive(AiInfo character, AiInfo target, int maxSpeed, float radius)
        {
            Character = character;
            Target = target;
            MaxSpeed = maxSpeed;
            Radius = radius;
        }

        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();
            
            // Calculate the distance vector to the target.
            result.Velocity = Target.Position - Character.Position; 
            
            // Check if the character is within radius.
            if (result.Velocity.Length() < Radius) return false;
            
            // Calculate the velocity.
            result.Velocity /= TimeToTarget;
            if (result.Velocity.Length() > MaxSpeed)
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

