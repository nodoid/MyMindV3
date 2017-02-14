using System;
using Xamarin.Forms;
using MyMindV3;
using MyMindV3.Droid;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using System.Reflection;

[assembly: ExportRenderer(typeof(PullToRefreshView), typeof(PullToRefreshViewRenderer))]
namespace MyMindV3.Droid
{
    [Preserve(AllMembers = true)]
    public class PullToRefreshViewRenderer : SwipeRefreshLayout, IVisualElementRenderer, SwipeRefreshLayout.IOnRefreshListener
    {
        public async static void Init() { var temp = DateTime.Now; }

        public PullToRefreshViewRenderer() : base(Forms.Context) { }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        bool init, refreshing;
        IVisualElementRenderer packed;
        BindableProperty rendererProperty = null;

        BindableProperty RendererProperty
        {
            get
            {
                if (rendererProperty != null)
                    return rendererProperty;

                var type = Type.GetType("Xamarin.Forms.Platform.Android.Platform, Xamarin.Forms.Platform.Android");
                var prop = type.GetField("RendererProperty", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var val = prop.GetValue(null);
                rendererProperty = val as BindableProperty;

                return rendererProperty;
            }
        }

        public override bool Refreshing
        {
            get
            {
                return refreshing;
            }
            set
            {
                try
                {
                    refreshing = value;
                    if (RefreshView != null && RefreshView.IsRefreshing != refreshing)
                        RefreshView.IsRefreshing = refreshing;

                    if (base.Refreshing == refreshing)
                        return;

                    base.Refreshing = refreshing;
                }
                catch (Exception ex)
                {

                }
            }
        }

        public PullToRefreshView RefreshView
        {
            get { return this.Element == null ? null : (PullToRefreshView)Element; }
        }

        public VisualElementTracker Tracker
        {
            get;
            private set;
        }

        public Android.Views.ViewGroup ViewGroup
        {
            get { return this; }
        }

        public VisualElement Element
        {
            get;
            private set;
        }

        public void SetElement(VisualElement element)
        {
            var oldElement = Element;

            if (oldElement != null)
                oldElement.PropertyChanged -= HandlePropertyChanged;

            Element = element;
            if (Element != null)
            {
                UpdateContent();
                Element.PropertyChanged += HandlePropertyChanged;
            }

            if (!init)
            {
                init = true;
                Tracker = new VisualElementTracker(this);
                SetOnRefreshListener(this);
            }

            UpdateColors();
            UpdateIsRefreshing();
            UpdateIsSwipeToRefreshEnabled();

            if (ElementChanged != null)
                ElementChanged(this, new VisualElementChangedEventArgs(oldElement, this.Element));
        }

        void UpdateContent()
        {
            if (RefreshView.Content == null)
                return;

            if (packed != null)
                RemoveView(packed.ViewGroup);

            packed = Platform.CreateRenderer(RefreshView.Content);

            try
            {
                RefreshView.Content.SetValue(RendererProperty, packed);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Unable to sent renderer property, maybe an issue: " + ex);
#endif
            }

            AddView(packed.ViewGroup, LayoutParams.MatchParent);

        }

        void UpdateColors()
        {
            if (RefreshView == null)
                return;
            if (RefreshView.RefreshColor != Color.Default)
                SetColorSchemeColors(RefreshView.RefreshColor.ToAndroid());
            if (RefreshView.RefreshBackgroundColor != Color.Default)
                SetProgressBackgroundColorSchemeColor(RefreshView.RefreshBackgroundColor.ToAndroid());
        }

        void UpdateIsRefreshing()
        {
            Refreshing = RefreshView.IsRefreshing;
        }

        void UpdateIsSwipeToRefreshEnabled()
        {
            Enabled = RefreshView.IsPullToRefreshEnabled;
        }

        public override bool CanChildScrollUp()
        {
            return CanScrollUp(packed.ViewGroup);
        }

        bool CanScrollUp(ViewGroup viewGroup)
        {
            if (viewGroup == null)
                return base.CanChildScrollUp();

            var sdk = (int)global::Android.OS.Build.VERSION.SdkInt;
            if (sdk >= 16)
            {
#if __ANDROID_16__
                if (viewGroup.IsScrollContainer)
                {
                    return base.CanChildScrollUp();
                }
#endif
            }

            for (var i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);
                if (child is Android.Widget.AbsListView)
                {
                    var list = child as Android.Widget.AbsListView;
                    if (list != null)
                    {
                        if (list.FirstVisiblePosition == 0)
                        {
                            var subChild = list.GetChildAt(0);

                            return subChild != null && subChild.Top != 0;
                        }

                        return true;
                    }

                }
                else if (child is Android.Widget.ScrollView)
                {
                    var scrollview = child as Android.Widget.ScrollView;
                    return (scrollview.ScrollY <= 0.0);
                }
                else if (child is Android.Webkit.WebView)
                {
                    var webView = child as Android.Webkit.WebView;
                    return (webView.ScrollY > 0.0);
                }
                else if (child is Android.Support.V4.Widget.SwipeRefreshLayout)
                {
                    return CanScrollUp(child as ViewGroup);
                }
            }

            return false;
        }

        public void OnRefresh()
        {
            if (RefreshView == null)
                return;

            var command = RefreshView.RefreshCommand;
            if (command == null)
                return;

            command.Execute(null);
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
                UpdateContent();
            else if (e.PropertyName == PullToRefreshView.IsPullToRefreshEnabledProperty.PropertyName)
                UpdateIsSwipeToRefreshEnabled();
            else if (e.PropertyName == PullToRefreshView.IsRefreshingProperty.PropertyName)
                UpdateIsRefreshing();
            else if (e.PropertyName == PullToRefreshView.RefreshColorProperty.PropertyName)
                UpdateColors();
            else if (e.PropertyName == PullToRefreshView.RefreshBackgroundColorProperty.PropertyName)
                UpdateColors();
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            packed.ViewGroup.Measure(widthConstraint, heightConstraint);

            return new SizeRequest(new Size(packed.ViewGroup.MeasuredWidth, packed.ViewGroup.MeasuredHeight));
        }

        public void UpdateLayout()
        {
            if (Tracker == null)
                return;

            Tracker.UpdateLayout();
        }

        public void SetLabelFor(int? id)
        {
            throw new NotImplementedException();
        }
    }
}
