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
using BadgerControlModule.ViewModels;
using BadgerControlModule.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;

namespace BadgerControlModule.Views
{
    public partial class BadgerControlLoggerView : UserControl
    {
        private static BadgerControlLoggerViewModel viewModel;

        public BadgerControlLoggerView()
        {      
            InitializeComponent();
            viewModel = new BadgerControlLoggerViewModel(ApplicationService.Instance.EventAggregator);
            DataContext = viewModel;
        }
    }
}
