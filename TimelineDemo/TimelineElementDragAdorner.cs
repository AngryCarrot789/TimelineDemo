using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TimelineDemo {
    public class TimelineElementDragAdorner : Adorner {
        private readonly ContentPresenter adorningPresenter;
        private readonly AdornerLayer layer;

        internal TimelineElementControl Data { get; set; }
        internal DataTemplate Template { get; set; }

        private Point _mousePosition;
        public Point MousePosition {
            get => this._mousePosition;
            set {
                if (this._mousePosition != value) {
                    this._mousePosition = value;
                    this.layer.Update(this.AdornedElement);
                }
            }
        }

        public TimelineElementDragAdorner(TimelineElementControl element) : base(element) {
            this.adorningPresenter = new ContentPresenter {Content = element, Opacity = 0.5};
            this.layer = AdornerLayer.GetAdornerLayer(element);
            this.layer.Add(this);
            this.IsHitTestVisible = false;
        }

        public void Detach() {
            this.layer.Remove(this);
        }

        protected override Visual GetVisualChild(int index) {
            return this.adorningPresenter;
        }

        protected override Size MeasureOverride(Size constraint) {
            //_adorningContentPresenter.Measure(constraint);
            return new Size(((TimelineElementControl) this.AdornedElement).Width, ((TimelineElementControl) this.AdornedElement).DesiredSize.Height);
        }

        protected override int VisualChildrenCount => 1;

        protected override Size ArrangeOverride(Size finalSize) {
            this.adorningPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform) {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.MousePosition.X - 4, this.MousePosition.Y - 4));
            return result;
        }
    }
}