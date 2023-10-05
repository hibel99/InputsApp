using InputsApp.DataAccess;
using InputsApp.FunctionsLibrary;
using InputsApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        ObservableCollection<SprinklerParts> SprinklerPartsOBS = new ObservableCollection<SprinklerParts>();
        ObservableCollection<SpareRelationship> RelationstOBS = new ObservableCollection<SpareRelationship>();


        ObservableCollection<SpareRelationship> PivotParentOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SpanParentOBS = new ObservableCollection<SpareRelationship>();
        ObservableCollection<SpareRelationship> SpareParentOBS = new ObservableCollection<SpareRelationship>();

        ObservableCollection<PivotTable> PivotSpanParentOBS = new ObservableCollection<PivotTable>();

        ObservableCollection<Brands> BrandsOBS = new ObservableCollection<Brands>();

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

                List<int> pivotsIDs = relations.Select(x=>x.pivotcode).ToList();
                item.ParentPivots = relations.Where(x => pivotsIDs.Contains(x.ID)).ToList();
                item.ParentType = "Pivot";
                
                List<int> SpanIDs = relations.Select(x => x.SpanID).ToList();
                item.ParentSpans = relations.Where(x => SpanIDs.Contains(x.ID)).ToList();
                item.ParentType = "Span";

                List<int> SparesIDs = relations.Select(x => x.SpareID).ToList();
                item.ParentSpares = relations.Where(x => SparesIDs.Contains(x.ID)).ToList();
                item.ParentType = "Spare";
            }

            //foreach (var item in JoinedSparePartsOBS)
            //{
            //    if (item.pivotcode is not null && item.pivotcode != "")
            //    {
            //        List<int> pivotsIDs = item.pivotcode.Split(",").Select(int.Parse).ToList();
            //        item.ParentPivots = PivotsOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
            //    }

            //    if (item.SpanID is not null && item.SpanID != "")
            //    {
            //        List<int> pivotsIDs = item.SpanID.Split(",").Select(int.Parse).ToList();
            //        item.ParentSpans = SpansOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
            //    }

            //    if (item.SpareID is not null && item.SpareID != "")
            //    {
            //        List<int> pivotsIDs = item.SpareID.Split(",").Select(int.Parse).ToList();
            //        item.ParentSpares = SparePartsOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
            //    }
            //}

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
            var Parts = await _pivotPartsRepository.GetPivotParts();
            var PartsR = await _pivotPartsRepository.GetPivotPartsRelationsJoined();
            foreach (var item in Parts)
            {
                SparePartsOBS.Add(item);
            }
            foreach (var item in PartsR)
            {
                JoinedSparePartsOBS.Add(item);
            }

            ALLpivotPartsGrid.ItemsSource = JoinedSparePartsOBS;
            pivotPartsGrid.ItemsSource = SparePartsOBS;
            PartNameCB.ItemsSource = SparePartsOBS; 

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

            #region Brands

            var brands = await _brandRepository.GetBrands();
            foreach (var item in brands)
            {
                BrandsOBS.Add(item);
            }
            BrandsDG.ItemsSource = BrandsOBS;
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

            var pivotPart = new SpareParts(
                   PivotCategoryCB.Text,
                   PivotPartTB.Text,
                   decimal.Parse(pivotCostTB.Text),
                   DateTime.UtcNow,
                   decimal.Parse(pivotHegitTB.Text),
                   decimal.Parse(pivotwidthTB.Text),
                   decimal.Parse(pivotlenghtTB.Text),
                   decimal.Parse(pivotWeightTB.Text),
                   0,
                   0,
                   0,
                   0,
                   double.Parse(pivotQTYTB.Text),
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
            foreach (var item in PivotParentOBS)
            {
                item.PivotPartID = pivotPart.ID;
                item.Quantity = pivotPart.Quantity;
            }
            foreach (var item in SpanParentOBS)
            {
                item.PivotPartID = pivotPart.ID;
                item.Quantity = pivotPart.Quantity;


            }
            foreach (var item in SpareParentOBS)
            {
                SpareParts sp = SparePartsOBS.Where(x => x.ID == item.SpareID).FirstOrDefault();
                sp.HasChild = true;
                sparesToChild.Add(sp);
                item.PivotPartID = pivotPart.ID;
                if (item.Quantity == 0)
                {
                    item.Quantity = pivotPart.Quantity;
                }

            }
            await _pivotPartsRepository.EditPivotPart(sparesToChild);
            #endregion

            pivotPart.ParentPivots = PivotParentOBS.ToList();
            pivotPart.ParentSpans = SpanParentOBS.ToList();
            pivotPart.ParentSpares = SpareParentOBS.ToList();

            await _pivotPartsRepository.AddPivotPartRelation(PivotParentOBS.ToList());
            await _pivotPartsRepository.AddPivotPartRelation(SpanParentOBS.ToList());
            await _pivotPartsRepository.AddPivotPartRelation(SpareParentOBS.ToList());


            SparePartsOBS.Add(pivotPart);



            //if (PivotParentOBS.Count != 0)
            //{

               

            //    if (SparePartsOBS.Any(s => HelperFunctions.CompareSpareParts(s, pivotPart)))
            //    {
            //        spareParts = SparePartsOBS.Where(s => HelperFunctions.CompareSpareParts(s, pivotPart)).First();
            //        List<PivotTable> pvs = spareParts.ParentPivots;
            //        pvs.AddRange(pivotParents);
            //        spareParts.pivotcode = string.Join(",", pvs.Distinct().Select(x => x.ID).ToList());
            //        spareParts.ParentPivots = pvs.Distinct().ToList();
            //    }
            //    if (spareParts.ID != 0)
            //    {
            //        await _pivotPartsRepository.EditPivotPart(spareParts);
            //    }
            //    else
            //    {
            //        pivotPart.ID = await _pivotPartsRepository.AddPivotPart(pivotPart);
            //    }

            //}


            //if (SpansParents.Count > 0)
            //{
            //    SpareParts spareParts = new SpareParts();

            //    var pivotPart = new SpareParts(
            //       PivotCategoryCB.Text,
            //       PivotPartTB.Text,
            //       decimal.Parse(pivotCostTB.Text),
            //       DateTime.UtcNow,
            //       decimal.Parse(pivotHegitTB.Text),
            //       decimal.Parse(pivotwidthTB.Text),
            //       decimal.Parse(pivotlenghtTB.Text),
            //       decimal.Parse(pivotWeightTB.Text),
            //       "",
            //       3,
            //       "",
            //       "",
            //       double.Parse(pivotQTYTB.Text),
            //       string.Join(",", SpansParents.Select(x => x.ID).ToList()),
            //       PivotPartARTB.Text,
            //       PivotSectionCB.Text,
            //       Brand
            //       );
            //    pivotPart.ParentSpans = SpansParents;

            //    if (SparePartsOBS.Any(s => HelperFunctions.CompareSpareParts(s, pivotPart)))
            //    {
            //        spareParts = SparePartsOBS.Where(s => HelperFunctions.CompareSpareParts(s, pivotPart)).First();
            //        List<Spans> pvs = spareParts.ParentSpans;
            //        pvs.AddRange(SpansParents);
            //        spareParts.SpanID = string.Join(",", pvs.Distinct().Select(x => x.ID).ToList());
            //        spareParts.ParentSpans = pvs.Distinct().ToList();
            //    }
            //    if (spareParts.ID != 0)
            //    {
            //        await _pivotPartsRepository.EditPivotPart(spareParts);
            //    }
            //    else
            //    {
            //        pivotPart.ID = await _pivotPartsRepository.AddPivotPart(pivotPart);
            //        SparePartsOBS.Add(pivotPart);
            //    }




            //}


            //if (SparesParents.Count > 0)
            //{
            //    var groupedLists = SparesParents.GroupBy(item => item.PartLevel);

            //    // Iterate over the grouped lists and print the items in each group
            //    foreach (var group in groupedLists)
            //    {
            //        int level = group.Key;
            //        List<SpareParts> itemsAtLevel = group.ToList();
            //        SpareParts spareParts = new SpareParts();

            //        var pivotPart = new SpareParts(
            //             PivotCategoryCB.Text,
            //             PivotPartTB.Text,
            //             decimal.Parse(pivotCostTB.Text),
            //             DateTime.UtcNow,
            //             decimal.Parse(pivotHegitTB.Text),
            //             decimal.Parse(pivotwidthTB.Text),
            //             decimal.Parse(pivotlenghtTB.Text),
            //             decimal.Parse(pivotWeightTB.Text),
            //             "",
            //             level + 1,
            //             "",
            //             string.Join(",", itemsAtLevel.Select(x => x.ID).ToList()),
            //             double.Parse(pivotQTYTB.Text),
            //             "",
            //             PivotPartARTB.Text,
            //             PivotSectionCB.Text,
            //             Brand
            //             );
            //        pivotPart.ParentSpares = itemsAtLevel;

            //        if (SparePartsOBS.Any(s => HelperFunctions.CompareSpareParts(s, pivotPart)))
            //        {
            //            spareParts = SparePartsOBS.Where(s => HelperFunctions.CompareSpareParts(s, pivotPart) && s.SpanID == "" && s.pivotcode == "").First();
            //            List<SpareParts> pvs = spareParts.ParentSpares;
            //            pvs.AddRange(itemsAtLevel);
            //            spareParts.SpareID = string.Join(",", pvs.Distinct().Select(x => x.ID).ToList());
            //            spareParts.ParentSpares = pvs.Distinct().ToList();
            //        }
            //        if (spareParts.ID != 0)
            //        {
            //            await _pivotPartsRepository.EditPivotPart(spareParts);
            //        }
            //        else
            //        {
            //            pivotPart.ID = await _pivotPartsRepository.AddPivotPart(pivotPart);
            //            SparePartsOBS.Add(pivotPart);
            //        }

            //    }
            //}




            await _pivotPartsRepository.AddPivotPart(pivotPart);
            //UpdateGridandCB();


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

                PivotBrandCB.SelectedItem = null;
                PivotSectionCB.SelectedItem = null;
                PivotCategoryCB.SelectedItem = null;
                SpanNameCB.SelectedItem = null;
                PivotNameCB.SelectedItem = null;
                PartNameCB.SelectedItem = null;
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
            pivotQTYTB.Text = pivotEdit.Quantity.ToString();
            PivotPartARTB.Text = pivotEdit.NameAR.ToString();

            PivotCategoryCB.Text = pivotEdit.PivotCategory;
            PivotSectionCB.Text = pivotEdit.Section;
            PivotBrandCB.SelectedItem = BrandsOBS.Where(x=>x.Brand == pivotEdit.Brand).FirstOrDefault();

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
                    SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = pivot.pivotname,
                        PivotCategory = pivot.pivotcategory,
                        pivotcode = pivot.ID,
                        ParentType = "Pivot",
                        PartLevel = 2,
                        
                    };

                    if (!PivotParentOBS.Contains(spareRelationship))
                    {
                        PivotParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if ((bool)SpanPartRD.IsChecked)
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

                    };

                    if (!SpanParentOBS.Contains(spareRelationship))
                    {
                        SpanParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if ((bool)SparePartRD.IsChecked)
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


                    };

                    if (!SpareParentOBS.Contains(spareRelationship))
                    {
                        SpareParentOBS.Add(spareRelationship);
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
            if (NewPivotConnectionsGrid.SelectedItem is SpareRelationship piv)
            {
                PivotParentOBS.Remove(piv);
            }
        }

        private void deleteSpanParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewSpanConnectionsGrid.SelectedItem is SpareRelationship spa)
            {
                SpanParentOBS.Remove(spa);
            }
        }

        private void deleteSpareParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewPartConnectionsGrid.SelectedItem is SpareRelationship spp)
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

        private void BrandsDG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BrandsDG.SelectedItem is Brands brands)
            {
                NewBrandTB.Text = brands.Brand;
                CategoryNewBrand.SelectedItem = CategoriesListOBS.Where(x => x.Name == brands.Category).FirstOrDefault();


    }       }

        private async void EditPivot_Button_Click(object sender, RoutedEventArgs e)
        {
            if (pivotPartsGrid.SelectedItem is SpareParts spare)
            {

                List<PivotTable> pivotParents = NewPivotConnectionsGrid.ItemsSource.Cast<PivotTable>().ToList();
                List<Spans> SpansParents = NewSpanConnectionsGrid.ItemsSource.Cast<Spans>().ToList();
                List<SpareParts> SparesParents = NewPartConnectionsGrid.ItemsSource.Cast<SpareParts>().ToList();
                string Brand = "";
                if (PivotBrandCB.SelectedItem is Brands selectedBrand)
                {
                    Brand = selectedBrand.Brand;
                }
                //if (pivotParents is not null)
                //{

                //    var pivotPart = new SpareParts(
                //        spare.ID,
                //        PivotCategoryCB.Text,
                //        PivotPartTB.Text,
                //        decimal.Parse(pivotCostTB.Text),
                //        DateTime.UtcNow,
                //        decimal.Parse(pivotHegitTB.Text),
                //        decimal.Parse(pivotwidthTB.Text),
                //        decimal.Parse(pivotlenghtTB.Text),
                //        decimal.Parse(pivotWeightTB.Text),
                //        string.Join(",", pivotParents.Select(x => x.ID).ToList()),
                //        2,
                //        "",
                //        "",
                //        double.Parse(pivotQTYTB.Text),
                //        "",
                //        PivotPartARTB.Text,
                //        PivotSectionCB.Text,
                //        Brand

                //        );
                //    pivotPart.ParentPivots = pivotParents;

                  
                //        await _pivotPartsRepository.EditPivotPart(pivotPart);


                //}


                //if (SpansParents.Count > 0)
                //{

                //    var pivotPart = new SpareParts(spare.ID,
                //       PivotCategoryCB.Text,
                //       PivotPartTB.Text,
                //       decimal.Parse(pivotCostTB.Text),
                //       DateTime.UtcNow,
                //       decimal.Parse(pivotHegitTB.Text),
                //       decimal.Parse(pivotwidthTB.Text),
                //       decimal.Parse(pivotlenghtTB.Text),
                //       decimal.Parse(pivotWeightTB.Text),
                //       "",
                //       3,
                //       "",
                //       "",
                //       double.Parse(pivotQTYTB.Text),
                //       string.Join(",", SpansParents.Select(x => x.ID).ToList()),
                //       PivotPartARTB.Text,
                //       PivotSectionCB.Text,
                //       Brand
                //       );
                //    pivotPart.ParentSpans = SpansParents;

                   
                //        await _pivotPartsRepository.EditPivotPart(pivotPart);





                //}


                //if (SparesParents.Count > 0)
                //{
                //    var groupedLists = SparesParents.GroupBy(item => item.PartLevel);

                //    // Iterate over the grouped lists and print the items in each group
                //    foreach (var group in groupedLists)
                //    {
                //        int level = group.Key;
                //        List<SpareParts> itemsAtLevel = group.ToList();

                //        var pivotPart = new SpareParts(
                //             spare.ID,
                //             PivotCategoryCB.Text,
                //             PivotPartTB.Text,
                //             decimal.Parse(pivotCostTB.Text),
                //             DateTime.UtcNow,
                //             decimal.Parse(pivotHegitTB.Text),
                //             decimal.Parse(pivotwidthTB.Text),
                //             decimal.Parse(pivotlenghtTB.Text),
                //             decimal.Parse(pivotWeightTB.Text),
                //             "",
                //             level + 1,
                //             "",
                //             string.Join(",", itemsAtLevel.Select(x => x.ID).ToList()),
                //             double.Parse(pivotQTYTB.Text),
                //             "",
                //             PivotPartARTB.Text,
                //             PivotSectionCB.Text,
                //             Brand
                //             );
                //            pivotPart.ParentSpares = itemsAtLevel;

                       
                        
                //            await _pivotPartsRepository.EditPivotPart(pivotPart);
                        
                       

                //    }
                //}
            }
        }
    }
}
