using System;
using System.ComponentModel;
using System.Diagnostics;
using Foundation;
using MyMindV3;
using MyMindV3.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PullToRefreshView), typeof(PullToRefreshViewRenderer))]
namespace MyMindV3.iOS
{
    [Preserve(AllMembers = true)]
    public class PullToRefreshViewRenderer : ViewRenderer<PullToRefreshView, UIView>
    {
        public async static void Init()
        {
            var temp = DateTime.Now;
        }

        UIRefreshControl refreshControl;
        BindableProperty rendererProperty;
        bool isRefreshing;

        BindableProperty RendererProperty
        {
            get
            {
                if (rendererProperty != null)
                    return rendererProperty;

                var type = Type.GetType("Xamarin.Forms.Platform.iOS.Platform, Xamarin.Forms.Platform.iOS");
                var prop = type.GetField("RendererProperty");
                var val = prop.GetValue(null);
                rendererProperty = val as BindableProperty;

                return rendererProperty;
            }
        }

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                if (isRefreshing)
                    refreshControl.BeginRefreshing();
                else
                    refreshControl.EndRefreshing();
            }
        }

        public PullToRefreshView RefreshView
        {
            get { return Element; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<PullToRefreshView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            refreshControl = new UIRefreshControl();

            refreshControl.ValueChanged += OnRefresh;

            try
            {
                TryInsertRefresh(this);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("View is not supported in PullToRefreshLayout: " + ex);
#endif
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();
        }

        bool TryInsertRefresh(UIView view, int index = 0)
        {

            if (view is UITableView)
            {
                view.InsertSubview(refreshControl, index);
                return true;
            }

            if (view is UICollectionView)
            {
                view.InsertSubview(refreshControl, index);
                return true;
            }

            if (view is UIWebView)
            {
                view.InsertSubview(refreshControl, index);
                return true;
            }

            var uIScrollView = view as UIScrollView;
            if (uIScrollView != null)
            {
                view.InsertSubview(refreshControl, index);
                uIScrollView.AlwaysBounceVertical = true;
                return true;
            }

            if (view.Subviews == null)
                return false;

            for (var i = 0; i < view.Subviews.Length; i++)
            {
                var control = view.Subviews[i];
                if (TryInsertRefresh(control, i))
                    return true;
            }

            return false;
        }

        void UpdateColors()
        {
            if (RefreshView == null)
                return;
            if (RefreshView.RefreshColor != Color.Default)
                refreshControl.TintColor = RefreshView.RefreshColor.ToUIColor();
            if (RefreshView.RefreshBackgroundColor != Color.Default)
                refreshControl.BackgroundColor = RefreshView.RefreshBackgroundColor.ToUIColor();
        }


        void UpdateIsRefreshing()
        {
            IsRefreshing = RefreshView.IsRefreshing;
        }

        void UpdateIsSwipeToRefreshEnabled()
        {
            refreshControl.Enabled = RefreshView.IsPullToRefreshEnabled;
        }

        void OnRefresh(object sender, EventArgs e)
        {
            if (RefreshView == null)
                return;

            var command = RefreshView.RefreshCommand;
            if (command == null)
                return;

            command.Execute(null);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == PullToRefreshView.IsPullToRefreshEnabledProperty.PropertyName)
                UpdateIsSwipeToRefreshEnabled();
            else if (e.PropertyName == PullToRefreshView.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == PullToRefreshView.RefreshColorProperty.PropertyName)
                UpdateColors();
            else if (e.PropertyName == PullToRefreshView.RefreshBackgroundColorProperty.PropertyName)
                UpdateColors();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (refreshControl != null)
            {
                refreshControl.ValueChanged -= OnRefresh;
            }
        }
    }
}
