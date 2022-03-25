using Godot;

namespace Ai.Steering
{
    public class Pursue : Arrive
    {
        // Pursue.Target is the target the character is pursuing.
        // base.Target is calculated target using Pursue.Target.
        public new AiInfo Target { get; set; }
        
        // The maximum prediction time.
        public float MaxPrediction { get; set; }

        public Pursue(
            AiInfo character, AiInfo target, int maxAcceleration, int maxSpeed, float maxPrediction
        )
        {
            Character = character;
            Target = target;
            MaxAcceleration = maxAcceleration;
            MaxSpeed = maxSpeed;
            MaxPrediction = maxPrediction;
            base.Target = new AiInfo();
        }

        public new bool GetSteering(out SteeringOutput result, float targetRadius, float slowRadius)
        {
            Vector3 direction = Target.Position - Character.Position; // distance vector
            float distance = direction.Length();
            float speed = Character.Velocity.Length();

            // Checks if speed gives a reasonable prediction time.
            // The character tracks a position closer to the target when the distance is closer than
            // the distance derived from the MaxPrediction.
            float prediction = MaxPrediction;
            if (speed > distance / MaxPrediction) prediction = distance / speed;

            // Calculates the true target position.
            base.Target.Position = Target.Position + Target.Velocity * prediction;
            
            return base.GetSteering(out result, targetRadius, slowRadius);
        }
    }
}
