// ***********************************************************************
// Assembly         : XLabs.Forms.iOS
// Author           : XLabs Team
// Created          : 12-27-2015
// 
// Last Modified By : XLabs Team
// Last Modified On : 01-04-2016
// ***********************************************************************
// <copyright file="RadioButtonView.cs" company="XLabs Team">
//     Copyright (c) XLabs Team. All rights reserved.
// </copyright>
// <summary>
//       This project is licensed under the Apache 2.0 license
//       https://github.com/XLabs/Xamarin-Forms-Labs/blob/master/LICENSE
//       
//       XLabs is a open source project that aims to provide a powerfull and cross 
//       platform set of controls tailored to work with Xamarin Forms.
// </summary>
// ***********************************************************************
// 

using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace MyMindV3.iOS
{
    [Register("RadioButtonView")]
    public class RadioButtonView : UIButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonView"/> class.
        /// </summary>
        public RadioButtonView()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonView"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public RadioButtonView(CGRect bounds)
            : base(bounds)
        {
            Initialize();
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadioButtonView"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public bool Checked
        {
            set { Selected = value; }
            get { return Selected; }
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            set { SetTitle(value, UIControlState.Normal); }

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize()
        {
            AdjustEdgeInsets();
            ApplyStyle();

            // set default color, because type is not UIButtonType.System 
            SetTitleColor(UIColor.White, UIControlState.Normal);
            SetTitleColor(UIColor.White, UIControlState.Selected);
            TouchUpInside += (sender, args) => Selected = !Selected;
        }

        /// <summary>
        /// Adjusts the edge insets.
        /// </summary>
        void AdjustEdgeInsets()
        {
            const float inset = 8f;

            HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            ImageEdgeInsets = new UIEdgeInsets(0f, inset, 0f, 0f);
            TitleEdgeInsets = new UIEdgeInsets(0f, inset * 2, 0f, 0f);
        }

        /// <summary>
        /// Applies the style.
        /// </summary>
        void ApplyStyle()
        {
            var testCheckedImage = UIImage.FromBundle("Images/checked.png");
            var testSet = new UIImageView(testCheckedImage)
            {
                Frame = new CGRect(0, 40, testCheckedImage.Size.Width, testCheckedImage.Size.Height * testCheckedImage.CurrentScale)
            };
            var testUnheckedImage = UIImage.FromBundle("Images/unchecked.png");
            var testUnset = new UIImageView(testUnheckedImage)
            {
                Frame = new CGRect(0, 40, testUnheckedImage.Size.Width, testUnheckedImage.Size.Height * testUnheckedImage.CurrentScale)
            };
            SetImage(testSet.Image, UIControlState.Selected);
            SetImage(testUnset.Image, UIControlState.Normal);
        }
    }
}
