
using System.Windows.Controls;
using BadgerControlModule.ViewModels;
using BadgerControlModule.Services;

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
