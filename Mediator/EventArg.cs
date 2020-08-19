using System;

namespace Real_time_Spectrum_Analyzer.Mediator
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }
        public T Value { get; private set; }
    }
}
