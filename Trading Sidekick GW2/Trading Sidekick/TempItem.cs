using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Trading_Sidekick
{
	public class TempItem
	{
		public string Heading { get; set; }
		public string Desc { get; set; }
		public int ImageResourceId { get; set; }
	}
}