using System;
using System.Windows.Forms;
namespace UserInterface
{
	/// <summary>
	/// Summary description for FlickerFreeRichEditTextBox.
	/// </summary>
	public class FlickerFreeRichEditTextBox : RichTextBox
	{

// #define WM_PAINT                        0x000F

//#define WM_CUT                          0x0300
//#define WM_COPY                         0x0301
//#define WM_PASTE                        0x0302
//#define WM_CLEAR                        0x0303
//#define WM_UNDO                         0x0304

		const short	WM_PAINT	= 0x00f;
		const short	WM_CUT		= 0x0300;
		const short WM_COPY		= 0x0301;
		const short WM_PASTE	= 0x0302;
		const short WM_CLEAR	= 0x0303;
		const short WM_UNDO		= 0x0304;

		private void InitializeComponent()
		{
			// 
			// FlickerFreeRichEditTextBox
			// 
			this.AcceptsTab = true;
			this.AutoWordSelection = true;
			this.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));

		}

		public FlickerFreeRichEditTextBox()
		{
			//
			// TODO: Add constructor logic here
			//
//			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
//			SetStyle(ControlStyles.DoubleBuffer , true);
//			SetStyle(ControlStyles.UserPaint , true);
		}


		public static bool _Paint = true;
		public int _LastNumberOfLines = -1;
		public int _LastLength = -1;
		protected override void WndProc(ref System.Windows.Forms.Message m) 
		{

			// sometimes we want to eat the paint message so we don't have to see all the 
			//  flicker from when we select the text to change the color.

			if (m.Msg == WM_PAINT) 
			{

				if (_Paint)

					base.WndProc(ref m);

				else

					m.Result = IntPtr.Zero;

			}
			else
			{
				base.WndProc (ref m);
			}
		}

	}
}
