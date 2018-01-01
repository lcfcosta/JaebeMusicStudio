﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using JaebeMusicStudio.Sound;

namespace JaebeMusicStudio.Widgets
{
    /// <summary>
    /// Interaction logic for NotesEdit.xaml
    /// </summary>
    public partial class NotesEdit : Page
    {
        private Notes notes;
        /// <summary>
        /// how many pixels represents one note
        /// </summary>
        double scaleX = 20;
        double scaleY = 20;
        int offsetY = 80;
        private Note editingElement;
        private FrameworkElement editingVisualElement;
        private Point editingStartposition;

        public NotesEdit(Notes notes)
        {
            this.notes = notes;
            InitializeComponent();
            synthSelect.Selected = notes.Sound;
            synthSelect.Generate();

            Sound.Player.positionChanged += Player_positionChanged;
            showContent();
            scrollHorizontal.ScrollChanged += showTimeLabels;
            notes.Items.CollectionChanged += NotesChanged;
        }

        private void NotesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var note in e.NewItems)
                    {
                        noteAdded(note as Note);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var note in e.OldItems)
                    {
                        noteRemoved(note as Note);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (var note in e.NewItems)
                    {
                        noteAdded(note as Note);
                    }
                    foreach (var note in e.OldItems)
                    {
                        noteRemoved(note as Note);
                    }
                    break;
            }
        }


        private void SynthSelect_OnChanged(SynthSelect arg1, INoteSynth sound)
        {
            notes.Sound = sound;
        }
        void showContent()
        {
            showTimeLabels();
            tracksGrid.Children.Clear();
            tracksContentStackGrid.Children.Clear();
            foreach (var note in notes.Items)
            {
                noteAdded(note);
            }
            Player_positionChanged();
            showNotePitches();
        }
        void showTimeLabels(object a = null, object b = null)
        {
            TimeLabels.Children.Clear();
            double pixelOffset = -scrollHorizontal.HorizontalOffset + scrollHorizontal.Margin.Left;
            var scale = 1 / scaleX * 50;
            var scale1 = Math.Pow(10, Math.Ceiling(Math.Log10(scale)));
            if (scale1 / 5 > scale)
                scale = scale1 / 5;
            else if (scale1 / 2 > scale)
                scale = scale1 / 2;
            else
                scale = scale1;
            for (double i = 0; pixelOffset + scale * i * scaleX < TimeLabels.ActualWidth; i++)
            {
                var text = new TextBlock();
                text.Margin = new Thickness(pixelOffset + scale * i * scaleX - 100, 0, 0, 0);
                text.HorizontalAlignment = HorizontalAlignment.Left;
                text.TextAlignment = TextAlignment.Center;
                text.Width = 200;
                text.Text = (i * scale).ToString();
                TimeLabels.Children.Add(text);

            }
        }
        void showNotePitches(object a = null, object b = null)
        {
            tracksGrid.Children.Clear();

            for (int i = 0; i < offsetY; i++)
            {
                var text = new TextBlock();
                text.Margin = new Thickness(0, (offsetY - i) * scaleY, 0, 0);
                text.HorizontalAlignment = HorizontalAlignment.Stretch;
                text.VerticalAlignment = VerticalAlignment.Top;
                text.TextAlignment = TextAlignment.Left;
                text.Height = scaleY;
                text.Text = Note.GetName(i);
                if (Note.IsPitchBlack(i))
                {
                    text.Background = new SolidColorBrush(Color.FromRgb(200,200,200));
                }
                else
                {
                    text.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                }
                tracksGrid.Children.Add(text);

            }
        }
        private void Player_positionChanged(float obj = 0)
        {
            Dispatcher.Invoke(() =>
            {
                playingPosition.Margin = new Thickness(Sound.Player.position * scaleX, 0, 0, 0);
            });
        }
        private void Page_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    scaleY *= Math.Pow(2, e.Delta / 200f);
                    WholeScrollable.ScrollToVerticalOffset((WholeScrollable.VerticalOffset) * Math.Pow(2, e.Delta / 200f));
                }
                else
                {
                    scaleX *= Math.Pow(2, e.Delta / 200f);
                    scrollHorizontal.ScrollToHorizontalOffset((scrollHorizontal.HorizontalOffset + scrollHorizontal.ActualWidth / 2) * Math.Pow(2, e.Delta / 200f) - scrollHorizontal.ActualWidth / 2);
                }
                showContent();
            }
        }


        private void Track_SoundElementRemoved(Track track, ISoundElement element)
        {

            Dispatcher.Invoke(() =>
            {
                foreach (var trackUI in tracksContentStackGrid.Children)
                {
                    if ((trackUI as FrameworkElement)?.Tag == track)
                    {
                        foreach (var elem in (trackUI as Grid).Children)
                        {
                            if ((elem as Grid)?.Tag == element)
                            {
                                (trackUI as Grid).Children.Remove(elem as Grid);
                                break;
                            }
                        }
                        break;
                    }
                }
            });
        }



        void noteAdded(Note element)
        {
            Dispatcher.Invoke(() =>
            {
                var grid = new Grid();
                var rect = new Rectangle();
                grid.Children.Add(rect);
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 1;

                rect.Fill = Brushes.Green;

                grid.Width = element.Length * scaleX;
                grid.Height = scaleY;
                grid.Margin = new Thickness(element.Offset * scaleX, (offsetY - element.Pitch) * scaleY, 0, 0);
                grid.VerticalAlignment = VerticalAlignment.Top;
                grid.HorizontalAlignment = HorizontalAlignment.Left;

                grid.Tag = element;
                grid.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                //element.positionChanged += Element_positionChanged;

                var menu = new ContextMenu();
                //if (element is Notes)
                //{
                //    var menuOpen = new MenuItem() { Header = "Otwórz" };
                //    menuOpen.Tag = new Object[] { element, trackContainer.Tag };
                //    menuOpen.Click += element_open_Click;
                //    menu.Items.Add(menuOpen);
                //}

                //var menuDuplicate = new MenuItem() { Header = "Duplikuj" };
                //menuDuplicate.Tag = new Object[] { element, trackContainer.Tag };
                //menuDuplicate.Click += element_duplicate_Click;
                //menu.Items.Add(menuDuplicate);

                //var menuClone = new MenuItem() { Header = "Klonuj" };
                //menuClone.Tag = new Object[] { element, trackContainer.Tag };
                //menuClone.Click += element_clone_Click;
                //menu.Items.Add(menuClone);

                var menuDelete = new MenuItem() { Header = "Usun" };
                menuDelete.Tag = element;
                menuDelete.Click += element_delete_Click;
                menu.Items.Add(menuDelete);


                grid.ContextMenu = menu;

                tracksContentStackGrid.Children.Add(grid);
            });
        }

        private void element_delete_Click(object sender, RoutedEventArgs e)
        {
            notes.Items.Remove((sender as FrameworkElement).Tag as Note);
        }

        private void noteRemoved(Note note)
        {
            foreach (var grid in tracksContentStackGrid.Children)
            {
                if ((grid as FrameworkElement)?.Tag == note)
                    tracksContentStackGrid.Children.Remove(grid as FrameworkElement);
                return;
            }
        }
        private void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            editingStartposition = e.GetPosition(this);
            editingElement = (Note)(sender as Grid).Tag;
            editingVisualElement = (FrameworkElement)sender;
        }
        float editCalcNewTime(MouseEventArgs e)
        {
            var timeDifference = (editingStartposition.X - e.GetPosition(this).X) / scaleX;
            var newTime = editingElement.Offset - timeDifference;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var scale = 1 / scaleX * 20;
                var scale1 = Math.Pow(10, Math.Ceiling(Math.Log10(scale)));
                if (scale1 / 5 > scale)
                    scale = scale1 / 5;
                else if (scale1 / 2 > scale)
                    scale = scale1 / 2;
                else
                    scale = scale1;
                newTime = Math.Round(newTime / scale) * scale;
            }
            return (float)newTime;
        }
        int editCalcNewPitch(MouseEventArgs e)
        {
            var timeDifference = -(editingStartposition.Y - e.GetPosition(this).Y) / scaleY;
            var newPitch = editingElement.Pitch - timeDifference;
            return (int)Math.Round(newPitch);
        }
        float AddCalcNewTime(MouseEventArgs e)
        {
            var timeDifference = e.GetPosition(tracksContentStackGrid).X / scaleX;
            var newTime = timeDifference;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var scale = 1 / scaleX * 20;
                var scale1 = Math.Pow(10, Math.Ceiling(Math.Log10(scale)));
                if (scale1 / 5 > scale)
                    scale = scale1 / 5;
                else if (scale1 / 2 > scale)
                    scale = scale1 / 2;
                else
                    scale = scale1;
                newTime = Math.Round(newTime / scale) * scale;
            }
            return (float)newTime;
        }
        int AddCalcNewPitch(MouseEventArgs e)
        {
            var timeDifference = e.GetPosition(tracksContentStackGrid).Y / scaleY;
            var newPitch = offsetY - timeDifference;
            return (int)newPitch;
        }


        private void Page_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && editingElement != null)
            {
                var newTime = editCalcNewTime(e);
                var newPitch = editCalcNewPitch(e);

                editingVisualElement.Margin = new Thickness(newTime * scaleX, (offsetY - newPitch) * scaleY, 0, 0);

            }
        }

        private void Page_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (editingElement != null)
                {
                    var newTime = editCalcNewTime(e);
                    var newPitch = editCalcNewPitch(e);

                    editingElement.Offset = newTime;
                    editingElement.Pitch = newPitch;
                    editingVisualElement = null;
                    editingElement = null;
                }
            }
        }
        static MouseButtonEventArgs lastClick;
        static DateTime lastClickTime;
        private void tracksContentStackGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lastClick != null && (DateTime.Now - lastClickTime).TotalSeconds < 1 && (lastClick.GetPosition(this) - lastClick.GetPosition(this)).Length < 100)
            {
                var newNote = new Note();
                newNote.Offset = AddCalcNewTime(e);
                newNote.Pitch = AddCalcNewPitch(e);
                newNote.Length = 1;
                notes.Items.Add(newNote);
                lastClick = null;
            }
            else
            {
                lastClick = e;
                lastClickTime = DateTime.Now;
            }
        }
    }
}
