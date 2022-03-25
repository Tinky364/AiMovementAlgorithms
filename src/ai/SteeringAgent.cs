using Ai.Steering;
using Godot;

public class SteeringAgent : KinematicBody
{
    [Export]
    public BehaviorType CurBehaviorType;
    [Export]
    public NodePath PlayerPath;
    
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxSpeed = 5;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxAcceleration = 20;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public int TargetPositionRadius = 2;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public int SlowPositionRadius = 6;
    [Export(PropertyHint.Range, "0,10,0.1,or_greater")]
    public float MaxPrediction = 3;
    
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxRotation = 90;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxAngularAcceleration = 240;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int TargetOrientationRadius = 1;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int SlowOrientationRadius = 30;
   
    public Vector3 Forward { get; private set; }
    private Player _player;
    private Spatial _pivot;
    private LineDrawer _lineDrawer;

    private AiInfo _aiInfo;
    
    public enum BehaviorType { Seek, Arrive, Align, VelocityMatch, Pursue }
    private Seek _seek;
    private Arrive _arrive;
    private Align _align;
    private VelocityMatch _velocityMatch;
    private Pursue _pursue;

    public override void _EnterTree()
    {
        _aiInfo = new AiInfo();
    }

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        
        _lineDrawer = new LineDrawer(); 
        AddChild(_lineDrawer);
        
        _player = GetNode<Player>(PlayerPath);
        
        _seek = new Seek(_aiInfo, _player.SteeringAiInfo, MaxAcceleration);
        _arrive = new Arrive(_aiInfo, _player.SteeringAiInfo, MaxSpeed, MaxAcceleration);
        _align = new Align(_aiInfo, _player.SteeringAiInfo, MaxRotation, MaxAngularAcceleration);
        _velocityMatch = new VelocityMatch(_aiInfo, _player.SteeringAiInfo, MaxAcceleration);
        _pursue = new Pursue(_aiInfo, _player.SteeringAiInfo, MaxAcceleration, MaxSpeed, MaxPrediction);
    }
    
    public override void _Process(float delta)
    {
        _lineDrawer.DrawLine(Vector3.Zero, Forward);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        switch (CurBehaviorType)
        {
            case BehaviorType.Seek:
                Seek(delta);
                break;
            case BehaviorType.Arrive:
                Arrive(delta);
                break;
            case BehaviorType.Align:
                Align(delta);
                break;
            case BehaviorType.VelocityMatch:
                VelocityMatch(delta);
                break;
            case BehaviorType.Pursue:
                Pursue(delta);
                break;
        }
    }

    private void Seek(float delta)
    {
        if (_seek.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.ProcessSpeeds(steering, delta);
            if (_aiInfo.Velocity.Length() > MaxSpeed) 
                _aiInfo.Velocity = _aiInfo.Velocity.Normalized() * MaxSpeed;
            MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.EqualizePositions(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }

    private void Arrive(float delta)
    {
        if (_arrive.GetSteering(out SteeringOutput steering, TargetPositionRadius, SlowPositionRadius))
        {
            _aiInfo.ProcessSpeeds(steering, delta);
            MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.EqualizePositions(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }

    private void Align(float delta)
    {
        if (_align.GetSteering(out SteeringOutput steering, TargetOrientationRadius, SlowOrientationRadius))
        {
            _aiInfo.ProcessPositionsAndSpeeds(steering, delta);
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            Forward = _pivot.Transform.basis.z;
        }
    }

    private void VelocityMatch(float delta)
    {
        if (_velocityMatch.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.ProcessSpeeds(steering, delta);
            MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.EqualizePositions(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }
    
    private void Pursue(float delta)
    {
        if (_pursue.GetSteering(out SteeringOutput steering, TargetPositionRadius, SlowPositionRadius))
        {
            _aiInfo.ProcessSpeeds(steering, delta);
            if (_aiInfo.Velocity.Length() > MaxSpeed) 
                _aiInfo.Velocity = _aiInfo.Velocity.Normalized() * MaxSpeed;
            MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.EqualizePositions(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }
} 
