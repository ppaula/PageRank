using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PageRank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //deklaracje wartości publicznych, widoczne w całym programie i w każdej funckji
        private AdjacencyMatrix adjacencyMatrix;
        public List<ComboBox> ListOfLeftComboBoxes;
        public List<ComboBox> ListOfRightComboBoxes;
        public int LeftComboBoxValue = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Probability_Of_Edge_Occurence.Text != "")
            {
                Probability_Of_Edge_Occurence.Background = Brushes.White;

                //StackPanelWithConnections.Children.Clear();

                Random r = new Random();
                int v = Int32.Parse(Number_Of_Vertex.Text);

                adjacencyMatrix = new AdjacencyMatrix(v);

                for (int i = 0; i < v; i++)
                {
                    for (int j = 0; j < v; j++)
                    {
                        int probability = r.Next(0, 100);

                        if (probability > Int32.Parse(Probability_Of_Edge_Occurence.Text))
                        {
                            adjacencyMatrix.AdjacencyArray[i, j] = 0;
                        }
                        else
                        {
                            adjacencyMatrix.AdjacencyArray[i, j] = 1;
                        }
                        adjacencyMatrix.AdjacencyArray[i, i] = 0;
                    }
                }

                adjacencyMatrix.Display(StackPanelForDisplayingAdjacencyMatrix, MyCanvas, StackPanelForDisplayingIncidenceMatrix, StackPanelForDisplayingAdjacencylist);
            }
            else
            {
                Probability_Of_Edge_Occurence.Background = Brushes.OrangeRed;
            }
        }

        private void NumbersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Num_of_E_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num_of_E.Background = Brushes.White;
            Num_of_V.Text = "";
            if (Num_of_E.Text == "")
            {
                Num_of_V.Text = "";
                Num_of_V.IsEnabled = false;
                StackPanelWithConnections.Children.Clear();
            }
            else
                Num_of_V.IsEnabled = true;
        }

        private void Num_of_V_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Num_of_V.Text == "")
                StackPanelWithConnections.Children.Clear();
            else
            {
                var num_v = Num_of_V.Text;
                var num_e = Num_of_E.Text;

                if (num_e != "")
                {
                    if ((Int32.Parse(num_v) * Int32.Parse(num_v) - Int32.Parse(num_v)) / 2 >= Int32.Parse(num_e))
                    {
                        GenerateConnections(Int32.Parse(num_v), Int32.Parse(num_e));
                        Num_of_E.Background = Brushes.White;
                    }
                    else
                        Num_of_E.Background = Brushes.OrangeRed;
                }
                else
                    StackPanelWithConnections.Children.Clear();

            }
        }

        private void GenerateConnections(int num_v, int num_e)
        {

            StackPanelWithConnections.Children.Clear();

            adjacencyMatrix = new AdjacencyMatrix(num_v);
            adjacencyMatrix.Display(StackPanelForDisplayingAdjacencyMatrix, MyCanvas, StackPanelForDisplayingIncidenceMatrix, StackPanelForDisplayingAdjacencylist);


            TextBlock tmpBlock = new TextBlock();
            tmpBlock.Text = "Uzupełnij połączenia:";
            tmpBlock.VerticalAlignment = VerticalAlignment.Center;
            tmpBlock.Margin = new Thickness(10, 20, 0, 20);
            StackPanelWithConnections.Children.Add(tmpBlock);

            int j = 0;

            ListOfLeftComboBoxes = new List<ComboBox>();
            ListOfRightComboBoxes = new List<ComboBox>();

            for (int i = 0; i < num_e; i++)
            {
                StackPanel stackPanelForConn = new StackPanel();
                stackPanelForConn.Orientation = Orientation.Horizontal;
                stackPanelForConn.Margin = new Thickness(20, 0, 0, 0);

                TextBlock fromInfo = new TextBlock();
                fromInfo.Text = "Połączenie od: ";

                TextBlock toInfo = new TextBlock();
                toInfo.Text = " do: ";

                ComboBox fromComboBox = new ComboBox();
                SetComboBox(num_v, fromComboBox);
                fromComboBox.Tag = j;
                ListOfRightComboBoxes.Add(fromComboBox);
                fromComboBox.SelectionChanged += FromComboBox_SelectionChanged;
                fromComboBox.DropDownOpened += FromComboBox_DropDownOpened;

                ComboBox toComboBox = new ComboBox();
                SetComboBox(num_v, toComboBox);
                toComboBox.Tag = j + 1;
                toComboBox.IsEnabled = false;
                ListOfLeftComboBoxes.Add(toComboBox);
                toComboBox.SelectionChanged += ToComboBox_SelectionChanged;


                stackPanelForConn.Children.Add(fromInfo);
                stackPanelForConn.Children.Add(fromComboBox);
                stackPanelForConn.Children.Add(toInfo);
                stackPanelForConn.Children.Add(toComboBox);

                StackPanelWithConnections.Children.Add(stackPanelForConn);

                j += 2;
            }

        }

        // Uzupełnienie ComboBoxa danymi
        private void SetComboBox(int e, ComboBox comboBox)
        {
            comboBox.Width = 40;
            for (int i = 1; i <= e; i++)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Content = i;
                comboBox.Items.Add(newItem);
            }
        }

        // Jeśli rozwiniemy listę w "połączenie od: "
        private void FromComboBox_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox myComboBox = sender as ComboBox;

            if (myComboBox.SelectedItem != null)
            {
                ComboBoxItem typeItemLeft = (ComboBoxItem)myComboBox.SelectedItem;
                int previousLeftValue = Int32.Parse(typeItemLeft.Content.ToString());

                var comboBoxRight = ListOfLeftComboBoxes.Find(x => (int)x.Tag == (int)myComboBox.Tag + 1);
                ComboBoxItem typeItemRight = (ComboBoxItem)comboBoxRight.SelectedItem;
                int previosuRightValue = Int32.Parse(typeItemRight.Content.ToString());

                adjacencyMatrix.AdjacencyArray[previousLeftValue - 1, previosuRightValue - 1] = 0;
                adjacencyMatrix.AdjacencyArray[previosuRightValue - 1, previousLeftValue - 1] = 0;

            }

        }

        // Jeśli zmienimy wartosc w "połączenie od: "
        private void FromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox myComboBox = sender as ComboBox;

            for (int i = 0; i < ListOfRightComboBoxes.Count; i++)
                ListOfRightComboBoxes[i].IsEnabled = false;

            var comboBoxRight = ListOfLeftComboBoxes.Find(x => (int)x.Tag == (int)myComboBox.Tag + 1);

            comboBoxRight.IsEnabled = true;

            ComboBoxItem typeItem = (ComboBoxItem)myComboBox.SelectedItem;
            LeftComboBoxValue = Int32.Parse(typeItem.Content.ToString());

            adjacencyMatrix.Display(StackPanelForDisplayingAdjacencyMatrix, MyCanvas, StackPanelForDisplayingIncidenceMatrix, StackPanelForDisplayingAdjacencylist);

        }

        // Jeśli zmienimy wartosc w "połączenie do: "
        private void ToComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox myComboBox = sender as ComboBox;

            ComboBoxItem typeItem = (ComboBoxItem)myComboBox.SelectedItem;
            int rightComboBoxValue = Int32.Parse(typeItem.Content.ToString());

            adjacencyMatrix.AdjacencyArray[LeftComboBoxValue - 1, rightComboBoxValue - 1] = 1;
            adjacencyMatrix.Display(StackPanelForDisplayingAdjacencyMatrix, MyCanvas, StackPanelForDisplayingIncidenceMatrix, StackPanelForDisplayingAdjacencylist);

            myComboBox.IsEnabled = false;

            for (int i = 0; i < ListOfRightComboBoxes.Count; i++)
            {
                ListOfRightComboBoxes[i].IsEnabled = true;
            }

        }

        // Rowniez sprawdza Num_of_E i inne, nie tylko Num_of_V
        private void Num_of_V_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PageRank1_Click(object sender, RoutedEventArgs e)
        {
            int v = adjacencyMatrix.AdjacencyArray.GetLength(0);
            double[] couter = new double[v];
            for(int i=0; i<v; i++)
            {
                couter[i] = 0.0;
            }
            double d = 0.15;
            int maxStep = 1000000;
            int step = 0;
            Random r = new Random();
            int currentV = 0;
            double prop;
            int numOfNeig;
            while (step < maxStep)
            {
                prop = r.Next(0, 1000001);
                prop = prop / 1000000.0;
                numOfNeig = 0;
                for (int i = 0; i < v; i++)
                {
                    if ( adjacencyMatrix.AdjacencyArray[currentV, i] == 1)
                    {
                        numOfNeig++;
                    }
                }
                if (prop <= d || numOfNeig==0)
                {
                    currentV = r.Next(0, v);
                    couter[currentV]++;
                } else
                {
                    int newV = r.Next(0, v);
                    while (adjacencyMatrix.AdjacencyArray[currentV, newV] == 0)
                    {
                        newV = r.Next(0, v);
                    }
                    currentV = newV;
                    couter[currentV]++;
                }
                step++;
            }
            for (int i = 0; i < v; i++)
            {
                couter[i] = couter[i] / 1000000.0;
            }
            string output = "";
            for (int i = 0; i < v; i++)
            {
                output = output + (i + 1).ToString() + ": " + couter[i].ToString("0.000") + "\n";
            }
            PageRank1TextBlock.Text = output;

        }

        private void PageRank2_Click(object sender, RoutedEventArgs e)
        {
            int v = adjacencyMatrix.AdjacencyArray.GetLength(0);
            double d = 0.85;
            double dTeleport = 0.15;
            double[] ptt = new double[v];
            double[] pt = new double[v];
            for (int i = 0; i < v; i++)
            {
                pt[i] = 1.0 / (double)v;
            }
            double [,] P = new double[v, v];
            for (int i = 0; i < v; i++)
            {
                for (int j = 0; j < v; j++)
                {
                    P[i, j] = 0.0;
                }
            }

            for (int id=0; id<1000000; id++)
            {
                //tutaj obliczamy P
                for (int i = 0; i < v; i++)
                {
                    int numOfNeig = 0;
                    for (int ig = 0; ig < v; ig++)
                    {
                        if (adjacencyMatrix.AdjacencyArray[ig, i] == 1)
                        {
                            numOfNeig++;
                        }
                    }
                    for (int j = 0; j < v; j++)
                    {

                        P[i, j] = d * ((double)adjacencyMatrix.AdjacencyArray[i, j] / (double)numOfNeig) + dTeleport / (int)v;
                    }
                }
                //tutaj mnozymy P * pt
                for (int i = 0; i < v; i++)
                {
                    double c = 0.0;
                    for (int j = 0; j < v; j++)
                    {
                        c = c + pt[j] * (double)adjacencyMatrix.AdjacencyArray[j, i];
                    }
                    ptt[i] = c;
                }
            
            }
            string output = "";
            for (int i = 0; i < v; i++)
            {
                output = output + (i + 1).ToString() + ": " + ptt[i].ToString("0.000") + "\n";
            }
            PageRank2TextBlock.Text = output;
        }
    }
}
