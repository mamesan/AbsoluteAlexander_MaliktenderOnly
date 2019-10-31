using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AbsoluteAlexander_MaliktenderOnly
{
    public partial class TeropsABCD : Form
    {

        public TeropsABCD()
        {
            InitializeComponent();
            ;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0.8;
        }
        private void SetTopMost()
        {
            const int HWND_TOPMOST = -1;
            const int SWP_NOSIZE = 0x0001;
            const int SWP_NOMOVE = 0x0002;
            const int SWP_NOACTIVATE = 0x0010;
            const int SWP_SHOWWINDOW = 0x0040;
            const int SWP_NOSENDCHANGING = 0x0400;
            // SetWindowPosはどこかでDllImportする
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSENDCHANGING | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        /// <summary>
        /// クリックスルー用のイベント
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd,
        int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);
        private void TeropsABCD_Load_1(object sender, EventArgs e)
        {
            //フォームの境界線をなくす
            SetTopMost();
        }
    }
}
