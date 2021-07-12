using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Core;

namespace Cauldron.UserControls.Windows
{
    /// <summary>
    /// Interaction logic for SceneWindow.xaml
    /// </summary>
    public partial class SceneWindow : UserControl
    {
        //TODO: Single viewport -> Multi Viewport
        public static OpenGLRenderer Renderer
        {
            get
            {
                return _renderer;
            }
            private set
            {
                if (_renderer == null)
                {
                    _renderer = value;
                }
                else throw new Exception("Multiple Renderer Initialized");
            }
        }

        private static OpenGLRenderer _renderer;

        public SceneWindow()
        {
            InitializeComponent();


            if (CommandArguments.TryGetArgument("ether"))
            {
                SceneWindowContainer.Children.Remove(OpenGL_Viewport);
            }
            else
            {
                SceneWindowContainer.Children.Remove(Ether_Viewport);
                Renderer = new OpenGLRenderer(OpenGL_Viewport);
                Unloaded += OnUnload;
                Loaded += OnLoad;
            }
        }

        private void RendererOnShaderError(string errorString)
        {
            ShaderCompiler_ErrorLabel.Visibility = errorString.Trim().Equals("") 
                ? Visibility.Hidden : Visibility.Visible;
            ShaderCompiler_ErrorLabel.Text = "\n" + errorString;
        }

        private void OnUnload(object sender, EventArgs e)
        {
            Renderer.ShaderError -= RendererOnShaderError;
            CompositionTarget.Rendering -= OnCompositionTargetOnRendering;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Renderer.ShaderError += RendererOnShaderError;
            CompositionTarget.Rendering += OnCompositionTargetOnRendering;
        }

        private void OnCompositionTargetOnRendering(object sender, EventArgs args)
        {
            Renderer.DoRender();
        }
    }
}
