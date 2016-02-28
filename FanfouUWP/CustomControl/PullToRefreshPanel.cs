using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace FanfouUWP.CustomControl
{
    public class PushToRefreshBorder : Panel
    {
        public Size myAvailableSize { get; set; }

        public Size myFinalSize { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.myAvailableSize = availableSize;
            // Children[0] is the outer ScrollViewer
            this.Children[0].Measure(availableSize);
            return this.Children[0].DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.myFinalSize = finalSize;
            // Children[0] is the outer ScrollViewer
            this.Children[0].Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }
    }

    public class PushToRefreshPanel : Panel, IScrollSnapPointsInfo
    {
        EventRegistrationTokenTable<EventHandler<object>> _verticaltable = new EventRegistrationTokenTable<EventHandler<object>>();
        EventRegistrationTokenTable<EventHandler<object>> _horizontaltable = new EventRegistrationTokenTable<EventHandler<object>>();

        protected override Size MeasureOverride(Size availableSize)
        {
            // need to get away from infinity
            var parent = this.Parent as FrameworkElement;
            while (!(parent is PushToRefreshBorder))
            {
                parent = parent.Parent as FrameworkElement;
            }

            var myBorder = parent as PushToRefreshBorder;

            // Children[0] is the Border that comprises the refresh UI
            this.Children[0].Measure(myBorder.myAvailableSize);
            // Children[1] is the ListView
            this.Children[1].Measure(new Size(myBorder.myAvailableSize.Width, myBorder.myAvailableSize.Height));
            return new Size(this.Children[1].DesiredSize.Width, this.Children[0].DesiredSize.Height + myBorder.myAvailableSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // need to get away from infinity
            var parent = this.Parent as FrameworkElement;
            while (!(parent is PushToRefreshBorder))
            {
                parent = parent.Parent as FrameworkElement;
            }

            var myBorder = parent as PushToRefreshBorder;

            // Children[0] is the Border that comprises the refresh UI
            this.Children[0].Arrange(new Rect(0, 0, this.Children[0].DesiredSize.Width, this.Children[0].DesiredSize.Height));
            // Children[1] is the ListView
            this.Children[1].Arrange(new Rect(0, this.Children[0].DesiredSize.Height, myBorder.myFinalSize.Width, myBorder.myFinalSize.Height));
            return finalSize;
        }

        bool IScrollSnapPointsInfo.AreHorizontalSnapPointsRegular
        {
            get { return false; }
        }

        bool IScrollSnapPointsInfo.AreVerticalSnapPointsRegular
        {
            get { return false; }
        }

        IReadOnlyList<float> IScrollSnapPointsInfo.GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment)
        {
            if (orientation == Orientation.Vertical)
            {
                var l = new List<float>();
                l.Add((float)this.Children[0].DesiredSize.Height);
                return l;
            }
            else
            {
                return new List<float>();
            }
        }

        float IScrollSnapPointsInfo.GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment alignment, out float offset)
        {
            throw new NotImplementedException();
        }

        event EventHandler<object> IScrollSnapPointsInfo.HorizontalSnapPointsChanged
        {
            add
            {
                return EventRegistrationTokenTable<EventHandler<object>>
                        .GetOrCreateEventRegistrationTokenTable(ref this._horizontaltable)
                        .AddEventHandler(value);

            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<object>>
                    .GetOrCreateEventRegistrationTokenTable(ref this._horizontaltable)
                    .RemoveEventHandler(value);
            }
        }

        event EventHandler<object> IScrollSnapPointsInfo.VerticalSnapPointsChanged
        {
            add
            {
                return EventRegistrationTokenTable<EventHandler<object>>
                        .GetOrCreateEventRegistrationTokenTable(ref this._verticaltable)
                        .AddEventHandler(value);

            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<object>>
                    .GetOrCreateEventRegistrationTokenTable(ref this._verticaltable)
                    .RemoveEventHandler(value);
            }
        }
    }
}
