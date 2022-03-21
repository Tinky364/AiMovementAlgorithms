using Godot;

namespace Ai.Kinematic
{
    public class KinematicWander
    {
        public AiInfo Character { get; set; }
        public int MaxSpeed { get; set; }
        public int MaxRotation { get; set; }

        private readonly RandomNumberGenerator _rng;

        public KinematicWander(AiInfo character, int maxSpeed, int maxRotation)
        {
            Character = character;
            MaxSpeed = maxSpeed;
            MaxRotation = maxRotation;
            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        public bool GetSteering(out SteeringOutput result)
        {
            result = new SteeringOutput();

            // Calculate the direction vector of the character from its orientation.
            Vector3 direction = new Vector3(
                Mathf.Sin(Mathf.Deg2Rad(Character.Orientation)), 0, 
                Mathf.Cos(Mathf.Deg2Rad(Character.Orientation))
            );
            
            // Calculate the velocity.
            result.Velocity = MaxSpeed * direction;

            // Change the character`s orientation randomly.
            result.Rotation = RandomBinomial() * MaxRotation;

            return true;
        }
        
        /// <summary>
        /// Returns a random number between -1 and 1, where values around zero are more likely.
        /// </summary>
        /// <returns></returns>
        private float RandomBinomial() => _rng.Randf() - _rng.Randf();
    }
}
