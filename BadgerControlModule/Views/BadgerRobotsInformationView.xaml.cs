
using System.Windows.Controls;
using BadgerControlModule.ViewModels;
using BadgerControlModule.Services;

namespace BadgerControlModule.Views
{
    /// <summary>
    /// Interaction logic for BadgerRobotsInformationView.xaml
    /// </summary>
    public partial class BadgerRobotsInformationView : UserControl
    {
        public BadgerRobotsInformationView()
        {
            InitializeComponent();
        }

        private void SubsystemTreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            BadgerRobotsInformationViewModel viewModel = DataContext as BadgerRobotsInformationViewModel;
            if (viewModel == null)
                return;

            viewModel.SelectedNodeComponentChanged(sender, e);
        }
    }
}
