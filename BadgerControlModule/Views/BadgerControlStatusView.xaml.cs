using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BadgerControlModule.Services;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;

using JoystickLibrary;

namespace BadgerControlModule.Views
{
    /// <summary>
    /// Interaction logic for BadgerControlStatusView.xaml
    /// </summary>
    public partial class BadgerControlStatusView : UserControl
    {
        JoystickQueryThread joystickQueryThread;
        protected readonly IEventAggregator _eventAggregator;
        public const int JOYSTICK_ID = 1;

        public BadgerControlStatusView()
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
            _eventAggregator.GetEvent<ConnectionDetailsEvent>().Subscribe((connection) =>
            {
                UpdateConnection(connection);
            });

            // create a ConnectionDetails to keep around. its fields will be used to help organize which 
            // components have which connection statuses
            ConnectionDetails connectionDetails = new ConnectionDetails();
            connectionDetails.ai = ConnectionOption.DISCONNECTED;
            connectionDetails.remote = ConnectionOption.DISCONNECTED;
            connectionDetails.direct = ConnectionOption.DISCONNECTED;
            UpdateConnection(connectionDetails);
        }

        public void OnRender(object sender, EventArgs e)
        {
            UpdateJoystick();
        }

        public void UpdateJoystick()
        {
            // display 0s if we have no joystick
            if (joystickQueryThread == null)
            {
                headingBox.Text = "0";
                velocityBox.Text = "0";
                return;
            }

            headingBox.Text = joystickQueryThread.Angle.ToString();
            velocityBox.Text = joystickQueryThread.Velocity.ToString();
        }

        // Don't worry guys, I write clean code! 
        // Dispatcher.Invoke must be used because the thread that calls this function is not the same thread that the original instance 
        // of the class is on. 
        // This function will do the actual updating of the connection icons
        public void UpdateConnection(ConnectionDetails connection)
        {      
            if (connection.ai == ConnectionOption.CONNECTED)
                aiDot.Dispatcher.Invoke(() => { aiDot.Fill = new SolidColorBrush(Colors.Green); });
            else if (connection.ai == ConnectionOption.REQUESTING_CONTROL)
                aiDot.Dispatcher.Invoke(() => { aiDot.Fill = new SolidColorBrush(Colors.Orange); });
            else if (connection.ai == ConnectionOption.AWAITING_STATUS)
                aiDot.Dispatcher.Invoke(() => { aiDot.Fill = new SolidColorBrush(Colors.Yellow); });
            else
                aiDot.Dispatcher.Invoke(() => { aiDot.Fill = new SolidColorBrush(Colors.Red); });

            if (connection.remote == ConnectionOption.CONNECTED)
                remoteDot.Dispatcher.Invoke(() => { remoteDot.Fill = new SolidColorBrush(Colors.Green); });
            else if (connection.remote == ConnectionOption.REQUESTING_CONTROL)
                remoteDot.Dispatcher.Invoke(() => { remoteDot.Fill = new SolidColorBrush(Colors.Orange); });
            else if (connection.remote == ConnectionOption.AWAITING_STATUS)
                remoteDot.Dispatcher.Invoke(() => { remoteDot.Fill = new SolidColorBrush(Colors.Yellow); });
            else
                remoteDot.Dispatcher.Invoke(() => { remoteDot.Fill = new SolidColorBrush(Colors.Red); });

            if (connection.direct == ConnectionOption.CONNECTED)
                directDot.Dispatcher.Invoke(() => { directDot.Fill = new SolidColorBrush(Colors.Green); });
            else if (connection.direct == ConnectionOption.REQUESTING_CONTROL)
                directDot.Dispatcher.Invoke(() => { directDot.Fill = new SolidColorBrush(Colors.Orange); });
            else if (connection.direct == ConnectionOption.AWAITING_STATUS)
                directDot.Dispatcher.Invoke(() => { directDot.Fill = new SolidColorBrush(Colors.Yellow); });
            else
                directDot.Dispatcher.Invoke(() => { directDot.Fill = new SolidColorBrush(Colors.Red); });
        }
    }
}
