using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

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
        private DispatcherTimer timer = new DispatcherTimer();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                hwndHostElement = Template.FindName("HwndHostElement", this) as Border;

                Loaded += (sender, args) => InitEther();
                SizeChanged += OnSizeChanged;

                CompositionTarget.Rendering += (sender, args) =>
                {
                    Ether.Update();
                    Ether.Render();
                };
            }
        }

        private bool etherInitialized;

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!etherInitialized) return;
            Size size = GetElementPixelSize(this);
            Ether.ResizeViewport((uint)size.Width, (uint)size.Height);
        }

        private void InitEther()
        {
            Size size = GetElementPixelSize(this);
            rendererControl = new RendererHost(size.Height, size.Width);
            hwndHostElement.Child = rendererControl;
            rendererControl.MessageHook += new HwndSourceHook(WndProcMsgFilter);
            Ether.ResizeViewport((uint)size.Width, (uint)size.Height);
            timer.Tick += (o, eventArgs) => {
                Ether.Update();
                Ether.Render();
            };
//            timer.Start();
            etherInitialized = true;
        }

        public Size GetElementPixelSize(UIElement element)
        {
            Matrix transformToDevice;
            var source = PresentationSource.FromVisual(element);
            if (source != null)
                transformToDevice = source.CompositionTarget.TransformToDevice;
            else
                using (var source2 = new HwndSource(new HwndSourceParameters()))
                    transformToDevice = source2.CompositionTarget.TransformToDevice;

            Size transformedSize =(Size)transformToDevice.Transform((Vector)element.RenderSize);
            transformedSize.Height = Math.Ceiling(transformedSize.Height);
            transformedSize.Width = Math.Ceiling(transformedSize.Width);
            return transformedSize;
        }

        private IntPtr WndProcMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg.Equals(0x0021)) RendererHost.SendMessage(rendererControl.HwndRenderer, 0x0006, new IntPtr(1), new IntPtr(0));
            handled = false;
            return IntPtr.Zero;
        }

        class Ether
        {
            [DllImport("Ether.dll")]
            public static extern IntPtr Initialize(IntPtr hwndParent);

            [DllImport("Ether.dll")]
            public static extern void Update();

            [DllImport("Ether.dll")]
            public static extern void Render();

            [DllImport("Ether.dll")]
            public static extern void Release();

            [DllImport("Ether.dll")]
            public static extern void ResizeViewport(uint width, uint height);
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

                hwndRenderer = Ether.Initialize(hwndHost);

                return new HandleRef(this, hwndHost);
            }

            protected override void DestroyWindowCore(HandleRef hwnd)
            {
                Ether.Release();
                DestroyWindow(hwnd.Handle);
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

            [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
            internal static extern int SendMessage(IntPtr hwnd,
                int msg,
                IntPtr wParam,
                IntPtr lParam);

            [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
            internal static extern int SendMessage(IntPtr hwnd,
                int msg,
                int wParam,
                [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lParam);

            [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SendMessage(IntPtr hwnd,
                int msg,
                IntPtr wParam,
                String lParam);


        }
    }
}
