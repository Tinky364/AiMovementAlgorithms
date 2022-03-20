using Godot;

namespace Ai.Kinematic
{
    public class KinematicSeek
    {
        public StaticInfo Character { get; set; }
        public StaticInfo Target { get; set; }
        public int MaxSpeed { get; set; }
        public float Radius { get; set; }
        
        private float _timeToTarget = 0.25f;

        public KinematicSeek(StaticInfo character, StaticInfo target, int maxSpeed, float radius)
        {
            Character = character;
            Target = target;
            MaxSpeed = maxSpeed;
            Radius = radius;
        }

        public bool GetSteering(out KinematicSteeringOutput result)
        {
            result = new KinematicSteeringOutput();
            
            // Get the direction to the target.
            result.Velocity = Target.Position - Character.Position; 
            
            // Check if the character is within radius.
            if (result.Velocity.Length() < Radius) return false;
            
            // Calculate velocity.
            result.Velocity /= _timeToTarget;
            if (result.Velocity.Length() > MaxSpeed)
                result.Velocity = result.Velocity.Normalized() * MaxSpeed;
            
            // Face in the direction the character wants to move.
            Character.Orientation = NewOrientation(Character.Orientation, result.Velocity); 
            
            result.Rotation = 0;
            return true;
        }

        private float NewOrientation(float current, Vector3 direction)
        {
            if (direction == Vector3.Zero) return current;
            float targetOrientation = Mathf.Rad2Deg(Mathf.Atan2(direction.x, direction.z));
            float diff = (targetOrientation - current + 180) % 360 - 180;
            diff = diff < -180 ? diff + 360 : diff;
            if (Mathf.Abs(diff) <= 0.1f) return current;
            current += diff;
            return current;
        }
    }
}

