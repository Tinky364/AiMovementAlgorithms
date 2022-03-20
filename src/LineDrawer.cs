using Godot;

public class LineDrawer : ImmediateGeometry
{
    public void DrawLine(Vector3 p1, Vector3 p2)
    {
        Clear();
        Begin(Mesh.PrimitiveType.Lines);
        AddVertex(p1);
        AddVertex(p2);
        End();
    }
}
