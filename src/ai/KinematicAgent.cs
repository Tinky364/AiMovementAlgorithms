using Ai.Kinematic;
using Godot;

public class KinematicAgent : KinematicBody
{
    [Export]
    public BehaviorType CurBehaviorType;
    [Export]
    public NodePath PlayerPath;
    
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxSpeed = 5;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float TargetPositionRadius = 2f;
    
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxRotation = 5;
    
    public Vector3 Forward { get; private set; }
    private Player _player;
    private Spatial _pivot;
    private LineDrawer _lineDrawer;
    
    private AiInfo _aiInfo;

    public enum BehaviorType { Arrive, Wander }
    private Arrive _arrive;
    private Wander _wander;

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        
        _lineDrawer = new LineDrawer(); 
        AddChild(_lineDrawer);
        
        _player = GetNode<Player>(PlayerPath);
        
        _aiInfo = new AiInfo();
        _arrive = new Arrive(_aiInfo, _player.KinematicAiInfo, MaxSpeed, TargetPositionRadius);
        _wander = new Wander(_aiInfo, MaxSpeed, MaxRotation);
    }

    public override void _Process(float delta)
    {
        _lineDrawer.DrawLine(Vector3.Zero, Forward);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        switch (CurBehaviorType)
        {
            case BehaviorType.Arrive:
                Arrive();
                break;
            case BehaviorType.Wander:
                Wander();
                break;
        }
    }

    private void Arrive()
    {
        if (_arrive.GetSteering(out SteeringOutput steering))
        {
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            Forward = _pivot.Transform.basis.z;
        
            MoveAndSlide(steering.Velocity, Vector3.Up);
        }
        _aiInfo.Equalize(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }

    private void Wander()
    {
        if (_wander.GetSteering(out SteeringOutput steering))
        {
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x,
                _pivot.RotationDegrees.y + steering.Rotation,
                _pivot.RotationDegrees.z
            );
            Forward = _pivot.Transform.basis.z;

            MoveAndSlide(steering.Velocity, Vector3.Up);
        }
        _aiInfo.Equalize(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }
}
