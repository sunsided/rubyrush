using System.Windows.Forms;

namespace Ruby_Rush
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            BackgroundImage = CaptureScreen.GetDesktopImage(Width, Height);
        }

        protected override void OnMove(System.EventArgs e)
        {
            base.OnMove(e);
            BackgroundImage = CaptureScreen.GetDesktopImage(Width, Height);
        }
    }
}
