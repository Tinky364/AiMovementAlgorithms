using Godot;

namespace Ai.Steering
{
    /// <summary>
    /// The behavior that causes the character to look around. This behavior defines a virtual
    /// circle in front of the character, generates random position on this circle, rotates the
    /// character to this position and moves it to the position. 
    /// </summary>
    public class Wander : Face
    {
        /// Specifies how far the circle is from the character`s position.
        public float WanderOffset { get; set; } 
        
        /// The radius of the circle.
        public float WanderRadius { get; set; }
        
        /// The maximum rate at which the wander orientation can change.
        public float WanderRate { get; set; } 
        
        /// The maximum linear acceleration the character can reach.
        public float MaxLinearAcceleration { get; set; }

        /// The current orientation of the wander target.
        private float _wanderOrientation;
        
        private readonly RandomNumberGenerator _rng;

        /// <summary>
        /// The behavior that causes the character to look around. This behavior defines a virtual
        /// circle in front of the character, generates random position on this circle, rotates the
        /// character to this position and moves it to the position. Properties can also be changed
        /// after construction. 
        /// </summary>
        /// <param name="character">The AiInfo of the character using the functionality.</param>
        /// <param name="maxAngularSpeed">The maximum angular speed the character can reach.</param>
        /// <param name="maxAngularAcceleration">The maximum angular acceleration the character can reach.</param>
        /// <param name="stopAngle">The angle that specifies how far from the target angle the function can stop working.</param>
        /// <param name="slowAngle">The angle that specifies how far from the target angle the function can slow the character down.</param>
        /// <param name="wanderOffset">Specifies how far the circle is from the character`s position.</param>
        /// <param name="wanderRadius">The radius of the circle.</param>
        /// <param name="wanderRate">The maximum rate at which the wander orientation can change.</param>
        /// <param name="maxLinearAcceleration">The maximum linear acceleration the character can reach.</param>
        public Wander(
            AiInfo character, int maxAngularSpeed, int maxAngularAcceleration, float stopAngle,
            float slowAngle, float wanderOffset, float wanderRadius, float wanderRate, 
            float maxLinearAcceleration
        ) : base(
            character, new AiInfo(), maxAngularSpeed, maxAngularAcceleration, stopAngle, slowAngle
        )
        {
            WanderOffset = wanderOffset;
            WanderRadius = wanderRadius;
            WanderRate = wanderRate;
            MaxLinearAcceleration = maxLinearAcceleration;
            
            _wanderOrientation = 0;
            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        /// <summary>
        /// Processes the behavior and if the behavior can find a result, returns true.
        /// </summary>
        /// <param name="result">The result of the behavior. Use its values if the function returns true.</param>
        /// <returns>If the result is valid, returns true.</returns>
        public new bool GetSteering(out SteeringOutput result)
        {
            // Updates the wander orientation
            _wanderOrientation += RandomBinomial() * WanderRate;
            
            // Calculates the new target orientation by adding the new WanderOrientation to the
            // current orientation of the character.
            float targetOrientation = _wanderOrientation + Character.Orientation;

            // Calculates the center of the wander circle.
            Target.Position = Character.Position +
                WanderOffset * Mathff.OrientationToDirection(Character.Orientation);

            // Calculates the target position.
            Target.Position += WanderRadius * Mathff.OrientationToDirection(targetOrientation);

            // Faces the target position calculated.
            if (!base.GetSteering(out result)) return false;
            
            // Increases the character's acceleration in its current orientation.
            result.LinearAcceleration = MaxLinearAcceleration * Mathff.OrientationToDirection(Character.Orientation);

            return true;
        }
        
        /// <summary>
        /// Returns a random number between -1 and 1, where values around zero are more likely.
        /// </summary>
        /// <returns></returns>
        private float RandomBinomial() => _rng.Randf() - _rng.Randf();
    }
}
