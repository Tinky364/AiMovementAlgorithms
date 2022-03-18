using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Manager
{
    public class TreeTimer : Singleton<TreeTimer>
    {
        private Queue<Timer> _timerPool;

        public override void _EnterTree()
        {
            SetSingleton();
            _timerPool = new Queue<Timer>();
        }

        public SignalAwaiter Wait(float duration)
        {
            Timer timer = Pull();
            timer.WaitTime = duration;
            timer.Start();
            return ToSignal(timer, "timeout");
        }

        public void Push(Timer timer)
        {
            timer.Disconnect("timeout", this, nameof(Push));
            timer.Stop();
            RemoveChild(timer);
            _timerPool.Enqueue(timer);
        }

        private Timer Pull()
        {
            if (_timerPool.Count <= 0) AddNewTimerToPool();
            
            Timer timer = _timerPool.Dequeue();
            AddChild(timer, true);
            timer.Connect("timeout", this, nameof(Push), new Array(timer));
            return timer;
        }

        private void AddNewTimerToPool()
        {
            Timer timer = new Timer();
            timer.Name = "Timer";
            _timerPool.Enqueue(timer);
        }

        public void ClearPool()
        {
            while (_timerPool.Count > 0) _timerPool.Dequeue().QueueFree();
        }
    }
}
