using Ai.Kinematic;
using Godot;

public class Agent : KinematicBody
{
    [Export]
    public NodePath PlayerPath;
    [Export]
    public int MaxChaseSpeed = 5;
    [Export]
    public int MaxRotation = 5;
    [Export]
    public float ChaseStopRadius = 2f;

    private StaticInfo _staticInfo;
    private KinematicSeek _kinematicSeek;
    private KinematicWander _kinematicWander;
    
    public Player Player { get; private set; }
    private Spatial _pivot;
    private LineDrawer _lineDrawer;
    
    public Vector3 Forward { get; private set; }

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        
        _lineDrawer = new LineDrawer(); 
        AddChild(_lineDrawer);
        
        Player = GetNode<Player>(PlayerPath);
        
        _staticInfo = new StaticInfo();
        _kinematicSeek = new KinematicSeek(
            _staticInfo, Player.StaticInfo, MaxChaseSpeed, ChaseStopRadius
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
