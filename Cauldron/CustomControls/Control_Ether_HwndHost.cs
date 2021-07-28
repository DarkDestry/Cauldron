using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cauldron.Core;
using Cauldron.Core.Logging;
using Cauldron.IPC;

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "HwndHostElement", Type = typeof(Border))]
    public class Control_Ether_HwndHost : Control
    {
        static Control_Ether_HwndHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Ether_HwndHost), new FrameworkPropertyMetadata(typeof(Control_Ether_HwndHost)));
        }

        public static RendererHost rendererControl;
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
            }
        }

        private bool etherInitialized;

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!etherInitialized) return;
            Resize();
        }

        public void Resize()
        {
            Size size = GetElementPixelSize(this);

            RendererHost.MoveWindow(rendererControl.HwndRenderer, 0, 0, (int) size.Width, (int) size.Height, true);
        }

        private void InitEther()
        {
            Size size = GetElementPixelSize(this);
            rendererControl = new RendererHost(size.Height, size.Width);
            rendererControl.MessageHook += new HwndSourceHook(WndProcMsgFilter);
            hwndHostElement.Child = rendererControl;
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
            if (rendererControl.HwndRenderer == default) return IntPtr.Zero;

            if (msg.Equals(0x0021))
            {
                Logger.LogDebug("Cauldron", $"Sending message 0x0006 (WM_ACTIVATE), 1, 0");
                RendererHost.SendMessage(rendererControl.HwndRenderer, 0x0006, new IntPtr(1), new IntPtr(0));
            }
            handled = false;
            return IntPtr.Zero;
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
                set
                {
                    if (hwndRenderer == default) hwndRenderer = value;
                }
            }

            private bool test = false;

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
                

                InitializeCommand initializeCommand = new InitializeCommand(hwndHost);
                ConnectionManager.SendCommand(initializeCommand);

                //TODO: Parse Incoming Initialize Command
                // hwndRenderer = hwndHost;

                return new HandleRef(this, hwndHost);
            }

            protected override void DestroyWindowCore(HandleRef hwnd)
            {
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
            
            [DllImport("user32.dll", EntryPoint = "SetActiveWindow", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SetActiveWindow(IntPtr hwnd);

            [DllImport("user32.dll", EntryPoint = "SetFocus", CharSet = CharSet.Unicode)]
            internal static extern IntPtr SetFocus(IntPtr hwnd);
            
            [DllImport("user32.dll", EntryPoint = "MoveWindow", CharSet = CharSet.Unicode)]
            internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct WordUnion
        {
            [FieldOffset(0)] public uint Number;

            [FieldOffset(0)] public IntPtr ptr;

            [FieldOffset(0)] public ushort Low;

            [FieldOffset(2)] public ushort High;
        }
    }
}
