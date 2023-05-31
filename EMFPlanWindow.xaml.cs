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
using System.Windows.Shapes;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for EMFPlanWindow.xaml
    /// </summary>
    public partial class EMFPlanWindow : Window
    {
        private Employee loggedInUser;
         
        private EMFPlan plan;
         
        private CommonQueries commonQueries;
         
        private List<BASIC_STRUCTS.EMF_STRUCT> emfPoints;

        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> planStatus;
        private List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT> engineers;

        private int viewAddCondition;


        public EMFPlanWindow(ref Employee mLoggedInUser, ref EMFPlan mPlan, ref int mViewAddCondition)
        {
            loggedInUser = mLoggedInUser;
            plan = mPlan;
            viewAddCondition = mViewAddCondition;

            commonQueries = new CommonQueries();

            emfPoints = new List<BASIC_STRUCTS.EMF_STRUCT>();
            planStatus = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();
            engineers = new List<BASIC_STRUCTS.KEY_VALUE_PAIR_STRUCT>();

            InitializeComponent();

            GetEmfPoints();
            GetEmfPlanStatus();
            GetEngineers();

            if (viewAddCondition == BASIC_STRUCTS.EMF_PLAN_ADD_CONDITION)
            {
                statusComboBox.SelectedIndex = 0;
                InitializePointsGrid();
            }
            else
            {
                SetUIValues();

                if(viewAddCondition == BASIC_STRUCTS.EMF_PLAN_VIEW_CONDITION)
                {
                    InitializePointsGridView();
                    DisableUIElements();
                }
                else
                {
                    InitializePointsGrid();
                }
            }

        }

        private void GetEmfPoints()
        {
            if (!commonQueries.GetEMFPoints(ref emfPoints))
                return;
        }

        private void GetEmfPlanStatus()
        {
            if (!commonQueries.GetEMFPlanStatus(ref planStatus))
                return;

            statusComboBox.Items.Clear();

            for(int i = 0; i < planStatus.Count; i++)
            {
                statusComboBox.Items.Add(planStatus[i].value);
            }

        }

        private void GetEngineers()
        {

            if (!commonQueries.GetEngineers(ref engineers))
                return;

            assignedEngineerComboBox.Items.Clear();

            for(int i = 0; i < engineers.Count; i++)
            {
                assignedEngineerComboBox.Items.Add(engineers[i].value);
            }
        }

        private void InitializePointsGrid()
        {
            emfPointsGrid.Children.Clear();

            int currentColumn = 0;
            int currentRow = 0;
            int counter = 0;

            for(int i = 0; i < emfPoints.Count; i++)
            {
                if (searchTextBox.Text != "")
                {
                    string search = searchTextBox.Text;
                    bool contains = emfPoints[i].name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (counter == 4)
                {
                    counter = 0;
                    currentRow += 1;
                    currentColumn = 0;
                    emfPointsGrid.RowDefinitions.Add(new RowDefinition());
                }

                CheckBox currentPoint = new CheckBox() { Style = (Style)FindResource("wideCheckBoxStyle"), Margin = new Thickness(12) };
                currentPoint.Content = emfPoints[i].name;
                currentPoint.Tag = i;

                if (plan.planPoints.Exists(x1 => x1.emf_serial == emfPoints[i].emf_serial))
                {
                    currentPoint.IsChecked = true;
                }

                currentPoint.Checked += OnCheckEMFPoint;
                currentPoint.Unchecked += OnUncheclEMFPoint;


                emfPointsGrid.Children.Add(currentPoint);
                Grid.SetRow(currentPoint, currentRow);
                Grid.SetColumn(currentPoint, currentColumn);
                currentColumn++;
                counter++;
            }
        }

        private void SetUIValues()
        {
            planNameTextBox.Text = plan.GetPlanName();
            assignedEngineerComboBox.SelectedIndex = engineers.FindIndex(x1 => x1.key == plan.GetAssignedEngineerId());
            statusComboBox.SelectedIndex = planStatus.FindIndex(x1 => x1.key == plan.GetPlanStatusId());
            deadlineDatePicker.SelectedDate = plan.GetPlanDeadlineDate();
        }

        private void DisableUIElements()
        {
            planNameTextBox.IsReadOnly = true;
            assignedEngineerComboBox.IsEnabled = false;
            deadlineDatePicker.IsEnabled = false;
            finishButton.IsEnabled = false;
        }

        private void InitializePointsGridView()
        {
            emfPointsGrid.Children.Clear();

            int currentColumn = 0;
            int currentRow = 0;
            int counter = 0;

            for (int i = 0; i < plan.planPoints.Count; i++)
            {
                if (searchTextBox.Text != "")
                {
                    string search = searchTextBox.Text;
                    bool contains = plan.planPoints[i].name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!contains)
                        continue;
                }

                if (counter == 4)
                {
                    counter = 0;
                    currentRow += 1;
                    currentColumn = 0;
                    emfPointsGrid.RowDefinitions.Add(new RowDefinition());
                }

                CheckBox currentPoint = new CheckBox() { Style = (Style)FindResource("wideCheckBoxStyle"), Margin = new Thickness(12) };
                currentPoint.Content = plan.planPoints[i].name;
                currentPoint.IsChecked = true;
                currentPoint.Tag = i;
                currentPoint.IsEnabled = false;
                currentPoint.Checked += OnCheckEMFPoint;
                currentPoint.Unchecked += OnUncheclEMFPoint;

                emfPointsGrid.Children.Add(currentPoint);
                Grid.SetRow(currentPoint, currentRow);
                Grid.SetColumn(currentPoint, currentColumn);
                currentColumn++;
                counter++;
            }
        }

        private void OnUncheclEMFPoint(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;

            for (int i = 0; i < plan.planPoints.Count; i++)
            {
                if (plan.planPoints[i].emf_serial == emfPoints[int.Parse(currentCheckBox.Tag.ToString())].emf_serial)
                    plan.planPoints.RemoveAt(i);
            }
        }

        private void OnCheckEMFPoint(object sender, RoutedEventArgs e)
        {
            CheckBox currentCheckBox = (CheckBox)sender;

            plan.planPoints.Add(emfPoints[int.Parse(currentCheckBox.Tag.ToString())]);
        }

        private void OnClickSearchButton(object sender, RoutedEventArgs e)
        {
            if (viewAddCondition != BASIC_STRUCTS.EMF_PLAN_VIEW_CONDITION)
                InitializePointsGrid();
            else
                InitializePointsGridView();
        }

        private void OnButtonClickSaveChanges(object sender, RoutedEventArgs e)
        {
            if(planNameTextBox.Text == "")
            {
                MessageBox.Show("Plan name must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (assignedEngineerComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Assigned engineer must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (statusComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Status must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (deadlineDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Deadline date must be specified!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (plan.GetPlanPoints().Count == 0)
            {
                MessageBox.Show("At least one EMF point must be selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            plan.SetPlanName(planNameTextBox.Text);
            plan.SetAssignedEngineer(engineers[assignedEngineerComboBox.SelectedIndex].value);
            plan.SetAssignedEngineerId(engineers[assignedEngineerComboBox.SelectedIndex].key);
            plan.SetPlanDeadlineDate((DateTime)deadlineDatePicker.SelectedDate);
            plan.SetPlanStatusId(planStatus[statusComboBox.SelectedIndex].key);
            plan.SetPlanStatus(planStatus[statusComboBox.SelectedIndex].value);


            if(viewAddCondition == BASIC_STRUCTS.EMF_PLAN_ADD_CONDITION)
            {
                if (!plan.IssueNewPlan())
                    return;

                this.Close();
            }
            else
            {
                if (!plan.UpdateEMFPlan())
                    return;
                if (!plan.DeleteEMFPlansPoints())
                    return;
                if (!plan.InsertIntoEMFPlansPoints())
                    return;

                this.Close();
            }
        }
    }
}
