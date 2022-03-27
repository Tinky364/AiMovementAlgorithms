using Godot;

namespace Ai.Steering
{
    public class Face : Align
    {
        // Face.Target is the target the character is facing.
        // base.Target is calculated target using Face.Target.
        public new AiInfo Target { get; set; }

        public Face(AiInfo character, AiInfo target, int maxRotation, int maxAngularAcceleration)
        {
            Character = character;
            Target = target;
            MaxRotation = maxRotation;
            MaxAngularAcceleration = maxAngularAcceleration;
            base.Target = new AiInfo();
        }

        public new bool GetSteering(out SteeringOutput result, float targetRadius, float slowRadius)
        {
            Vector3 direction = Target.Position - Character.Position;

            // If the direction is a zero direction, does not rotates. 
            if (direction.Length() == 0)
            {
                result = null;
                return false;
            }

            // Calculates the target orientation from the direction.
            base.Target.Orientation = MathfExtension.DirectionToOrientation(direction);
            
            return base.GetSteering(out result, targetRadius, slowRadius);
        }
    }
}
