using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        /// <para>
        /// TODO: Remove this because the rendering offset should be done somewhere better! This just introduces more hassle and bugs
        /// </para>
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

        public TimelineLayerControl TimelineLayer { get; set; }

        private bool isUpdatingUnitZoom;
        private bool isUpdatingFrameOffset;
        private bool isUpdatingFrameBegin;
        private bool isUpdatingFrameDuration;

        private TimelineElementMoveData moveDrag;

        private bool isMovingControl;
        private Point lastMousePoint;

        public TimelineElementControl() {
            this.InitializeComponent();
            this.Focusable = true;
        }

        public Point GetMousePositionRelativeToTimelineLayer(MouseDevice mouse) {
            return mouse.GetPosition(this.TimelineLayer);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            this.moveDrag = new TimelineElementMoveData(this) {
                FrameBegin = this.FrameBegin
            };

            this.lastMousePoint = e.GetPosition(this);
            this.CaptureMouse();
            this.Focus();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (this.IsMouseCaptured) {
                this.FinishCompletedDragMove();
                this.ReleaseMouseCapture();
            }
        }

        private void FinishCompletedDragMove() {
            if (this.moveDrag != null) {
                this.moveDrag.OnDragComplete();
                this.moveDrag = null;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            if (this.moveDrag == null) {
                return;
            }

            switch (e.Key) {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    this.HandleDrag_CopyClipMode();
                    return;
                case Key.LeftShift:
                case Key.RightShift:
                    this.HandleDrag_MoveClipMode();
                    return;
                case Key.Escape:
                    this.HandleDrag_CancelDrag();
                    return;
            }
        }

        private const bool CREATE_COPY_AT_DRAG_START_LOCATION = true;

        private void HandleDrag_CopyClipMode() {
            if (this.moveDrag.CopiedElement == null) {
                this.moveDrag.CopiedElement = this.TimelineLayer.CreateClonedElement(this);
                if (CREATE_COPY_AT_DRAG_START_LOCATION) {
                    this.moveDrag.CopiedElement.FrameBegin = this.moveDrag.FrameBegin;
                }

                this.moveDrag.IsCopyDrop = true;
            }
        }

        private void HandleDrag_MoveClipMode() {
            this.moveDrag.DestroyCopiedClip();
            this.moveDrag.IsCopyDrop = false;
        }
        private void HandleDrag_CancelDrag() {
            this.moveDrag.OnDragCancelled();
            this.moveDrag = null;
            this.ReleaseMouseCapture();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (this.isMovingControl) {
                return;
            }

            if (this.IsMouseCaptured) {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) {
                    this.HandleDrag_CopyClipMode();
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) {
                    this.HandleDrag_MoveClipMode();
                }

                if (Keyboard.IsKeyDown(Key.Escape)) {
                    this.HandleDrag_CancelDrag();
                    return;
                }

                Point mousePoint = e.GetPosition(this);
                double difference = mousePoint.X - this.lastMousePoint.X;
                if (Math.Abs(difference) >= 1.0d) {
                    this.isMovingControl = true;
                    this.FrameBegin += (int) (difference / this.UnitZoom);
                    this.isMovingControl = false;
                }
            }
            else if (this.moveDrag != null) {
                if (e.LeftButton == MouseButtonState.Released) {
                    return;
                }
                else {
                    this.moveDrag.OnDragCancelled();
                    this.moveDrag = null;
                }
            }
        }

        private void OnUnitZoomChanged(double oldZoom, double newZoom) {
            if (this.isUpdatingUnitZoom)
                return;
            this.isUpdatingUnitZoom = true;
            if (Math.Abs(oldZoom - newZoom) > TimelineUtils.MinUnitZoom)
                this.UpdatePositionAndSize();
            this.isUpdatingUnitZoom = false;
        }

        private void OnFrameOffsetChanged(double oldOffset, double newOffset) {
            if (this.isUpdatingFrameOffset)
                return;

            this.isUpdatingFrameOffset = true;
            if (Math.Abs(oldOffset - newOffset) > TimelineUtils.MinUnitZoom)
                this.UpdatePosition();
            this.isUpdatingFrameOffset = false;
        }

        private void OnFrameBeginChanged(int oldStart, int newStart) {
            if (this.isUpdatingFrameBegin)
                return;
            this.isUpdatingFrameBegin = true;
            if (oldStart != newStart)
                this.UpdatePosition();
            this.isUpdatingFrameBegin = false;
        }

        private void OnFrameDurationChanged(int oldDuration, int newDuration) {
            if (this.isUpdatingFrameDuration)
                return;
            this.isUpdatingFrameDuration = true;
            if (oldDuration != newDuration)
                this.UpdateSize();
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
