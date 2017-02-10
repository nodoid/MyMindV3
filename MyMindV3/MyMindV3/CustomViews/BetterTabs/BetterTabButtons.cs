using System;
using Xamarin.Forms;

namespace BetterTabControls
{
    public class BetterTabButtons : StackLayout { }

    public class BetterTabButton : Button
    {

        public BetterTabButton()
        {
            Clicked += ThisTabButtonClicked;
        }

        public void ThisTabButtonClicked(object s, EventArgs e)
        {
            BetterTabs prnt = validParentBetterTabs();
            if (prnt == null) return;

            prnt.SelectedTabButton = this;
        }

        private BetterTabs validParentBetterTabs()
        {
            // Work your way up to the grandparent; parent should be
            // BetterTabButtons and grandparent should be BetterTabs
            if (Parent != null && Parent.Parent != null &&
               Parent.Parent.GetType() == typeof(BetterTabs))
                return ((BetterTabs)Parent.Parent);
            else
            {
                throw new Exception(
                   "Grandparent of a BetterTabButton " +
                   "must be a BetterTabs");
            }
        }
    }
}
