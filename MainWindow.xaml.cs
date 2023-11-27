using InputsApp.DataAccess;
using InputsApp.FunctionsLibrary;
using InputsApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
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
        private readonly ISetRepository _setRepository;

        private readonly ICatergoryRepository _catergoryRepository;
        private readonly IBrandRepository _brandRepository;
        private SprinklerParts sprinklerEdit = null;
        private SpareParts pivotEdit = null;
        ObservableCollection<Categories> CategoriesListOBS = new ObservableCollection<Categories>();
        ObservableCollection<Categories> SectionsListOBS = new ObservableCollection<Categories>();
        ObservableCollection<PivotTable> PivotsOBS = new ObservableCollection<PivotTable>();
        ObservableCollection<Spans> SpansOBS = new ObservableCollection<Spans>();

        ObservableCollection<SpareParts> SparePartsOBS = new ObservableCollection<SpareParts>();
        ObservableCollection<SpareParts> JoinedSparePartsOBS = new ObservableCollection<SpareParts>();
        ObservableCollection<SpareParts> FilteredSparePartsOBS = new ObservableCollection<SpareParts>();
        ObservableCollection<SprinklerParts> SprinklerPartsOBS = new ObservableCollection<SprinklerParts>();
        ObservableCollection<SpareRelationship> RelationstOBS = new ObservableCollection<SpareRelationship>();


        ObservableCollection<SpareRelationship> PivotParentOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SpanParentOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SpareParentOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SetParentOBS = new ObservableCollection<SpareRelationship>();



        ObservableCollection<SpareRelationship> PivotParentRelationsOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SpanParentRelationsOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SpareParentRelationsOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SetParentRelationsOBS = new ObservableCollection<SpareRelationship>();


        ObservableCollection<PivotTable> PivotSpanParentOBS = new ObservableCollection<PivotTable>();

        ObservableCollection<Brands> BrandsOBS = new ObservableCollection<Brands>();
        ObservableCollection<Set> SetOBS = new ObservableCollection<Set>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            _sqlDataAccess = new SqlDataAccess();
            _pivotPartsRepository = new PivotPartsRepository(_sqlDataAccess);
            _sprinklerPartsRepository = new SprinklerPartsRepository(_sqlDataAccess);
            _PivotRepository = new PivotRepository(_sqlDataAccess);
            _spanRepository = new SpanRepository(_sqlDataAccess);
            _setRepository = new SetRepository(_sqlDataAccess);
            _catergoryRepository = new CatergoryRepository(_sqlDataAccess);
            _brandRepository = new BrandRepository(_sqlDataAccess);
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
                List<SpareRelationship> relations = RelationstOBS.Where(x => x.PivotPartID == item.ID).ToList();

                if (relations.Count > 0)
                {
                    item.ParentPivots = relations.Where(x => x.ParentType == "Pivot").ToList();

                    item.ParentSpans = relations.Where(x => x.ParentType == "Span").ToList();

                    item.ParentSpares = relations.Where(x => x.ParentType == "Spare").ToList();

                    item.ParentSets = relations.Where(x => x.ParentType == "Set").ToList();
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

            #region pivot parts
            var PartsR = await _pivotPartsRepository.GetPivotPartsRelationsJoined();
            var PartQty = PartsR.DistinctBy(p => p.PivotPartID).ToList();

            var Parts = await _pivotPartsRepository.GetPivotParts();

     
            int index = 0;
            foreach (var item in Parts)
            {
                
                item.Quantity = index < PartQty.Count ? PartQty[index++].Quantity : 0;
                SparePartsOBS.Add(item);
            }
            foreach (var item in PartsR)
            {
                JoinedSparePartsOBS.Add(item);
            }

            ALLpivotPartsGrid.ItemsSource = SparePartsOBS;
            pivotPartsGrid.ItemsSource = SparePartsOBS;
            PartNameCB.ItemsSource = SparePartsOBS;
            pivotPartsRelationsGrid.ItemsSource = SparePartsOBS;

            var relations = await _pivotPartsRepository.GetPivotPartsRelations();
      
            foreach (var rel in relations)
            {
                RelationstOBS.Add(rel);
            }
            #endregion

            #region pivots
            var pivots = await _PivotRepository.GetPivots();
            foreach (var piv in pivots)
            {
                PivotsOBS.Add(piv);
            }
            PivotNameCB.ItemsSource = PivotsOBS;
            PivotsDG.ItemsSource = PivotsOBS;
            PivotTypeCB.ItemsSource = PivotsOBS;
            #endregion

            #region To delete

            var overhang = Parts.Where(x => x.PivotCategory == "Overhang");
            OverhangDG.ItemsSource = overhang;



            var sprinklerParts = await _sprinklerPartsRepository.GetSprinklerParts();
            foreach (var sps in sprinklerParts)
            {
                SprinklerPartsOBS.Add(sps);
            }
            SprinklerPartsDG.ItemsSource = SprinklerPartsOBS;
            #endregion

            #region Spans

            var result = await _spanRepository.GetSpans();
            foreach (var item in result)
            {
                SpansOBS.Add(item);
            }
            SpansDG.ItemsSource = SpansOBS;
            SpanNameCB.ItemsSource = SpansOBS;


            #endregion

            #region Set

            var sets = await _setRepository.GetSets();
            foreach (var item in sets)
            {
                SetOBS.Add(item);
            }
            SetDG.ItemsSource = SetOBS;
            SetNameCB.ItemsSource = SetOBS;


            #endregion

            #region Brands

            var brands = await _brandRepository.GetBrands();
            foreach (var item in brands)
            {
                BrandsOBS.Add(item);
            }
            BrandsDG.ItemsSource = BrandsOBS;
            BrandsFilterIC.ItemsSource = BrandsOBS.Distinct();
            ListCollectionView lcv = new ListCollectionView(BrandsOBS);
            lcv.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            PivotBrandCB.ItemsSource = lcv; 
            #endregion

            #region Categories
            List<Categories> CategoriesList = await _catergoryRepository.GetCategories();



            foreach (var item in CategoriesList.Where(x => x.Type == "Category"))
            {
                CategoriesListOBS.Add(item);
            }
            CategoriesDG.ItemsSource = CategoriesListOBS;
            PivotCategoryCB.ItemsSource = CategoriesListOBS;
            CategoryNewBrand.ItemsSource = CategoriesListOBS;
            CategoriesFilterIC.ItemsSource = CategoriesListOBS;
            PivotPartCategoriesFilterIC.ItemsSource = CategoriesListOBS;


            foreach (var item in CategoriesList.Where(x => x.Type == "Section"))
            {
                SectionsListOBS.Add(item);
            }
            SectionsDG.ItemsSource = SectionsListOBS;
            PivotSectionCB.ItemsSource = SectionsListOBS;
            SectionsFilterIC.ItemsSource = SectionsListOBS;
            #endregion

            //hk1
            #region parents stuff
            NewPivotConnectionsGrid.ItemsSource = PivotParentOBS;
            NewSpanConnectionsGrid.ItemsSource = SpanParentOBS;
            NewPartConnectionsGrid.ItemsSource = SpareParentOBS;
            NewSetPartConnectionsGrid.ItemsSource = SetParentOBS;


            NewPivotRelationsConnectionsGrid.ItemsSource = PivotParentRelationsOBS;
            NewSpanRelationsConnectionsGrid.ItemsSource = SpanParentRelationsOBS;
            NewPartRelationsConnectionsGrid.ItemsSource = SpareParentRelationsOBS;
            NewSetPartRelationsConnectionsGrid.ItemsSource = SetParentRelationsOBS;



            //NewPivotForSpansConnectionsGrid.ItemsSource = PivotSpanParentOBS;
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

                SparePartsOBS.Remove(part);
                JoinedSparePartsOBS.Remove(part);

                PivotParentOBS.Remove(PivotParentOBS.Where((item) => item.PivotPartID == part.ID).FirstOrDefault());
                SpanParentOBS.Remove(SpanParentOBS.Where((item) => item.PivotPartID == part.ID).FirstOrDefault());
                SpareParentOBS.Remove(SpareParentOBS.Where((item) => item.PivotPartID == part.ID).FirstOrDefault());
                SetParentOBS.Remove( SetParentOBS.Where((item) => item.PivotPartID == part.ID).FirstOrDefault());

                EditPivot_Button.Visibility = Visibility.Collapsed;
                AddPivot_Button.Visibility = Visibility.Visible;
                

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
            //if ((bool)SpinklersParts.IsChecked)
            //{
            //    var result = await _sprinklerPartsRepository.GetSprinklerParts();
            //    SprinklerPartsDG.ItemsSource = result;
            //}
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

            //if ((bool)OverhangParts.IsChecked)
            //{
            //    var result = await _pivotPartsRepository.GetPivotParts();
            //    var overhang = result.Where(x => x.PivotCategory == "Overhang");
            //    OverhangDG.ItemsSource = overhang;
            //}

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
            
            List<SpareParts> sparesToChild = new List<SpareParts>();

            string Brand = "";
            if (PivotBrandCB.SelectedItem is Brands selectedBrand)
            {
                Brand = selectedBrand.Brand;
            }

            if (IsAnyFieldEmpty(PivotCategoryCB.Text, PivotPartTB.Text, pivotCostTB.Text, pivotHegitTB.Text, pivotwidthTB.Text, pivotlenghtTB.Text,
                pivotWeightTB.Text, PivotPartARTB.Text, PivotSectionCB.Text, Brand
                ,pivotHegitUnitCB.Text,pivotWidthUnitCB.Text,pivotLengthUnitCB.Text,pivotWeightUnitCB.Text))
            {
                MessageBox.Show("Fill in all required fields.","Missing Information",MessageBoxButton.OK);
               
                return;
            }
        
            double QTYInSet = 0;
            double QTY = 0;
            if (!string.IsNullOrEmpty(pivotQTYInSetTB.Text))
            {
                QTYInSet = double.Parse(pivotQTYInSetTB.Text);
                if (string.IsNullOrEmpty(pivotQTYTB.Text))
                {
                    QTY = QTYInSet;
                }  
            }

            if (!string.IsNullOrEmpty(pivotQTYTB.Text))
            {
                QTY = double.Parse(pivotQTYTB.Text);
            }
  
            var pivotPart = new SpareParts(
                   PivotCategoryCB.Text,
                   PivotPartTB.Text,
                   decimal.Parse(pivotCostTB.Text),
                   DateTime.UtcNow,
                   decimal.Parse(pivotHegitTB.Text),
                   pivotHegitUnitCB.Text,
                   decimal.Parse(pivotwidthTB.Text),
                   pivotWidthUnitCB.Text,
                   decimal.Parse(pivotlenghtTB.Text),
                   pivotLengthUnitCB.Text,
                   decimal.Parse(pivotWeightTB.Text),
                   pivotWeightUnitCB.Text,
                   0,
                   0,
                   0,
                   0,
                   quantity: QTY,
                   0,
                   PivotPartARTB.Text,
                   PivotSectionCB.Text,
                   Brand

                   );
            if (SparePartsOBS.Where(x=>x.Name == PivotPartTB.Text).FirstOrDefault() is not null)
            {
                MessageBox.Show("يوجد قطعة أخرى بنفس الاسم, يرجى تغيير الاسم");
            }
            pivotPart.ID = await _pivotPartsRepository.AddPivotPart(pivotPart);
            #region set spare IDs
            await CreatePivotPartRelations(pivotPart, sparesToChild);

            await _pivotPartsRepository.EditPivotPart(sparesToChild);
            #endregion


            //await _pivotPartsRepository.AddPivotPartRelation(PivotParentOBS.ToList());
            //await _pivotPartsRepository.AddPivotPartRelation(SpanParentOBS.ToList());
            //await _pivotPartsRepository.AddPivotPartRelation(SpareParentOBS.ToList());
            //await _pivotPartsRepository.AddPivotPartRelation(SetParentOBS.ToList());



            SparePartsOBS.Add(pivotPart);




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
            //if ((bool)SpinklersParts.IsChecked)
            //{
            //    SprinklerCategoryCB.Text = string.Empty;
            //    SprinklerPartTB.Text = string.Empty;
            //    SprinklerCostTB.Text = string.Empty;
            //    SprinklerHeightTB.Text = string.Empty;
            //    SprinklerwidthTB.Text = string.Empty;
            //    SprinklerlengthTB.Text = string.Empty;
            //    SprinklerWeightTB.Text = string.Empty;
            //}
            if ((bool)PivotParts.IsChecked)
            {
                PivotCategoryCB.Text = string.Empty;
                PivotPartTB.Text = string.Empty;
                pivotCostTB.Text = string.Empty;
                pivotHegitTB.Text = string.Empty;
                pivotwidthTB.Text = string.Empty;
                pivotlenghtTB.Text = string.Empty;
                pivotWeightTB.Text = string.Empty;
                PivotPartARTB.Text = string.Empty;
                pivotQTYTB.Text = string.Empty;
                pivotQTYInSetTB.Text = string.Empty;

                PivotBrandCB.SelectedItem = null;
                PivotSectionCB.SelectedItem = null;
                PivotCategoryCB.SelectedItem = null;
                SpanNameCB.SelectedItem = null;
                PivotNameCB.SelectedItem = null;
                PartNameCB.SelectedItem = null;

                pivotHegitUnitCB.SelectedItem = null;
                pivotLengthUnitCB.SelectedItem = null;
                pivotWidthUnitCB.SelectedItem = null;
                pivotWeightUnitCB.SelectedItem = null;

                RelationTabCB.IsEnabled = true;
                SpanNameCB.IsEnabled = true;
                PartNameCB.IsEnabled = true;
                SetNameCB.IsEnabled = true;
                PivotNameCB.IsEnabled = true;
                AddToParents.IsEnabled = true;


                RelationTabCB.SelectedIndex = -1;
                SpanNameCB.SelectedIndex = -1;
                PartNameCB.SelectedIndex = -1;
                PivotNameCB.SelectedIndex = -1;
                SetNameCB.SelectedIndex = -1;

                SpareParentOBS.Clear();
                SpanParentOBS.Clear();
                PivotParentOBS.Clear();
                SetParentOBS.Clear();


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
                SpanCostTB.Text = string.Empty;
                //PipeTypeCBCB.SelectedIndex = 0;
                SpanNameTB.Text = string.Empty; 
            }
           
            //if ((bool)OverhangParts.IsChecked)
            //{
            //    OHLengthTB.Text = string.Empty;
            //    OHDiameterTB.Text = string.Empty;
            //    OHCostTB.Text = string.Empty;
            //    OHNameTB.Text = string.Empty;
            //    OHTypeCBCB.SelectedIndex = 0;

            //}
            sprinklerEdit = null;
            pivotEdit = null;
            
        }

        private void pivotPartsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditPivot_Button.Visibility = Visibility.Visible;
            AddPivot_Button.Visibility = Visibility.Collapsed;
            //RelationTabCB.IsEnabled = false;
            //PivotNameCB.IsEnabled = false;
            //AddToParents.IsEnabled = false;
            //SpanNameCB.IsEnabled = false;
            //PartNameCB.IsEnabled = false;
            //SetNameCB.IsEnabled = false;
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

        private void NewPivotConnectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivotQTYTB.Visibility = Visibility.Visible;
            pivotQTYInSetTB.Visibility = Visibility.Collapsed;
            if (NewPivotConnectionsGrid.SelectedItem is SpareRelationship piv)
            {
                pivotQTYTB.Text = piv.Quantity.ToString();
                RelationTabCB.SelectedIndex = 0;
                var selectedPivotID = RelationstOBS.FirstOrDefault(x => x.pivotcode == piv.pivotcode);
                PivotNameCB.Text = PivotsOBS.Where(s => s.ID == selectedPivotID.pivotcode).FirstOrDefault().Name;
                NewSpanConnectionsGrid.UnselectAllCells();
                NewPartConnectionsGrid.UnselectAllCells();
                NewSetPartConnectionsGrid.UnselectAllCells();
                NewPartConnectionsGrid.SelectedItem = null;
                NewSetPartConnectionsGrid.SelectedItem = null;
                NewSpanConnectionsGrid.SelectedItem = null;
            }

        }

        private void NewSpanConnectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivotQTYTB.Visibility = Visibility.Visible;
            pivotQTYInSetTB.Visibility = Visibility.Collapsed;
            if (NewSpanConnectionsGrid.SelectedItem is SpareRelationship span)
            {
                pivotQTYTB.Text = span.Quantity.ToString();
                RelationTabCB.SelectedIndex = 1;
                var selectedSpanID = RelationstOBS.FirstOrDefault(x => x.SpanID == span.SpanID);
                SpanNameCB.Text = SpansOBS.Where(s => s.ID == selectedSpanID.SpanID).FirstOrDefault().Name;
                
                NewPivotConnectionsGrid.UnselectAllCells();
                NewPartConnectionsGrid.UnselectAllCells();
                NewSetPartConnectionsGrid.UnselectAllCells();

                NewPivotConnectionsGrid.SelectedItem = null;
                NewPartConnectionsGrid.SelectedItem = null;
                NewSetPartConnectionsGrid.SelectedItem = null;
            }

        }
        
        private void NewPartConnectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivotQTYTB.Visibility = Visibility.Visible;
            pivotQTYInSetTB.Visibility = Visibility.Collapsed;
            if (NewPartConnectionsGrid.SelectedItem is SpareRelationship part)
            {
                pivotQTYTB.Text = part.Quantity.ToString();
                RelationTabCB.SelectedIndex = 2;
                var selectedPartID = RelationstOBS.FirstOrDefault(x => x.SpareID == part.SpareID);
                var SelectedPart = SparePartsOBS.Where(s => s.ID == selectedPartID.SpareID).FirstOrDefault();
                PartNameCB.Text = SelectedPart.Name;
                NewPivotConnectionsGrid.UnselectAllCells();
                NewSpanConnectionsGrid.UnselectAllCells();
                NewSetPartConnectionsGrid.UnselectAllCells();

                NewPivotConnectionsGrid.SelectedItem = null;
                NewSpanConnectionsGrid.SelectedItem = null;
                NewSetPartConnectionsGrid.SelectedItem = null;
            }
        }
        
        private void NewSetPartConnectionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivotQTYTB.Visibility = Visibility.Collapsed;
            pivotQTYInSetTB.Visibility = Visibility.Visible;
            if (NewSetPartConnectionsGrid.SelectedItem is SpareRelationship set)
            {
                pivotQTYInSetTB.Text = set.Quantity.ToString();
                RelationTabCB.SelectedIndex = 3;
                var selectedSetID = RelationstOBS.FirstOrDefault(x => x.SetID == set.SetID);
                var SelectedSet = SetOBS.Where(s => s.ID == selectedSetID.SetID).FirstOrDefault();
                SetNameCB.Text = SelectedSet.Name;
                NewPivotConnectionsGrid.UnselectAllCells();
                NewSpanConnectionsGrid.UnselectAllCells();
                NewPartConnectionsGrid.UnselectAllCells();

                NewPivotConnectionsGrid.SelectedItem = null;
                NewSpanConnectionsGrid.SelectedItem = null;
                NewPartConnectionsGrid.SelectedItem = null;
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
            pivotQTYTB.Text = pivotEdit.Quantity.ToString();
            
            PivotPartARTB.Text = pivotEdit.NameAR.ToString();

            PivotCategoryCB.Text = pivotEdit.PivotCategory;
            PivotSectionCB.Text = pivotEdit.Section;

            pivotHegitUnitCB.Text = pivotEdit.HeightUnit;
            pivotLengthUnitCB.Text = pivotEdit.LengthUnit;
            pivotWidthUnitCB.Text = pivotEdit.WidthUnit;
            pivotWeightUnitCB.Text = pivotEdit.WeightUnit;

            PivotBrandCB.SelectedItem = BrandsOBS.Where(x=>x.Brand == pivotEdit.Brand).FirstOrDefault();


            // var QTYInRels = RelationstOBS.Where(rels => rels.PivotPartID == pivotEdit.ID).FirstOrDefault();
            //double QTY = 0;
            //if (QTYInRels != null) QTY = QTYInRels.Quantity;


            //pivotQTYTB.Text = QTY.ToString();

            var QTYInSetRels = RelationstOBS.Where(rels => rels.ParentType == "Set" && rels.PivotPartID == pivotEdit.ID).FirstOrDefault();
            double QTYInSet = 0;
            if (QTYInSetRels != null) QTYInSet = QTYInSetRels.Quantity;
            pivotQTYInSetTB.Text = QTYInSet.ToString();
            SpareParentOBS.Clear();
            SpanParentOBS.Clear();
            PivotParentOBS.Clear();
            SetParentOBS.Clear();

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

            if (pivotEdit.ParentSets is not null)
            {
                foreach (var item3 in pivotEdit.ParentSets)
                {
                    SetParentOBS.Add(item3);
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

            if (IsAnyFieldEmpty(PivotName.Text, PivotCategory.Text))
            {
                MessageBox.Show("Fill in all required fields.", "Missing Information", MessageBoxButton.OK);

                return;
            }

            var Pivot = new PivotTable(
                PivotName.Text,
                PivotCategory.Text,
                0);

            PivotsOBS.Add(Pivot);
                
            await _PivotRepository.AddPivot(Pivot);
            PivotsDG.ItemsSource = PivotsOBS;
            //UpdateGridandCB();
            ClearTextBoxes();
        }

        async private void PivotNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pivotQTYTB.Text = string.Empty;
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
                PivotsOBS.Remove(part);
                PivotsDG.ItemsSource = PivotsOBS;   
                //UpdateGridandCB();
            }
            ClearTextBoxes();
        }

        private void SpanParts_Click(object sender, RoutedEventArgs e)
        {
            OpenFilterForSpansDG.IsChecked = false;
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


            if (IsAnyFieldEmpty(LengthTB.Text, DiameterTB.Text, SpanNameTB.Text, SpanCostTB.Text, SpanCostTB.Text, SpanOutletsTB.Text, SpanHeightFromGroundTB.Text, HeightFromGroundUnitCB.Text))
            {
                MessageBox.Show("Fill in all required fields.", "Missing Information", MessageBoxButton.OK);

                return;
            }

            var Span = new Spans
                (
                decimal.Parse(LengthTB.Text),
                decimal.Parse(DiameterTB.Text),
                "Span",
                SpanNameTB.Text,
                decimal.Parse(SpanCostTB.Text),
                int.Parse(SpanOutletsTB.Text),
                decimal.Parse(SpanHeightFromGroundTB.Text),
                HeightFromGroundUnitCB.Text,
                string.Join(",", PivotSpanParentOBS.Select(x=>x.ID).ToList())
            );

            Span.ID = await _spanRepository.AddSpan(Span);
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

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (CategoriesDG.SelectedItem is Categories cate)
                {
                    await _catergoryRepository.DeleteCategories(cate);
                    CategoriesListOBS.Remove(cate);
                } 
            }
        }

        private async void DeleteSection_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this category?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (SectionsDG.SelectedItem is Categories cate)
                {
                    await _catergoryRepository.DeleteCategories(cate);
                    SectionsListOBS.Remove(cate);
                }
            }
        }

        private async void AddNewCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesListOBS.Where(x => x.Name == NameNewCategory.Text && x.NameAR == NameARNewCategory.Text).FirstOrDefault() is null)
            {
                if (NameNewCategory.Text != "" && NameARNewCategory.Text != "")
                {
                    Categories categories = new Categories("Category", NameNewCategory.Text, NameARNewCategory.Text);
                    await _catergoryRepository.AddCategories(categories);
                    CategoriesListOBS.Add(categories);

                } 
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

        private async void AddNewSet_Click(object sender, RoutedEventArgs e)
        {
            if (SetName.Text != "" && SetNameAR.Text != "")
            {
                Set set = new Set(SetName.Text, SetNameAR.Text);
                await _setRepository.AddSet(set);
                SetOBS.Add(set);
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
          var s =  RelationstOBS;

            double QTYInSet = 0;
            double QTY = 0;
            if (!string.IsNullOrEmpty(pivotQTYInSetTB.Text))
            {
                QTYInSet = double.Parse(pivotQTYInSetTB.Text);
                if (string.IsNullOrEmpty(pivotQTYTB.Text))
                {
                    QTY = QTYInSet;
                }
            }
            if (!string.IsNullOrEmpty(pivotQTYTB.Text))
            {
                QTY = double.Parse(pivotQTYTB.Text);
            }


            if (RelationTabCB.SelectedIndex == 0)
            {
                
                if (PivotNameCB.SelectedItem is PivotTable pivot)
                {
                    SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = pivot.pivotname,
                        PivotCategory = pivot.pivotcategory,
                        pivotcode = pivot.ID,
                        ParentType = "Pivot",
                        PartLevel = 2,
                        Quantity= QTY,

                    };

                    if (!PivotParentOBS.Contains(spareRelationship))
                    {
                        PivotParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if (RelationTabCB.SelectedIndex == 1)
            {
                if (SpanNameCB.SelectedItem is Spans span)
                {
                    SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = span.Name,
                        PivotCategory = span.Category,
                        SpanID = span.ID,
                        ParentType = "Span",
                        PartLevel = 3,
                        Quantity = QTY,

                    };

                    if (!SpanParentOBS.Contains(spareRelationship))
                    {
                        SpanParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if (RelationTabCB.SelectedIndex == 2)
            {
                if (PartNameCB.SelectedItem is SpareParts spares)
                {
                    SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = spares.Name,
                        PivotCategory = spares.PivotCategory,
                        SpareID = spares.ID,
                      
                        ParentType = "Spare",
                        PartLevel = 3,
                        Quantity = QTY,


                    };

                    if (!SpareParentOBS.Contains(spareRelationship))
                    {
                        SpareParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if (RelationTabCB.SelectedIndex == 3)
            {
                if (SetNameCB.SelectedItem is Set set)
                {
                    SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = set.Name,
                        PivotCategory = "",
                        SetID = set.ID,
                        ParentType = "Set",
                        PartLevel = 3,
                        Quantity = QTYInSet,


                    };

                    if (!SetParentOBS.Contains(spareRelationship))
                    {
                        SetParentOBS.Add(spareRelationship);
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

        private async void deletePivotParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewPivotConnectionsGrid.SelectedItem is SpareRelationship piv)
            {
                if (MessageBox.Show($"Are you sure you want to remove this part from pivot with ID {piv.pivotcode}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    PivotParentOBS.Remove(piv);
                    if (piv.ID != 0)
                    {
                        await _pivotPartsRepository.DeletePivotPartRelation(piv.ID);
                    }
                    NewPivotConnectionsGrid.ItemsSource = PivotParentOBS;
                    NewPivotConnectionsGrid.Items.Refresh();
                    EditPivot_Button.Visibility = Visibility.Collapsed;
                    AddPivot_Button.Visibility = Visibility.Visible;
                }
            }
        }

        private async void deleteSpanParents_Click(object sender, RoutedEventArgs e)
        {
                if (NewSpanConnectionsGrid.SelectedItem is SpareRelationship spa)
                {
                if (MessageBox.Show($"Are you sure you want to remove this part from span with ID {spa.SpanID}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SpanParentOBS.Remove(spa);
                    if (spa.ID != 0)
                    {
                        await _pivotPartsRepository.DeletePivotPartRelation(spa.ID);
                    }
                    NewSpanConnectionsGrid.ItemsSource = SpanParentOBS;
                    NewSpanConnectionsGrid.Items.Refresh();
                    EditPivot_Button.Visibility = Visibility.Collapsed;
                    AddPivot_Button.Visibility = Visibility.Visible;
                }
                    
                }
        }

        private async void deleteSpareParents_Click(object sender, RoutedEventArgs e)
        {
                if (NewPartConnectionsGrid.SelectedItem is SpareRelationship spp)
                {
                if (MessageBox.Show($"Are you sure you want to remove this part from spare part with ID {spp.SpareID}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SpareParentOBS.Remove(spp);
                    if (spp.ID != 0)
                    {
                        await _pivotPartsRepository.DeletePivotPartRelation(spp.ID);
                    }
                    NewPartConnectionsGrid.ItemsSource = SpareParentOBS;
                    NewPartConnectionsGrid.Items.Refresh();
                    EditPivot_Button.Visibility = Visibility.Collapsed;
                    AddPivot_Button.Visibility = Visibility.Visible;
                }
                    
                }
        }


        private async void deleteSetParents_Click(object sender, RoutedEventArgs e)
        {
                if (NewSetPartConnectionsGrid.SelectedItem is SpareRelationship spp)
                {
                if (MessageBox.Show($"Are you sure you want to remove this part from set with ID {spp.SetID}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    SetParentOBS.Remove(spp);
                    if (spp.ID != 0)
                    {
                        await _pivotPartsRepository.DeletePivotPartRelation(spp.ID);
                    }
                    NewSetPartConnectionsGrid.ItemsSource = SetParentOBS;
                    NewSetPartConnectionsGrid.Items.Refresh();
                    EditPivot_Button.Visibility = Visibility.Collapsed;
                    AddPivot_Button.Visibility = Visibility.Visible;
                }
                    
                }
        }


        private async void deletePivotForSpanParents_Click(object sender, RoutedEventArgs e)
        {
            //if (NewPivotForSpansConnectionsGrid.SelectedItem is PivotTable pivot)
            //{
            //    PivotSpanParentOBS.Remove(pivot);
            //}
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

        private async void AddNewBrand_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryNewBrand.SelectedItem is not null && NewBrandTB.Text != "")
            {

                if (BrandsOBS.Where(x => x.Brand == NewBrandTB.Text && x.Category == CategoryNewBrand.Text).FirstOrDefault() is null)
                {
                    Categories categories = CategoryNewBrand.SelectedItem as Categories;
                    Brands brands = new Brands(categories.Name, NewBrandTB.Text);

                    brands.ID = await _brandRepository.AddBrands(brands);
                    BrandsOBS.Add(brands);  
                }
                
            }
        }

        private async void DeleteBrand_Click(object sender, RoutedEventArgs e)
        {
            if (BrandsDG.SelectedItem is Brands brands)
            {
                if (MessageBox.Show("Are you sure you want to delete this brand?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _brandRepository.DeleteBrands(brands);
                    BrandsOBS.Remove(brands);
                } 
            }
        }



        private async void DeleteSet_Click(object sender, RoutedEventArgs e)
        {
            if (SetDG.SelectedItem is Set set)
            {
                if (MessageBox.Show("Are you sure you want to delete this brand?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    await _setRepository.DeleteSet(set.ID);
                    SetOBS.Remove(set);
                }
            }
        }

        private void BrandsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandsDG.SelectedItem is Brands brands)
            {
                NewBrandTB.Text = brands.Brand;
                CategoryNewBrand.SelectedItem = CategoriesListOBS.Where(x => x.Name == brands.Category).FirstOrDefault();


        
            }       
        }





        private void SetDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SetDG.SelectedItem is Set set)
            {
                SetName.Text = set.Name;
                SetNameAR.Text = set.NameAR;


            }
        }

        private bool EditSparePartFields(SpareParts part,double qtyInSet)
        {
            bool isQtyChanged = false;
           

            string Brand = "";
            if (PivotBrandCB.SelectedItem is Brands selectedBrand)
            {
                Brand = selectedBrand.Brand;
            }
            double QTYInSet = 0;
            double QTY = 0;
            if (!string.IsNullOrEmpty(pivotQTYInSetTB.Text))
            {
                QTYInSet = double.Parse(pivotQTYInSetTB.Text);
                if (string.IsNullOrEmpty(pivotQTYTB.Text))
                {
                    QTY = QTYInSet;
                }
            }

            if (!string.IsNullOrEmpty(pivotQTYTB.Text))
            {
                QTY = double.Parse(pivotQTYTB.Text);
            }
            if(QTY != part.Quantity)
            {
                isQtyChanged = true;
            }
            if(qtyInSet > -1 && qtyInSet != QTYInSet)
            {
                isQtyChanged = true;
            }
            part.PivotCategory = PivotCategoryCB.Text;
            part.PivotPart = PivotPartTB.Text;
            part.Cost = decimal.Parse(pivotCostTB.Text);
            part.Height = decimal.Parse(pivotHegitTB.Text);
            part.HeightUnit = pivotHegitUnitCB.Text;
            part.Width = decimal.Parse(pivotwidthTB.Text);
            part.WidthUnit = pivotWidthUnitCB.Text;
            part.Length = decimal.Parse(pivotlenghtTB.Text);
            part.LengthUnit = pivotLengthUnitCB.Text;
            part.Weight = decimal.Parse(pivotWeightTB.Text);
            part.WeightUnit = pivotWeightUnitCB.Text;
            part.Quantity = QTY;
            part.NameAR = PivotPartARTB.Text;
            part.Section = PivotSectionCB.Text;
            part.Brand = Brand;

            return isQtyChanged;
        }

        private async Task EditSparePartReilations(SpareRelationship partRels,double newQty)
        {
            double QTYInSet = 0;

            if (!string.IsNullOrEmpty(pivotQTYInSetTB.Text))
            {
                QTYInSet = double.Parse(pivotQTYInSetTB.Text);
            }

            if(partRels.SetID > 0)
            {
                partRels.Quantity = QTYInSet;
            }
            else
            {
                partRels.Quantity = newQty;
            }
            await _pivotPartsRepository.EditPivotPartRelation(partRels);

            //foreach (var rel in partRels)
            //{
            //    if (rel.SetID > 0)
            //    {
            //    }
            //    else
            //    {
            //        rel.Quantity = newQty;
            //    }
            //}
        }
        private async void EditPivot_Button_Click(object sender, RoutedEventArgs e)
        {
            if (pivotPartsGrid.SelectedItem is SpareParts pivotPart)
            {
                string Brand = "";
                if (PivotBrandCB.SelectedItem is Brands selectedBrand)
                {
                    Brand = selectedBrand.Brand;
                }

                if (IsAnyFieldEmpty(PivotCategoryCB.Text, PivotPartTB.Text, pivotCostTB.Text, pivotHegitTB.Text, pivotwidthTB.Text, pivotlenghtTB.Text,
                    pivotWeightTB.Text, PivotPartARTB.Text, PivotSectionCB.Text, Brand
                    , pivotHegitUnitCB.Text, pivotWidthUnitCB.Text, pivotLengthUnitCB.Text, pivotWeightUnitCB.Text))
                {
                    MessageBox.Show("Fill in all required fields.", "Missing Information", MessageBoxButton.OK);

                    return;
                }



                List<SpareParts> sparesToChild = new List<SpareParts>();

                await CreatePivotPartRelations(pivotPart, sparesToChild);

                var PartRelations = RelationstOBS.Where((partRelation) => partRelation.PivotPartID == pivotPart.ID).ToList();
                var rel = PartRelations.Where((partRel) => partRel.SetID > 0).FirstOrDefault();

                double qtyInSet = -1;
                if (rel is not null)
                {
                    qtyInSet = rel.Quantity;
                }
                var isQtyChanged = EditSparePartFields(pivotPart, qtyInSet);

        

                if (isQtyChanged)
                {
                    var selectedRelation = rel;
                    if (NewPivotConnectionsGrid.SelectedItem is SpareRelationship piv)
                    {
                        selectedRelation = PartRelations.Where(x => x.pivotcode == piv.pivotcode).FirstOrDefault();
                    }
                    if (NewSpanConnectionsGrid.SelectedItem is SpareRelationship span)
                    {
                        selectedRelation = PartRelations.Where(x => x.SpanID == span.SpanID).FirstOrDefault();
                    }
                    if (NewPartConnectionsGrid.SelectedItem is SpareRelationship part)
                    {
                        selectedRelation = PartRelations.Where(x => x.SpareID == part.SpareID).FirstOrDefault();
                    }
                    if (NewSetPartConnectionsGrid.SelectedItem is SpareRelationship set)
                    {
                        selectedRelation = PartRelations.Where(x => x.SetID == set.SetID).FirstOrDefault();
                    }
                    await EditSparePartReilations(selectedRelation, pivotPart.Quantity);
                }


                await _pivotPartsRepository.EditPivotPart(pivotPart);

                ClearTextBoxes();
                pivotPartsGrid.SelectedItem = null;
                EditPivot_Button.Visibility = Visibility.Collapsed;
                AddPivot_Button.Visibility = Visibility.Visible;
                //pivotPartsGrid.Items.Refresh();


               
            }


        }

        private async Task CreatePivotPartRelations(SpareParts pivotPart, List<SpareParts> sparesToChild)
        {
            double QTYInSet = 0;
            double QTY = 0;
            if (!string.IsNullOrEmpty(pivotQTYInSetTB.Text))
            {
                QTYInSet = double.Parse(pivotQTYInSetTB.Text);
                if (string.IsNullOrEmpty(pivotQTYTB.Text))
                {
                    QTY = QTYInSet;
                }
            }

            if (!string.IsNullOrEmpty(pivotQTYTB.Text))
            {
                QTY = double.Parse(pivotQTYTB.Text);
            }


            foreach (var item in PivotParentOBS)
            {
                if (item.ID != 0) continue;
                item.PivotPartID = pivotPart.ID;
                item.Quantity = pivotPart.Quantity;
                item.ID = await _pivotPartsRepository.AddPivotPartRelation(item);
                RelationstOBS.Add(item);
                NewPivotConnectionsGrid.Items.Refresh();
            }
            foreach (var item in SpanParentOBS)
            {
                if (item.ID != 0) continue;
                item.PivotPartID = pivotPart.ID;
                item.Quantity = pivotPart.Quantity;
                item.ID = await _pivotPartsRepository.AddPivotPartRelation(item);
                RelationstOBS.Add(item);
                NewSpanConnectionsGrid.Items.Refresh();
            }
            foreach (var item in SpareParentOBS)
            {
                if (item.ID != 0) continue;
                SpareParts sp = SparePartsOBS.Where(x => x.ID == item.SpareID).FirstOrDefault();
                sp.HasChild = true;
                sparesToChild.Add(sp);
                item.PivotPartID = pivotPart.ID;
                if (item.Quantity == 0)
                {
                    item.Quantity = pivotPart.Quantity;
                }
                item.ID = await _pivotPartsRepository.AddPivotPartRelation(item);
                RelationstOBS.Add(item);
                NewPartConnectionsGrid.Items.Refresh();
            }

            foreach (var item in SetParentOBS)
            {
                if (item.ID != 0) continue;
                item.PivotPartID = pivotPart.ID;
                item.Quantity = QTYInSet;
                item.ID = await _pivotPartsRepository.AddPivotPartRelation(item);
                RelationstOBS.Add(item);
                NewSetPartConnectionsGrid.Items.Refresh();
            }

            pivotPart.ParentPivots = PivotParentOBS.ToList();
            pivotPart.ParentSpans = SpanParentOBS.ToList();
            pivotPart.ParentSpares = SpareParentOBS.ToList();
            pivotPart.ParentSets = SetParentOBS.ToList();
        }

        private void UsersFilterSelectAll_Checked(object sender, RoutedEventArgs e)
        {

            foreach (var item in CategoriesListOBS)
            {

                item.IsSelect = true;

            }
            CategoriesFilterIC.Items.Refresh();

        }

        private void UsersFilterSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in CategoriesListOBS)
            {

                item.IsSelect = false;

            }
            CategoriesFilterIC.Items.Refresh();
        }

        private void UsersFilterTitleSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in SectionsListOBS)
            {

                item.IsSelect = true;

            }
            SectionsFilterIC.Items.Refresh();

        }

        private void UsersFilterTitleSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in SectionsListOBS)
            {

                item.IsSelect = false;

            }
            SectionsFilterIC.Items.Refresh();

        }

        private void checkAllbrands_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in BrandsOBS)
            {
                item.IsSelect = true;
            }
            BrandsFilterIC.Items.Refresh();
        }

        private void checkAllbrands_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in BrandsOBS)
            {
                item.IsSelect = false;
            }
            BrandsFilterIC.Items.Refresh();
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            List<string> Targetbrands = BrandsFilterIC.ItemsSource.Cast<Brands>().Where(x => x.IsSelect).Select(x => x.Brand).ToList();
            List<string> Targetsections = SectionsFilterIC.ItemsSource.Cast<Categories>().Where(x => x.IsSelect).Select(x => x.NameAR).ToList();
            List<string> Targetcategories = CategoriesFilterIC.ItemsSource.Cast<Categories>().Where(x => x.IsSelect).Select(x => x.NameAR).ToList();

           // JoinedSparePartsOBS = HelperFunctions.ToObservableCollection(SparePartsOBS.Where(x => Targetbrands.Contains(x.Brand)
           //&& Targetcategories.Contains(x.PivotCategory.ToLower())
           //&& Targetsections.Contains(x.Section)).ToList());

            JoinedSparePartsOBS = HelperFunctions.ToObservableCollection(SparePartsOBS.Where(x =>
        (Targetbrands.Count == 0 || Targetbrands.Contains(x.Brand)) &&
        (Targetcategories.Count == 0 || Targetcategories.Contains(x.PivotCategory.ToLower())) &&
        (Targetsections.Count == 0 || Targetsections.Contains(x.Section))
         ).ToList());
            //ALLpivotPartsGrid.ItemsSource = FilteredSparePartsOBS;
            ALLpivotPartsGrid.ItemsSource = JoinedSparePartsOBS;
            OpenFilter.IsChecked = false;

        }

        private async void AddNewSetBT_Click(object sender, RoutedEventArgs e)
        {
            var set = new Set(SetName.Text,   SetNameAR.Text);
            await _setRepository.AddSet(set);
            SetOBS.Add(set);

            //UpdateGridandCB();
            
        }

        private void SetsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DeleteButtoninSetDG_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool IsAnyFieldEmpty(params string[] fields) => fields.Any((field) => field.Length == 0);

        

        private void CancelAddPartBT_Click(object sender, RoutedEventArgs e)
        {

            ClearTextBoxes();


            pivotPartsGrid.SelectedItem = null;
            pivotPartsGrid.SelectedIndex = -1;

            NewPivotConnectionsGrid.SelectedItem = null;
            NewPivotConnectionsGrid.SelectedIndex = -1;

            NewSpanConnectionsGrid.SelectedItem = null;
            NewSpanConnectionsGrid.SelectedIndex = -1;

            NewPartConnectionsGrid.SelectedItem = null;
            NewPartConnectionsGrid.SelectedIndex = -1;

            NewSetPartConnectionsGrid.SelectedItem = null;
            NewSetPartConnectionsGrid.SelectedIndex = -1;

            RelationTabCB.SelectedIndex = -1;

            SpanNameCB.SelectedIndex = -1;
            PartNameCB.SelectedIndex = -1;
            PivotNameCB.SelectedIndex = -1;
            SetNameCB.SelectedIndex = -1;

            EditPivot_Button.Visibility = Visibility.Collapsed;
            AddPivot_Button.Visibility = Visibility.Visible;


        }

        private void ApplyFiltersForSpansDG_Click(object sender, RoutedEventArgs e)
        {


            string nameFilter = SpanNameFilterTB.Text;

            decimal minLengthFilter, maxLengthFilter;
            decimal diameterFilter;
            int outletsFilter;
            decimal heightFilter;
            decimal minCostFilter, maxCostFilter;
         


            decimal.TryParse(SpanMinLengthFilterTB.Text, out minLengthFilter);
            decimal.TryParse(SpanMaxLengthFilterTB.Text, out maxLengthFilter);
            decimal.TryParse(SpanDiameterFilterTB.Text, out diameterFilter);
            int.TryParse(SpanOutletsFilterTB.Text, out outletsFilter);
            decimal.TryParse(SpanHeightFromGroundFilterTB.Text, out heightFilter);
            decimal.TryParse(SpanMinCostFilterTB.Text, out minCostFilter);
            decimal.TryParse(SpanMaxCostFilterTB.Text, out maxCostFilter);


            var filteredItems = SpansOBS.Where(item =>
                (string.IsNullOrEmpty(nameFilter) || item.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) &&
                (minLengthFilter == 0 || item.Length >= minLengthFilter) &&
                (maxLengthFilter == 0 || item.Length <= maxLengthFilter) &&
                (diameterFilter == 0 || item.Diameter == diameterFilter) &&
                (outletsFilter == 0 || item.Outlets == outletsFilter) &&
                (heightFilter == 0 || item.HeightFromGround == heightFilter) &&
                (minCostFilter == 0 || item.Cost == minCostFilter) &&
                (maxCostFilter == 0 || item.Cost == maxCostFilter));

            SpansDG.ItemsSource = new ObservableCollection<Spans>(filteredItems);

            OpenFilterForSpansDG.IsChecked = false;

        }

        private void ClearFilterForSpansDG_Click(object sender, RoutedEventArgs e)
        {
            SpansDG.ItemsSource = SpansOBS;

            SpanNameFilterTB.Text = string.Empty;
            SpanMinLengthFilterTB.Text = string.Empty;
            SpanMaxLengthFilterTB.Text = string.Empty;
            SpanDiameterFilterTB.Text = string.Empty;
            SpanOutletsFilterTB.Text = string.Empty;
            SpanHeightFromGroundFilterTB.Text = string.Empty;
            SpanMinCostFilterTB.Text = string.Empty;
            SpanMaxCostFilterTB.Text = string.Empty;

        }

        private void PivotFilterApplyBT_Click(object sender, RoutedEventArgs e)
        {
            string nameFilter = PivotNameFilterTB.Text;
            string categoryFilter = PivotCategoryFilterTB.Text;


            var filteredItems = PivotsOBS.Where(item =>
                (string.IsNullOrEmpty(nameFilter) || item.pivotname.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(categoryFilter) || item.pivotcategory.Contains(categoryFilter, StringComparison.OrdinalIgnoreCase)));

            PivotsDG.ItemsSource = new ObservableCollection<PivotTable>(filteredItems);

        }

        private void CategoryFilterApplyBT_Click(object sender, RoutedEventArgs e)
        {
          
            string nameFilter = NameCategoryFilterTB.Text;
            string nameARFilter = NameARCategoryFilterTB.Text;

           var filteredItems = CategoriesListOBS.Where(item =>
                    (string.IsNullOrEmpty(nameFilter) || item.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(nameARFilter) || item.NameAR.Contains(nameARFilter, StringComparison.OrdinalIgnoreCase))
                );


            CategoriesDG.ItemsSource = new ObservableCollection<Categories>(filteredItems);
        }

        private void BrandFilterApplyBT_Click(object sender, RoutedEventArgs e)
        {
            string brandCategoryFilter = BrandCategoryFilterTB.Text;
            string brandFilter = BrandFilterTB.Text;
          

            var filteredItems = BrandsOBS.Where(item =>       
            (string.IsNullOrEmpty(brandCategoryFilter) || item.Category.Contains(brandCategoryFilter, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(brandFilter) || item.Brand.Contains(brandFilter, StringComparison.OrdinalIgnoreCase))
        );


            BrandsDG.ItemsSource = new ObservableCollection<Brands>(filteredItems);
        }

        private void SectionFilterBT_Click(object sender, RoutedEventArgs e)
        {
           
            string sectionNameFilter = SectionNameFilterTB.Text;
            string sectionNameARFilter = SectionNameARFilterTB.Text;

            var filteredItems = SectionsListOBS.Where(item =>
                     (string.IsNullOrEmpty(sectionNameFilter) || item.Name.Contains(sectionNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                     (string.IsNullOrEmpty(sectionNameARFilter) || item.NameAR.Contains(sectionNameARFilter, StringComparison.OrdinalIgnoreCase))
                 );


            SectionsDG.ItemsSource = new ObservableCollection<Categories>(filteredItems);

        }

        private void SetFilterBT_Click(object sender, RoutedEventArgs e)
        {
            string setNameFilter = SetNameFilterTB.Text;
            string setNameARFilter = SetNameARFilterTB.Text;

            var filteredItems = SetOBS.Where(item =>
                    (string.IsNullOrEmpty(setNameFilter) || item.Name.Contains(setNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(setNameARFilter) || item.NameAR.Contains(setNameARFilter, StringComparison.OrdinalIgnoreCase))
                );

            SetDG.ItemsSource = new ObservableCollection<Set>(filteredItems);

        }

        private void EditPivotPartRelationsButtoninDG_Click(object sender, RoutedEventArgs e)
        {

            SpareParts part = (SpareParts)((Button)sender).DataContext;

            PivotPartsRelations.IsChecked = false;

            pivotPartsGrid.SelectedItem = part;
            EditPivot_Button.Visibility = Visibility.Visible;
            AddPivot_Button.Visibility = Visibility.Collapsed;

            PivotParts.IsChecked = true;
        }

        private void PivotPartFilterSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in CategoriesListOBS)
            {

                item.IsSelect = true;

            }
            PivotPartCategoriesFilterIC.Items.Refresh();
        }

        private void PivotPartFilterSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in CategoriesListOBS)
            {

                item.IsSelect = false;

            }
            PivotPartCategoriesFilterIC.Items.Refresh();
        }

        private void ApplyFiltersForPivotPartsDG_Click(object sender, RoutedEventArgs e)
        {




            string nameFilter = PivotPartFilterTB.Text;
            string nameARFilter = PivotPartARNameFilterTB.Text;

            decimal minCostFilter, maxCostFilter;


            decimal.TryParse(PivotPartMinCorstFilterTB.Text, out minCostFilter);
            decimal.TryParse(PivotPartMaxCostFiltrerTB.Text, out maxCostFilter);


            List<string> Targetcategories = PivotPartCategoriesFilterIC.ItemsSource.Cast<Categories>().Where(x => x.IsSelect).Select(x => x.NameAR).ToList();

            var filtered = (
                SparePartsOBS.Where(item =>
                    (Targetcategories.Count == 0 || Targetcategories.Contains(item.PivotCategory.ToLower())) &&
                    (string.IsNullOrEmpty(nameFilter) || item.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(nameARFilter) || item.NameAR.Contains(nameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (minCostFilter == 0 || item.Cost >= minCostFilter) &&
                    (maxCostFilter == 0 || item.Cost <= maxCostFilter) 
                 ));
            
            pivotPartsRelationsGrid.ItemsSource = new ObservableCollection<SpareParts>(filtered);
            OpenFilterForPivotPartsDG.IsChecked = false;

        }

        private void pivotPartsRelationsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (pivotPartsRelationsGrid.SelectedItem is SpareParts selectedItem)
            {
                var pivotPart = selectedItem;


                if (pivotPart.ParentSpans is not null)
                {
                    SpanParentRelationsOBS.Clear();
                    foreach (var item in pivotPart.ParentSpans)
                    {
                        SpanParentRelationsOBS.Add(item);
                    }
                }

                if (pivotPart.ParentSpares is not null)
                {
                    SpareParentRelationsOBS.Clear();
                    foreach (var item2 in pivotPart.ParentSpares)
                    {
                        SpareParentRelationsOBS.Add(item2);
                    }
                }

                if (pivotPart.ParentPivots is not null)
                {   
                    PivotParentRelationsOBS.Clear();
                    foreach (var item3 in pivotPart.ParentPivots)
                    {
                        PivotParentRelationsOBS.Add(item3);
                    }
                }

                if (pivotPart.ParentSets is not null)
                {  
                    SetParentRelationsOBS.Clear();
                    foreach (var item3 in pivotPart.ParentSets)
                    {
                        SetParentRelationsOBS.Add(item3);
                    }
                }
            }


        }

        private void ClearFilterForPivotPartDG_Click(object sender, RoutedEventArgs e)
        {
            pivotPartsRelationsGrid.ItemsSource = SparePartsOBS;
            PivotPartFilterTB.Text = string.Empty;
            PivotPartARNameFilterTB.Text = string.Empty;
            PivotPartMaxCostFiltrerTB.Text = string.Empty;
            PivotPartMinCorstFilterTB.Text = string.Empty;
            PivotPartFilterSelectAll_Unchecked(null, null);


        }
    }
}



