using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace BetterTabControls
{
    public class BetterTab : StackLayout { }

    public class BetterTabs : StackLayout
    {
        Color _selectedColor = Color.White;
        public Color SelectedColor
        {
            get { return _selectedColor; }
            set { _selectedColor = value; }
        }

        Color _unselectedColor = Color.Silver;
        public Color UnselectedColor
        {
            get { return _unselectedColor; }
            set
            {
                _unselectedColor = value;
            }
        }

        internal List<BetterTabButton> TabButtons
        {
            get
            {
                var tabButtons = (BetterTabButtons)Children.First(c => c.GetType() == typeof(BetterTabButtons));
                var buttonEnumerable = tabButtons.Children.Select(c => (BetterTabButton)c);
                var buttonList = buttonEnumerable.Where(c => c.GetType() == typeof(BetterTabButton)).ToList();
                return buttonList;
            }
        }

        internal List<BetterTab> Tabs
        {
            get
            {
                var childList = Children.Where(c => c.GetType() == typeof(BetterTab));
                var tabList = childList.Select(c => (BetterTab)c).ToList();
                return tabList;
            }
        }

        int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;

                if (Tabs.Count > 0)
                    SelectionUIUpdate();
            }
        }

        public BetterTabButton SelectedTabButton
        {
            get { return TabButtons[_selectedTabIndex]; }
            set
            {
                var tabIndex = TabButtons.FindIndex(t => t == value);
                if (tabIndex == -1)
                    throw new Exception(
                       "SelectedTabButton assigned a button " +
                       "that isn't among the children  of " +
                       "BetterTabButtons.");

                if (tabIndex != _selectedTabIndex)
                    SelectedTabIndex = tabIndex;
            }
        }

        public BetterTab SelectedTab
        {
            get { return Tabs[_selectedTabIndex]; }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (TabButtons.Count != Tabs.Count)
            {
                throw new Exception(
                   "The number of tab buttons and the " +
                   "number of tabs to not match.");
            }

            SelectionUIUpdate();
        }

        private void SelectionUIUpdate()
        {
            foreach (var btn in TabButtons)
                btn.BackgroundColor = UnselectedColor;
            SelectedTabButton.BackgroundColor = SelectedColor;

            foreach (var tb in Tabs)
                tb.IsVisible = false;
            SelectedTab.IsVisible = true;
        }
    }
}
