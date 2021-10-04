using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace PasteHandler
{

		internal static class NativeMethods
		{
			//Reference https://docs.microsoft.com/en-us/windows/desktop/dataxchg/wm-clipboardupdate
			public const int WM_CLIPBOARDUPDATE = 0x031D;
			//Reference https://www.pinvoke.net/default.aspx/Constants.HWND
			public static IntPtr HWND_MESSAGE = new IntPtr(-3);
			//Reference https://www.pinvoke.net/default.aspx/user32/AddClipboardFormatListener.html
			[DllImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool AddClipboardFormatListener(IntPtr hwnd);
			//Reference https://www.pinvoke.net/default.aspx/user32.setparent
			[DllImport("user32.dll", SetLastError = true)]
			public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		}

		public sealed class ClipboardNotification
		{
			private class NotificationForm : Form
			{
				public NotificationForm()
				{
					// Turn the child window into a message-only window (refer to Microsoft docs)
					NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
					// Place window in the system-maintained clipboard format listener list
					NativeMethods.AddClipboardFormatListener(Handle);
				}

				protected override void WndProc(ref Message m)
				{
					// Listen for operating system messages
					if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
					{
						Console.WriteLine("CLIPBOARD_CHANGE");
						HandleClipboardChange();
					}
					// Called for any unhandled messages
					base.WndProc(ref m);
				}
			}

			public static void HandleClipboardChange()
			{
				try
				{
					IDataObject iData = new DataObject();
					iData = Clipboard.GetDataObject();

					if (iData.GetDataPresent(DataFormats.Rtf))
					{
						Clipboard.Clear();
						//Console.WriteLine(iData.GetDataPresent(DataFormats.Text));
					}
					else
					{
						Console.WriteLine("in else");
						//Console.WriteLine("[Clipboard data is not RTF or ASCII Text or FileDrop]");
					}
				}
				catch (Exception e)
				{
					MessageBox.Show(e.ToString());
				}
			}

			[STAThread]
			private static void Main(string[] args)
			{
				Application.Run(new NotificationForm());
			}
		}
	}
