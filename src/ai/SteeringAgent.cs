using System;
using Ai.SteeringBehavior;
using Godot;

public class SteeringAgent : KinematicBody
{
    [Export]
    public MoveType CurMoveType;
    [Export]
    public NodePath PlayerPath;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxChaseSpeed = 5;
    [Export(PropertyHint.Range, "0,100,or_greater")]
    public int MaxAcceleration = 20;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public int TargetRadius = 2;
    [Export(PropertyHint.Range, "0,10,or_greater")]
    public int SlowRadius = 4;
   
    public Vector3 Forward { get; private set; }
    private Player _player;
    private Spatial _pivot;
    private LineDrawer _lineDrawer;
    
    public enum MoveType { Seek, Arrive }

    private AiInfo _aiInfo;
    private Seek _seek;
    private Arrive _arrive;
    
    public override void _Ready()
    {
        _pivot= GetNode<Spatial>("Pivot");
        
        _lineDrawer = new LineDrawer(); 
        AddChild(_lineDrawer);
        
        _player = GetNode<Player>(PlayerPath);
        
        _aiInfo = new AiInfo();
        _seek = new Seek(_aiInfo, _player.SteeringAiInfo, MaxAcceleration);
        _arrive = new Arrive(_aiInfo, _player.SteeringAiInfo, MaxChaseSpeed, MaxAcceleration);
    }
    
    public override void _Process(float delta)
    {
        _lineDrawer.DrawLine(Vector3.Zero, Forward);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        switch (CurMoveType)
        {
            case MoveType.Seek:
                SeekMove(delta);
                break;
            case MoveType.Arrive:
                ArriveMove(delta);
                break;
        }
    }

    private void SeekMove(float delta)
    {
        if (_seek.GetSteering(out SteeringOutput steering))
        {
            _aiInfo.ProcessSpeeds(steering, MaxChaseSpeed, delta);
            MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.Equalize(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }

    private void ArriveMove(float delta)
    {
        if (_arrive.GetSteering(out SteeringOutput steering, TargetRadius, SlowRadius))
        {
            _aiInfo.ProcessSpeeds(steering, MaxChaseSpeed, delta);
            MoveAndSlide(_aiInfo.Velocity, Vector3.Up);
        }
        _aiInfo.Equalize(GlobalTransform.origin, _pivot.RotationDegrees.y);
    }
} 
