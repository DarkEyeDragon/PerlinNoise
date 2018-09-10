using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;

namespace SimplexNoise
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // 1D noise variables
        private float[] _fNoiseSeed1D;
        private float[] fPerlinNoise1D;
        private int _nOutputSize;


        int nOctaveCount = 1;
        float fScalingBias = 2.0f;


        //2D noise variables
        private readonly int _nOutputWidth;
        private readonly int _nOutputHeight;
        private float[] _fNoiseSeed2D;
        private float[] fPerlinNoise2D;

        private Random rand;

        private PerlinNoise perlinNoise;

        public MainWindow()
        {
            InitializeComponent();
            perlinNoise = new PerlinNoise();
            _nOutputSize = (int) Canvas.Width;
            _nOutputWidth = (int)Canvas.Width;
            _nOutputHeight = (int)Canvas.Height;
            _nOutputSize = (int)Canvas.Width;
            _fNoiseSeed1D = new float[_nOutputSize];
            fPerlinNoise1D = new float[_nOutputSize];
            rand = new Random();
            for (int i = 0; i < _nOutputSize; i++)
            {
                _fNoiseSeed1D[i] = (float) rand.NextDouble();
            }
            _fNoiseSeed2D = new float[_nOutputWidth * _nOutputHeight];
            fPerlinNoise2D = new float[_nOutputWidth * _nOutputHeight];
            for (int i = 0; i < _nOutputWidth * _nOutputHeight; i++) _fNoiseSeed2D[i] = (float)rand.NextDouble();
        }

        private void ButtonMode2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonMode3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            nOctaveCount = (int)Math.Round(e.NewValue);
            if (nOctaveCount >= 9)
            {
                nOctaveCount = 1;
            }
            DrawOnCanvas();
        }

        private void Draw1D()
        {
            Canvas.Children.Clear();
            for (int x = 0; x < _nOutputSize; x++)
            {
                double y = Canvas.Height - (fPerlinNoise1D[x]*Canvas.Height/2);
                var line = new Line
                {
                    X1 = x,
                    X2 = x,
                    Y1 = y,
                    Y2 = Canvas.Height,
                    StrokeThickness = 1,
                    Stroke = new SolidColorBrush(Colors.Green)
                };
                Canvas.Children.Add(line);
            }
        }

        private void Draw2D()
        {

            Image i = new Image();
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(i, EdgeMode.Aliased);

            int width = (int)Canvas.Width, height = (int)Canvas.Height;
            WriteableBitmap writableBitmap = new WriteableBitmap((int)Canvas.Width, (int)Canvas.Height, 1,1, PixelFormats.Bgr32, null);

            i.Source = writableBitmap;
            i.Stretch = Stretch.None;
            i.HorizontalAlignment = HorizontalAlignment.Left;
            i.VerticalAlignment = VerticalAlignment.Top;
            byte[] colorData = new byte[(_nOutputWidth^2)*3];
            Dictionary<Point, byte[]> dictionary = new Dictionary<Point, byte[]>();
            Task.Run(() =>
            {
                var w = width;
                var h = height;
                WriteableBitmap wb = new WriteableBitmap(w, h, 1, 1, PixelFormats.Bgr32, null);
                for (int x = 0; x < _nOutputWidth; x++)
                {
                    for (int y = 0; y < _nOutputHeight; y++)
                    {
                        Color bg_col = Colors.Red;
                        int pixel_bw = (int) (fPerlinNoise2D[y * _nOutputWidth + x] * 12.0f);
                        switch (pixel_bw)
                        {
                            case 0:
                                bg_col = Colors.DodgerBlue;
                                break;

                            case 1:
                                bg_col = Colors.DeepSkyBlue;
                                break;
                            case 2:
                                bg_col = Colors.SkyBlue;
                                break;
                            case 3:
                                bg_col = Colors.LightSkyBlue;
                                break;
                            case 4:
                                bg_col = Colors.DarkGreen;
                                break;
                            case 5:
                                bg_col = Colors.Green;
                                break;
                            case 6:
                                bg_col = Colors.MediumSeaGreen;
                                break;
                            case 7:
                                bg_col = Colors.LightGreen;
                                break;

                            case 8:
                                bg_col = Colors.SandyBrown;
                                break;
                            case 9:
                                bg_col = Colors.SaddleBrown;
                                break;
                            case 10:
                                bg_col = Colors.Brown;
                                break;
                            case 12:
                                bg_col = Colors.DarkRed;
                                break;
                        }

                        colorData[0] = bg_col.B;
                        colorData[1] = bg_col.G;
                        colorData[2] = bg_col.R;
                        var x1 = x;
                        var y1 = y;
                        wb.WritePixels(new Int32Rect(x1, y1, 1, 1), colorData, 500, 0);
                        /*Application.Current.Dispatcher.Invoke(() =>
                        {
                            wb.WritePixels(new Int32Rect(x1, y1, 1, 1), colorData, 500, 0);
                        });*/

                        //dictionary.Add(new Point(x,y), colorData);
                    }

                }
                wb.Freeze();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    writableBitmap = wb;
                    ImageViewer.Source = BitmapToImageSource(writableBitmap);
                });
                /*Application.Current.Dispatcher.Invoke(() =>
                {
                    int counter = 0;
                    byte[] byteArray = new byte[dictionary.Count];
                    foreach (var bytes in dictionary)
                    {
                        for (int j = 0; j < bytes.Value.Length; j++)
                        {
                            byteArray[j + counter] = bytes.Value[j];
                        }
                        Point point = bytes.Key;
                        counter += 3;
                    }
                    /*for (int x = 0; x < dictionary.Count; x++)
                    {
                        for (int y = 0; y < resultY.Count; y++)
                        {
                            byte[] bytes = resultX[x][y];
                            writableBitmap.WritePixels(new Int32Rect(x, y, 1, 1), bytes, 10, 0);
                        }
                    }

                });*/
            });
            

            /*Canvas.Children.Clear();
            for (int x = 0; x < _nOutputWidth; x += 2)
            {
                Rectangle[] rectangles = new Rectangle[_nOutputHeight];
                for (int y = 0; y < _nOutputHeight; y += 2)
                {
                    Color bg_col = Colors.Red;
                    int pixel_bw = (int) (fPerlinNoise2D[y * _nOutputWidth + x] * 12.0f);
                    switch (pixel_bw)
                    {
                        case 0:
                            bg_col = Colors.Black;
                            break;

                        case 1:
                            bg_col = Colors.DeepSkyBlue;
                            break;
                        case 2:
                            bg_col = Colors.SkyBlue;
                            break;
                        case 3:
                            bg_col = Colors.LightSkyBlue;
                            break;
                        case 4:
                            bg_col = Colors.SandyBrown;
                            break;

                        case 5:
                            bg_col = Colors.DarkGreen;
                            break;
                        case 6:
                            bg_col = Colors.Green;
                            break;
                        case 7:
                            bg_col = Colors.MediumSeaGreen;
                            break;
                        case 8:
                            bg_col = Colors.LightGreen;
                            break;

                        case 9:
                            bg_col = Colors.SaddleBrown;
                            break;
                        case 10:
                            bg_col = Colors.Snow;
                            break;
                        case 11:
                            bg_col = Colors.Snow;
                            break;
                        case 12:
                            bg_col = Colors.LightGray;
                            break;
                    }
                    var pixel = new Rectangle()
                    {
                        Margin = new Thickness(x, y, 0, 0),
                        Width = 2,
                        Height = 2,
                        StrokeThickness = 1,
                        Stroke = new SolidColorBrush(bg_col)
                    };
                    Canvas.Children.Add(pixel);
                }
            }*/
        }
        private void ChangeSeed_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _nOutputSize; i++)
            {
                _fNoiseSeed1D[i] = (float)rand.NextDouble();
                _fNoiseSeed2D[i] = (float)rand.NextDouble();
            }

            DrawOnCanvas();
        }

        private void ButtonMode1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DrawOnCanvas_Click(object sender, RoutedEventArgs e)
        {
            DrawOnCanvas();

        }


        BitmapImage BitmapToImageSource(WriteableBitmap bitmap)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        //Application.Current.Dispatcher.Invoke(new Action(() => { /* Your code here */ }));

        public void DrawOnCanvas()
        {
            if (perlinNoise == null) return;
            Task.Run(() =>
            {
                perlinNoise.Generate1D(_nOutputSize, _fNoiseSeed1D, nOctaveCount);
                perlinNoise.Generate2D(_nOutputWidth, _nOutputHeight, _fNoiseSeed2D, nOctaveCount, fScalingBias);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    fPerlinNoise1D = perlinNoise.Output;
                    fPerlinNoise2D = perlinNoise.Output;
                    Draw1D();
                    Draw2D();
                });

            });
        }
    }
}