using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TimelineDemo.Timeline {
    /// <summary>
    /// Interaction logic for TimelineControl.xaml
    /// </summary>
    public partial class TimelineControl : UserControl {
        public static readonly DependencyProperty UnitZoomProperty =
            DependencyProperty.Register(
                "UnitZoom",
                typeof(double),
                typeof(TimelineControl),
                new FrameworkPropertyMetadata(
                    1d,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) => ((TimelineControl) d).OnUnitZoomChanged((double) e.OldValue, (double) e.NewValue),
                    (d, v) => TimelineUtils.ClampUnitZoom(v)));

        public static readonly DependencyProperty FrameOffsetProperty =
            DependencyProperty.Register(
                "FrameOffset",
                typeof(double),
                typeof(TimelineControl),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.None,
                    (d, e) => ((TimelineControl) d).OnFrameOffsetChanged((double) e.OldValue, (double) e.NewValue)));

        private bool isUpdatingUnitZoom;
        private bool isUpdatingFrameOffset;

        /// <summary>
        /// The zoom level of all timeline layers
        /// <para>
        /// This is a value used for converting frames into pixels
        /// </para>
        /// </summary>
        public double UnitZoom {
            get => (double) this.GetValue(UnitZoomProperty);
            set => this.SetValue(UnitZoomProperty, value);
        }

        public double FrameOffset {
            get => (double) this.GetValue(FrameOffsetProperty);
            set => this.SetValue(FrameOffsetProperty, value);
        }

        public TimelineControl() {
            this.InitializeComponent();
            this.CreateLayer();
            this.CreateLayer();
            this.CreateLayer();
        }

        private void OnUnitZoomChanged(double oldZoom, double newZoom) {
            if (this.isUpdatingUnitZoom)
                return;
            this.isUpdatingUnitZoom = true;
            if (Math.Abs(oldZoom - newZoom) > TimelineUtils.MinUnitZoom) {
                foreach (TimelineLayerControl element in this.GetLayers()) {
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
                foreach (TimelineLayerControl element in this.GetLayers()) {
                    element.FrameOffset = newOffset;
                }
            }
            this.isUpdatingFrameOffset = false;
        }

        public IEnumerable<TimelineLayerControl> GetLayers() {
            return this.LayersList.Items.Cast<TimelineLayerControl>();
        }

        public TimelineLayerControl CreateLayer() {
            TimelineLayerControl layer = new TimelineLayerControl {
                Timeline = this
            };

            this.LayersList.Items.Add(layer);
            this.OnLayerCollectionChanged();
            return layer;
        }

        public void OnLayerCollectionChanged() {
            // int i = 0;
            // foreach (TimelineLayerControl layer in this.GetLayers()) {
            //     layer.LayerIndexBox.Text = i.ToString();
            //     i++;
            // }
        }
    }
}
