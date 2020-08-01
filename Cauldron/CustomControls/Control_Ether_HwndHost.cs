using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "HwndHostElement", Type = typeof(Border))]
    public class Control_Ether_HwndHost : Control
    {
        static Control_Ether_HwndHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Ether_HwndHost), new FrameworkPropertyMetadata(typeof(Control_Ether_HwndHost)));
        }

        private RendererHost rendererControl;
        private Border hwndHostElement;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                hwndHostElement = Template.FindName("HwndHostElement", this) as Border;

                CompositionTarget.Rendering += OnCompositionTargetOnRendering;
            }
        }

        private bool hostContainerInitialized = false;
        private void OnCompositionTargetOnRendering(object sender, EventArgs args)
        {
            if (!hostContainerInitialized)
            {
                rendererControl = new RendererHost(hwndHostElement.ActualHeight, hwndHostElement.ActualWidth);
                hwndHostElement.Child = rendererControl;
                Ether.Initialize(rendererControl.HwndRenderer);
                hostContainerInitialized = true;
            }
            Ether.Update();
        }


        class Ether
        {
            [DllImport("Ether.dll")]
            public static extern void Initialize(IntPtr window);

            [DllImport("Ether.dll")]
            public static extern void Update();

            [DllImport("Ether.dll")]
            public static extern void Release();

            [DllImport("Ether.dll")]
            public static extern IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        }

        public class RendererHost : HwndHost
        {
            internal const int
                WS_CHILD = 0x40000000,
                WS_VISIBLE = 0x10000000,
                LBS_NOTIFY = 0x00000001,
                HOST_ID = 0x00000002,
                CHILD_ID = 0x00000001,
                WS_VSCROLL = 0x00200000,
                WS_BORDER = 0x00800000;


            IntPtr hwndRenderer;
            IntPtr hwndHost;
            int hostHeight, hostWidth;

            public RendererHost(double height, double width)
            {
                hostHeight = (int)height;
                hostWidth = (int)width;
            }

            public IntPtr HwndRenderer
            {
                get { return hwndRenderer; }
            }

            protected override HandleRef BuildWindowCore(HandleRef hwndParent)
            {
                hwndRenderer = IntPtr.Zero;
                hwndHost = IntPtr.Zero;

                hwndHost = CreateWindowEx(0, "static", "",
                    WS_CHILD | WS_VISIBLE,
                    0, 0,
                    hostWidth, hostHeight,
                    hwndParent.Handle,
                    (IntPtr)HOST_ID,
                    IntPtr.Zero,
                    0);

                hwndRenderer = CreateWindowEx(0, "static", "",
                    WS_CHILD | WS_VISIBLE,
                    0, 0,
                    hostWidth, hostHeight,
                    hwndHost,
                    (IntPtr)CHILD_ID,
                    IntPtr.Zero,
                    0);

                return new HandleRef(this, hwndHost);
            }

            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                DestroyWindow(hwnd.Handle);
            }

            protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                handled = false;
                return IntPtr.Zero;
            }

            [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Unicode)]
            internal static extern bool DestroyWindow(IntPtr hwnd);

            //PInvoke declarations
            [DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Unicode)]
            internal static extern IntPtr CreateWindowEx(int dwExStyle,
                string lpszClassName,
                string lpszWindowName,
                int style,
                int x, int y,
                int width, int height,
                IntPtr hwndParent,
                IntPtr hMenu,
                IntPtr hInst,
                [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        }
    }
}
