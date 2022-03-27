using Godot;

namespace Ai.Steering
{
    public class Wander : Face
    {
        public float WanderOffset { get; set; } // How far the circle is from the character.
        public float WanderRadius { get; set; } // Radius of the circle.
        public float WanderRate { get; set; } // The maximum rate at which the wander orientation can change.
        public float WanderOrientation { get; set; } // The current orientation of the wander target.
        public float MaxAcceleration { get; set; } // The maximum acceleration while moving.
        
        private readonly RandomNumberGenerator _rng;

        public Wander(
            AiInfo character, int maxRotation, int maxAngularAcceleration, float wanderOffset,
            float wanderRadius, float wanderRate, float maxAcceleration
        ) : base(character, new AiInfo(), maxRotation, maxAngularAcceleration)
        {
            WanderOffset = wanderOffset;
            WanderRadius = wanderRadius;
            WanderRate = wanderRate;
            WanderOrientation = 0;
            MaxAcceleration = maxAcceleration;
            
            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        public new bool GetSteering(out SteeringOutput result, float targetRadius, float slowRadius)
        {
            // Updates the wander orientation
            WanderOrientation += RandomBinomial() * WanderRate;
            
            // Calculates the new target orientation by adding the new WanderOrientation to the
            // current orientation of the character.
            float targetOrientation = WanderOrientation + Character.Orientation;

            // Calculates the center of the wander circle.
            Target.Position = Character.Position +
                WanderOffset * Mathff.OrientationToDirection(Character.Orientation);

            // Calculates the target position.
            Target.Position += WanderRadius * Mathff.OrientationToDirection(targetOrientation);

            // Faces the target position calculated.
            if (!base.GetSteering(out result, targetRadius, slowRadius)) return false;
            
            // Increases the character's acceleration in its current orientation.
            result.Linear = MaxAcceleration * Mathff.OrientationToDirection(Character.Orientation);

            return true;
        }
        
        /// <summary>
        /// Returns a random number between -1 and 1, where values around zero are more likely.
        /// </summary>
        /// <returns></returns>
        private float RandomBinomial() => _rng.Randf() - _rng.Randf();
    }
}
