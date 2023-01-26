using System;
using System.Windows;
using System.Windows.Input;

namespace TimelineDemo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            this.InitializeComponent();
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            double normal_delta = e.Delta > 0 ? 1d : -1d;

            if (this.MyTimeline.IsMouseOver && e.Delta != 0d) {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                    double mouseX = e.GetPosition(this.MyTimeline).X;
                    double cursor_percent = 1d - (2 / this.MyTimeline.ActualWidth * mouseX);
                    double frame_offset = this.MyTimeline.FrameOffset;
                    double oldZoom = this.MyTimeline.UnitZoom;
                    double newZoom = this.MyTimeline.UnitZoom = (oldZoom + (e.Delta > 0 ? 0.1d : -0.1d));

                    // i give up trying to "zoom" into the cursor. fook this

                    // double normalized_offset_px = frame_offset / newZoom;
                    // 
                    // double newX = (mouseX - mouseX * (frame_offset / (frame_offset + (normalized_offset_px * newZoom))));
                    // this.MyTimeline.FrameOffset = frame_offset - (frame_offset / (normalized_offset_px * newZoom));
                    // this.MyTimeline.FrameOffset = normal_delta - newZoom / oldZoom * (normal_delta - (frame_offset * oldZoom));

                    // double final_delta = ((e.Delta / 120d) / oldZoom) * newZoom;
                    // double val = (normal_delta * newZoom) * cursor_percent;
                    // this.MyTimeline.FrameOffset += (1 / frame) * final_delta * cursor_percent * newZoom;

                    e.Handled = true;
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) {
                    this.MyTimeline.FrameOffset += normal_delta * (1d / this.MyTimeline.UnitZoom) * 20d;
                    e.Handled = true;
                }
            }
        }
    }
}
