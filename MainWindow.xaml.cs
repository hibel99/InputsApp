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

        private readonly IPivotPartsRepository _pivotPartsRepository;
        private readonly ISprinklerPartsRepository _sprinklerPartsRepository;
        private readonly IPivotRepository _PivotRepository;
        private readonly ISpanRepository _spanRepository;
        private SprinklerParts sprinklerEdit = null;
        private PivotParts pivotEdit = null;


        public MainWindow()
        {
            InitializeComponent();
            _sqlDataAccess = new SqlDataAccess();
            _pivotPartsRepository = new PivotPartsRepository(_sqlDataAccess);
            _sprinklerPartsRepository = new SprinklerPartsRepository(_sqlDataAccess);
            _PivotRepository = new PivotRepository(_sqlDataAccess);
            _spanRepository = new SpanRepository(_sqlDataAccess);
            UpdateGridandCB();
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

                UpdateGridandCB();
            }
        }

        async private void UpdateGridandCB()
        {
            var PivotNames = await _PivotRepository.GetPivots();

            if ((bool)PivotParts.IsChecked)
            {
                var result = await _pivotPartsRepository.GetPivotParts();
                pivotPartsGrid.ItemsSource = result;
                PivotNameCB.ItemsSource = PivotNames.Select(x => x.pivotname);
            }
            if ((bool)SpinklersParts.IsChecked)
            {
                var result = await _sprinklerPartsRepository.GetSprinklerParts();
                SprinklerPartsDG.ItemsSource = result;
                PivotforSprinklerCB.ItemsSource = PivotNames.Select(x => x.pivotname);
            }
            if ((bool)Pivots.IsChecked)
            {
                PivotsDG.ItemsSource = PivotNames;
            }
            if((bool)SpanParts.IsChecked)
            {
                var result = await _spanRepository.GetSpans();
                SpansDG.ItemsSource = result;
            }
        }

        private async void AddPivot_Button_Click(object sender, RoutedEventArgs e)
        {
            if (pivotEdit != null)
            {
                await UpdateItemFromTextBoxes_Pivots(pivotEdit);
            }
            else
            {
                var selectedPivot = await _PivotRepository.GetPivots();
                var PivCat = selectedPivot.FirstOrDefault(x => x.pivotname == PivotNameCB.Text);

                var pivotPart = new PivotParts(
                PivotCategoryCB.Text,
                PivotPartTB.Text,
                decimal.Parse(pivotCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(pivotHegitTB.Text),
                decimal.Parse(pivotwidthTB.Text),
                decimal.Parse(pivotlenghtTB.Text),
                decimal.Parse(pivotWeightTB.Text),
                PivCat.ID );

                await _pivotPartsRepository.AddPivotPart(pivotPart);
                UpdateGridandCB();
            }

            ClearTextBoxes();
        }

        private async void AddSpinkler_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sprinklerEdit != null)
            {
                UpdateItemFromTextBoxes_Sprinklers(sprinklerEdit);
            }
            else
            {
                var selectedPivot = await _PivotRepository.GetPivots();
                var Pivotid = selectedPivot.FirstOrDefault(x => x.pivotname == PivotforSprinklerCB.Text);

                var sprinklerPart = new SprinklerParts(
                SprinklerCategoryCB.Text,
                SprinklerPartTB.Text,
                decimal.Parse(SprinklerCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(SprinklerHeightTB.Text),
                decimal.Parse(SprinklerwidthTB.Text),
                decimal.Parse(SprinklerlengthTB.Text),
                decimal.Parse(SprinklerWeightTB.Text),
                Pivotid.ID
                );

                await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);
                UpdateGridandCB();
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
                SprinklerParts part = (SprinklerParts)((Button)sender).DataContext;

                await _sprinklerPartsRepository.DeleteSprinklerPart(part.ID);

                UpdateGridandCB();
            }
        }

        private void Pivot_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridandCB();
        }

        private void SpinklersParts_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridandCB();
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
            SprinklerCategoryCB.Text = sprinklerEdit.SprinklerCategory;
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
            var selectedPivot = await _pivotPartsRepository.GetPivotParts();
            var Pivotid = selectedPivot.FirstOrDefault(x => x.PivotCategory == PivotforSprinklerCB.Text);

            var sprinklerPart = new SprinklerParts(
                sprinklerEdit.ID,
                SprinklerCategoryCB.Text,
                SprinklerPartTB.Text,
                decimal.Parse(SprinklerCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(SprinklerHeightTB.Text),
                decimal.Parse(SprinklerwidthTB.Text),
                decimal.Parse(SprinklerlengthTB.Text),
                decimal.Parse(SprinklerWeightTB.Text), Pivotid.ID
                );

            await _sprinklerPartsRepository.EditSprinklerPart( sprinklerPart );
            UpdateGridandCB();
        }

        private void ClearTextBoxes()
        {
            if ((bool)SpinklersParts.IsChecked)
            {
                SprinklerCategoryCB.Text = string.Empty;
                SprinklerPartTB.Text = string.Empty;
                SprinklerCostTB.Text = string.Empty;
                SprinklerDatePicker.SelectedDate = DateTime.Now;
                SprinklerHeightTB.Text = string.Empty;
                SprinklerwidthTB.Text = string.Empty;
                SprinklerlengthTB.Text = string.Empty;
                SprinklerWeightTB.Text = string.Empty;
            }
            if ((bool)PivotParts.IsChecked)
            {
                PivotCategoryCB.Text = string.Empty;
                PivotPartTB.Text = string.Empty;
                pivotCostTB.Text = string.Empty;
                pivotDatePicker.SelectedDate = DateTime.Now;
                pivotHegitTB.Text = string.Empty;
                pivotwidthTB.Text = string.Empty;
                pivotlenghtTB.Text = string.Empty;
                pivotWeightTB.Text = string.Empty;
            }
            if((bool)Pivots.IsChecked)
            {
                PivotCategory.Text = string.Empty;
                PivotName.Text  = string.Empty;
            }
            if((bool)SpanParts.IsChecked)
            {
                LengthTB.Text = string.Empty;
                DiameterTB.Text = string.Empty;
                SpanCategoryCB.Text = string.Empty;
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
            PivotCategoryCB.Text = pivotEdit.PivotCategory;
            PivotPartTB.Text = pivotEdit.PivotPart;
            pivotCostTB.Text = pivotEdit.Cost.ToString();
            pivotDatePicker.SelectedDate = pivotEdit.Date;
            pivotHegitTB.Text = pivotEdit.Height.ToString();
            pivotwidthTB.Text = pivotEdit.Width.ToString();
            pivotlenghtTB.Text = pivotEdit.Length.ToString();
            pivotWeightTB.Text = pivotEdit.Weight.ToString();
        }

        async private Task UpdateItemFromTextBoxes_Pivots(PivotParts pivotEdit)
        {


            var selectedPivot = await _PivotRepository.GetPivots();
            var PivCat = selectedPivot.FirstOrDefault(x => x.pivotname == PivotNameCB.Text);
            var pivotPart = new PivotParts(
                       pivotEdit.ID,
                       PivotCategoryCB.Text,
                       PivotPartTB.Text,
                       decimal.Parse(pivotCostTB.Text),
                       DateTime.UtcNow,
                       decimal.Parse(pivotHegitTB.Text),
                       decimal.Parse(pivotwidthTB.Text),
                       decimal.Parse(pivotlenghtTB.Text),
                       decimal.Parse(pivotWeightTB.Text),
                       (PivCat is not null ? PivCat.ID : -1)
                       );
        

               await _pivotPartsRepository.EditPivotPart(pivotPart);
            UpdateGridandCB();
        }

        private void PivotParts_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridandCB();
        }

        async private void AddNewPivotBT_Click(object sender, RoutedEventArgs e)
        {
            var Pivot = new PivotTable(
                PivotName.Text,
                PivotCategory.Text);

            await _PivotRepository.AddPivot(Pivot);
            UpdateGridandCB();
            ClearTextBoxes();
        }

        async private void PivotNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPivot = await _PivotRepository.GetPivots();
            var PivCat = selectedPivot.FirstOrDefault(x => x.pivotname == PivotNameCB.Text);

            PivotCategoryCB.Text = PivCat.pivotcategory;
        }

        async private void DeleteButtoninPivotDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                PivotTable part = (PivotTable)((Button)sender).DataContext;

                await _PivotRepository.DeletePivot(part.ID);

                UpdateGridandCB();
            }
            ClearTextBoxes();
        }

        private void SpanParts_Click(object sender, RoutedEventArgs e)
        {
            UpdateGridandCB();
        }

        async private void DeleteButtoninSpansDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Spans part = (Spans)((Button)sender).DataContext;

                await _spanRepository.DeleteSpan(part.ID);

                UpdateGridandCB();
            }
            ClearTextBoxes();
        }

        async private void AddSpanBT_Click(object sender, RoutedEventArgs e)
        {
            var Span = new Spans(
                decimal.Parse(LengthTB.Text),
                decimal.Parse(DiameterTB.Text),
                SpanCategoryCB.Text);

            await _spanRepository.AddSpan(Span);
            UpdateGridandCB();
            ClearTextBoxes();
        }
    }
}
