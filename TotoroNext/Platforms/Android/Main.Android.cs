using Android.Runtime;

namespace TotoroNext.Droid;

[global::Android.App.ApplicationAttribute(
	Label = "@string/ApplicationName",
	Icon = "@mipmap/icon",
	LargeHeap = true,
	HardwareAccelerated = true,
	Theme = "@style/AppTheme"
)]
public class Application : Microsoft.UI.Xaml.NativeApplication
{
	static Application()
	{
	}

	public Application(IntPtr javaReference, JniHandleOwnership transfer)
		: base(() => new App(), javaReference, transfer)
	{
	}
}
