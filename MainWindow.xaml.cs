using InputsApp.DataAccess;
using InputsApp.FunctionsLibrary;
using InputsApp.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using Microsoft.Xaml.Behaviors;
using SharpVectors.Dom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;


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
			// For each item in the SpansOBS collection
			foreach (var item in SpansOBS)
            {
				// Check if the item has associated Pivot IDs
				if (item.PivotID is not null && item.PivotID != "")
                {
					// Split the Pivot IDs into a list of integers
					List<int> pivotsIDs = item.PivotID.Split(",").Select(int.Parse).ToList();
					// Find and set the parent Pivots for the current item
					item.ParentPivots = PivotsOBS.Where(x => pivotsIDs.Contains(x.ID)).ToList();
                }
            }


			// For each item in the SparePartsOBS collection
			foreach (var item in SparePartsOBS)
            {

				// Retrieve a list of SpareRelationships where the PivotPartID matches the current item's ID
				List<SpareRelationship> relations = RelationstOBS.Where(x => x.PivotPartID == item.ID).ToList();


				// Check if there are any relations
				if (relations.Count > 0)
                {

					// Set the parent Pivots for the current item
					item.ParentPivots = relations.Where(x => x.ParentType == "Pivot").ToList();

					// Set the parent Spans for the current item
					item.ParentSpans = relations.Where(x => x.ParentType == "Span").ToList();

					// Set the parent Spares for the current item
					item.ParentSpares = relations.Where(x => x.ParentType == "Spare").ToList();


					// Set the parent Sets for the current item
					item.ParentSets = relations.Where(x => x.ParentType == "Set").ToList();
                }
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

            //end
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
			// Retrieve all pivot parts and their relations
			var Parts = await _pivotPartsRepository.GetPivotParts();
            var PartsR = await _pivotPartsRepository.GetPivotPartsRelationsJoined();

			// Add pivot parts to the SparePartsOBS observable collection
			foreach (var item in Parts)
            {
                SparePartsOBS.Add(item);
            }

			// Add pivot parts with their relations to the JoinedSparePartsOBS observable collection
			foreach (var item in PartsR)
            {
                JoinedSparePartsOBS.Add(item);
            }

			// Set the data sources for data grids and combo boxes
			ALLpivotPartsGrid.ItemsSource = JoinedSparePartsOBS;
            pivotPartsGrid.ItemsSource = SparePartsOBS;
            PartNameCB.ItemsSource = SparePartsOBS;

			// Retrieve pivot parts relations
			var relations = await _pivotPartsRepository.GetPivotPartsRelations();


			// Add the retrieved relations to the RelationstOBS observable collection
			foreach (var rel in relations)
            {
                RelationstOBS.Add(rel);
            }
			#endregion







			
			#region pivots
			// Retrieve a list of pivot components
			var pivots = await _PivotRepository.GetPivots();

			// Add each pivot component to the PivotsOBS observable collection
			foreach (var piv in pivots)
            {
                PivotsOBS.Add(piv);
            }

			// Set the data sources for combo boxes and a data grid
			PivotNameCB.ItemsSource = PivotsOBS;
            PivotsDG.ItemsSource = PivotsOBS;
            PivotTypeCB.ItemsSource = PivotsOBS;
			#endregion


			
			#region To delete


			// Retrieve pivot parts with "Overhang" category and assign them to the 'overhang' variable
			var overhang = Parts.Where(x => x.PivotCategory == "Overhang");

			// Set the data source for the 'OverhangDG' data grid to display the retrieved pivot parts
			OverhangDG.ItemsSource = overhang;


			// Retrieve sprinkler parts from the _sprinklerPartsRepository
			var sprinklerParts = await _sprinklerPartsRepository.GetSprinklerParts();


			// Add each retrieved sprinkler part to the 'SprinklerPartsOBS' observable collection
			foreach (var sps in sprinklerParts)
            {
                SprinklerPartsOBS.Add(sps);
            }

			// Set the data source for the 'SprinklerPartsDG' data grid to display the retrieved sprinkler parts
			SprinklerPartsDG.ItemsSource = SprinklerPartsOBS;
			#endregion
			//end





			#region Spans


			// Retrieve "Span" components from the _spanRepository
			var result = await _spanRepository.GetSpans();

			// Add each retrieved "Span" component to the 'SpansOBS' observable collection
			foreach (var item in result)
            {
                SpansOBS.Add(item);
            }
			// Set the data source for the 'SpansDG' data grid to display the retrieved "Span" components
			SpansDG.ItemsSource = SpansOBS;

			// Set the data source for the 'SpanNameCB' combo box to display the retrieved "Span" components
			SpanNameCB.ItemsSource = SpansOBS;


			#endregion


			#region Set


			// Retrieve "Set" components from the _setRepository
			var sets = await _setRepository.GetSets();


			// Add each retrieved "Set" component to the 'SetOBS' observable collection
			foreach (var item in sets)
            {
                SetOBS.Add(item);
            }

			// Set the data source for the 'SetDG' data grid to display the retrieved "Set" components
			SetDG.ItemsSource = SetOBS;

			// Set the data source for the 'SetNameCB' combo box to display the retrieved "Set" components
			SetNameCB.ItemsSource = SetOBS;

			


			#endregion
		

			#region Brands


			// Retrieve a list of brands asynchronously from the _brandRepository
			var brands = await _brandRepository.GetBrands();

			// Populate an ObservableCollection called BrandsOBS with the retrieved brands
			foreach (var item in brands)
            {
                BrandsOBS.Add(item);
            }


			// Set the data source for a DataGrid named BrandsDG to the BrandsOBS collection
			BrandsDG.ItemsSource = BrandsOBS;

			// Populate the data source for an ItemsControl named BrandsFilterIC with distinct brand items
			BrandsFilterIC.ItemsSource = BrandsOBS.Distinct();


			// Create a ListCollectionView called lcv with BrandsOBS as its source and group it by the "Category" property
			ListCollectionView lcv = new ListCollectionView(BrandsOBS);
            lcv.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

			// Set the data source for a ComboBox named PivotBrandCB to the ListCollectionView lcv
			PivotBrandCB.ItemsSource = lcv;

			#endregion

			#region Categories
			// Retrieve a list of categories asynchronously from the _catergoryRepository
			List<Categories> CategoriesList = await _catergoryRepository.GetCategories();


			// Populate an ObservableCollection called CategoriesListOBS with category items where the Type is "Category"
			foreach (var item in CategoriesList.Where(x => x.Type == "Category"))
            {
                CategoriesListOBS.Add(item);
            }
			// Set the data source for a DataGrid named CategoriesDG to CategoriesListOBS
			CategoriesDG.ItemsSource = CategoriesListOBS;
			// Set the data source for a ComboBox named PivotCategoryCB to CategoriesListOBS
			PivotCategoryCB.ItemsSource = CategoriesListOBS;
			// Set the data source for another ComboBox named CategoryNewBrand to CategoriesListOBS
			CategoryNewBrand.ItemsSource = CategoriesListOBS;
			// Set the data source for an ItemsControl named CategoriesFilterIC to CategoriesListOBS
			CategoriesFilterIC.ItemsSource = CategoriesListOBS;


			// Populate an ObservableCollection called SectionsListOBS with category items where the Type is "Section"
			foreach (var item in CategoriesList.Where(x => x.Type == "Section"))
            {
                SectionsListOBS.Add(item);
            }

			// Set the data source for a DataGrid named SectionsDG to SectionsListOBS
			SectionsDG.ItemsSource = SectionsListOBS;
			// Set the data source for a ComboBox named PivotSectionCB to SectionsListOBS
			PivotSectionCB.ItemsSource = SectionsListOBS;
			// Set the data source for an ItemsControl named SectionsFilterIC to SectionsListOBS
			SectionsFilterIC.ItemsSource = SectionsListOBS;
			#endregion

			//hk1
			#region parents stuff
			// Set the data source for a grid named NewPivotConnectionsGrid to PivotParentOBS
			NewPivotConnectionsGrid.ItemsSource = PivotParentOBS;
			// Set the data source for a grid named NewSpanConnectionsGrid to SpanParentOBS
			NewSpanConnectionsGrid.ItemsSource = SpanParentOBS;
			// Set the data source for a grid named NewPartConnectionsGrid to SpareParentOBS
			NewPartConnectionsGrid.ItemsSource = SpareParentOBS;
			// Set the data source for a grid named NewSetPartConnectionsGrid to SetParentOBS
			NewSetPartConnectionsGrid.ItemsSource = SetParentOBS;
			// Set the data source for a grid named NewPivotForSpansConnectionsGrid to PivotSpanParentOBS
			NewPivotForSpansConnectionsGrid.ItemsSource = PivotSpanParentOBS;
            #endregion

            //end
            //************************************************

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

			// Check if the "PivotParts" checkbox is checked
			if ((bool)PivotParts.IsChecked)
			{    
                // Asynchronously retrieve pivot parts from the _pivotPartsRepository
				var Parts = await _pivotPartsRepository.GetPivotParts();

				// If the SparePartsOBS collection is empty, populate it with pivot parts
				if (SparePartsOBS.Count == 0)
                {
                    foreach (var item in Parts)
                    {
                        SparePartsOBS.Add(item);
                    } 
                }

				// Set the data source for a grid named pivotPartsGrid to the retrieved pivot parts
				pivotPartsGrid.ItemsSource = Parts;

				// Set the data source for a ComboBox named PivotNameCB to the PivotsOBS collection
				PivotNameCB.ItemsSource = PivotsOBS;


            }
			//if ((bool)SpinklersParts.IsChecked)
			//{
			//    var result = await _sprinklerPartsRepository.GetSprinklerParts();
			//    SprinklerPartsDG.ItemsSource = result;
			//}


			// Check if the "Pivots" checkbox is checked
			if ((bool)Pivots.IsChecked)
			

			{

				// Set the data source for a DataGrid named PivotsDG to the PivotsOBS collection
				PivotsDG.ItemsSource = PivotsOBS;
				// Set the data source for a ComboBox named PivotNameCB to the PivotsOBS collection
				PivotNameCB.ItemsSource = PivotsOBS;
            }


			// Check if the "SpanParts" checkbox is checked
			if ((bool)SpanParts.IsChecked)
            {
				// Asynchronously retrieve span parts from the _spanRepository
				var result = await _spanRepository.GetSpans();


				// Populate the SpansOBS collection with the retrieved span parts
				foreach (var item in result)
                {
                    SpansOBS.Add(item);
                }

				// Set the data source for a DataGrid named SpansDG to the SpansOBS collection
				SpansDG.ItemsSource = SpansOBS;
				// Set the data source for a ComboBox named SpanNameCB to the SpansOBS collection
				SpanNameCB.ItemsSource = SpansOBS;
            }

			//if ((bool)OverhangParts.IsChecked)
			//{
			//    var result = await _pivotPartsRepository.GetPivotParts();
			//    var overhang = result.Where(x => x.PivotCategory == "Overhang");
			//    OverhangDG.ItemsSource = overhang;
			//}



			// Check if the "CategoriesRD" checkbox is checked
			if ((bool)CategoriesRD.IsChecked)
            {
				// Asynchronously retrieve a list of categories from the _catergoryRepository
				List<Categories> CategoriesList = await _catergoryRepository.GetCategories();

				// If the CategoriesListOBS collection is empty, populate it with categories of "Category" type
				if (CategoriesListOBS.Count == 0)
                {

                    foreach (var item in CategoriesList.Where(x => x.Type == "Category"))
                    {
                        CategoriesListOBS.Add(item);
                    }

					// Set the data source for a DataGrid named CategoriesDG to the CategoriesListOBS collection
					CategoriesDG.ItemsSource = CategoriesListOBS;

					// Set the data source for a ComboBox named PivotCategoryCB to the CategoriesListOBS collection
					PivotCategoryCB.ItemsSource = CategoriesListOBS;
                }

				// If the SectionsListOBS collection is empty, populate it with categories of "Section" type
				if (SectionsListOBS.Count == 0)
                {
                    foreach (var item in CategoriesList.Where(x => x.Type == "Section"))
                    {
                        SectionsListOBS.Add(item);
                    }
					// Set the data source for a DataGrid named SectionsDG to the SectionsListOBS collection
					SectionsDG.ItemsSource = SectionsListOBS;

					// Set the data source for a ComboBox named PivotSectionCB to the SectionsListOBS collection
					PivotSectionCB.ItemsSource = SectionsListOBS;

                }


            }


        }

        private async void AddPivot_Button_Click(object sender, RoutedEventArgs e)
        {
			// Create a list to store spare parts related to the pivot part
			List<SpareParts> sparesToChild = new List<SpareParts>();

			// Initialize a string variable to store the selected brand name
			string Brand = "";

			// Check if an item is selected in the PivotBrandCB ComboBox
			if (PivotBrandCB.SelectedItem is Brands selectedBrand)
            {
                Brand = selectedBrand.Brand;
            }

			// Create a new pivot part object with various properties
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


			// Check if a spare part with the same name already exists
			if (SparePartsOBS.Where(x=>x.Name == PivotPartTB.Text).FirstOrDefault() is not null)
            {
                MessageBox.Show("يوجد قطعة أخرى بنفس الاسم, يرجى تغيير الاسم");
            }

			// Add the new pivot part to the repository and get its ID
			pivotPart.ID = await _pivotPartsRepository.AddPivotPart(pivotPart);
			#region set spare IDs


			// Set the PivotPartID and Quantity for each item in PivotParentOBS
			foreach (var item in PivotParentOBS)
            {
                item.PivotPartID = pivotPart.ID;
                item.Quantity = pivotPart.Quantity;
            }

			// Set the PivotPartID and Quantity for each item in SpanParentOBS
			foreach (var item in SpanParentOBS)
            {
                item.PivotPartID = pivotPart.ID;
                item.Quantity = pivotPart.Quantity;
            }

			// Update spare parts related to the pivot part and collect them in sparesToChild
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


			// Parse and store the quantity in set parts
			double QTYInSet = 0;
            if (!string.IsNullOrEmpty(pivotQTYInSetTB.Text))
            {
                QTYInSet = double.Parse(pivotQTYInSetTB.Text);
            }

			// Set the PivotPartID and Quantity for each item in SetParentOBS
			foreach (var item in SetParentOBS)
            {
                item.PivotPartID = pivotPart.ID;
                item.Quantity = QTYInSet;
            }


			// Edit the pivot part to update the related spare parts
			await _pivotPartsRepository.EditPivotPart(sparesToChild);
			#endregion

			// Assign the parent parts lists to the pivot part
			pivotPart.ParentPivots = PivotParentOBS.ToList();
            pivotPart.ParentSpans = SpanParentOBS.ToList();
            pivotPart.ParentSpares = SpareParentOBS.ToList();
            pivotPart.ParentSets = SetParentOBS.ToList();

			// Add pivot part relationships to the repository
			await _pivotPartsRepository.AddPivotPartRelation(PivotParentOBS.ToList());
            await _pivotPartsRepository.AddPivotPartRelation(SpanParentOBS.ToList());
            await _pivotPartsRepository.AddPivotPartRelation(SpareParentOBS.ToList());
            await _pivotPartsRepository.AddPivotPartRelation(SetParentOBS.ToList());


			// Add the new pivot part to the SparePartsOBS collection
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




			//await _pivotPartsRepository.EditPivotPart(pivotPart);
			//UpdateGridandCB();




			// Clear the text boxes in the user interface
			ClearTextBoxes();
        }

        private async void AddSpinkler_Button_Click(object sender, RoutedEventArgs e)
        {
			// Check if the sprinklerEdit variable is not null, indicating that an existing sprinkler part is being edited
			if (sprinklerEdit != null)
            {
                await UpdateItemFromTextBoxes_Sprinklers(sprinklerEdit);
            }
            else
            {
				// If no existing sprinkler part is being edited, proceed with adding a new sprinkler part

				// Retrieve a list of selected pivot items 
				var selectedPivot = await _PivotRepository.GetPivots();

				//var Pivotid = selectedPivot.FirstOrDefault(x => x.pivotname == PivotforSprinklerCB.Text);


				// Create a new sprinkler part object with various properties
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

				// Add the newly created sprinkler part to the _sprinklerPartsRepository
				await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);
                //UpdateGridandCB();
            }

			// Clear the text boxes in the user interface
			ClearTextBoxes();
        }



        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
			// Get the sender object and assign it to the textBox variable
			TextBox textBox = sender as TextBox;

			// Check if the input text is numeric or a decimal point
			// Loop through each character in the input text provided by the event arguments (e.Text)
			foreach (char c in e.Text)
            {
				// Loop through each character in the input text provided by the event arguments (e.Text)
				if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true; // Prevent non-numeric characters from being entered

					// Exit the loop as soon as a non-numeric character is encountered
					break;
                }
            }

			// Ensure only one decimal point is allowed


			// Check if the input text is a decimal point ('.') and the current text in the TextBox already contains a decimal point
			if (e.Text == "." && textBox.Text.Contains("."))
            {
				// Prevent the input of another decimal point
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
			// Check if an item in the data grid (SprinklerPartsDG) is selected and if the selected item is of type SprinklerParts
			if (SprinklerPartsDG.SelectedItem is SprinklerParts selectedItem)
            {
				// Set the sprinklerEdit variable to the selected item for editing
				sprinklerEdit = selectedItem;

				// Update the text boxes in the user interface with data from the selected item
				UpdateTextBoxesFromItem_Sprinklers(sprinklerEdit);
            }
            else
            {
				// If no item is selected, set the sprinklerEdit variable to null
				sprinklerEdit = null;

				// If no item is selected, set the sprinklerEdit variable to null
				ClearTextBoxes();
            }
        }
        private void UpdateTextBoxesFromItem_Sprinklers(SprinklerParts sprinklerEdit)
        {
			// Set the text of the SprinklerCategoryCB ComboBox to the SprinklerCategory property of the sprinklerEdit object
			SprinklerCategoryCB.Text = sprinklerEdit.SprinklerCategory;
			// Set the text of the SprinklerPartTB TextBox to the SprinklerPart property of the sprinklerEdit object
			SprinklerPartTB.Text = sprinklerEdit.SprinklerPart;
			// Set the text of the SprinklerCostTB TextBox to the Cost property of the sprinklerEdit object, converted to a string
			SprinklerCostTB.Text = sprinklerEdit.Cost.ToString();
			// Set the text of the SprinklerHeightTB TextBox to the Height property of the sprinklerEdit object, converted to a string
			SprinklerHeightTB.Text = sprinklerEdit.Height.ToString();
			// Set the text of the SprinklerwidthTB TextBox to the Width property of the sprinklerEdit object, converted to a string
			SprinklerwidthTB.Text = sprinklerEdit.Width.ToString();
			// Set the text of the SprinklerlengthTB TextBox to the Length property of the sprinklerEdit object, converted to a string
			SprinklerlengthTB.Text = sprinklerEdit.Length.ToString();
			// Set the text of the SprinklerWeightTB TextBox to the Weight property of the sprinklerEdit object, converted to a string
			SprinklerWeightTB.Text = sprinklerEdit.Weight.ToString();
        }

        async private Task UpdateItemFromTextBoxes_Sprinklers(SprinklerParts sprinklerEdit)
        {
			// Retrieve a list of pivot parts from the _pivotPartsRepository
			var selectedPivot = await _pivotPartsRepository.GetPivotParts();
			//var Pivotid = selectedPivot.FirstOrDefault(x => x.PivotCategory == PivotforSprinklerCB.Text);


			// Create a new sprinkler part object with updated properties
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


			// Call the _sprinklerPartsRepository's method to edit the sprinkler part with the updated values
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
			// Check if an item in the data grid (pivotPartsGrid) is selected, and if the selected item is of type SpareParts
			if (pivotPartsGrid.SelectedItem is SpareParts selectedItem)
            {
				// Set the pivotEdit variable to the selected item for potential editing
				pivotEdit = selectedItem;
				// Update the text boxes in the user interface with data from the selected item
				UpdateTextBoxesFromItem_Pivots(pivotEdit);
            }
            else
            {
				// If no item is selected, set the pivotEdit variable to null
				pivotEdit = null;
				// Clear the text boxes in the user interface
				ClearTextBoxes();
            }
            
        }

        private void UpdateTextBoxesFromItem_Pivots(SpareParts pivotEdit)
        {
			// Set various UI elements to display or edit the properties of the pivotEdit object
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


			// Set the selected brand in the PivotBrandCB combo box
			PivotBrandCB.SelectedItem = BrandsOBS.Where(x=>x.Brand == pivotEdit.Brand).FirstOrDefault();

			// Retrieve the quantity in relations for the pivot part
			var QTYInRels = RelationstOBS.Where(rels => rels.PivotPartID == pivotEdit.ID).FirstOrDefault();
            double QTY = 0;
            if (QTYInRels != null) QTY = QTYInRels.Quantity;

			// Set the quantity text box
			pivotQTYTB.Text = QTY.ToString();


			// Retrieve the quantity in set relations for the pivot part
			var QTYInSetRels = RelationstOBS.Where(rels => rels.ParentType == "Set" && rels.PivotPartID == pivotEdit.ID).FirstOrDefault();
            double QTYInSet = 0;
            if (QTYInSetRels != null) QTYInSet = QTYInSetRels.Quantity;
            pivotQTYInSetTB.Text = QTYInSet.ToString();


			// Clear the collections for related parts
			SpareParentOBS.Clear();
            SpanParentOBS.Clear();
            PivotParentOBS.Clear();
            SetParentOBS.Clear();


			// Populate the collections with related parts based on the pivotEdit object
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
            // Create a new PivotTable object with properties from user input
			var Pivot = new PivotTable(
                PivotName.Text,
                PivotCategory.Text,
                decimal.Parse(PivotLength.Text));

			// Add the new PivotTable object to the observable collection (PivotsOBS)
			PivotsOBS.Add(Pivot);

			// Save the new PivotTable object to the repository
			await _PivotRepository.AddPivot(Pivot);

			// Set the data grid's item source to the updated PivotsOBS collection
			PivotsDG.ItemsSource = PivotsOBS;

			//UpdateGridandCB();

			// Clear the text boxes in the user interface
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
                PivotsOBS.Remove(part);
                PivotsDG.ItemsSource = PivotsOBS;   
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

			// Create a new Spans object with the following properties and related data
			var Span = new Spans
                (
				// Set the length property from user input
				decimal.Parse(LengthTB.Text),
				// Set the diameter property from user input
				decimal.Parse(DiameterTB.Text),

				// Set the type as "Span"
				"Span",
				// Set the name property from user input
				SpanNameTB.Text,

				// Set the cost property from user input
				decimal.Parse(SpanCostTB.Text),

				// Set the PivotIDs property by joining IDs from the PivotSpanParentOBS collection
				string.Join(",", PivotSpanParentOBS.Select(x=>x.ID).ToList())
            );

			// Add the newly created Spans object to the _spanRepository and retrieve its ID
			Span.ID = await _spanRepository.AddSpan(Span);

			// Add the new Spans object to the SpansOBS observable collection
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

			// Check which radio button is checked (user's selection)
			if ((bool)PivotPartRD.IsChecked)
            {
				// If the PivotPartRD radio button is checked, and a PivotTable item is selected
				if (PivotNameCB.SelectedItem is PivotTable pivot)
                {
					// Create a SpareRelationship with information related to a PivotTable
					SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = pivot.pivotname,
                        PivotCategory = pivot.pivotcategory,
                        pivotcode = pivot.ID,
                        ParentType = "Pivot",
                        PartLevel = 2,
                        
                    };

					// Add the SpareRelationship to the PivotParentOBS collection if it doesn't already exist
					if (!PivotParentOBS.Contains(spareRelationship))
                    {
                        PivotParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if ((bool)SpanPartRD.IsChecked)
            {
				// If the SpanPartRD radio button is checked, and a Spans item is selected
				if (SpanNameCB.SelectedItem is Spans span)
                {
					// Create a SpareRelationship with information related to a Spans item
					SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = span.Name,
                        PivotCategory = span.Category,
                        SpanID = span.ID,
                        ParentType = "Span",
                        PartLevel = 3,

                    };

					// Add the SpareRelationship to the SpanParentOBS collection if it doesn't already exist
					if (!SpanParentOBS.Contains(spareRelationship))
                    {
                        SpanParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if ((bool)SparePartRD.IsChecked)
            {

				// If the SparePartRD radio button is checked, and a SpareParts item is selected
				if (PartNameCB.SelectedItem is SpareParts spares)
                {

					// Create a SpareRelationship with information related to a SpareParts item
					SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = spares.Name,
                        PivotCategory = spares.PivotCategory,
                        SpareID = spares.ID,
                        ParentType = "Spare",
                        PartLevel = 3,


                    };

					// Add the SpareRelationship to the SpareParentOBS collection if it doesn't already exist
					if (!SpareParentOBS.Contains(spareRelationship))
                    {
                        SpareParentOBS.Add(spareRelationship);
                    }
                }
            }

            else if ((bool)SetPartRD.IsChecked)
            {
				// If the SetPartRD radio button is checked, and a Set item is selected
				if (SetNameCB.SelectedItem is Set set)
                {

					// Create a SpareRelationship with information related to a Set item
					SpareRelationship spareRelationship = new SpareRelationship()
                    {
                        PivotPart = set.Name,
                        PivotCategory = "",
                        SetID = set.ID,
                        ParentType = "Set",
                        PartLevel = 3,


                    };

					// Add the SpareRelationship to the SetParentOBS collection if it doesn't already exist
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


        private void deleteSetParents_Click(object sender, RoutedEventArgs e)
        {
            if (NewSetPartConnectionsGrid.SelectedItem is SpareRelationship spp)
            {
                SetParentOBS.Remove(spp);
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
            // Check if an item is selected in the CategoryNewBrand combo box and if NewBrandTB is not empty
			if (CategoryNewBrand.SelectedItem is not null && NewBrandTB.Text != "")
            {

				// Check if there is no existing brand with the same name and category
				if (BrandsOBS.Where(x => x.Brand == NewBrandTB.Text && x.Category == CategoryNewBrand.Text).FirstOrDefault() is null)
                {
					// Retrieve the selected category from CategoryNewBrand combo box
					Categories categories = CategoryNewBrand.SelectedItem as Categories;

					// Create a new Brands object with the selected category and the new brand name
					Brands brands = new Brands(categories.Name, NewBrandTB.Text);

					// Add the new brand to the _brandRepository and retrieve its ID
					brands.ID = await _brandRepository.AddBrands(brands);

					// Add the new brand to the BrandsOBS observable collection
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

			// Check if an item in the data grid (BrandsDG) is selected, and if the selected item is of type Brands
			if (BrandsDG.SelectedItem is Brands brands)
            {
				// Set the text of the NewBrandTB text box to the brand's name
				NewBrandTB.Text = brands.Brand;

				// Set the selected item in the CategoryNewBrand combo box to the category associated with the selected brand
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

        private async void EditPivot_Button_Click(object sender, RoutedEventArgs e)
        {

			// Check if an item in the data grid (pivotPartsGrid) is selected, and if the selected item is of type SpareParts
			if (pivotPartsGrid.SelectedItem is SpareParts spare)
            {
				// Retrieve a list of pivot parents from NewPivotConnectionsGrid
				List<PivotTable> pivotParents = NewPivotConnectionsGrid.ItemsSource.Cast<PivotTable>().ToList();

				// Retrieve a list of span parents from NewSpanConnectionsGrid
				List<Spans> SpansParents = NewSpanConnectionsGrid.ItemsSource.Cast<Spans>().ToList();
				// Retrieve a list of spare parents from NewPartConnectionsGrid
				List<SpareParts> SparesParents = NewPartConnectionsGrid.ItemsSource.Cast<SpareParts>().ToList();

				// Initialize the Brand variable
				string Brand = "";


				// Check if an item is selected in the PivotBrandCB combo box and set Brand to the selected brand's name
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
			// Create a list of selected brand names from the BrandsFilterIC collection
			List<string> Targetbrands = BrandsFilterIC.ItemsSource.Cast<Brands>().Where(x => x.IsSelect).Select(x => x.Brand).ToList();

			// Create a list of selected section names from the SectionsFilterIC collection
			List<string> Targetsections = SectionsFilterIC.ItemsSource.Cast<Categories>().Where(x => x.IsSelect).Select(x => x.NameAR).ToList();

			// Create a list of selected category names from the CategoriesFilterIC collection
			List<string> Targetcategories = CategoriesFilterIC.ItemsSource.Cast<Categories>().Where(x => x.IsSelect).Select(x => x.NameAR).ToList();

			// Filter the SparePartsOBS collection based on the selected brands
			JoinedSparePartsOBS = HelperFunctions.ToObservableCollection(SparePartsOBS.Where(x => Targetbrands.Contains(x.Brand)
           //&& TargetTitles.Contains(x.Title)
           //&& TargetGrades.Contains(x.JobGrades)
           && Targetcategories.Contains(x.PivotCategory.ToLower())
           && Targetsections.Contains(x.Section)).ToList());
            //ALLpivotPartsGrid.ItemsSource = FilteredSparePartsOBS;

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
    }
}



