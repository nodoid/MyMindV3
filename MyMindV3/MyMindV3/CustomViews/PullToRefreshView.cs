using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyMindV3
{
    public class PullToRefreshView : ContentView
    {
        public PullToRefreshView()
        {
            IsClippedToBounds = true;
            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        public static readonly BindableProperty IsRefreshingProperty = BindableProperty.Create(nameof(IsRefreshing), typeof(bool), typeof(PullToRefreshView), false);

        public bool IsRefreshing { get { return (bool)GetValue(IsRefreshingProperty); } set { if ((bool)GetValue(IsRefreshingProperty) == value) OnPropertyChanged(nameof(IsRefreshing)); SetValue(IsRefreshingProperty, value); } }

        public static readonly BindableProperty IsPullToRefreshEnabledProperty = BindableProperty.Create(nameof(IsPullToRefreshEnabled), typeof(bool), typeof(PullToRefreshView), true);

        public bool IsPullToRefreshEnabled { get { return (bool)GetValue(IsPullToRefreshEnabledProperty); } set { SetValue(IsPullToRefreshEnabledProperty, value); } }

        public static readonly BindableProperty RefreshCommandProperty = BindableProperty.Create(nameof(RefreshCommand), typeof(ICommand), typeof(PullToRefreshView));

        public ICommand RefreshCommand { get { return (ICommand)GetValue(RefreshCommandProperty); } set { SetValue(RefreshCommandProperty, value); } }

        public static readonly BindableProperty RefreshColorProperty = BindableProperty.Create(nameof(RefreshColor), typeof(Color), typeof(PullToRefreshView), Color.Default);

        public Color RefreshColor { get { return (Color)GetValue(RefreshColorProperty); } set { SetValue(RefreshColorProperty, value); } }

        public static readonly BindableProperty RefreshBackgroundColorProperty = BindableProperty.Create(nameof(RefreshBackgroundColor), typeof(Color), typeof(PullToRefreshView), Color.Default);

        public Color RefreshBackgroundColor { get { return (Color)GetValue(RefreshBackgroundColorProperty); } set { SetValue(RefreshBackgroundColorProperty, value); } }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) { if (Content == null) return new SizeRequest(new Size(100, 100)); return base.OnMeasure(widthConstraint, heightConstraint); }
    }

}

