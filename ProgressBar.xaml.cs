using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace myRequirements
{
	public partial class ProgressBar : UserControl
	{
		public ProgressBar()
		{
			// Required to initialize variables
			InitializeComponent();
            progressAnimation.Begin();
		}
	}
}