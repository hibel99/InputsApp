using InputsApp.DataAccess;
using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
        private readonly ICatergoryRepository _catergoryRepository;
        private SprinklerParts sprinklerEdit = null;
        private SpareParts pivotEdit = null;
        ObservableCollection<Categories> CategoriesListOBS = new ObservableCollection<Categories>();
        ObservableCollection<Categories> SectionsListOBS = new ObservableCollection<Categories>();
        ObservableCollection<PivotTable> PivotsOBS = new ObservableCollection<PivotTable>();
        ObservableCollection<Spans> SpansOBS = new ObservableCollection<Spans>();
        ObservableCollection<SpareParts> SparePartsOBS = new ObservableCollection<SpareParts>();
        ObservableCollection<SprinklerParts> SprinklerPartsOBS = new ObservableCollection<SprinklerParts>();
        ObservableCollection<PivotTable> PivotParentOBS = new ObservableCollection<PivotTable>();
        ObservableCollection<PivotTable> PivotSpanParentOBS = new ObservableCollection<PivotTable>();
        ObservableCollection<Spans> SpanParentOBS = new ObservableCollection<Spans>();
        ObservableCollection<SpareParts> SpareParentOBS = new ObservableCollection<SpareParts>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            _sqlDataAccess = new SqlDataAccess();
            _pivotPartsRepository = new PivotPartsRepository(_sqlDataAccess);
            _sprinklerPartsRepository = new SprinklerPartsRepository(_sqlDataAccess);
            _PivotRepository = new PivotRepository(_sqlDataAccess);
            _spanRepository = new SpanRepository(_sqlDataAccess);
            _catergoryRepository = new CatergoryRepository(_sqlDataAccess);
            GetDataOBS();
        }

        private void ArrangeConnections()
        {
            foreach (var item in SpansOBS)
            {
                if (item.PivotID is not null && item.PivotID != "")
                {
                    List<int> pivotsIDs = item.PivotID.Split(",").Select(int.Parse).ToList();
                    item.ParentPivots = PivotsOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
                }
            }

            foreach (var item in SparePartsOBS)
            {
                if (item.pivotcode is not null && item.pivotcode != "")
                {
                    List<int> pivotsIDs = item.pivotcode.Split(",").Select(int.Parse).ToList();
                    item.ParentPivots = PivotsOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
                }

                if (item.SpanID is not null && item.SpanID != "")
                {
                    List<int> pivotsIDs = item.SpanID.Split(",").Select(int.Parse).ToList();
                    item.ParentSpans = SpansOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
                }

                if (item.SpareID is not null && item.SpareID != "")
                {
                    List<int> pivotsIDs = item.SpareID.Split(",").Select(int.Parse).ToList();
                    item.ParentSpares = SparePartsOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private async void GetDataOBS()
        {
          
           var Parts = await _pivotPartsRepository.GetPivotParts();
           foreach (var item in Parts)
           {
                SparePartsOBS.Add(item);
           }
           pivotPartsGrid.ItemsSource = SparePartsOBS;
           PartNameCB.ItemsSource = SparePartsOBS;

            var pivots = await _PivotRepository.GetPivots();
            foreach (var piv in pivots)
            {
                PivotsOBS.Add(piv);
            }
            PivotNameCB.ItemsSource = PivotsOBS;
            PivotsDG.ItemsSource = PivotsOBS;
            PivotTypeCB.ItemsSource = PivotsOBS;


            var overhang = Parts.Where(x => x.PivotCategory == "Overhang");
            OverhangDG.ItemsSource = overhang;



            var sprinklerParts = await _sprinklerPartsRepository.GetSprinklerParts();
                foreach (var sps in sprinklerParts)
                {
                    SprinklerPartsOBS.Add(sps);
                }
                    SprinklerPartsDG.ItemsSource = SprinklerPartsOBS;
            
           


            
            
                var result = await _spanRepository.GetSpans();
                foreach (var item in result)
                {
                    SpansOBS.Add(item);
                }
                SpansDG.ItemsSource = SpansOBS;
                SpanNameCB.ItemsSource = SpansOBS;
            

           
               



            #region Categories
            List<Categories> CategoriesList = await _catergoryRepository.GetCategories();



            foreach (var item in CategoriesList.Where(x => x.Type == "Category"))
            {
                CategoriesListOBS.Add(item);
            }
            CategoriesDG.ItemsSource = CategoriesListOBS;
            PivotCategoryCB.ItemsSource = CategoriesListOBS;


            foreach (var item in CategoriesList.Where(x => x.Type == "Section"))
            {
                SectionsListOBS.Add(item);
            }
            SectionsDG.ItemsSource = SectionsListOBS;
            PivotSectionCB.ItemsSource = SectionsListOBS;
            #endregion



            #region parents stuff
            NewPivotConnectionsGrid.ItemsSource = PivotParentOBS;
            NewSpanConnectionsGrid.ItemsSource = SpanParentOBS;
            NewPartConnectionsGrid.ItemsSource = SpareParentOBS;
            NewPivotForSpansConnectionsGrid.ItemsSource = PivotSpanParentOBS;
            #endregion

            ArrangeConnections();


        }
        async private void DeleteButtoninDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SpareParts part = (SpareParts)((Button)sender).DataContext;

                await _pivotPartsRepository.DeletePivotPart(part.ID);

                //UpdateGridandCB();
            }
        }

        async private void UpdateGridandCB()
        {
            

            if ((bool)PivotParts.IsChecked)
            {
                var Parts = await _pivotPartsRepository.GetPivotParts();

                if (SparePartsOBS.Count == 0)
                {
                    foreach (var item in Parts)
                    {
                        SparePartsOBS.Add(item);
                    } 
                }

                pivotPartsGrid.ItemsSource = Parts;
                PivotNameCB.ItemsSource = PivotsOBS;


            }
            if ((bool)SpinklersParts.IsChecked)
            {
                var result = await _sprinklerPartsRepository.GetSprinklerParts();
                SprinklerPartsDG.ItemsSource = result;
            }
            if ((bool)Pivots.IsChecked)
            {
                

                PivotsDG.ItemsSource = PivotsOBS;
                PivotNameCB.ItemsSource = PivotsOBS;
            }
            if((bool)SpanParts.IsChecked)
            {
                var result = await _spanRepository.GetSpans();
                foreach (var item in result)
                {
                    SpansOBS.Add(item);
                }
                SpansDG.ItemsSource = SpansOBS;
                SpanNameCB.ItemsSource = SpansOBS;
            }

            if ((bool)OverhangParts.IsChecked)
            {
                var result = await _pivotPartsRepository.GetPivotParts();
                var overhang = result.Where(x => x.PivotCategory == "Overhang");
                OverhangDG.ItemsSource = overhang;
            }

            if ((bool)CategoriesRD.IsChecked)
            {
                List<Categories> CategoriesList = await _catergoryRepository.GetCategories();

                if (CategoriesListOBS.Count == 0)
                {

                    foreach (var item in CategoriesList.Where(x => x.Type == "Category"))
                    {
                        CategoriesListOBS.Add(item);
                    }
                    CategoriesDG.ItemsSource = CategoriesListOBS;
                    PivotCategoryCB.ItemsSource = CategoriesListOBS;
                }
                if (SectionsListOBS.Count == 0)
                {
                    foreach (var item in CategoriesList.Where(x => x.Type == "Section"))
                    {
                        SectionsListOBS.Add(item);
                    }
                    SectionsDG.ItemsSource = SectionsListOBS;
                    PivotSectionCB.ItemsSource = SectionsListOBS;

                }


            }


        }

        private async void AddPivot_Button_Click(object sender, RoutedEventArgs e)
        {
            if (pivotEdit != null)
            {
                //await UpdateItemFromTextBoxes_Pivots(pivotEdit);
            }
            else
            {

                List<PivotTable> pivotParents = NewPivotConnectionsGrid.ItemsSource.Cast<PivotTable>().ToList();
                List<Spans> SpansParents = NewSpanConnectionsGrid.ItemsSource.Cast<Spans>().ToList();
                List<SpareParts> SparesParents = NewPartConnectionsGrid.ItemsSource.Cast<SpareParts>().ToList();

                if (pivotParents is not null)
                {
                    var pivotPart = new SpareParts(
                    PivotCategoryCB.Text,
                    PivotPartTB.Text,
                    decimal.Parse(pivotCostTB.Text),
                    DateTime.UtcNow,
                    decimal.Parse(pivotHegitTB.Text),
                    decimal.Parse(pivotwidthTB.Text),
                    decimal.Parse(pivotlenghtTB.Text),
                    decimal.Parse(pivotWeightTB.Text),
                    string.Join(",",pivotParents.Select(x=>x.ID).ToList()),
                    2,
                    "",
                    "",
                    double.Parse(pivotQTYTB.Text),
                    "",
                    PivotPartARTB.Text,
                    PivotSectionCB.Text
                    );
                    pivotPart.ParentPivots = pivotParents;

                    await _pivotPartsRepository.AddPivotPart(pivotPart);
                    SparePartsOBS.Add(pivotPart);
                }


                if (SpansParents.Count > 0)
                {
                    var pivotPart = new SpareParts(
                   PivotCategoryCB.Text,
                   PivotPartTB.Text,
                   decimal.Parse(pivotCostTB.Text),
                   DateTime.UtcNow,
                   decimal.Parse(pivotHegitTB.Text),
                   decimal.Parse(pivotwidthTB.Text),
                   decimal.Parse(pivotlenghtTB.Text),
                   decimal.Parse(pivotWeightTB.Text),
                   "",
                   3,
                   "",
                   "",
                   double.Parse(pivotQTYTB.Text),
                   string.Join(",", SpansParents.Select(x => x.ID).ToList()),
                   PivotPartARTB.Text,
                   PivotSectionCB.Text
                   );
                    pivotPart.ParentSpans = SpansParents;

                    await _pivotPartsRepository.AddPivotPart(pivotPart);
                    SparePartsOBS.Add(pivotPart);
                }


                if (SparesParents.Count > 0)
                {
                    var groupedLists = SparesParents.GroupBy(item => item.PartLevel);

                    // Iterate over the grouped lists and print the items in each group
                    foreach (var group in groupedLists)
                    {
                        int level = group.Key;
                        List<SpareParts> itemsAtLevel = group.ToList();

                        var pivotPart = new SpareParts(
                         PivotCategoryCB.Text,
                         PivotPartTB.Text,
                         decimal.Parse(pivotCostTB.Text),
                         DateTime.UtcNow,
                         decimal.Parse(pivotHegitTB.Text),
                         decimal.Parse(pivotwidthTB.Text),
                         decimal.Parse(pivotlenghtTB.Text),
                         decimal.Parse(pivotWeightTB.Text),
                         "",
                         level+1,
                         "",
                         string.Join(",", itemsAtLevel.Select(x => x.ID).ToList()),
                         double.Parse(pivotQTYTB.Text),
                         "",
                         PivotPartARTB.Text,
                         PivotSectionCB.Text
                         );
                        pivotPart.ParentSpares = itemsAtLevel;
                        await _pivotPartsRepository.AddPivotPart(pivotPart);
                        SparePartsOBS.Add(pivotPart);
                    }
                }




                //await _pivotPartsRepository.AddPivotPart(pivotPart);
                //UpdateGridandCB();
            }

            ClearTextBoxes();
        }

        private async void AddSpinkler_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sprinklerEdit != null)
            {
                await UpdateItemFromTextBoxes_Sprinklers(sprinklerEdit);
            }
            else
            {
                var selectedPivot = await _PivotRepository.GetPivots();
                //var Pivotid = selectedPivot.FirstOrDefault(x => x.pivotname == PivotforSprinklerCB.Text);

                var sprinklerPart = new SprinklerParts(
                SprinklerCategoryCB.Text,
                SprinklerPartTB.Text,
                decimal.Parse(SprinklerCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(SprinklerHeightTB.Text),
                decimal.Parse(SprinklerwidthTB.Text),
                decimal.Parse(SprinklerlengthTB.Text),
                decimal.Parse(SprinklerWeightTB.Text)
                );

                await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);
                //UpdateGridandCB();
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

        private void Empty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if the input text is numeric or a decimal point
            //if (textBox.Text == "")
            //{
            //    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(199, 45, 50)); 
            //}
            //else
            //{
            //    textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(253, 130, 7));

            //}
        }


        async private void DeleteButtoninSPrinklerDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SprinklerParts part = (SprinklerParts)((Button)sender).DataContext;

                await _sprinklerPartsRepository.DeleteSprinklerPart(part.ID);

                //UpdateGridandCB();
            }
        }

        private void Pivot_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGridandCB();
        }

        private void SpinklersParts_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGridandCB();
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
            SprinklerHeightTB.Text = sprinklerEdit.Height.ToString();
            SprinklerwidthTB.Text = sprinklerEdit.Width.ToString(); 
            SprinklerlengthTB.Text = sprinklerEdit.Length.ToString();
            SprinklerWeightTB.Text = sprinklerEdit.Weight.ToString();
        }

        async private Task UpdateItemFromTextBoxes_Sprinklers(SprinklerParts sprinklerEdit)
        {
            var selectedPivot = await _pivotPartsRepository.GetPivotParts();
            //var Pivotid = selectedPivot.FirstOrDefault(x => x.PivotCategory == PivotforSprinklerCB.Text);

            var sprinklerPart = new SprinklerParts(
                sprinklerEdit.ID,
                SprinklerCategoryCB.Text,
                SprinklerPartTB.Text,
                decimal.Parse(SprinklerCostTB.Text),
                DateTime.UtcNow,
                decimal.Parse(SprinklerHeightTB.Text),
                decimal.Parse(SprinklerwidthTB.Text),
                decimal.Parse(SprinklerlengthTB.Text),
                decimal.Parse(SprinklerWeightTB.Text)
                );

            await _sprinklerPartsRepository.EditSprinklerPart( sprinklerPart );
            //UpdateGridandCB();
        }

        private void ClearTextBoxes()
        {
            if ((bool)SpinklersParts.IsChecked)
            {
                SprinklerCategoryCB.Text = string.Empty;
                SprinklerPartTB.Text = string.Empty;
                SprinklerCostTB.Text = string.Empty;
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
                pivotHegitTB.Text = string.Empty;
                pivotwidthTB.Text = string.Empty;
                pivotlenghtTB.Text = string.Empty;
                pivotWeightTB.Text = string.Empty;
            }
            if((bool)Pivots.IsChecked)
            {
                PivotCategory.Text = string.Empty;
                PivotName.Text  = string.Empty;
                PivotLength.Text = string.Empty;
            }
            if((bool)SpanParts.IsChecked)
            {
                LengthTB.Text = string.Empty;
                DiameterTB.Text = string.Empty;
                SpanCostTB.Text = string.Empty;
                //PipeTypeCBCB.SelectedIndex = 0;
                SpanNameTB.Text = string.Empty; 
            }
            if ((bool)OverhangParts.IsChecked)
            {
                OHLengthTB.Text = string.Empty;
                OHDiameterTB.Text = string.Empty;
                OHCostTB.Text = string.Empty;
                OHNameTB.Text = string.Empty;
                OHTypeCBCB.SelectedIndex = 0;

            }
            sprinklerEdit = null;
            pivotEdit = null;
            
        }

        private void pivotPartsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivotPartsGrid.SelectedItem is SpareParts selectedItem)
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

        private void UpdateTextBoxesFromItem_Pivots(SpareParts pivotEdit)
        {
            PivotCategoryCB.Text = pivotEdit.PivotCategory;
            PivotPartTB.Text = pivotEdit.PivotPart;
            pivotCostTB.Text = pivotEdit.Cost.ToString();
            pivotHegitTB.Text = pivotEdit.Height.ToString();
            pivotwidthTB.Text = pivotEdit.Width.ToString();
            pivotlenghtTB.Text = pivotEdit.Length.ToString();
            pivotWeightTB.Text = pivotEdit.Weight.ToString();

            PivotCategoryCB.Text = pivotEdit.PivotCategory;
            pivotWeightTB.Text = pivotEdit.Section;

            SpareParentOBS.Clear();
            SpanParentOBS.Clear();
            PivotParentOBS.Clear();

            if (pivotEdit.ParentSpans is not null)
            {
                foreach (var item in pivotEdit.ParentSpans)
                {
                    SpanParentOBS.Add(item);
                }
            }

            if (pivotEdit.ParentSpares is not null)
            {
                foreach (var item2 in pivotEdit.ParentSpares)
                {
                    SpareParentOBS.Add(item2);
                }
            }

            if (pivotEdit.ParentPivots is not null)
            {
                foreach (var item3 in pivotEdit.ParentPivots)
                {
                    PivotParentOBS.Add(item3);
                }
            }



        }

        async private Task UpdateItemFromTextBoxes_Pivots(SpareParts pivotEdit)
        {


            //var selectedPivot = await _PivotRepository.GetPivots();
            //var PivCat = selectedPivot.FirstOrDefault(x => x.pivotname == PivotNameCB.Text);
            //var pivotPart = new PivotParts(
            //           pivotEdit.ID,
            //           PivotCategoryCB.Text,
            //           PivotPartTB.Text,
            //           decimal.Parse(pivotCostTB.Text),
            //           DateTime.UtcNow,
            //           decimal.Parse(pivotHegitTB.Text),
            //           decimal.Parse(pivotwidthTB.Text),
            //           decimal.Parse(pivotlenghtTB.Text),
            //           decimal.Parse(pivotWeightTB.Text),
            //           (PivCat is not null ? PivCat.ID : pivotEdit.pivotcode)
            //           );
        

            //   await _pivotPartsRepository.EditPivotPart(pivotPart);
            //UpdateGridandCB();
        }

        private void PivotParts_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGridandCB();
        }

        async private void AddNewPivotBT_Click(object sender, RoutedEventArgs e)
        {
            var Pivot = new PivotTable(
                PivotName.Text,
                PivotCategory.Text,
                decimal.Parse(PivotLength.Text));

            await _PivotRepository.AddPivot(Pivot);
            //UpdateGridandCB();
            ClearTextBoxes();
        }

        async private void PivotNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selectedPivot = await _PivotRepository.GetPivots();
            //var PivCat = selectedPivot.FirstOrDefault(x => x.pivotname == PivotNameCB.Text);

            //PivotCategoryCB.Text = PivCat.pivotcategory;
        }

        async private void DeleteButtoninPivotDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                PivotTable part = (PivotTable)((Button)sender).DataContext;

                await _PivotRepository.DeletePivot(part.ID);

                //UpdateGridandCB();
            }
            ClearTextBoxes();
        }

        private void SpanParts_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGridandCB();
        }

        async private void DeleteButtoninSpansDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Spans part = (Spans)((Button)sender).DataContext;

                await _spanRepository.DeleteSpan(part.ID);

                //UpdateGridandCB();
            }
            ClearTextBoxes();
        }

        async private void AddSpanBT_Click(object sender, RoutedEventArgs e)
        {
            

            var Span = new Spans
                (
                decimal.Parse(LengthTB.Text),
                decimal.Parse(DiameterTB.Text),
                "Span",
                SpanNameTB.Text,
                decimal.Parse(SpanCostTB.Text),
                string.Join(",", PivotSpanParentOBS.Select(x=>x.ID).ToList())
                );

            await _spanRepository.AddSpan(Span);
            SpansOBS.Add(Span);
            //UpdateGridandCB();
            ClearTextBoxes();
        }

        private void Overhang_Click(object sender, RoutedEventArgs e)
        {
            //UpdateGridandCB();
        }

        private async void AddOverhangBT_Click_1(object sender, RoutedEventArgs e)
        {
            //var Overhang = new Spans(
            //   decimal.Parse(OHLengthTB.Text),
            //   decimal.Parse(OHDiameterTB.Text),
            //   "Overhang",
            //   OHNameTB.Text,
            //   decimal.Parse(OHCostTB.Text),
            //   "", OHTypeCBCB.Text
            //   );

            //await _spanRepository.AddSpan(Overhang);
            ////UpdateGridandCB();
            //ClearTextBoxes();
        }

        private async void DeleteButtoninOHDG_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Spans part = (Spans)((Button)sender).DataContext;

                await _spanRepository.DeleteSpan(part.ID);

                //UpdateGridandCB();
            }
            ClearTextBoxes();
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteSection_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void AddNewCategory_Click(object sender, RoutedEventArgs e)
        {
            if (NameNewCategory.Text != "" && NameARNewCategory.Text != "")
            {
                Categories categories = new Categories("Category", NameNewCategory.Text, NameARNewCategory.Text);
                await _catergoryRepository.AddCategories(categories);
                CategoriesListOBS.Add(categories);

            }
        }

        private async void AddNewSection_Click(object sender, RoutedEventArgs e)
        {
            if (NameNewSection.Text != "" && NameARNewSection.Text != "")
            {
                Categories categories = new Categories("Section", NameNewSection.Text, NameARNewSection.Text);
                await _catergoryRepository.AddCategories(categories);
                SectionsListOBS.Add(categories);
            }
        }

        private void PivotPartRD_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SpanPartRD_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SparePartRD_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AddToParents_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)PivotPartRD.IsChecked)
            {
                if (PivotNameCB.SelectedItem is PivotTable pivot)
                {
                    if (!PivotParentOBS.Contains(pivot))
                    {
                        PivotParentOBS.Add(pivot);
                    }
                }
            }

            else if ((bool)SpanPartRD.IsChecked)
            {
                if (SpanNameCB.SelectedItem is Spans span)
                {
                    if (!SpanParentOBS.Contains(span))
                    {
                        SpanParentOBS.Add(span);
                    }
                }
            }

            else if ((bool)SparePartRD.IsChecked)
            {
                if (PartNameCB.SelectedItem is SpareParts spares)
                {
                    if (!SpareParentOBS.Contains(spares))
                    {
                        SpareParentOBS.Add(spares);
                    }
                }
            }
        }

       

        private void PivotCategoryCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (PivotCategoryCB.SelectedItem is Categories cate)
            //{
            //    .Where(x => x.PivotCategory == cate.Name).ToList();
            //}
        }

        private void deletePivotParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewPivotConnectionsGrid.SelectedItem is PivotTable piv)
            {
                PivotParentOBS.Remove(piv);
            }
        }

        private void deleteSpanParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewSpanConnectionsGrid.SelectedItem is Spans spa)
            {
                SpanParentOBS.Remove(spa);
            }
        }

        private void deleteSpareParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewPartConnectionsGrid.SelectedItem is SpareParts spp)
            {
                SpareParentOBS.Remove(spp);
            }
        }

        private void deletePivotForSpanParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewPivotForSpansConnectionsGrid.SelectedItem is PivotTable pivot)
            {
                PivotSpanParentOBS.Remove(pivot);
            }
        }

        private void AddPivotParentToSpans_Click(object sender, RoutedEventArgs e)
        {
            if (PivotTypeCB.SelectedItem is PivotTable pivot)
            {
                if (!PivotSpanParentOBS.Contains(pivot))
                {
                    PivotSpanParentOBS.Add(pivot);
                }
            }
        }
    }
}
