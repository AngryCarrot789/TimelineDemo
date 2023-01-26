using System;
using System.Windows;
using System.Windows.Controls;

namespace TimelineDemo {
    /// <summary>
    /// Interaction logic for TimelineElementControl.xaml
    /// </summary>
    public partial class TimelineElementControl : UserControl {
        public static readonly DependencyProperty UnitZoomProperty =
            DependencyProperty.Register(
                "UnitZoom",
                typeof(double),
                typeof(TimelineElementControl),
                new FrameworkPropertyMetadata(
                    1d,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) => ((TimelineElementControl) d).OnUnitZoomChanged((double) e.OldValue, (double) e.NewValue),
                    (d, v) => TimelineUtils.ClampUnitZoom(v)));

        public static readonly DependencyProperty FrameOffsetProperty =
            DependencyProperty.Register(
                "FrameOffset",
                typeof(double),
                typeof(TimelineElementControl),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) => ((TimelineElementControl) d).OnFrameOffsetChanged((double) e.OldValue, (double) e.NewValue)));

        public static readonly DependencyProperty FrameBeginProperty =
            DependencyProperty.Register(
                "FrameBegin",
                typeof(int),
                typeof(TimelineElementControl),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((TimelineElementControl) d).OnFrameBeginChanged((int) e.OldValue, (int) e.NewValue),
                    (d, v) => (int) v < 0 ? 0 : v));

        public static readonly DependencyProperty FrameDurationProperty =
            DependencyProperty.Register(
                "FrameDuration",
                typeof(int),
                typeof(TimelineElementControl),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                    (d, e) => ((TimelineElementControl) d).OnFrameDurationChanged((int) e.OldValue, (int) e.NewValue),
                    (d, v) => (int) v < 0 ? 0 : v));

        public TimelineLayerControl TimelineLayer { get; set; }

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
        /// The number of frames this element's render is offset by
        /// </summary>
        public double FrameOffset {
            get => (double) this.GetValue(FrameOffsetProperty);
            set => this.SetValue(FrameOffsetProperty, value);
        }

        /// <summary>
        /// This element's duration, in frames
        /// </summary>
        public int FrameDuration {
            get => (int) this.GetValue(FrameDurationProperty);
            set => this.SetValue(FrameDurationProperty, value);
        }

        /// <summary>
        /// The zero-based frame index where this element begins (relative to the parent timeline layer)
        /// </summary>
        public int FrameBegin {
            get => (int) this.GetValue(FrameBeginProperty);
            set => this.SetValue(FrameBeginProperty, value);
        }

        /// <summary>
        /// The rendered X position of this element
        /// </summary>
        public double RealPixelX {
            get => this.TimelineLayer?.GetRenderX(this) ?? 0;
            set => this.TimelineLayer?.SetRenderX(this, value);
        }

        /// <summary>
        /// The rendered Y position of this element (should not be modified generally)
        /// </summary>
        public double RealPixelY {
            get => this.TimelineLayer?.GetRenderY(this) ?? 0;
            set => this.TimelineLayer?.SetRenderY(this, value);
        }

        /// <summary>
        /// The calculated width of this element based on the frame duration and zoom
        /// </summary>
        public double PixelWidth {
            get => this.FrameDuration * this.UnitZoom;
        }

        /// <summary>
        /// The calculated render X position of this element based on the start frame, frame offset and zoom
        /// </summary>
        public double PixelStartWithOffset {
            get => (this.FrameBegin + this.FrameOffset) * this.UnitZoom;
        }

        /// <summary>
        /// The calculated render X position of this element based on the start frame and zoom
        /// </summary>
        public double PixelStartWithoutOffset {
            get => this.FrameBegin * this.UnitZoom;
        }

        private bool isUpdatingUnitZoom;
        private bool isUpdatingFrameOffset;
        private bool isUpdatingFrameBegin;
        private bool isUpdatingFrameDuration;

        public TimelineElementControl() {
            this.InitializeComponent();
        }

        private void OnUnitZoomChanged(double oldZoom, double newZoom) {
            if (this.isUpdatingUnitZoom)
                return;
            this.isUpdatingUnitZoom = true;
            if (Math.Abs(oldZoom - newZoom) > TimelineUtils.MinUnitZoom) {
                this.UpdatePositionAndSize();
            }
            this.isUpdatingUnitZoom = false;
        }

        private void OnFrameOffsetChanged(double oldOffset, double newOffset) {
            if (this.isUpdatingFrameOffset)
                return;

            this.isUpdatingFrameOffset = true;
            if (Math.Abs(oldOffset - newOffset) > TimelineUtils.MinUnitZoom) {
                this.UpdatePosition();
            }
            this.isUpdatingFrameOffset = false;
        }

        private void OnFrameBeginChanged(int oldStart, int newStart) {
            if (this.isUpdatingFrameBegin)
                return;
            this.isUpdatingFrameBegin = true;
            if (Math.Abs(oldStart - newStart) > TimelineUtils.MinUnitZoom) {
                this.UpdatePosition();
            }
            this.isUpdatingFrameBegin = false;
        }

        private void OnFrameDurationChanged(int oldDuration, int newDuration) {
            if (this.isUpdatingFrameDuration)
                return;
            this.isUpdatingFrameDuration = true;
            if (Math.Abs(oldDuration - newDuration) > TimelineUtils.MinUnitZoom) {
                this.UpdateSize();
            }
            this.isUpdatingFrameDuration = false;
        }

        public void UpdatePositionAndSize() {
            this.UpdatePosition();
            this.UpdateSize();
        }

        public void UpdatePosition() {
            this.RealPixelX = this.PixelStartWithOffset;
        }

        public void UpdateSize() {
            this.Width = this.PixelWidth;
        }
    }
}
