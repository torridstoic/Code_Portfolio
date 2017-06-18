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

using System.Threading.Tasks;


namespace Trading_Sidekick
{
	[Activity(Label = "Trading_Sidekick", MainLauncher = true, Icon = "@drawable/icon")]
	public class SplashActivity : Activity
	{
		protected async override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.SplashLayout);

			await Load();
			StartActivity(new Intent(this, typeof(MainActivity)));
		}

		private async Task Load()
		{
			Task delay = Task.Delay(3000);
			if (await Global.ReadWatchListAsync())
			{
				Toast.MakeText(this, "Watch list loaded!", ToastLength.Short)
					.Show();
			}
			else
			{
				Toast.MakeText(this, "Error loading watch list file.", ToastLength.Short)
					.Show();
			}
			await delay;
		}
	}
}