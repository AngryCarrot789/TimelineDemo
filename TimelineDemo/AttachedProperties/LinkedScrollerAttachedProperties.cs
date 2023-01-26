using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;

namespace TimelineDemo.AttachedProperties {
    // https://stackoverflow.com/questions/21039495/synchronising-offsets-between-two-scrollviewers-without-code-behind
    public static class LinkedScrollerAttachedProperties {
        public class LinkedScrollViewer {
            public ScrollViewer ScrollViewer { get; set; }

            public bool ScrollHorizontally { get; set; } = false;
            public bool ScrollVertically { get; set; } = true;
        }

        public static readonly DependencyProperty LinkedScrollViewersProperty =
            DependencyProperty.RegisterAttached(
                "LinkedScrollViewers",
                typeof(ObservableCollection<LinkedScrollViewer>),
                typeof(LinkedScrollerAttachedProperties),
                new UIPropertyMetadata(null, OnLinkedScrollViewersChanged));

        public static readonly DependencyProperty IsBeingScrolledProperty =
            DependencyProperty.RegisterAttached(
                "IsBeingScrolled",
                typeof(bool),
                typeof(LinkedScrollerAttachedProperties),
                new UIPropertyMetadata(false));

        public static ObservableCollection<LinkedScrollViewer> GetLinkedScrollViewers(DependencyObject o) {
            return (ObservableCollection<LinkedScrollViewer>) o.GetValue(LinkedScrollViewersProperty);
        }

        public static void SetLinkedScrollViewers(DependencyObject o, ObservableCollection<LinkedScrollViewer> value) {
            o.SetValue(LinkedScrollViewersProperty, value);
        }

        public static bool GetIsBeingScrolled(DependencyObject o) {
            return (bool) o.GetValue(LinkedScrollViewersProperty);
        }

        public static void SetIsBeingScrolled(DependencyObject o, bool value) {
            o.SetValue(LinkedScrollViewersProperty, value);
        }

        public static void OnLinkedScrollViewersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.OldValue is ObservableCollection<LinkedScrollViewer> oldList) {
                foreach (LinkedScrollViewer item in oldList) {
                    if (item.ScrollViewer != null) {
                        item.ScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                    }
                }

                oldList.CollectionChanged -= OnListChanged;
            }

            if (e.NewValue is ObservableCollection<LinkedScrollViewer> newList) {
                foreach (LinkedScrollViewer item in newList) {
                    if (item.ScrollViewer != null) {
                        item.ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                    }
                }

                newList.CollectionChanged += OnListChanged;
            }

            ScrollViewer scroller = (ScrollViewer) d;
            ScrollViewer linkedScroller = e.NewValue as ScrollViewer;
            if (linkedScroller != null) {
                linkedScroller.ScrollToHorizontalOffset(scroller.HorizontalOffset);
                linkedScroller.ScrollToVerticalOffset(scroller.VerticalOffset);
            }
        }

        private static void OnListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            foreach (LinkedScrollViewer item in e.OldItems) {
                if (item.ScrollViewer != null) {
                    item.ScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                }
            }

            foreach (LinkedScrollViewer item in e.NewItems) {
                if (item.ScrollViewer != null) {
                    item.ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            // a -> b, c
            // b -> a, c
            // c -> a, b
            // Scrolling a will scroll b which scrolls a again which scrolls b
            if (!(sender is ScrollViewer source)) {
                return;
            }

            ObservableCollection<LinkedScrollViewer> linked = GetLinkedScrollViewers(source);
            if (linked != null && linked.Count > 0) {
                foreach (LinkedScrollViewer viewer in linked) {
                    if (viewer.ScrollViewer == null || GetIsBeingScrolled(viewer.ScrollViewer)) {
                        continue;
                    }

                    try {
                        SetIsBeingScrolled(viewer.ScrollViewer, true);
                        if (viewer.ScrollHorizontally) {
                            viewer.ScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
                        }

                        if (viewer.ScrollVertically) {
                            viewer.ScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                        }
                    }
                    finally {
                        SetIsBeingScrolled(viewer.ScrollViewer, false);
                    }
                }
            }
        }
    }
}
