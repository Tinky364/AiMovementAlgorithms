using Ai.Steering;
using Godot;
using Manager;

public class SteeringAgent : KinematicBody
{
    public enum BehaviorType
    {
        Seek, Arrive, Align, VelocityMatch, Pursue, Face, LookWhereGo, Wander, FollowPath
    }
    
    [Export]
    public BehaviorType CurBehaviorType;
    [Export]
    public NodePath PlayerPath;
    
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxLinearSpeed = 5;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxLinearAcceleration = 20;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public int StopDistance = 2;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public int SlowDistance = 6;
    [Export(PropertyHint.Range, "0,10,0.1,or_greater")]
    public float MaxPredictionTime = 2;
    
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxAngularSpeed = 90;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxAngularAcceleration = 240;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int StopAngle = 0;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int SlowAngle = 30;
    
    [Export(PropertyHint.Range, "0,40,0.2,or_greater")]
    public float WanderOffset = 20;
    [Export(PropertyHint.Range, "0,20,0.2,or_greater")]
    public float WanderRadius = 5;
    [Export(PropertyHint.Range, "0,100,1,or_greater")]
    public float WanderRate = 30;

    [Export]
    public NodePath PathPath;
    [Export(PropertyHint.Range, "1,20,0.2,or_greater")]
    public float PathOffset = 5;
    
    public Vector3 Forward { get; private set; }
    
    private Spatial _pivot;
    private Path _path;
    private Player _player;
    private AiInfo _aiInfo;

    // Behaviors
    private Seek _seek;
    private Arrive _arrive;
    private Align _align;
    private VelocityMatch _velocityMatch;
    private Pursue _pursue;
    private Face _face;
    private LookWhereGo _lookWhereGo;
    private Wander _wander;
    private FollowPath _followPath;
   
    public override void _EnterTree()
    {
        _aiInfo = new AiInfo();
    }

    public override void _Ready()
    {
        _pivot = GetNode<Spatial>("Pivot");
        _path = GetNode<Path>(PathPath);
        _player = GetNode<Player>(PlayerPath);
        
        _seek = new Seek(_aiInfo, _player.AiInfo, MaxLinearAcceleration);
        _arrive = new Arrive(
            _aiInfo, _player.AiInfo, MaxLinearSpeed, MaxLinearAcceleration, StopDistance, 
            SlowDistance
        );
        _align = new Align(
            _aiInfo, _player.AiInfo, MaxAngularSpeed, MaxAngularAcceleration, StopAngle, 
            SlowAngle
        );
        _velocityMatch = new VelocityMatch(_aiInfo, _player.AiInfo, MaxLinearAcceleration);
        _pursue = new Pursue(
            _aiInfo, _player.AiInfo, MaxLinearSpeed, MaxLinearAcceleration, StopDistance, 
            SlowDistance, MaxPredictionTime
        );
        _face = new Face(
            _aiInfo, _player.AiInfo, MaxAngularSpeed, MaxAngularAcceleration, StopAngle, 
            SlowAngle
        );
        _lookWhereGo = new LookWhereGo(
            _aiInfo, MaxAngularSpeed, MaxAngularAcceleration, StopAngle, SlowAngle
        );
        _wander = new Wander(
            _aiInfo, MaxAngularSpeed, MaxAngularAcceleration, StopAngle, SlowAngle, WanderOffset, 
            WanderRadius, WanderRate, MaxLinearAcceleration
        );
        _followPath = new FollowPath(_aiInfo, MaxLinearAcceleration, _path, PathOffset);
    }
    
    public override void _Process(float delta)
    {
        Drawer.S.DrawLine(
            this, new Vector3(0, 0.1f, 0), new Vector3(Forward.x * 100, 0.1f, Forward.z * 100)
        );
        Drawer.S.DrawPath(_path, _path);
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
            case BehaviorType.Face: 
                Face(delta);
                break;
            case BehaviorType.LookWhereGo: 
                LookWhereGo(delta);
                break;
            case BehaviorType.Wander:
                Wander(delta);
                break;
            case BehaviorType.FollowPath:
                FollowPath(delta);
                break;
        }
    }

    private void Seek(float delta)
    {
        if (_seek.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            if (_aiInfo.Velocity.Length() > MaxLinearSpeed) 
                _aiInfo.Velocity = _aiInfo.Velocity.Normalized() * MaxLinearSpeed;
            _aiInfo.Velocity = MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
            
            Forward = _aiInfo.Velocity.Normalized();
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void Arrive(float delta)
    {
        if (_arrive.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            _aiInfo.Velocity = MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
            
            Forward = _aiInfo.Velocity.Normalized();
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void Align(float delta)
    {
        if (_align.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            
            Forward = _pivot.Transform.basis.z;
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void VelocityMatch(float delta)
    {
        if (_velocityMatch.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            _aiInfo.Velocity = MoveAndSlide(_aiInfo.Velocity, Vector3.Up);

            Forward = _aiInfo.Velocity.Normalized();
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }
    
    private void Pursue(float delta)
    {
        if (_pursue.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            if (_aiInfo.Velocity.Length() > MaxLinearSpeed) 
                _aiInfo.Velocity = _aiInfo.Velocity.Normalized() * MaxLinearSpeed;
            _aiInfo.Velocity = MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
            
            Forward = _aiInfo.Velocity.Normalized();
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void Face(float delta)
    {
        if (_face.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            
            Forward = _pivot.Transform.basis.z;
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void LookWhereGo(float delta)
    {
        if (_lookWhereGo.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            
            Forward = _pivot.Transform.basis.z;
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void Wander(float delta)
    {
        if (_wander.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
           
            _pivot.RotationDegrees = new Vector3(
                _pivot.RotationDegrees.x, _aiInfo.Orientation, _pivot.RotationDegrees.z
            );
            
            Forward = _pivot.Transform.basis.z;
        
            if (_aiInfo.Velocity.Length() > MaxLinearSpeed) 
                _aiInfo.Velocity = _aiInfo.Velocity.Normalized() * MaxLinearSpeed;
            _aiInfo.Velocity = MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }

    private void FollowPath(float delta)
    {
        if (_followPath.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.Process(steering, delta);
            
            if (_aiInfo.Velocity.Length() > MaxLinearSpeed) 
                _aiInfo.Velocity = _aiInfo.Velocity.Normalized() * MaxLinearSpeed;
            _aiInfo.Velocity = MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
            
            Forward = _aiInfo.Velocity.Normalized();
        }
        _aiInfo.Sync(
            GlobalTransform.origin, _pivot.RotationDegrees.y, _aiInfo.Velocity, _aiInfo.Rotation
        );
    }
}
