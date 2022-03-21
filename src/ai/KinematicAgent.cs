using Ai.Kinematic;
using Godot;

public class KinematicAgent : KinematicBody
{
    [Export]
    public NodePath PlayerPath;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxChaseSpeed = 5;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxRotation = 5;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public float ChaseStopRadius = 2f;
    
    public Vector3 Forward { get; private set; }
    private Player _player;
    private Spatial _pivot;
    private LineDrawer _lineDrawer;
    
    private AiInfo _aiInfo;
    private KinematicSeek _kinematicSeek;
    private KinematicWander _kinematicWander;

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        
        _lineDrawer = new LineDrawer(); 
        AddChild(_lineDrawer);
        
        _player = GetNode<Player>(PlayerPath);
        
        _aiInfo = new AiInfo();
        _kinematicSeek = new KinematicSeek(
            _aiInfo, _player.KinematicAiInfo, MaxChaseSpeed, ChaseStopRadius
        );
        _kinematicWander = new KinematicWander(_aiInfo, MaxChaseSpeed, MaxRotation);
    }

    public override void _Process(float delta)
    {
        _lineDrawer.DrawLine(Vector3.Zero, Forward);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        SeekMove();
    }

    private void SeekMove()
    {
        if (_kinematicSeek.GetSteering(out SteeringOutput steering))
        {
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            Forward = _pivot.Transform.basis.z;
        
            MoveAndSlide(steering.Velocity, Vector3.Up);
        }
        _aiInfo.Equalize(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }

    private void WanderMove()
    {
        if (_kinematicWander.GetSteering(out SteeringOutput steering))
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
