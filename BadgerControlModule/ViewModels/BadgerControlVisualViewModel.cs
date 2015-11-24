
using System.Threading;

using Prism.Mvvm;
using Prism.Events;

using JoystickLibrary;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlVisualViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        JoystickQueryThread joystick;

        public BadgerControlVisualViewModel(IEventAggregator eventAggregator)
        {
            joystick = new JoystickQueryThread();
            Thread joystickThread = new Thread(joystick.QueryJoystick);
            joystickThread.Start();

            this._eventAggregator = eventAggregator;
        }
    }
}
