using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Core;
using SciChart.Data.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

namespace Evaluation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            Example1();
        }

        void Example1()
        {
            var chartFile = $"{AppDomain.CurrentDomain.BaseDirectory}chart1.jpg";
            bool useXamlRenderSurface = false;
            Size exportedSize = new Size(600, 400);
            Surface.ExportToFile(chartFile, ExportType.Jpeg, useXamlRenderSurface, exportedSize);
            Process.Start(chartFile);
        }

        void Example2()
        {
            BitmapSource bitmap = Surface.ExportToBitmapSource();
            var chartFile = $"{AppDomain.CurrentDomain.BaseDirectory}chart2.jpg";
            SaveImageToFile(chartFile, bitmap);
            Process.Start(chartFile);
        }

        void Example3()
        {
            var chartFile = $"{AppDomain.CurrentDomain.BaseDirectory}chart3.jpg";
            bool useXamlRenderSurface = false;
            Size? exportedSize = null;
            var bitmapSource = Surface.ExportToBitmapSource(useXamlRenderSurface, exportedSize);
            SaveImageToFile(chartFile, bitmapSource);
            Process.Start(chartFile);
        }

        private void Print2(object sender, RoutedEventArgs e)
        {
            Example2();
        }

        private void Print3(object sender, RoutedEventArgs e)
        {
            Example3();
        }

        public void SaveImageToFile(string filePath, BitmapSource source)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(fileStream);
            }
        }

        private void PrintUIEl(object sender, RoutedEventArgs e)
        {
            var chartFile = $"{AppDomain.CurrentDomain.BaseDirectory}chart4.jpg";

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)Surface.ActualWidth, (int)Surface.ActualHeight, 96, 96, System.Windows.Media.PixelFormats.Default);
            rtb.Render(Surface);
  
            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var fs = System.IO.File.OpenWrite(chartFile))
            {
                jpgEncoder.Save(fs);
            }

            Process.Start(chartFile);
        }
    }
    public class MainWindowViewModel : BindableObject
    {
        DateTime now = DateTime.Now;
        private ObservableCollection<IAxisViewModel> _yAxes = new ObservableCollection<IAxisViewModel>();
        private ObservableCollection<IAxisViewModel> _xAxes = new ObservableCollection<IAxisViewModel>();
        private ObservableCollection<IRenderableSeriesViewModel> _renderableSeries = new ObservableCollection<IRenderableSeriesViewModel>();

        public ObservableCollection<IAxisViewModel> YAxes { get { return _yAxes; } }
        public ObservableCollection<IAxisViewModel> XAxes { get { return _xAxes; } }
        public ObservableCollection<IRenderableSeriesViewModel> RenderableSeries { get { return _renderableSeries; } }

        public MainWindowViewModel()
        {
            YAxes.Add(new NumericAxisViewModel()
            {
                VisibleRange = new DoubleRange(0, 9999),
                VisibleRangeLimit = new DoubleRange(0, 9999),
                Id = "DEFAULT_Y_AXIS",
            });
            XAxes.Add(new DateTimeAxisViewModel()
            {
                VisibleRange = new DateRange(now.AddHours(-6), now),
                VisibleRangeLimit = new DateRange(now.AddHours(-6), now),
                Id = "DefaultXAxis",
            });
            RenderableSeries.Add(GenerateRenderableSeries());
        }

        private IRenderableSeriesViewModel GenerateRenderableSeries()
        {
            return new LineRenderableSeriesViewModel()
            {
                YAxisId = "DEFAULT_Y_AXIS",
                XAxisId = "DefaultXAxis",
                StrokeThickness = 3,
                Stroke = Colors.Red,
                DataSeries = CreateDataSeries()
            };
        }

        private IDataSeries CreateDataSeries()
        {
            var lineData = new XyDataSeries<DateTime, double>();
            Random random = new Random();   
            var start = now.AddHours(-6);
            while(start < now)
            {
                lineData.Append(start, random.Next(0, 9999));
                start = start.AddMinutes(1);
            }
            return lineData;
        }
    }
}
