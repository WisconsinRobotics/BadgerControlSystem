
using System.Windows.Controls;
using BadgerControlModule.ViewModels;
using BadgerControlModule.Services;

using BadgerJaus.Util;

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
            BadgerRobotsInformationViewModel viewModel = new BadgerRobotsInformationViewModel(ApplicationService.Instance.EventAggregator);
            DataContext = viewModel;
        }

        private void SubsystemTreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            BadgerRobotsInformationViewModel viewModel = DataContext as BadgerRobotsInformationViewModel;
            if (viewModel == null)
                return;

            viewModel.SelectedNodeComponentChanged(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BadgerRobotsInformationViewModel viewModel = DataContext as BadgerRobotsInformationViewModel;
            if (viewModel == null)
                return;

            viewModel.SubsystemSelected(sender, e);

            //SubsystemTreeView.ItemsSource = viewModel.CurrentNodes;
            SubsystemTreeView.Items.Clear();
            foreach(Node node in viewModel.CurrentNodes)
            {
                SubsystemTreeView.Items.Add(node);
            }
        }
    }
}
