using System;
using System.Windows;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Documents;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;

namespace HappyAssignments
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsClickthrough = false;
        protected override void OnSourceInitialized(EventArgs e)
        {
            //Registers the CTRL+F12 global hotkey to hide/unhide things
            HotkeyManager.Current.AddOrReplace("Transparency", Key.F12, ModifierKeys.Control, OnTransparency);
            base.OnSourceInitialized(e);
        }

        private void OnTransparency(object sender, HotkeyEventArgs e)
        {
            //Grabs window handler to make/undo clickthrough
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            
            // Handles hiding elements and checking if its clickthrough. Else handles the opposite.
            if (e.Name == "Transparency" && !IsClickthrough)
            {
                IsClickthrough = true;
                Win32.MakeClickthrough(hwnd);
                Background = Brushes.Transparent;
                btnClose.Visibility = Visibility.Hidden;
                btnNew.Visibility = Visibility.Hidden;
                btnRefresh.Visibility = Visibility.Hidden;
                //cbFileSelect.Visibility = Visibility.Hidden;
                //lblFileSelect.Visibility = Visibility.Hidden;
                ResizeMode = ResizeMode.NoResize;
                rtbContent.IsEnabled = false;
                e.Handled = true;
            }
            else
            {
                IsClickthrough = false;
                Win32.UndoClickthrough(hwnd);
                Background = Brushes.White;
                btnClose.Visibility = Visibility.Visible;
                btnNew.Visibility = Visibility.Visible;
                btnRefresh.Visibility = Visibility.Visible;
                //cbFileSelect.Visibility = Visibility.Visible;
                //lblFileSelect.Visibility = Visibility.Visible;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                rtbContent.IsEnabled = true;
                e.Handled = true;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Lets us move the window just by clicking on the form.
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            TextRange range;
            FileStream fStream;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".rtf";
            dialog.Filter = "Fight Strategy (.rtf)|*.rtf";

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                if (string.IsNullOrEmpty(dialog.FileName))
                {
                    throw new ArgumentNullException();
                }
                // open the file for reading
                using (FileStream stream = File.OpenWrite(dialog.FileName))
                {
                    // create a TextRange around the entire document
                    TextRange documentTextRange = new TextRange(rtbContent.Document.ContentStart, rtbContent.Document.ContentEnd);


                    // sniff out what data format you've got
                    string dataFormat = DataFormats.Text;
                    string ext = System.IO.Path.GetExtension(dialog.FileName);
                    if (String.Compare(ext, ".xaml", true) == 0)
                    {
                        dataFormat = DataFormats.Xaml;
                    }
                    else if (String.Compare(ext, ".rtf", true) == 0)
                    {
                        dataFormat = DataFormats.Rtf;
                    }
                    documentTextRange.Save(stream, dataFormat);
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Fight Strategy (.rtf)|*.rtf";
            dialog.DefaultExt = ".rtf";

            Nullable<bool> result = dialog.ShowDialog();
            if (result == true)
            {
                if (string.IsNullOrEmpty(dialog.FileName))
                {
                    throw new ArgumentNullException();
                }
                if (!File.Exists(dialog.FileName))
                {
                    throw new FileNotFoundException();
                }

                // open the file for reading
                using (FileStream stream = File.OpenRead(dialog.FileName))
                {
                    // create a TextRange around the entire document
                    TextRange documentTextRange = new TextRange(rtbContent.Document.ContentStart, rtbContent.Document.ContentEnd);

                    // sniff out what data format you've got
                    string dataFormat = DataFormats.Text;
                    string ext = System.IO.Path.GetExtension(dialog.FileName);
                    if (String.Compare(ext, ".xaml", true) == 0)
                    {
                        dataFormat = DataFormats.Xaml;
                    }
                    else if (String.Compare(ext, ".rtf", true) == 0)
                    {
                        dataFormat = DataFormats.Rtf;
                    }
                    documentTextRange.Load(stream, dataFormat);
                }
            }
        }
    }
}
