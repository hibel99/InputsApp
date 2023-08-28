using InputsApp.DataAccess;
using InputsApp.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InputsApp
{
    public partial class MainWindow : Window
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        private readonly IPivotRepository _pivotRepository;
        private readonly IPivotPartsRepository _pivotPartsRepository;
        private readonly ISprinklerPartsRepository _sprinklerPartsRepository;
        private readonly ISpanRepository _spanRepository;

        private SprinklerParts sprinklerEdit = null;
        private PivotParts pivotEdit = null;


        public MainWindow()
        {
            InitializeComponent();
            _sqlDataAccess = new SqlDataAccess();
            _pivotRepository = new PivotRepository(_sqlDataAccess);
            _pivotPartsRepository = new PivotPartsRepository(_sqlDataAccess);
            _sprinklerPartsRepository = new SprinklerPartsRepository(_sqlDataAccess);
            _spanRepository = new SpanRepository(_sqlDataAccess);
            UpdateGrid();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        async private void DeleteButtoninDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                PivotParts part = (PivotParts)((Button)sender).DataContext;

                await _pivotPartsRepository.DeletePivotPart(part.ID);

                UpdateGrid();
            }
        }

        async private void UpdateGrid()
        {

            if ((bool)Pivot.IsChecked)
            {
                var result = await _pivotPartsRepository.GetPivotParts();
                pivotPartsGrid.ItemsSource = result;
            }
            if ((bool)SpinklersParts.IsChecked)
            {
                var result = await _sprinklerPartsRepository.GetSprinklerParts();
                SprinklerPartsDG.ItemsSource = result;
            }
        }

            //PivotCategoryTB
            //PivotPartTB
            //pivotCostTB
            //pivotwidthTB
            //pivotlenghtTB
            //pivotWeightTB
            //AddPivot_Button
        private async void AddPivot_Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsAnyFieldEmpty(PivotCategoryTB, PivotPartTB, pivotCostTB,
                 pivotwidthTB, pivotlenghtTB, pivotWeightTB))
            {
                return;
            }

            if (pivotEdit != null)
            {
                UpdateItemFromTextBoxes_Pivots(pivotEdit);
            }
            else
            {
                var pivotPart = new PivotParts(
                PivotCategoryTB.Text,
                PivotPartTB.Text,
                decimal.Parse(pivotCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(pivotHegitTB.Text),
                decimal.Parse(pivotwidthTB.Text),
                decimal.Parse(pivotlenghtTB.Text),
                decimal.Parse(pivotWeightTB.Text));


                await _pivotPartsRepository.AddPivotPart(pivotPart);
                UpdateGrid();
            }

            ClearTextBoxes();
        }




        private bool IsAnyFieldEmpty(params TextBox[] fields)
        {
           var emptyFields = fields.Where((field) => (field.Text.Length == 0)).ToList();
            if(emptyFields.Count() > 0)
            {
                emptyFields.ForEach(field => field.BorderBrush = new SolidColorBrush(Colors.LightGreen));
                return true;
            }
            return false;
        }

        //      SprinklerCategoryTB
        //      SprinklerPartTB
        //      SprinklerCostTB
        //      SprinklerHeightTB
        //      SprinklerwidthTB
        //      SprinklerlengthTB
        //      SprinklerWeightTB
        //      AddSpinkler_Button

        private async void AddSpinkler_Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsAnyFieldEmpty(SprinklerCategoryTB, SprinklerPartTB, SprinklerCostTB, SprinklerHeightTB, 
                SprinklerwidthTB, SprinklerlengthTB, SprinklerWeightTB))
            {
                return;
            }


            if (sprinklerEdit != null)
            {
                UpdateItemFromTextBoxes_Sprinklers(sprinklerEdit);
            }
            else
            {
                var sprinklerPart = new SprinklerParts(
                SprinklerCategoryTB.Text,
                SprinklerPartTB.Text,
                decimal.Parse(SprinklerCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(SprinklerHeightTB.Text),
                decimal.Parse(SprinklerwidthTB.Text),
                decimal.Parse(SprinklerlengthTB.Text),
                decimal.Parse(SprinklerWeightTB.Text),
                8
                );

                await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);
                UpdateGrid();
            }

            ClearTextBoxes();
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

        async private void DeleteButtoninSPrinklerDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                PivotParts part = (PivotParts)((Button)sender).DataContext;

                await _sprinklerPartsRepository.DeleteSprinklerPart(part.ID);

                UpdateGrid();
            }
        }

        private void Pivot_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void SpinklersParts_Click(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        private void SprinklerPartsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SprinklerPartsDG.SelectedItem is SprinklerParts selectedItem)
            {
                sprinklerEdit = selectedItem;
                UpdateTextBoxesFromItem_Sprinklers(sprinklerEdit);
            }
            else
            {
                sprinklerEdit = null;
                ClearTextBoxes();
            }
        }
        private void UpdateTextBoxesFromItem_Sprinklers(SprinklerParts sprinklerEdit)
        {
            SprinklerCategoryTB.Text = sprinklerEdit.SprinklerCategory;
            SprinklerPartTB.Text = sprinklerEdit.SprinklerPart;
            SprinklerCostTB.Text = sprinklerEdit.Cost.ToString();
            SprinklerDatePicker.SelectedDate = sprinklerEdit.Date;
            SprinklerHeightTB.Text = sprinklerEdit.Height.ToString();
            SprinklerwidthTB.Text = sprinklerEdit.Width.ToString(); 
            SprinklerlengthTB.Text = sprinklerEdit.Length.ToString();
            SprinklerWeightTB.Text = sprinklerEdit.Weight.ToString();
        }

        async private void UpdateItemFromTextBoxes_Sprinklers(SprinklerParts sprinklerEdit)
        {
            var sprinklerPart = new SprinklerParts(
                sprinklerEdit.ID,
                SprinklerCategoryTB.Text,
                SprinklerPartTB.Text,
                decimal.Parse(SprinklerCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(SprinklerHeightTB.Text),
                decimal.Parse(SprinklerwidthTB.Text),
                decimal.Parse(SprinklerlengthTB.Text),
                decimal.Parse(SprinklerWeightTB.Text),7
                );

            await _sprinklerPartsRepository.EditSprinklerPart( sprinklerPart );
            UpdateGrid();
        }

        private void ClearTextBoxes()
        {
            if ((bool)SpinklersParts.IsChecked)
            {
                SprinklerCategoryTB.Text = string.Empty;
                SprinklerPartTB.Text = string.Empty;
                SprinklerCostTB.Text = string.Empty;
                SprinklerDatePicker.SelectedDate = DateTime.Now;
                SprinklerHeightTB.Text = string.Empty;
                SprinklerwidthTB.Text = string.Empty;
                SprinklerlengthTB.Text = string.Empty;
                SprinklerWeightTB.Text = string.Empty;
            }
            if ((bool)Pivot.IsChecked)
            {
                PivotCategoryTB.Text = string.Empty;
                PivotPartTB.Text = string.Empty;
                pivotCostTB.Text = string.Empty;
                pivotDatePicker.SelectedDate = DateTime.Now;
                pivotHegitTB.Text = string.Empty;
                pivotwidthTB.Text = string.Empty;
                pivotlenghtTB.Text = string.Empty;
                pivotWeightTB.Text = string.Empty;
            }

            sprinklerEdit = null;
            pivotEdit = null;
            
        }

        private void pivotPartsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivotPartsGrid.SelectedItem is PivotParts selectedItem)
            {
                pivotEdit = selectedItem;
                UpdateTextBoxesFromItem_Pivots(pivotEdit);
            }
            else
            {
                pivotEdit = null;
                ClearTextBoxes();
            }
        }

        private void UpdateTextBoxesFromItem_Pivots(PivotParts pivotEdit)
        {
            PivotCategoryTB.Text = pivotEdit.PivotCategory;
            PivotPartTB.Text = pivotEdit.PivotPart;
            pivotCostTB.Text = pivotEdit.Cost.ToString();
            pivotDatePicker.SelectedDate = pivotEdit.Date;
            pivotHegitTB.Text = pivotEdit.Height.ToString();
            pivotwidthTB.Text = pivotEdit.Width.ToString();
            pivotlenghtTB.Text = pivotEdit.Length.ToString();
            pivotWeightTB.Text = pivotEdit.Weight.ToString();
        }

        async private void UpdateItemFromTextBoxes_Pivots(PivotParts pivotEdit)
        {
            var pivotPart = new PivotParts(
                pivotEdit.ID,
                PivotCategoryTB.Text,
                PivotPartTB.Text,
                decimal.Parse(pivotCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(pivotHegitTB.Text),
                decimal.Parse(pivotwidthTB.Text),
                decimal.Parse(pivotlenghtTB.Text),
                decimal.Parse(pivotWeightTB.Text));

            await _pivotPartsRepository.EditPivotPart(pivotPart);
            UpdateGrid();
        }

        private void TextBlock_KeyDown_DefaultColor(object sender, KeyEventArgs e)
        {
            
            //Color defaultColor = (Color)ColorConverter.ConvertFromString("#434343");

            //((TextBox)sender).BorderBrush = new SolidColorBrush(defaultColor);
        }

        private void TextBox_GotFocus_DefaultColor(object sender, RoutedEventArgs e)
        {
            Color defaultColor = (Color)ColorConverter.ConvertFromString("#434343");

            ((TextBox)sender).BorderBrush = new SolidColorBrush(defaultColor);
        }


    }
}
