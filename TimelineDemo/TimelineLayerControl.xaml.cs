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

        /// <summary>
        /// Creates a deep cloned timeline element using the exact same data as the given element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TimelineElementControl CreateClonedElement(TimelineElementControl element) {
            if (element.TimelineLayer != this) {
                throw new ArgumentException("Element's timeline layer does not equal the current instance");
            }

            TimelineElementControl cloned = new TimelineElementControl {
                TimelineLayer = this
            };

            this.ElementGrid.Children.Add(cloned);
            this.OnElementChildrenChanged();

            cloned.FrameOffset = element.FrameOffset;
            cloned.UnitZoom = element.UnitZoom;
            cloned.FrameBegin = element.FrameBegin;
            cloned.FrameDuration = element.FrameDuration;
            return cloned;
        }

        public TimelineElementControl CreateElement(int startFrame, int durationFrames) {
            TimelineElementControl element = new TimelineElementControl {
                TimelineLayer = this
            };

            this.ElementGrid.Children.Add(element);
            this.OnElementChildrenChanged();

            element.FrameOffset = this.FrameOffset;
            element.UnitZoom = this.UnitZoom;
            element.FrameBegin = startFrame;
            element.FrameDuration = durationFrames;
            return element;
        }

        /// <summary>
        /// Removes the given clip from this timeline layer
        /// </summary>
        /// <param name="element"></param>
        public bool DestroyClip(TimelineElementControl element) {
            int index = this.ElementGrid.Children.IndexOf(element);
            if (index == -1) {
                return false;
            }

            this.ElementGrid.Children.Remove(element);
            this.OnElementChildrenChanged();
            return true;
        }

        public void OnElementChildrenChanged() {

        }

        public void OnClipDragged(TimelineElementControl element, TimelineElementMoveData data) {

        }
    }
}
