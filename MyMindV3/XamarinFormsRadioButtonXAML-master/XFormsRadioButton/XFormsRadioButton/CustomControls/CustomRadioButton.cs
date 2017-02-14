using System;
using Xamarin.Forms;

namespace XFormsRadioButton.CustomControls
{
    public class CustomRadioButton : View
    {
        public static readonly BindableProperty CheckedProperty = BindableProperty.Create(nameof(Checked), typeof(bool), typeof(CustomRadioButton), false);

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CustomRadioButton), string.Empty);

        public EventHandler<EventArgs<bool>> CheckedChanged;

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomRadioButton), Color.Black);


        /// <summary>
        /// Gets or sets a value indicating whether the control is checked.
        /// </summary>
        /// <value>The checked state.</value>
        public bool Checked
        {
            get
            {
                return this.GetValue<bool>(CheckedProperty);
            }

            set
            {
                this.SetValue(CheckedProperty, value);
                var eventHandler = this.CheckedChanged;
                if (eventHandler != null)
                {

                    eventHandler.Invoke(this, value);
                }
            }
        }

        public string Text
        {
            get
            {
                return this.GetValue<string>(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        public Color TextColor
        {
            get
            {
                return this.GetValue<Color>(TextColorProperty);
            }

            set
            {
                this.SetValue(TextColorProperty, value);
            }
        }

        public int Id { get; set; }



    }


}
