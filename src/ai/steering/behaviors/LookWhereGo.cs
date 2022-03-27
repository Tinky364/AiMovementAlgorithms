using Godot;

namespace Ai.Steering
{
    public class LookWhereGo : Align
    {
        public LookWhereGo(
            AiInfo character, AiInfo target, int maxRotation, int maxAngularAcceleration)
            : base(character, target, maxRotation, maxAngularAcceleration) { }

        public new bool GetSteering(out SteeringOutput result, float targetRadius, float slowRadius)
        {
            Vector3 direction = Character.Velocity;
            
            // If the direction is a zero direction, does not rotates. 
            if (direction.Length() == 0)
            {
                result = null;
                return false;
            }

            // Calculates the target orientation from the direction.
            Target.Orientation = Mathff.DirectionToOrientation(direction);

            // Aligns with the target orientation calculated.
            return base.GetSteering(out result, targetRadius, slowRadius);
        }
    }
}
