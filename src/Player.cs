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

    private Spatial _pivot;
    private Vector3 _velocity;
    private Vector3 _inputAxis;
    private Vector3 _desiredVelocity;

    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
    }

    public override void _PhysicsProcess(float delta)
    {
        _inputAxis = new Vector3
        {
            x = InputManager.GetAxis("move_left", "move_right"),
            y = 0,
            z = InputManager.GetAxis("move_forward", "move_back")
        };
        if (_inputAxis != Vector3.Zero) _inputAxis = _inputAxis.Normalized();

        _desiredVelocity = MoveSpeed * _inputAxis;
        _velocity.x = Mathf.MoveToward(_velocity.x, _desiredVelocity.x, MoveAcceleration * delta);
        _velocity.z = Mathf.MoveToward(_velocity.z, _desiredVelocity.z, MoveAcceleration * delta);
        if (IsOnFloor()) _velocity.y = -FallAcceleration * delta;
        else _velocity.y -= FallAcceleration * delta;
        
        _velocity = MoveAndSlide(_velocity, Vector3.Up);
    }

    public override void _Process(float delta)
    {
        float fps = GM.FramesPerSecond;
        var lerpInterval = _velocity / fps;
        var lerpPosition = GlobalTransform.origin + lerpInterval;
        
        if (fps > GM.PhysicsFramesPerSecond)
        {
            _pivot.SetAsToplevel(true);
            Transform globalTransform = _pivot.GlobalTransform;
            globalTransform.origin = _pivot.GlobalTransform.origin.LinearInterpolate(
                lerpPosition, 20 * delta
            );
            _pivot.GlobalTransform = globalTransform;
        }
        else
        {
            _pivot.GlobalTransform = GlobalTransform;
            _pivot.SetAsToplevel(false);
        }
    }
}