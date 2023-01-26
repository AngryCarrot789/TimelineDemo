using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TimelineDemo {
    /// <summary>
    /// Interaction logic for TimelineLayerControl.xaml
    /// </summary>
    public partial class TimelineLayerControl : UserControl {
        public TimelineControl Timeline { get; set; }

        public static readonly DependencyProperty UnitZoomProperty =
            DependencyProperty.Register(
                "UnitZoom",
                typeof(double),
                typeof(TimelineLayerControl),
                new FrameworkPropertyMetadata(
                    1d,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) => ((TimelineLayerControl) d).OnUnitZoomChanged((double) e.OldValue, (double) e.NewValue),
                    (d, v) => TimelineUtils.ClampUnitZoom(v)));

        public static readonly DependencyProperty FrameOffsetProperty =
            DependencyProperty.Register(
                "FrameOffset",
                typeof(double),
                typeof(TimelineLayerControl),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) => ((TimelineLayerControl) d).OnFrameOffsetChanged((double) e.OldValue, (double) e.NewValue)));

        private bool isUpdatingUnitZoom;
        private bool isUpdatingFrameOffset;

        public TimelineLayerControl() {
            this.InitializeComponent();
            this.CreateElement(0, 100);
            this.CreateElement(105, 30);
            this.CreateElement(200, 50);
        }

        private void OnUnitZoomChanged(double oldZoom, double newZoom) {
            if (this.isUpdatingUnitZoom)
                return;

            this.isUpdatingUnitZoom = true;
            if (Math.Abs(oldZoom - newZoom) > TimelineUtils.MinUnitZoom) {
                foreach (TimelineElementControl element in this.GetElements()) {
                    element.UnitZoom = newZoom;
                }
            }
            this.isUpdatingUnitZoom = false;
        }

        private void OnFrameOffsetChanged(double oldOffset, double newOffset) {
            if (this.isUpdatingFrameOffset)
                return;

            this.isUpdatingFrameOffset = true;
            if (Math.Abs(oldOffset - newOffset) > TimelineUtils.MinUnitZoom) {
                foreach (TimelineElementControl element in this.GetElements()) {
                    element.FrameOffset = newOffset;
                }
            }
            this.isUpdatingFrameOffset = false;
        }

        /// <summary>
        /// The zoom level of this timeline layer
        /// <para>
        /// This is a value used for converting frames into pixels
        /// </para>
        /// </summary>
        public double UnitZoom {
            get => (double) this.GetValue(UnitZoomProperty);
            set => this.SetValue(UnitZoomProperty, value);
        }

        /// <summary>
        /// The offset (in frames as a decimal number) of the timeline elements (0 or above)
        /// </summary>
        public double FrameOffset {
            get => (double) this.GetValue(FrameOffsetProperty);
            set => this.SetValue(FrameOffsetProperty, value);
        }

        public double GetRenderX(TimelineElementControl control) {
            return control.Margin.Left;
            // return Canvas.GetLeft(control);
        }

        public void SetRenderX(TimelineElementControl control, double value) {
            control.Margin = new Thickness(value, control.Margin.Top, control.Margin.Right, control.Margin.Bottom);
            // Canvas.SetLeft(control, value);
        }

        public double GetRenderY(TimelineElementControl control) {
            return control.Margin.Top;
            // return Canvas.GetTop(control);
        }

        public void SetRenderY(TimelineElementControl control, double value) {
            control.Margin = new Thickness(control.Margin.Left, value, control.Margin.Right, control.Margin.Bottom);
            // Canvas.SetTop(control, value);
        }

        public IEnumerable<TimelineElementControl> GetElements() {
            return this.ElementGrid.Children.Cast<TimelineElementControl>();
        }

        public TimelineElementControl CreateElement(int startFrame, int durationFrames) {
            TimelineElementControl element = new TimelineElementControl {
                TimelineLayer = this
            };

            this.ElementGrid.Children.Add(element);
            this.OnElementChildrenChanged();

            element.FrameBegin = startFrame;
            element.FrameDuration = durationFrames;
            element.UnitZoom = this.UnitZoom;
            return element;
        }

        public void OnElementChildrenChanged() {

        }
    }
}
