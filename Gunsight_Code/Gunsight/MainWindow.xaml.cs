using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;
using Helper;


namespace Gunsight
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double Ximg = Properties.Settings.Default.x, Yimg = Properties.Settings.Default.y;
        public int aim = 1, unique = 1;
        public List<string> imgs = new List<string>() { "Gunsights/1.png", "Gunsights/2.png", "Gunsights/3.png", "Gunsights/4.png", "Gunsights/5.png", "Gunsights/6.png", "Gunsights/7.png" };
        KeyHelper kh = new KeyHelper();

        public MainWindow()
        {
            InitializeComponent();
            Img.Source = new BitmapImage(new Uri(Properties.Settings.Default.Path, UriKind.RelativeOrAbsolute));
            foreach (string item in imgs)
                if (item == Properties.Settings.Default.Path)
                    unique = 0;
            if (unique == 1)
                imgs.Add(Properties.Settings.Default.Path);
            Img.Opacity = Properties.Settings.Default.Opacity;
            Img.Margin = new Thickness(Ximg, Yimg, 0, 0);
            Img.IsEnabled = false;
            kh.KeyDown += Kh_KeyDown;
        }
        private void Kh_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((int)e.KeyCode == (int)Keys.Scroll)
                Img.Opacity = Img.Opacity != 0 ? 0 : 1;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }
        public static class WindowsServices
        {
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int GWL_EXSTYLE = (-20);

            [DllImport("user32.dll")]
            static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

            public static void SetWindowExTransparent(IntPtr hwnd)
            {
                var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
        }
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Insert:
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    openFileDialog.Filter = "Imgs files (*.png)|*.png|All files (*.*)|*.*";
                    if (openFileDialog.ShowDialog() == true)
                        foreach (string filename in openFileDialog.FileNames)
                            imgs.Add(filename);
                    break;
                case Key.Delete:
                    Img.Opacity = 1.0;
                    Ximg = 0;
                    Yimg = 0;
                    Img.Margin = new Thickness(Ximg, Yimg, 0, 0);
                    Properties.Settings.Default.x = Ximg;
                    Properties.Settings.Default.y = Yimg;
                    Properties.Settings.Default.Opacity = Img.Opacity;
                    Properties.Settings.Default.Save();
                    break;
                case Key.Home:
                    if (aim < imgs.Count())
                    {
                        Img.Source = new BitmapImage(new Uri(imgs[aim], UriKind.RelativeOrAbsolute));
                        aim++;
                    }
                    else
                    {
                        Img.Source = new BitmapImage(new Uri(imgs[0], UriKind.RelativeOrAbsolute));
                        aim = 1;
                    }
                    break;
                case Key.PageUp:
                    if (Img.Opacity < 1)
                        Img.Opacity += 0.1;
                    break;
                case Key.PageDown:
                    if (Img.Opacity > 0)
                        Img.Opacity -= 0.1;
                    if (Img.Opacity < 0.1)
                        Img.Opacity = 0;
                    break;
                case Key.Left:
                    Ximg--;
                    Img.Margin = new Thickness(Ximg, Yimg, 0, 0);
                    break;
                case Key.Right:
                    Ximg++;
                    Img.Margin = new Thickness(Ximg, Yimg, 0, 0);
                    break;
                case Key.Up:
                    Yimg--;
                    Img.Margin = new Thickness(Ximg, Yimg, 0, 0);
                    break;
                case Key.Down:
                    Yimg++;
                    Img.Margin = new Thickness(Ximg, Yimg, 0, 0);
                    break;
                case Key.End:
                    Properties.Settings.Default.Path = Img.Source.ToString();
                    Properties.Settings.Default.x = Ximg;
                    Properties.Settings.Default.y = Yimg;
                    Properties.Settings.Default.Opacity = Img.Opacity;
                    Properties.Settings.Default.Save();
                    Close();
                    break;
            }
        }

    }
}
