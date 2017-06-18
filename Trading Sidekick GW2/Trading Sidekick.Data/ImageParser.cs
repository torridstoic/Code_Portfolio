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

using Android.Graphics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace Trading_Sidekick.Data
{
	/// <summary>
	/// This is a quick wrapper class,
	/// written once here to ease my job of repeatedly acquiring images from the API endpoint.
	/// </summary>
	public static class ImageParser
	{
		public static Bitmap GetBitmap(string iconUrl)
		{
			Bitmap result = null;

			using (WebClient wc = new WebClient())
			{
				try
				{
					byte[] imageBytes = wc.DownloadData(iconUrl);
					if (imageBytes != null && imageBytes.Length > 0)
					{
						result = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
					}
				}
				catch (WebException)
				{
					result = null;
				}
			}

			return result;
		}

		/// <summary>
		/// This method is used in place of the above for cases in which the image must be acquired asynchronously,
		/// in order to not stop the page from loading while acquiring many images.
		/// </summary>
		/// <param name="iconUrl">URL of the desired icon</param>
		/// <returns>The icon in BMP format</returns>
		public static async Task<Bitmap> GetBitmapAsync(string iconUrl)
		{
			Bitmap result = null;

			// We use a HttpClient instead of WebClient here for its improved async functionality.
			using (HttpClient hc = new HttpClient())
			{
				try
				{
					byte[] imageBytes = await hc.GetByteArrayAsync(iconUrl);
					if (imageBytes != null && imageBytes.Length > 0)
					{
						result = await BitmapFactory.DecodeByteArrayAsync(imageBytes, 0, imageBytes.Length);
					}
				}
				catch (HttpRequestException)
				{
					result = null;
				}
			}

			return result;
		}
	}
}