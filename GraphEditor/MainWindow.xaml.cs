using GraphEditor.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : VertexAndEdges
    {
        UndoRedoStack undoRedoStack;
        VertexAndEdges vertexAndEdges;
        FileModel fileModel;
        List<List<int>> adj;

        UIElement dragObject = null;
        Random r = new Random();
        Brush customColor;
        Point offset;

        int numberOfEdges;
        int numberOfVertex;
        int theEdgesWasPicked;
        int prevVertex;
        int currentVertex;


        bool pressedRemove = false;
        bool pressedCircle = false;
        bool pressedSave = false;
        bool pressedAddVertex = false;
        bool pressedAddEdge = false;
        bool pressedDeleteVertex = false;
        bool pressedDeleteEdge = false;
        bool pressedDoAlgorithm = false;

        private Double zoomMax = 5;
        private Double zoomMin = 0.5;
        private Double zoomSpeed = 0.001;
        private Double zoom = 1;
        public MainWindow()
        {
            InitializeComponent();
            fileModel = new FileModel();
            vertexAndEdges = new VertexAndEdges();
            undoRedoStack = new UndoRedoStack();
            numberOfEdges = 0;
            numberOfVertex = 0;
            theEdgesWasPicked = 0;
            prevVertex = -1;
            currentVertex = -1;
            myCanvas.MouseWheel += MyCanvas_MouseWheel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MyCanvas_AddItems(object sender, MouseButtonEventArgs e)
        {
            if (pressedCircle)
            {
                if (numberOfVertex != 10)
                {
                    Vertex vertex = new Vertex();
                    VertexModel vertexModel = new VertexModel();

                    var text = new TextBlock()
                    {
                        Text = numberOfVertex.ToString(),
                        Width = 7,
                        Height = 15,
                        Foreground = Brushes.White,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    };
                    vertex.textInsideEllipse = numberOfVertex.ToString();

                    vertexModel.textInsideVertex = text;
                    vertex.vertexModel = vertexModel;

                    Canvas.SetLeft(vertex.vertexModel, Mouse.GetPosition(myCanvas).X);
                    Canvas.SetTop(vertex.vertexModel, Mouse.GetPosition(myCanvas).Y);
                    vertex.positionX = (int)Canvas.GetLeft(vertex.vertexModel);
                    vertex.positionY = (int)Canvas.GetTop(vertex.vertexModel);
                    Canvas.SetLeft(vertex.vertexModel.textInsideVertex, (Mouse.GetPosition(myCanvas).X + 17));
                    Canvas.SetTop(vertex.vertexModel.textInsideVertex, (Mouse.GetPosition(myCanvas).Y + 13));
                    vertex.counterOfVertex = numberOfVertex + 1;
                    vertex.vertexModel.DragStarted += onDragStarted;
                    vertex.vertexModel.DragCompleted += onDragCompleted;
                    vertex.vertexModel.DragDelta += NewEllipse_PreviewMouseDown;
                    customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));
                    vertex.vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);


                    vertexAndEdges.listOfVertex.Add(vertex);

                    undoRedoStack.Execute(vertexAndEdges.listOfVertex[numberOfVertex]);

                    myCanvas.Children.Add(vertexAndEdges.listOfVertex[numberOfVertex].vertexModel);
                    myCanvas.Children.Add(vertexAndEdges.listOfVertex[numberOfVertex].vertexModel.textInsideVertex);
                    pressedAddEdge = false;
                    pressedDeleteVertex = false;
                    pressedDeleteEdge = false;
                    pressedAddVertex = true;
                    pressedDoAlgorithm = false;
                    undoRedoStack.stackOfUndoOperation.Push("pressedAddVertex");

                    numberOfVertex++;
                }
                else
                    MessageBox.Show("You cannot add more than 10 vertexes");
            }
            pressedCircle = false;
        }
        private void ConnectEdges()
        {
            Edges edge = new Edges();
            Path path = new Path();

            path.Stroke = Brushes.Black;
            path.StrokeThickness = 2;

            vertexAndEdges.listOfEdges.Add(edge);
            AddPathToList(path, numberOfEdges, vertexAndEdges.listOfEdges[numberOfEdges]);

            myCanvas.Children.Add(GetLastAddedPath(vertexAndEdges.listOfEdges[numberOfEdges]));


            LineGeometry line = new LineGeometry();
            GetLastAddedPath(vertexAndEdges.listOfEdges[numberOfEdges]).Data = line;
            undoRedoStack.Execute(vertexAndEdges.listOfEdges[numberOfEdges]);
            numberOfEdges++;

            AddLineToList(line, vertexAndEdges.listOfEdges[numberOfEdges - 1].counterOfLine, vertexAndEdges.listOfVertex[prevVertex]);
            AddLineToList(line, vertexAndEdges.listOfEdges[numberOfEdges - 1].counterOfLine, vertexAndEdges.listOfVertex[currentVertex]);

            UpdateLine(vertexAndEdges.listOfVertex[prevVertex], vertexAndEdges.listOfVertex[currentVertex]);
        }
        private void UpdateLine(Vertex prevVertex, Vertex currentVertex)
        {
            int indexForPrevVertex = GetLineByIndex(prevVertex.connectingLine, currentVertex.connectingLine);
            int indexForCurrentVertex = GetLineByIndex(currentVertex.connectingLine, prevVertex.connectingLine);

            double left1 = Canvas.GetLeft(prevVertex.vertexModel);
            double top1 = Canvas.GetTop(prevVertex.vertexModel);

            double left2 = Canvas.GetLeft(currentVertex.vertexModel);
            double top2 = Canvas.GetTop(currentVertex.vertexModel);

            prevVertex.connectingLine[indexForPrevVertex].StartPoint = new Point(left1 + prevVertex.vertexModel.ActualWidth / 2, top1 + prevVertex.vertexModel.ActualHeight / 2);
            prevVertex.connectingLine[indexForPrevVertex].EndPoint = new Point(left2 + currentVertex.vertexModel.ActualWidth / 2, top2 + currentVertex.vertexModel.ActualHeight / 2);

            currentVertex.connectingLine[indexForCurrentVertex].StartPoint = new Point(left2 + currentVertex.vertexModel.ActualWidth / 2, top2 + currentVertex.vertexModel.ActualHeight / 2);
            currentVertex.connectingLine[indexForCurrentVertex].EndPoint = new Point(left1 + prevVertex.vertexModel.ActualWidth / 2, top1 + prevVertex.vertexModel.ActualHeight / 2);
        }
        void onDragStarted(object sender, DragStartedEventArgs e)
        {
            VertexModel vertexModel = e.Source as VertexModel;
            Vertex vertex = new Vertex();
            vertexModel.Style = (Style)(this.Resources["PickedRoundThumb"]);
            int foundIndex = FindThePickedVertex(vertexAndEdges.listOfVertex, vertexModel);
            int counterOfDeletedEdges = 0;
            vertex = vertexAndEdges.listOfVertex[foundIndex];
            if (pressedRemove)
            {
                pressedDeleteVertex = true;
                undoRedoStack.stackOfUndoOperation.Push("pressedDeleteVertex");
                pressedDeleteEdge = true;
                pressedAddVertex = false;
                pressedAddEdge = false;
                pressedDoAlgorithm = false;

                vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);
                undoRedoStack.Execute(vertexAndEdges.listOfVertex[foundIndex]);

                List<int> indexOfConnectedLine = new List<int>();

                List<int> valueOfConnectedLine = new List<int>();

                List<int> indexOfConnectedVertex = new List<int>();

                for (int i = 0; i < vertex.connectingLine.Count; i++)
                {
                    FindTheEqualConnectedVertex(vertexAndEdges.listOfVertex, vertex.connectingLine[i], ref indexOfConnectedVertex, vertex);
                }

                for (int i = 0; i < vertex.connectingLine.Count; i++)
                {
                    FindValueOfConnectedLine(i, ref valueOfConnectedLine, vertex);
                }

                myCanvas.Children.Remove(vertexAndEdges.listOfVertex[foundIndex].vertexModel);
                myCanvas.Children.Remove(vertexAndEdges.listOfVertex[foundIndex].vertexModel.textInsideVertex);


                foreach (var value in valueOfConnectedLine)
                {
                    int indexOfEdges = 0;
                    foreach (var edge in vertexAndEdges.listOfEdges)
                    {
                        if (edge.counterOfLine == value)
                        {
                            undoRedoStack.Execute(vertexAndEdges.listOfEdges[indexOfEdges]);
                            myCanvas.Children.Remove(vertexAndEdges.listOfEdges[indexOfEdges].listOfPath[0]);
                            indexOfConnectedLine.Add(indexOfEdges);
                            counterOfDeletedEdges++;
                        }
                        indexOfEdges++;
                    }
                }


                int index = 0;
                int j = 0;
                bool wasDeleted;

                foreach (var line in vertex.connectingLine)
                {
                    wasDeleted = false;

                    for (int i = j; i < indexOfConnectedVertex.Count; i++)
                    {
                        index = indexOfConnectedVertex[i];
                        foreach (var l in vertex.connectingLine)
                        {
                            if (line == l && wasDeleted == false)
                            {
                                vertexAndEdges.listOfVertex[index].connectingLine.Remove(l);
                                vertexAndEdges.listOfVertex[index].counterOfLine.Remove(valueOfConnectedLine[j]);
                                vertexAndEdges.listOfVertex[index].numberOfConnection--;
                                wasDeleted = true;
                            }
                        }
                        if (wasDeleted)
                            break;
                    }
                    j++;
                }


                while (vertexAndEdges.listOfVertex[foundIndex].connectingLine.Count != 0)
                {
                    vertexAndEdges.listOfVertex[foundIndex].connectingLine.RemoveAt(0);
                }

                vertexAndEdges.listOfVertex.RemoveAt(foundIndex);

                indexOfConnectedLine.Sort();
                indexOfConnectedLine.Reverse();

                for (int i = 0; i < indexOfConnectedLine.Count; i++)
                {
                    vertexAndEdges.listOfEdges.RemoveAt(indexOfConnectedLine[i]);
                    numberOfEdges--;
                }

                numberOfVertex--;
                pressedRemove = false;
            }
            else
                theEdgesWasPicked++;

            prevVertex = currentVertex;
            currentVertex = foundIndex;
        }
        void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            VertexModel vertexModel = e.Source as VertexModel;
            int foundIndex = FindThePickedVertex(vertexAndEdges.listOfVertex, vertexModel);
            if (foundIndex != -1)
            {
                Vertex vertex = vertexAndEdges.listOfVertex[foundIndex];

                if (theEdgesWasPicked == 2 && pressedDoAlgorithm == false && currentVertex != prevVertex && currentVertex != -1 && prevVertex != -1)
                {
                    ConnectEdges();
                    vertex.vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);
                    vertexAndEdges.listOfVertex[prevVertex].vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);
                    vertexAndEdges.listOfVertex[prevVertex].counterOfConnectedVertex.Add(vertex.counterOfVertex - 1);
                    vertexAndEdges.listOfVertex[currentVertex].counterOfConnectedVertex.Add(vertexAndEdges.listOfVertex[prevVertex].counterOfVertex - 1);
                    pressedAddEdge = true;
                    undoRedoStack.stackOfUndoOperation.Push("pressedAddEdges");
                    pressedAddVertex = false;
                    pressedDeleteVertex = false;
                    pressedDoAlgorithm = false;
                    theEdgesWasPicked = 0;
                    prevVertex = -1;
                    currentVertex = -1;
                }
                else if (theEdgesWasPicked == 2 && pressedDoAlgorithm == false)
                {
                    vertex.vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);
                    theEdgesWasPicked = 0;
                }
            }
        }

        private void NewEllipse_PreviewMouseDown(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            VertexModel vertexModel = e.Source as VertexModel;
            if (pressedRemove == false)
                pressedRemove = false;
            int foundIndex = FindThePickedVertex(vertexAndEdges.listOfVertex, vertexModel);
            if (foundIndex != -1)
            {
                Vertex vertex = vertexAndEdges.listOfVertex[foundIndex];
                List<int> indexOfConnectedVertex = new List<int>();

                Canvas.SetLeft(vertex.vertexModel, Canvas.GetLeft(vertex.vertexModel) + e.HorizontalChange);
                Canvas.SetTop(vertex.vertexModel, Canvas.GetTop(vertex.vertexModel) + e.VerticalChange);
                Canvas.SetLeft(vertex.vertexModel.textInsideVertex, (Canvas.GetLeft(vertex.vertexModel) + 17));
                Canvas.SetTop(vertex.vertexModel.textInsideVertex, (Canvas.GetTop(vertex.vertexModel) + 13));

                vertexAndEdges.listOfVertex[foundIndex].positionX = (int)Canvas.GetLeft(vertex.vertexModel);
                vertexAndEdges.listOfVertex[foundIndex].positionY = (int)Canvas.GetTop(vertex.vertexModel);
                for (int i = 0; i < vertex.connectingLine.Count; i++)
                {
                    FindTheEqualConnectedVertex(vertexAndEdges.listOfVertex, vertex.connectingLine[i], ref indexOfConnectedVertex, vertex);
                }

                foreach (var index in indexOfConnectedVertex)
                {
                    UpdateLine(vertex, vertexAndEdges.listOfVertex[index]);
                }
            }
        }


        private void Vertex_Click(object sender, RoutedEventArgs e)
        {
            pressedCircle = true;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            pressedRemove = true;
        }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            int indexOfUndoVertex = 0;

            if (undoRedoStack.stackOfUndoOperation.Count != 0)
            {
                string operation = undoRedoStack.stackOfUndoOperation.Pop();
                if (operation == "pressedAddVertex")
                {
                    if (numberOfVertex != 0)
                    {
                        Vertex vertex = undoRedoStack.Undo(vertexAndEdges.listOfVertex[numberOfVertex - 1]);
                        myCanvas.Children.Remove(vertex.vertexModel);
                        myCanvas.Children.Remove(vertex.vertexModel.textInsideVertex);
                        vertexAndEdges.listOfVertex.Remove(vertex);
                        undoRedoStack.stackOfRedoOperation.Push(operation);
                        numberOfVertex--;
                    }
                    else
                    {
                        MessageBox.Show("There are no vertex");
                    }
                }
                else if (operation == "pressedAddEdges")
                {
                    if (numberOfEdges != 0)
                    {
                        Edges edge = undoRedoStack.Undo();
                        myCanvas.Children.Remove(GetLastAddedPath(edge));
                        vertexAndEdges.listOfEdges.Remove(edge);
                        undoRedoStack.stackOfRedoOperation.Push(operation);
                        numberOfEdges--;
                    }
                }
                else if (operation == "pressedDeleteVertex")
                {
                    Vertex vertex = new Vertex();
                    if (numberOfVertex != 0)
                    {
                        vertex = undoRedoStack.Undo(vertexAndEdges.listOfVertex[numberOfVertex - 1]);
                        myCanvas.Children.Add(vertex.vertexModel);
                        myCanvas.Children.Add(vertex.vertexModel.textInsideVertex);
                        vertexAndEdges.listOfVertex.Add(vertex);
                        vertexAndEdges.listOfVertex.Sort((x, y) => x.counterOfVertex.CompareTo(y.counterOfVertex));
                        indexOfUndoVertex = vertexAndEdges.listOfVertex.IndexOf(vertex);
                        undoRedoStack.stackOfRedoOperation.Push(operation);
                        numberOfVertex++;
                    }
                    else
                    {
                        MessageBox.Show("There are no vertex");
                    }

                    if (pressedDeleteEdge)
                    {
                        int counterOfConnection = 0;
                        List<int> indexOfConnectedVertex = new List<int>();

                        for (int i = 0; i < vertex.counterOfConnectedVertex.Count; i++)
                        {
                            indexOfConnectedVertex.Add(vertex.counterOfConnectedVertex[i]);
                        }
                        while (vertex.counterOfConnectedVertex.Count != counterOfConnection)
                        {
                            Edges edge = undoRedoStack.Undo();
                            if (edge != null)
                            {
                                myCanvas.Children.Add(GetLastAddedPath(edge));
                                vertexAndEdges.listOfEdges.Add(edge);

                                LineGeometry line = new LineGeometry();
                                GetLastAddedPath(vertexAndEdges.listOfEdges[numberOfEdges]).Data = line;
                                numberOfEdges++;

                                AddLineToList(line, vertexAndEdges.listOfVertex[indexOfUndoVertex]);
                                AddLineToList(line, vertexAndEdges.listOfVertex[indexOfConnectedVertex[counterOfConnection]]);

                                UpdateLine(vertexAndEdges.listOfVertex[indexOfConnectedVertex[counterOfConnection]], vertexAndEdges.listOfVertex[indexOfUndoVertex]);
                            }
                            counterOfConnection++;
                        }
                    }
                }
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (undoRedoStack.stackOfRedoOperation.Count != 0)
            {
                string operation = undoRedoStack.stackOfRedoOperation.Pop();
                if (operation == "pressedAddVertex")
                {
                    Vertex vertex = undoRedoStack.Redo();
                    if (vertex != null)
                    {
                        myCanvas.Children.Add(vertex.vertexModel);
                        myCanvas.Children.Add(vertex.vertexModel.textInsideVertex);
                        vertexAndEdges.listOfVertex.Add(vertex);
                        pressedAddVertex = true;
                        undoRedoStack.stackOfUndoOperation.Push(operation);
                        numberOfVertex++;
                    }
                }
                else if (operation == "pressedAddEdges")
                {
                    Edges edge = undoRedoStack.RedoForEdges();
                    if (edge != null)
                    {
                        myCanvas.Children.Add(GetLastAddedPath(edge));
                        vertexAndEdges.listOfEdges.Add(edge);
                        pressedAddEdge = true;
                        undoRedoStack.stackOfUndoOperation.Push(operation);
                        numberOfEdges++;
                    }
                }
                else if (operation == "pressedDeleteVertex")
                {
                    Vertex vertex = undoRedoStack.Redo();
                    if (vertex != null)
                    {
                        myCanvas.Children.Remove(vertex.vertexModel);
                        myCanvas.Children.Remove(vertex.vertexModel.textInsideVertex);
                        vertexAndEdges.listOfVertex.Remove(vertex);
                        undoRedoStack.stackOfUndoOperation.Push(operation);
                        numberOfVertex--;
                    }
                    if (pressedDeleteEdge)
                    {
                        int index = 0;
                        while (vertex.counterOfConnectedVertex.Count != index)
                        {
                            Edges edge = undoRedoStack.RedoForEdges();
                            if (edge != null)
                            {
                                myCanvas.Children.Remove(GetLastAddedPath(edge));
                                vertexAndEdges.listOfEdges.Remove(edge);
                                numberOfEdges--;
                            }
                            index++;
                        }
                    }
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string filePath = fileModel.ChooseFolderToSave(myCanvas);
            fileModel.XmlSerialize(typeof(VertexAndEdges), vertexAndEdges, filePath);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            string filePath = fileModel.Load();
            vertexAndEdges = (VertexAndEdges)fileModel.XmlDeserialize(typeof(VertexAndEdges), filePath);

            foreach (var vertex in vertexAndEdges.listOfVertex)
            {
                vertex.vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);
                var text = new TextBlock()
                {
                    Text = vertex.textInsideEllipse,
                    Width = 7,
                    Height = 15,
                    Foreground = Brushes.White,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center
                };
                vertex.vertexModel.textInsideVertex = text;
                Canvas.SetLeft(vertex.vertexModel, vertex.positionX);
                Canvas.SetTop(vertex.vertexModel, vertex.positionY);
                Canvas.SetLeft(vertex.vertexModel.textInsideVertex, vertex.positionX + 17);
                Canvas.SetTop(vertex.vertexModel.textInsideVertex, vertex.positionY + 13);
                vertex.vertexModel.DragStarted += onDragStarted;
                vertex.vertexModel.DragCompleted += onDragCompleted;
                vertex.vertexModel.DragDelta += NewEllipse_PreviewMouseDown;
                myCanvas.Children.Add(vertex.vertexModel);
                myCanvas.Children.Add(vertex.vertexModel.textInsideVertex);
                numberOfVertex++;
            }

            List<int> listOfEdg = new List<int>();
            int index = 0;
            bool wasAdded;
            if (numberOfEdges != vertexAndEdges.listOfEdges.Count)
                foreach (var edges in vertexAndEdges.listOfEdges)
                {
                    foreach (var _vertex in vertexAndEdges.listOfVertex)
                    {
                        foreach (var counter in _vertex.counterOfLine)
                        {
                            wasAdded = false;
                            foreach (var vert in vertexAndEdges.listOfVertex)
                            {
                                foreach (var item in vert.counterOfLine)
                                {
                                    if (item == counter && vert != _vertex)
                                        currentVertex = index;
                                }
                                index++;
                            }
                            index = 0;

                            prevVertex = vertexAndEdges.listOfVertex.FindIndex(x => x == _vertex);
                            if (currentVertex != -1 && prevVertex != -1)
                            {
                                foreach (var edg in listOfEdg)
                                {
                                    if (counter == edg)
                                        wasAdded = true;
                                }
                                if (wasAdded == false)
                                {
                                    Path path = new Path();


                                    path.Stroke = Brushes.Black;
                                    path.StrokeThickness = 2;

                                    AddPathToList(path, numberOfEdges, vertexAndEdges.listOfEdges[numberOfEdges]);

                                    myCanvas.Children.Add(GetLastAddedPath(vertexAndEdges.listOfEdges[numberOfEdges]));


                                    LineGeometry line = new LineGeometry();
                                    GetLastAddedPath(vertexAndEdges.listOfEdges[numberOfEdges]).Data = line;
                                    undoRedoStack.Execute(vertexAndEdges.listOfEdges[numberOfEdges]);
                                    numberOfEdges++;

                                    AddLineToListAfterSaving(line, vertexAndEdges.listOfEdges[numberOfEdges - 1].counterOfLine, vertexAndEdges.listOfVertex[prevVertex]);
                                    AddLineToListAfterSaving(line, vertexAndEdges.listOfEdges[numberOfEdges - 1].counterOfLine, vertexAndEdges.listOfVertex[currentVertex]);

                                    UpdateLine(vertexAndEdges.listOfVertex[prevVertex], vertexAndEdges.listOfVertex[currentVertex]);
                                    listOfEdg.Add(counter);
                                    currentVertex = -1;
                                    prevVertex = -1;
                                }
                            }
                        }
                    }
                }
        }

        private void ShortestAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            pressedAddEdge = true;
            pressedAddVertex = false;
            pressedDeleteVertex = false;
            pressedDoAlgorithm = true;

            adj = new List<List<int>>(numberOfVertex);

            for (int i = 0; i < numberOfVertex; i++)
            {
                adj.Add(new List<int>());
            }

            foreach (var vertex in vertexAndEdges.listOfVertex)
            {
                foreach (var connectedVertex in vertex.counterOfConnectedVertex)
                {
                    addEdge(adj, vertex.counterOfVertex - 1, connectedVertex);
                }
            }
            if (prevVertex != -1)
            {
                prevVertex = 0;
                int source = prevVertex, dest = currentVertex;
                printShortestDistance(adj, source, dest, numberOfVertex);
            }
        }
        private void addEdge(List<List<int>> adj, int i, int j)
        {
            adj[i].Add(j);
            adj[j].Add(i);
        }
        private void printShortestDistance(List<List<int>> adj, int s, int dest, int v)
        {

            int[] pred = new int[v];

            int[] dist = new int[v];

            if (BFS(adj, s, dest, v, pred, dist) == false)
            {
                MessageBox.Show("Given source and destination" + "are not connected");
                return;
            }

            List<int> path = new List<int>();

            int crawl = dest;

            path.Add(crawl);

            while (pred[crawl] != -1)
            {
                path.Add(pred[crawl]);
                crawl = pred[crawl];
            }

            path.Reverse();
            int indexOfLine = 0;
            bool wasFirst = true;
            Vertex copyVertex = new Vertex();

            foreach (var vertex in path)
            {
                if (wasFirst == false)
                {
                    indexOfLine = FindIndexOfLine(copyVertex, vertexAndEdges.listOfVertex[vertex].counterOfLine);
                    vertexAndEdges.listOfEdges[indexOfLine].listOfPath[0].Stroke = Brushes.Red;
                }
                vertexAndEdges.listOfVertex[vertex].vertexModel.Style = (Style)(this.Resources["PickedRoundThumb"]);
                copyVertex = vertexAndEdges.listOfVertex[vertex];
                wasFirst = false;
            }

            crawl = dest;
        }
        private bool BFS(List<List<int>> adj, int src, int dest, int v, int[] pred, int[] dist)
        {
            List<int> queue = new List<int>();

            bool[] visited = new bool[v];

            for (int i = 0; i < v; i++)
            {
                visited[i] = false;
                dist[i] = int.MaxValue;
                pred[i] = -1;
            }
            visited[src] = true;
            dist[src] = 0;
            queue.Add(src);

            while (queue.Count != 0)
            {
                int u = queue[0];

                queue.RemoveAt(0);
                for (int i = 0; i < adj[u].Count; i++)
                {
                    if (visited[adj[u][i]] == false)
                    {
                        visited[adj[u][i]] = true;

                        dist[adj[u][i]] = dist[u] + 1;

                        pred[adj[u][i]] = u;

                        queue.Add(adj[u][i]);

                        if (adj[u][i] == dest)
                            return true;
                    }
                }
            }
            return false;
        }
        private void EndAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            foreach (var vertex in vertexAndEdges.listOfVertex)
            {
                vertex.vertexModel.Style = (Style)(this.Resources["TestRoundThumb"]);
            }
            foreach (var edge in vertexAndEdges.listOfEdges)
            {
                foreach (var path in edge.listOfPath)
                {
                    path.Stroke = Brushes.Black;
                }
            }
            prevVertex = -1;
            currentVertex = -1;
            theEdgesWasPicked = 0;
            pressedDoAlgorithm = false;
        }

        private void DeleteSection_Click(object sender, RoutedEventArgs e)
        {
            while (vertexAndEdges.listOfVertex.Count != 0)
            {
                myCanvas.Children.Remove(vertexAndEdges.listOfVertex[0].vertexModel);
                myCanvas.Children.Remove(vertexAndEdges.listOfVertex[0].vertexModel.textInsideVertex);
                vertexAndEdges.listOfVertex.RemoveAt(0);
            }
            while (vertexAndEdges.listOfEdges.Count != 0)
            {
                myCanvas.Children.Remove(GetLastAddedPath(vertexAndEdges.listOfEdges[0]));
                vertexAndEdges.listOfEdges.RemoveAt(0);
            }

            prevVertex = -1;
            currentVertex = -1;
            theEdgesWasPicked = 0;
            numberOfEdges = 0;
            numberOfVertex = 0;
            pressedRemove = false;
            pressedCircle = false;
            pressedSave = false;
            pressedAddVertex = false;
            pressedAddEdge = false;
            pressedDeleteVertex = false;
            pressedDeleteEdge = false;
            pressedDoAlgorithm = false;
        }
        private void MyCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragObject == null)
                return;
            var position = e.GetPosition(sender as IInputElement);
            Canvas.SetTop(this.dragObject, position.Y - this.offset.Y);
            Canvas.SetLeft(this.dragObject, position.X - this.offset.X);
        }

        private void MyCanvas_PreviewUp(object sender, MouseButtonEventArgs e)
        {
            this.dragObject = null;
            this.myCanvas.ReleaseMouseCapture();
        }
        private void MyCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += zoomSpeed * e.Delta;
            if (zoom < zoomMin) { zoom = zoomMin; }
            if (zoom > zoomMax) { zoom = zoomMax; }

            Point mousePos = e.GetPosition(myCanvas);

            if (zoom > 1)
            {
                myCanvas.RenderTransform = new ScaleTransform(zoom, zoom, mousePos.X, mousePos.Y);
            }
            else
            {
                myCanvas.RenderTransform = new ScaleTransform(zoom, zoom);
            }
        }
        public int FindIndexOfLine(Vertex _vertex, List<int> _counterOfLine)
        {
            int indexOfLine = 0;

            foreach (var firstVertexLine in _counterOfLine)
            {
                foreach (var secondVertexLine in _vertex.counterOfLine)
                {
                    if (firstVertexLine == secondVertexLine)
                    {
                        indexOfLine = firstVertexLine;
                        break;
                    }
                }
            }
            return indexOfLine - 1;
        }

        public void FindTheEqualConnectedLine(List<Vertex> listOfVertex, LineGeometry line, ref List<int> foundIndex, Vertex copyVertex)
        {
            foreach (var _vertex in listOfVertex)
            {
                if (_vertex != copyVertex)
                {
                    for (int i = 0; i < _vertex.connectingLine.Count; i++)
                    {
                        if (_vertex.connectingLine[i] == line)
                        {
                            foundIndex.Add(i);
                        }
                    }
                }
            }
        }

        public void FindValueOfConnectedLine(int index, ref List<int> foundIndex, Vertex _vertex)
        {
            int number = _vertex.counterOfLine[index];
            foundIndex.Add(number);
        }
        public void FindTheEqualConnectedVertex(List<Vertex> listOfVertex, LineGeometry line, ref List<int> foundIndex, Vertex copyVertex)
        {
            int index = 0;
            //int ls1 = listOfVertex[0].connectingLine.Count;
            //int ls2 = listOfVertex[1].connectingLine.Count;
            //int ls3 = listOfVertex[2].connectingLine.Count;
            foreach (var _vertex in listOfVertex)
            {
                if (_vertex != copyVertex)
                {
                    for (int i = 0; i < _vertex.connectingLine.Count; i++)
                    {
                        if (_vertex.connectingLine[i] == line)
                        {
                            foundIndex.Add(index);
                        }
                    }
                }
                index++;
            }
        }
        public int FindThePickedVertex(List<Vertex> listOfEllipses, VertexModel copyVertex)
        {
            int foundIndex;

            foundIndex = listOfEllipses.FindIndex(x => x.vertexModel == copyVertex);

            return foundIndex;
        }
        public int GetLineByIndex(List<LineGeometry> firstLines, List<LineGeometry> secondLines)
        {
            int index = 0;

            for (int i = 0; i < firstLines.Count; i++)
            {
                for (int j = 0; j < secondLines.Count; j++)
                {
                    if (firstLines[i] == secondLines[j])
                        index = i;
                }
            }

            return index;
        }
        public void AddLineToList(LineGeometry _line, int _counterOfLine, Vertex copyVertex)
        {
            copyVertex.connectingLine.Add(_line);
            copyVertex.counterOfLine.Add(_counterOfLine);
            copyVertex.numberOfConnection++;
        }
        public void AddLineToList(LineGeometry _line, Vertex copyVertex)
        {
            copyVertex.connectingLine.Add(_line);
        }
        public void AddLineToListAfterSaving(LineGeometry _line, int _counterOfLine, Vertex copyVertex)
        {
            copyVertex.connectingLine.Add(_line);
        }

        public Path GetLastAddedPath(Edges copyEdge)
        {
            return copyEdge.listOfPath[copyEdge.numberOfLines - 1];
        }
        public void AddPathToList(Path path, int _numberOfEdges, Edges copyEdge)
        {
            copyEdge.listOfPath.Add(path);
            copyEdge.numberOfLines++;
            copyEdge.counterOfLine = _numberOfEdges + 1;
        }
    }

}