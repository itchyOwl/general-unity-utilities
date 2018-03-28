using System;

namespace ItchyOwl.General
{
    public interface IToggleable
    {
        /// <summary>
        /// The desired state provided as the event argument
        /// Note that the class that implements this interface has to implement the event explicitly, like so:
        /// public event EventHandler<ToggleEvent> Toggled;
        /// event EventHandler<ToggleEvent> IToggleable.Toggled
        /// {
        ///     add { Toggled += value; }
        ///     remove { Toggled -= value; }
        /// }
        /// </summary>
        event EventHandler<ToggleEvent> Toggled;
        void Toggle();
        void SetOn();
        void SetOff();
        bool IsOn { get; }
    }

    public class ToggleEvent : EventArgs
    {
        public readonly bool value;

        public ToggleEvent(bool value)
        {
            this.value = value;
        }
    }
}
