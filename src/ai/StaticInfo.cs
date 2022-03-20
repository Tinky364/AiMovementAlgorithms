using Godot;

namespace Ai
{
    public class StaticInfo
    {
        public Vector3 Position { get; set; }
        public float Orientation { get; set; }

        public void Update(Vector3 position, float orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }
}
