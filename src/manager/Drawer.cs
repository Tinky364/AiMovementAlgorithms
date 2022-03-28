using Godot;

namespace Manager
{
    public class Drawer : Singleton<Drawer>
    {
        private ImmediateGeometry D { get; set; }

        public override void _EnterTree()
        {
            SetSingleton();
            D = new ImmediateGeometry();
            D.Name = "ImmediateGeometry";
            AddChild(D);
        }
        
        public override async void _Process(float delta)
        {
            await ToSignal(GetTree(), "idle_frame");
            D.Clear();
        }

        public void DrawLine(Spatial from, Vector3 p1, Vector3 p2)
        {
            D.Begin(Mesh.PrimitiveType.Lines);
            D.AddVertex(from.ToGlobal(p1));
            D.AddVertex(from.ToGlobal(p2));
            D.End();
        }

        public void DrawPath(Spatial from, Path path)
        {
            D.Begin(Mesh.PrimitiveType.Lines);
            foreach (Vector3 point in path.Curve.GetBakedPoints())
            {
                D.AddVertex(from.ToGlobal(point));
            }
            D.End();
        }
    }
}

