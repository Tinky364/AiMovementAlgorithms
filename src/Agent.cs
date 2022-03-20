using Ai;
using Ai.Kinematic;
using Godot;

public class Agent : KinematicBody
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

    private StaticInfo _staticInfo;
    private KinematicSeek _kinematicSeek;
    private KinematicWander _kinematicWander;
    private Player _player;
    private Spatial _pivot;
    private LineDrawer _lineDrawer;

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        
        _lineDrawer = new LineDrawer(); 
        AddChild(_lineDrawer);
        
        _player = GetNode<Player>(PlayerPath);
        
        _staticInfo = new StaticInfo();
        _kinematicSeek = new KinematicSeek(
            _staticInfo, _player.StaticInfo, MaxChaseSpeed, ChaseStopRadius
        );
        _kinematicWander = new KinematicWander(_staticInfo, MaxChaseSpeed, MaxRotation);
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
        if (_kinematicSeek.GetSteering(out KinematicSteeringOutput steeringOutput))
        {
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _staticInfo.Orientation, _pivot.RotationDegrees.z
            );
            Forward = _pivot.Transform.basis.z;
        
            MoveAndSlide(steeringOutput.Velocity, Vector3.Up);
        }
        _staticInfo.Update(GlobalTransform.origin, _staticInfo.Orientation);
    }

    private void WanderMove()
    {
        if (_kinematicWander.GetSteering(out KinematicSteeringOutput steeringOutput))
        {
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x,
                _pivot.RotationDegrees.y + steeringOutput.Rotation,
                _pivot.RotationDegrees.z
            );
            Forward = _pivot.Transform.basis.z;

            MoveAndSlide(steeringOutput.Velocity, Vector3.Up);
        }
        _staticInfo.Update(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }
}
