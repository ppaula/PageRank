using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace PageRank
{
    class AdjacencyMatrix
    {
        // Brzydkoszybka enkapsulacja
        public int[,] AdjacencyArray;
        public int[,] AdjacencyArrayWeights;

        // Konstruktor
        public AdjacencyMatrix(int n)
        {
            AdjacencyArray = new int[n, n];
            AdjacencyArrayWeights = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    AdjacencyArray[i, j] = 0;
                    AdjacencyArrayWeights[i, j] = 0;
                }
            }
        }

        // Wyświetlanie macierzy na stack panelu 
        public void Display(StackPanel StackPanelForDisplayingAdjacencyMatrix, Canvas MyCanvas, StackPanel StackPanelForDisplayingIncidenceMatrix, StackPanel StackPanelForDisplayingAdjacencylist)
        {
            StackPanelForDisplayingAdjacencyMatrix.Children.Clear();

            string myString = "";

            for (int i = 0; i < AdjacencyArray.GetLength(0); i++)
            {
                for (int j = 0; j < AdjacencyArray.GetLength(1); j++)
                {
                    myString += AdjacencyArray[i, j].ToString() + "  ";
                }
                myString += "\n";
            }

            TextBlock myBlock = new TextBlock();
            myBlock.Text = myString;
            myBlock.FontSize = 16;
            StackPanelForDisplayingAdjacencyMatrix.Children.Add(myBlock);

            DrawGraph(AdjacencyArray.GetLength(0), MyCanvas);

            // Nowa maceirz incydencji
            //IncidenceMatrix incidenceMatix = new IncidenceMatrix(this);
            //incidenceMatix.Display(StackPanelForDisplayingIncidenceMatrix, StackPanelForDisplayingAdjacencylist);
        }

        // Wizualizacja grafu na macierzy
        private void DrawGraph(int num_v, Canvas MyCanvas)
        {
            MyCanvas.Children.Clear();

            var width = MyCanvas.Width;
            var height = MyCanvas.Height;

            Ellipse myEllipse = new Ellipse();
            myEllipse.Height = 400;
            myEllipse.Width = 400;
            myEllipse.Fill = Brushes.Transparent;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.LightGray;
            Canvas.SetLeft(myEllipse, width / 2 - 200);
            Canvas.SetTop(myEllipse, height / 2 - 200);
            MyCanvas.Children.Add(myEllipse);

            var r = 200;    //radius

            var x_m = width / 2;    //x middle
            var y_m = height / 2;   //y middle


            for (int i = 1; i <= num_v; i++)
            {
                var angle = (2 * Math.PI) / num_v * i;

                var x_oc = r * Math.Cos(angle) + x_m;   //x on cirlce
                var y_oc = r * Math.Sin(angle) + y_m;   //y on circle

                Ellipse smallPoint = new Ellipse();
                smallPoint.Height = 8;
                smallPoint.Width = 8;
                smallPoint.Fill = Brushes.Black;
                smallPoint.StrokeThickness = 1;
                smallPoint.Stroke = Brushes.Black;
                Canvas.SetLeft(smallPoint, x_oc - 3);
                Canvas.SetTop(smallPoint, y_oc - 3);

                TextBlock smallPointNumber = new TextBlock();
                smallPointNumber.Text = i.ToString();
                smallPointNumber.RenderTransform = new TranslateTransform
                {
                    X = (r + 10) * Math.Cos(angle) + x_m,
                    Y = (r + 15) * Math.Sin(angle) + y_m - 8
                };

                for (int j = 1; j <= AdjacencyArray.GetLength(0); j++)
                {
                    if (AdjacencyArray[i - 1, j - 1] == 1)
                    {
                        var angle_2 = (2 * Math.PI) / num_v * j;

                        var x_oc_2 = r * Math.Cos(angle_2) + x_m;   //x on cirlce
                        var y_oc_2 = r * Math.Sin(angle_2) + y_m;   //y on circle

                        // drawing arrow line
                        Point point11 = new Point(x_oc, y_oc);
                        Point point12 = new Point(x_oc_2, y_oc_2);
                        DrawArrow(point11, point12, AdjacencyArrayWeights[i - 1, j - 1], MyCanvas);
                    }
                }

                MyCanvas.Children.Add(smallPoint);
                MyCanvas.Children.Add(smallPointNumber);

            }
        }

        private void DrawArrow(Point p1, Point p2, int weight, Canvas MyCanvas)
        {
            GeometryGroup lineGroup = new GeometryGroup();
            double theta = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            Point p = new Point(p1.X + ((p2.X - p1.X) / 1.35), p1.Y + ((p2.Y - p1.Y) / 1.35));
            pathFigure.StartPoint = p;

            Point lpoint = new Point(p.X + 6, p.Y + 15);
            Point rpoint = new Point(p.X - 6, p.Y + 15);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = p;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);

            RotateTransform transform = new RotateTransform();
            transform.Angle = theta + 90;
            transform.CenterX = p.X;
            transform.CenterY = p.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);

            LineGeometry connectorGeometry = new LineGeometry();
            connectorGeometry.StartPoint = p1;
            connectorGeometry.EndPoint = p2;
            lineGroup.Children.Add(connectorGeometry);

            Path path = new Path();
            path.Data = lineGroup;
            path.StrokeThickness = 2;
            path.Stroke = path.Fill = Brushes.Black;

            MyCanvas.Children.Add(path);

            Label label = new Label();
            label.Foreground = Brushes.PaleVioletRed;
            label.Content = weight.ToString();
            label.FontWeight = FontWeights.Bold;
            label.FontSize = 15;
            Canvas.SetLeft(label, p.X);
            Canvas.SetTop(label, p.Y);
            MyCanvas.Children.Add(label);
        }

        //========================================================
        // Algorytm Kosaraju
        //========================================================

        // Algorytm przejścia DFStack
        private void DFStack(int v, bool[] visited, Stack<int> S, int[,] graph, TextBlock SCC)
        {
            visited[v] = true;
            for (int u = 0; u < graph.GetLength(0); u++)
                if (graph[v, u] == 1)
                    if (visited[u] == false)
                        DFStack(u, visited, S, graph, SCC);
            S.Push(v);
            //SCC.Text += "\nPushed";
        }

        // Algorytm przejścia DFSprint
        private void DFSprint(int v, bool[] visited, int[,] graph, TextBlock SCC, List<int> currCompList)
        {
            visited[v] = true;
            //
            // Pisz v
            SCC.Text += (v + 1).ToString() + " ";
            currCompList.Add(v);
            //
            for (int u = 0; u < graph.GetLength(0); u++)
                if (graph[v, u] == 1)
                    if (visited[u] == false)
                        DFSprint(u, visited, graph, SCC, currCompList);
        }

        public List<List<int>> KosarajuAlgorithm(TextBlock SCC)
        {
            SCC.Text = "";

            int n = AdjacencyArray.GetLength(0);

            // Kopiujemy macierz sąsiedztwa do nowego grafu
            int[,] graph = new int[AdjacencyArray.GetLength(0), AdjacencyArray.GetLength(1)];
            for (int i = 0; i < AdjacencyArray.GetLength(0); i++)
                for (int j = 0; j < AdjacencyArray.GetLength(1); j++)
                    graph[i, j] = AdjacencyArray[i, j];

            bool[] visited = new bool[n];   // domyślnie wszystko jest ustawione na false

            Stack<int> S = new Stack<int>();

            for (int v = 0; v < n; v++)
                if (visited[v] == false)
                    DFStack(v, visited, S, graph, SCC);

            TransposeGraph(graph, n);

            for (int i = 0; i < n; i++)
                visited[i] = false;

            List<List<int>> listOfSSC = new List<List<int>>();

            int cn = 0;     // licznik silnie spójnych składowych
            while (S.Count != 0)
            {
                int v = S.Peek();
                S.Pop();

                if (visited[v] != true)
                {
                    cn++;
                    List<int> currCompList = new List<int>();
                    //
                    // Pisz SCC, cn, :
                    SCC.Text += "SCC " + cn.ToString() + ": ";
                    //
                    DFSprint(v, visited, graph, SCC, currCompList);
                    SCC.Text += "\n";
                    listOfSSC.Add(currCompList);
                }
            }
            return listOfSSC;
        }

        private void TransposeGraph(int[,] graph, int n)
        {
            int[,] tmpGraph = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    tmpGraph[i, j] = graph[i, j];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    graph[i, j] = tmpGraph[j, i];
        }

        //Stwórz silnie spójny digraf
        internal void RandomStronglyConnectedDigraph(TextBlock SCC, int num_of_e, int probability)
        {
            int n = AdjacencyArray.GetLength(0);

            Random r = new Random();
            int i_to_draw = r.Next(0, n);
            int j_to_draw = r.Next(0, n);
            while (num_of_e > 0)
            {
                //int probability_curr = r.Next(0, 100);
                // if (probability_curr < probability)
                //  {
                while ((AdjacencyArray[i_to_draw, j_to_draw] != 0) || (i_to_draw == j_to_draw))
                {
                    i_to_draw = r.Next(0, n);
                    j_to_draw = r.Next(0, n);
                }
                AdjacencyArray[i_to_draw, j_to_draw] = 1;
                num_of_e--;
                // }
            }


            //=================================
            List<List<int>> listOfSCC = KosarajuAlgorithm(SCC);

            for (int i = 0; i < listOfSCC.Count - 1; i++)
            {
                AdjacencyArray[listOfSCC[i][0], listOfSCC[i + 1][0]] = 1;
                AdjacencyArray[listOfSCC[i + 1][listOfSCC[i + 1].Count - 1], listOfSCC[i][listOfSCC[i].Count - 1]] = 1;
            }
            KosarajuAlgorithm(SCC);


            //=================================
            // losowanie wag na łukach
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (AdjacencyArray[i, j] == 1)
                        AdjacencyArrayWeights[i, j] = r.Next(-5, 11);
                }
            }
        }


        //========================================================
        // Algorytm Bellmana-Forda
        //========================================================

        internal bool BellmanFordAlorithm(TextBlock SP)
        {
            int n = AdjacencyArray.GetLength(0);

            // Kopiujemy macierz sąsiedztwa do nowego grafu
            int[,] graph = new int[AdjacencyArray.GetLength(0), AdjacencyArray.GetLength(1)];
            for (int i = 0; i < AdjacencyArray.GetLength(0); i++)
                for (int j = 0; j < AdjacencyArray.GetLength(1); j++)
                    graph[i, j] = AdjacencyArray[i, j];

            //wierzchołek startowy
            int v = 0;

            //elementy pomocnicze
            bool test;

            //utworzenie n-elementowej tablicy d i wypełnienie jej wartościami maksymalnymi
            //oraz n-elementowej tablicy p i wypełnienie jej wartościami -1
            Int32[] d = new Int32[n];
            Int32[] p = new Int32[n];
            for (int i = 0; i < n; i++)
            {
                d[i] = Int32.MaxValue;
                p[i] = -1;
            }
            d[v] = 0;
            for (int i = 2; i < n; i++)
            {
                test = true;
                for (int x = 0; x < n - 1; x++)
                {
                    for (int y = 0; y < n; y++)
                    {
                        if (graph[x, y] == 1)
                        {
                            if (d[y] > d[x] + AdjacencyArrayWeights[x, y])
                            {
                                test = false;
                                d[y] = d[x] + AdjacencyArrayWeights[x, y];
                                p[y] = x;
                            }
                        }
                    }
                }
                //if (test == true)
                //  return true;
            }
            for (int x = 0; x < n - 1; x++)
            {
                for (int y = 0; y < n; y++)
                {
                    if (graph[x, y] == 1)
                    {
                        if (d[y] > d[x] + AdjacencyArrayWeights[x, y])
                            return false;
                    }
                }
            }
            WriteShortestPaths(d, p, SP);
            return true;
        }

        private void WriteShortestPaths(Int32[] d, Int32[] p, TextBlock SP)
        {
            int n = d.Length;
            SP.Text = "Najkrótsze ścieżki:\n ";
            for (int i = 0; i < n; i++)
            {
                SP.Text += d[i] + "\n ";
            }
            SP.Text += "\n";
            for (int i = 0; i < n; i++)
            {
                SP.Text += p[i] + " ";
            }
        }
    }
}
