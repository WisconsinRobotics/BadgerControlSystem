using System;
using System.Collections.Generic;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using BadgerControlModule.Services;

using Prism.Events;

using JoystickLibrary;

namespace BadgerControlModule.Views
{
    public partial class BadgerControlVisualView : UserControl
    {
        JoystickQueryThread joystickQueryThread;
        protected readonly IEventAggregator _eventAggregator;
        private long angle, velocity;

        public const int JOYSTICK_ID = 2;

        private const int TOP = 0;
        private const int CENTER = 75;
        private const int BOTTOM = 150;

        public BadgerControlVisualView()
        {
            InitializeComponent();
            joystickQueryThread = null;
            CompositionTarget.Rendering += OnRender;

            _eventAggregator = ApplicationService.Instance.EventAggregator;
            _eventAggregator.GetEvent<DeliverJoystickEvent>().Subscribe((joystick) =>
            {
                // send a confirmation back when we receive a joystick
                joystickQueryThread = joystick;
                _eventAggregator.GetEvent<ConfirmJoystickEvent>().Publish(JOYSTICK_ID);        
            });
        }

        public void OnRender(object sender, EventArgs e)
        {
            if (joystickQueryThread == null)
            {
                return;
            }

            angle = joystickQueryThread.Angle;
            velocity = joystickQueryThread.Velocity;

            /* JOYSTICK BOUNDS:
             *  velocity ~ [-100, 100]
             *  angle    ~ [-180, 180]
             */

            double velocityDouble = (double)velocity;
            double angleDouble = (double)angle;
            velocityDouble /= 100.0;
            angleDouble /= 180.0;

            Canvas canvas = dotCanvas;
            Ellipse theDot = dot;
            Canvas.SetLeft(theDot, CENTER + angleDouble * (BOTTOM - TOP) / 2);
            Canvas.SetTop(theDot, CENTER - velocityDouble * (BOTTOM - TOP) / 2);

            Border theBorder = joystickBorder;
            Ellipse theCenter = center;

            canvas.Children.Clear();
            canvas.Children.Add(theDot);
            canvas.Children.Add(theBorder);
            canvas.Children.Add(theCenter);
        }
    }
}

