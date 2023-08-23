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

            var pivotPart = new PivotParts(6, "TestCat6FromEdit", "PivotTestName3FromEdit", 122, DateTime.Now, 20, 20, 20, 20);
            //var sprinklerPart = new SprinklerParts(1,"sprinklerPartCatFromEdit", "sprinklerTestName1FromEdit", 122, DateTime.Now, 25, 20, 20, 20);
            //await _sprinklerPartsRepository.AddSprinklerPart(sprinklerPart);
            //await _pivotPartsRepository.DeletePivotPart(2);
            //await _pivotPartsRepository.AddPivotPart(pivotPart);
            //await _sprinklerPartsRepository.EditSprinklerPart(sprinklerPart);
            await _pivotPartsRepository.EditPivotPart(pivotPart);
            var result = await _pivotPartsRepository.GetPivotParts();
            pivotPartsGrid.ItemsSource = result;
        }
    }
}
