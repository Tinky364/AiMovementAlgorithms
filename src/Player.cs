using Ai.Kinematic;
using Godot;
using Manager;

public class Player : KinematicBody
{
    [Export]
    public int MoveSpeed = 20;
    [Export]
    public int MoveAcceleration = 80;
    [Export]
    public int FallAcceleration = 60;
    [Export]
    public int JitterAcceleration = 20;
    [Export]
    public int RotationAcceleration = 40;

    public StaticInfo StaticInfo;
    private Spatial _pivot;
    private LineDrawer _lineDrawer;
    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;
    private Vector3 _inputAxis;
    public Vector3 Forward { get; private set; }

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        _lineDrawer = new LineDrawer();
        AddChild(_lineDrawer);
        StaticInfo = new StaticInfo();
    }

    public override void _Process(float delta)
    {
        _lineDrawer.DrawLine(Vector3.Zero, Forward);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        _inputAxis = CalculateAxisInput();
       
        _velocity = CalculateVelocity(_velocity, _inputAxis, MoveSpeed, MoveAcceleration, delta);
        _velocity = CalculateGravity(_velocity, FallAcceleration, delta);
        
        _pivot.RotationDegrees = CalculateOrientationY(
            _pivot.RotationDegrees, _inputAxis, RotationAcceleration, delta
        );
        Forward = _pivot.Transform.basis.z;
        
        _velocity = MoveAndSlide(_velocity, Vector3.Up);
        
        StaticInfo.Update(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }

    private Vector3 CalculateAxisInput()
    {
        Vector3 inputAxis = new Vector3
        {
            x = InputManager.GetAxis("move_right", "move_left"),
            y = 0,
            z = InputManager.GetAxis("move_back", "move_forward")
        };
        if (inputAxis != Vector3.Zero) inputAxis = inputAxis.Normalized();
        return inputAxis;
    }

    private Vector3 CalculateOrientationY(
        Vector3 rotation, Vector3 direction, float acceleration, float delta
    )
    {
        if (direction == Vector3.Zero) return rotation;
        
        float targetRotationY = Mathf.Rad2Deg(Mathf.Atan2(direction.x, direction.z));
        float diff = (targetRotationY - rotation.y + 180) % 360 - 180;
        diff = diff < -180 ? diff + 360 : diff;
        if (Mathf.Abs(diff) <= 0.1f) return rotation;
        
        rotation.y = Mathf.MoveToward(rotation.y, rotation.y + diff, acceleration * delta);
        return rotation;
    }

    private Vector3 CalculateVelocity(
        Vector3 velocity, Vector3 direction, float maxMoveSpeed, float moveAcceleration, float delta
    )
    {
        Vector3 desiredVelocity = maxMoveSpeed * direction;
        velocity.x = Mathf.MoveToward(velocity.x, desiredVelocity.x, moveAcceleration * delta);
        velocity.z = Mathf.MoveToward(velocity.z, desiredVelocity.z, moveAcceleration * delta);
        return velocity;
    }

    private Vector3 CalculateGravity(Vector3 velocity, float fallAcceleration, float delta)
    {
        if (IsOnFloor()) velocity.y = -fallAcceleration * delta;
        else velocity.y -= fallAcceleration * delta;
        return velocity;
    }

    private void InterpolateTranslate(float delta)
    {
        float fps = GM.FramesPerSecond;
        var lerpInterval = _velocity / fps;
        var lerpPosition = GlobalTransform.origin + lerpInterval;
        if (Mathf.FloorToInt(fps) > GM.PhysicsFramesPerSecond + 2f)
        {
            _pivot.SetAsToplevel(true);
            Transform globalTransform = _pivot.GlobalTransform;
            globalTransform.origin = _pivot.GlobalTransform.origin.LinearInterpolate(
                lerpPosition, JitterAcceleration * delta
            );
            _pivot.GlobalTransform = globalTransform;
        }
        else
        {
            _pivot.GlobalTransform = GlobalTransform;
            _pivot.SetAsToplevel(false);
        }
    }
    
    /*private void CalculateRot(float delta)
    {
        if (_inputAxis == Vector3.Zero) return;
        
        float targetRotationY = Mathf.Rad2Deg(Mathf.Atan2(_inputAxis.x, _inputAxis.z));
        var targetQuat = new Quat(Vector3.Up, targetRotationY);
        
        float curRotationY = _pivot.RotationDegrees.y;
        if (Math.Abs(curRotationY - targetRotationY) <= 0.1f) return;

        var curQuat = Transform.basis.Quat();
        curQuat = RotateToward(
            curQuat, targetQuat, RotationAcceleration * delta
        );
        Transform = new Transform(curQuat, Transform.origin);
    }
    
    private static Quat RotateToward(Quat from, Quat to, float delta)
    {
        float angle = from.AngleTo(to);
        if (angle == 0.0f) return to;
        return from.Slerp(to, Mathf.Min(1.0f, delta / angle));
    }*/
}