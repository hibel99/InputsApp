using InputsApp.DataAccess;
using InputsApp.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace InputsApp
{
    public partial class MainWindow : Window
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        private readonly IPivotPartsRepository _pivotPartsRepository;
        private readonly ISprinklerPartsRepository _sprinklerPartsRepository;



        public MainWindow()
        {
            InitializeComponent();
            _sqlDataAccess = new SqlDataAccess();
            _pivotPartsRepository = new PivotPartsRepository(_sqlDataAccess);
            _sprinklerPartsRepository = new SprinklerPartsRepository(_sqlDataAccess);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void pivotPartsGrid_Loaded(object sender, RoutedEventArgs e)
        {

            //var pivotPart = new PivotParts(6, "TestCat6FromEdit", "PivotTestName3FromEdit", 122, DateTime.Now, 20, 20, 20, 20);
            //var sprinklerPart = new SprinklerParts(1,"sprinklerPartCatFromEdit", "sprinklerTestName1FromEdit", 122, DateTime.Now, 25, 20, 20, 20);
            //await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);
            //await _pivotPartsRepository.DeletePivotPart(2);
            //await _pivotPartsRepository.AddPivotPart(pivotPart);
            //await _sprinklerPartsRepository.EditSprinklerPart(sprinklerPart);
            //await _pivotPartsRepository.EditPivotPart(pivotPart);
            var result = await _pivotPartsRepository.GetPivotParts();
            pivotPartsGrid.ItemsSource = result;
        }

     //private bool ValidateIfEmptyInput(params string[] inputs) => inputs.Any((input) => input.Length == 0);

        private async void AddPivot_Button_Click(object sender, RoutedEventArgs e)
        {


            var pivotPart = new PivotParts(
                PivotCategoryCB.Text,
                PivotPartTB.Text,
                decimal.Parse(pivotCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(pivotHegitTB.Text),
                decimal.Parse(pivotwidthTB.Text),
                decimal.Parse(pivotlenghtTB.Text),
                decimal.Parse(pivotWeightTB.Text));


            await _pivotPartsRepository.AddPivotPart(pivotPart);
            pivotPartsGrid_Loaded(null, null);
        }

        private async void AddSpinkler_Button_Click(object sender, RoutedEventArgs e)
        {
            var sprinklerPart = new SprinklerParts(
                SpinklersPartsCategory.Text,
                SpinklersPartsname.Text,
                decimal.Parse(SpinklersPartscost.Text),
                DateTime.UtcNow,
                decimal.Parse(SpinklersPartsHeight.Text),
                decimal.Parse(SpinklersPartsWidth.Text),
                decimal.Parse(SpinklersPartsLength.Text),
                decimal.Parse(SpinklersPartsWeight.Text)
                );

            await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);

        }



        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if the input text is numeric or a decimal point
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true; // Prevent non-numeric characters from being entered
                    break;
                }
            }

            // Ensure only one decimal point is allowed
            if (e.Text == "." && textBox.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

    }
}
