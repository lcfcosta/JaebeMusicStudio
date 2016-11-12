﻿using System;
using System.Collections.Generic;
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

namespace JaebeMusicStudio.Widgets
{
    /// <summary>
    /// Interaction logic for Timeline.xaml
    /// </summary>
    public partial class Timeline : Page
    {
        /// <summary>
        /// how many pixels represents one note
        /// </summary>
        double scaleX = 1;
        public Timeline()
        {
            InitializeComponent();
            Sound.Project.current.trackAdded += project_trackAdded;
            Sound.Player.positionChanged += Player_positionChanged;
            showContent();
        }

        private void Player_positionChanged(float obj = 0)
        {
            Dispatcher.Invoke(() =>
            {
                playingPosition.Margin = new Thickness(Sound.Player.position * scaleX, 0, 0, 0);
            });
        }

        void showContent()
        {
            tracksStack.Children.RemoveRange(0, tracksStack.Children.Count - 2);
            tracksContentStack.Children.Clear();
            var index = 0;
            foreach (var track in Sound.Project.current.tracks)
            {
                project_trackAdded(index, track);
                index++;
            }
            Player_positionChanged();
        }
        private void project_trackAdded(int index, Sound.Track track)
        {
            var grid = new Grid();
            grid.Background = Brushes.Aqua;
            grid.Height = 40;
            tracksStack.Children.Insert(index, grid);

            var content = new Grid();
            var line = new Rectangle();
            line.HorizontalAlignment = HorizontalAlignment.Stretch;
            line.VerticalAlignment = VerticalAlignment.Bottom;
            line.Height = 1;
            line.Fill = Brushes.Black;
            content.Children.Add(line);
            content.Height = 40;
            tracksContentStack.Children.Insert(index, content);
            foreach (var element in track.elements)
            {
                project_soundElementAdded(content, element);
            }
        }

        void project_soundElementAdded(Grid trackContainer, Sound.SoundElement element)
        {
            var grid = new Grid();
            var rect = new Rectangle();
            rect.Stroke = Brushes.Black;
            rect.StrokeThickness = 1;
            if (element.GetType() == typeof(Sound.OneSample))
                rect.Fill = Brushes.Red;
            else
                rect.Fill = Brushes.Blue;
            grid.Children.Add(rect);
            grid.Width = element.length * scaleX;
            grid.Margin = new Thickness(element.offset * scaleX, 0, 0, 0);
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            trackContainer.Children.Add(grid);

        }
        private void addNewTrackButton_Click(object sender, RoutedEventArgs e)
        {
            Sound.Project.current.addEmptyTrack();
        }

        private void openFileSampleButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "pliki dźwiękowe|*.wav,*.wave,*.mp3|Wszystkie Pliki|*.*";
            dialog.ShowDialog();
            try
            {
                if (dialog.FileName != "")
                {

                    var ss = new Sound.SampledSound(dialog.FileName);
                }
            }
            catch
            {
                MainWindow.error("Błąd otwarcia pliku");
            }
        }

        private void Page_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                scaleX *= Math.Pow(2, e.Delta / 200f);
                showContent();
            }
        }
    }
}
