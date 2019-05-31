using Grafik.Controllers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Grafik.Views
{
    public partial class MainWindow : Window
    {
        //scale of OX and OY axes
        const double defaultScale = 0.1;
        double xScale = defaultScale;
        double yScale = defaultScale;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWindow();
            addWindow.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
                return;

            //find selected function and remove from list
            Controller.Functions.Remove(Controller.Functions.Find(el => el.ID == int.Parse(((ListBoxItem)listBox.SelectedItem).Name.Substring(4))));
            RefreshList();
            Draw();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteButton_Click(null, null);
            else if (e.Key == Key.Add)
                AddButton_Click(null, null);
        }

        private void XSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (xLabel == null)
                return;

            xScale = xSlider.Value / 100;
            xLabel.Content = Math.Round(xScale, 2).ToString() + 'x';
            xScale *= defaultScale;
            Draw();
        }

        private void YSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (yLabel == null)
                return;

            yScale = ySlider.Value / 100;
            yLabel.Content = Math.Round(yScale, 2).ToString() + 'x';
            yScale *= defaultScale;
            Draw();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Draw();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            RefreshList();
            Draw();
        }

        private void RefreshList()
        {
            listBox.Items.Clear();
            foreach (var i in Controller.Functions)
            {
                //create custom item of list
                listBox.Items.Add(new ListBoxItem
                {
                    Foreground = i.Brush,
                    Height = 50,
                    Content = i.ReadableForm,
                    FontSize = 17,
                    FontWeight = FontWeights.Medium,
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = Brushes.Gray,
                    Padding = new Thickness(20, 0, 0, 0),
                    Name = "item" + i.ID.ToString()
                });
            }

        }

        private void Draw()
        {
            //draw axes and functions on canvas
            canvas.Children.Clear();
            DrawAxes();
            DrawCoordinates();
            DrawFunctions();
        }

        private void DrawAxes()
        {
            var black = Brushes.Black;
            const double thickness = 2;

            //offset that is used to center OX and OY axes on canvas
            const int xOffset = 20;
            const int yOffset = 160;

            //draw OY and OY axes
            var xLine = CreateLine(0, ActualHeight / 2 - xOffset, ActualWidth, ActualHeight / 2 - xOffset, thickness, black);
            canvas.Children.Add(xLine);
            var yLine = CreateLine(ActualWidth / 2 - yOffset, 0, ActualWidth / 2 - yOffset, ActualHeight, thickness, black);
            canvas.Children.Add(yLine);

            const int arrowSize = 7;
            const int yArrowOffset = yOffset + 157;

            //draw arrows on OX
            var xArrow1 = CreateLine(ActualWidth - yArrowOffset - arrowSize, ActualHeight / 2 - xOffset - arrowSize, ActualWidth - yArrowOffset, ActualHeight / 2 - xOffset, thickness, black);
            var xArrow2 = CreateLine(ActualWidth - yArrowOffset - arrowSize, ActualHeight / 2 - xOffset + arrowSize, ActualWidth - yArrowOffset, ActualHeight / 2 - xOffset, thickness, black);
            canvas.Children.Add(xArrow1);
            canvas.Children.Add(xArrow2);

            //draw arrows on OY
            var yArrow1 = CreateLine(ActualWidth / 2 - yOffset - arrowSize, arrowSize, ActualWidth / 2 - yOffset, 0, thickness, black);
            var yArrow2 = CreateLine(ActualWidth / 2 - yOffset + arrowSize, arrowSize, ActualWidth / 2 - yOffset, 0, thickness, black);
            canvas.Children.Add(yArrow1);
            canvas.Children.Add(yArrow2);
        }

        private void DrawCoordinates()
        {
            //step and custom offset for OX axes
            const int xOffset = 350;
            double xStep = (ActualWidth - xOffset) / 9;
            for (double x = 0; x < ActualWidth; x += xStep)
            {
                //calculate value of coordinate
                var value = -(ActualWidth - xOffset) / 2 + x;
                value *= xScale;

                //add label
                var label = new Label
                {
                    Content = Math.Round(value, (value > 10 || value < -10) ? 0 : 1),
                    FontSize = 11,
                    Margin = new Thickness(x, ActualHeight / 2 - 15, 0, 0)
                };
                canvas.Children.Add(label);
            }

            //step and custom offset for OY axes
            const int yOffset = 70;
            double yStep = (ActualHeight - yOffset) / 9;
            for (double y = 0; y < ActualHeight; y += yStep)
            {
                //calculate value of coordinate
                var value = (ActualHeight - yOffset) / 2 - y;
                value *= yScale;

                //add label
                var label = new Label
                {
                    Content = Math.Round(value, (value > 10 || value < -10) ? 0 : 1),
                    FontSize = 11,
                    Margin = new Thickness(ActualWidth / 2 - 155, y, 0, 0)
                };
                canvas.Children.Add(label);
            }
        }

        private void DrawFunctions()
        {
            //offset that is used to center functions on canvas
            const int xOffset = 320;
            const int yOffset = 40;

            //draw functions
            foreach (var i in Controller.Functions)
            {
                var line = new Polyline
                {
                    Stroke = i.Brush,
                    StrokeThickness = 2
                };

                //calculate values and add points
                for (int x = 0; x < ActualWidth; x += 2)
                {
                    var valueOfX = -(ActualWidth - xOffset) / 2 + x;
                    valueOfX *= xScale;
                    var y = -i.GetValueInX(valueOfX) / yScale + (ActualHeight - yOffset) / 2;

                    line.Points.Add(new Point(x, y));
                }

                canvas.Children.Add(line);
            }
        }

        private Line CreateLine(double x1, double y1, double x2, double y2, double thickness, Brush brush)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = brush,
                StrokeThickness = thickness
            };
        }
    }
}
