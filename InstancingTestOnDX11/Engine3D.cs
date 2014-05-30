using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;
using SlimDX.Direct3D11;
using System.Drawing;
using SlimDX.D3DCompiler;
using System.Runtime.InteropServices;

namespace InstancingTestOnDX11
{
    struct VertexPointColor
    {
        public Vector3 Position;
        public Vector3 Color;

        public static readonly InputElement[] VertexElements = new[]
        {
            new InputElement
            {
                SemanticName = "SV_Position",
                Format = SlimDX.DXGI.Format.R32G32B32_Float
                
            },
            new InputElement
            {
                SemanticName = "COLOR",
                Format = SlimDX.DXGI.Format.R32G32B32_Float,
                AlignedByteOffset = InputElement.AppendAligned,
            },
            new InputElement
            {
                SemanticName = "MATRIX",
                SemanticIndex = 0,
                Slot = 1,
                Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
                Classification = InputClassification.PerInstanceData,
                AlignedByteOffset = InputElement.AppendAligned,
                InstanceDataStepRate = 1,
            },
            new InputElement
            {
                SemanticName = "MATRIX",
                SemanticIndex = 1,
                Slot = 1,
                Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
                Classification = InputClassification.PerInstanceData,
                AlignedByteOffset = InputElement.AppendAligned,
                InstanceDataStepRate = 1,
            },
            new InputElement
            {
                SemanticName = "MATRIX",
                SemanticIndex = 2,
                Slot = 1,
                Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
                Classification = InputClassification.PerInstanceData,
                AlignedByteOffset = InputElement.AppendAligned,
                InstanceDataStepRate = 1,
            },
            new InputElement
            {
                SemanticName = "MATRIX",
                SemanticIndex = 3,
                Slot = 1,
                Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
                Classification = InputClassification.PerInstanceData,
                AlignedByteOffset = InputElement.AppendAligned,
                InstanceDataStepRate = 1,
            },
        };

        public static int SizeInBytes { get { return Marshal.SizeOf(typeof(VertexPointColor)); } }
        
    }
    class Engine3D
    {
        private Device device = null;
        private SlimDX.DXGI.SwapChain swapChain = null;
        private RenderTargetView renderTarget = null;

        Effect effect;
        InputLayout vertexLayout;
        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer inputBuffer;
        DepthStencilView depthStencil;

        public Camera camera = new Camera();

        ShaderResourceView srv;
        SamplerState samplerState;

        Size clientSize;

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        Queue<long> fpsQueue = new Queue<long>();
        System.Windows.Forms.Form form;

        List<IRenderObject> renderObjectArray = new List<IRenderObject>();

        public void Run()
        {
                SlimDX.Windows.MessagePump.Run(MainLoop);
        }

        public void SetRenderObject(IRenderObject renderObject)
        {
            renderObjectArray.Add(renderObject);
        }

        public void OnInitialize(System.Windows.Forms.Form form)
        {
            this.form = form;
            clientSize = new Size(form.Width, form.Height);

            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.Debug,
                new SlimDX.DXGI.SwapChainDescription
                {
                    BufferCount = 1,
                    OutputHandle = form.Handle,
                    IsWindowed = true,
                    SampleDescription = new SlimDX.DXGI.SampleDescription
                    {
                        Count = 1,
                        Quality = 0
                    },
                    ModeDescription = new SlimDX.DXGI.ModeDescription
                    {
                        Width = form.Width,
                        Height = form.Height,
                        RefreshRate = new SlimDX.Rational(60,1),
                        Format = SlimDX.DXGI.Format.R8G8B8A8_UNorm
                    },
                    Usage = SlimDX.DXGI.Usage.RenderTargetOutput
                },
                out device,
                out swapChain
                );

            using (Texture2D backBuffer =
                SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain,0))
            {
                renderTarget = new RenderTargetView(device, backBuffer);
                device.ImmediateContext.OutputMerger.SetTargets(renderTarget);
            }

            device.ImmediateContext.Rasterizer.SetViewports(
                new Viewport
                {
                    Width = form.Width,
                    Height = form.Height,
                    MaxZ = 1
                }
                );

            using(ShaderBytecode shaderBytecode = ShaderBytecode.CompileFromFile(
                "simple.fx","fx_5_0",
                ShaderFlags.None,
                EffectFlags.None))
            {
                effect = new Effect(device, shaderBytecode);
            }

            vertexLayout = new InputLayout(
                device,
                effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature,
                VertexPointColor.VertexElements
                );

            // VertexBufer
            VertexPointColor[] vertices = 
            {
                new VertexPointColor
                {
                    Position = new Vector3(    0, 0.5f, 0),
                    Color = new Vector3(0,0,1)
                },
                new VertexPointColor
                {
                    Position = new Vector3( 0.5f,    0, 0),
                    Color = new Vector3(0,0,1)
                },
                new VertexPointColor
                {
                    Position = new Vector3(-0.5f,    0, 0),
                    Color = new Vector3(1,0,0)
                },
            };
            DataStream stream = new DataStream(vertices,true, true);
            vertexBuffer = new SlimDX.Direct3D11.Buffer(
                device,
                stream,
                new BufferDescription
                {
                    SizeInBytes = (int)stream.Length,
                    BindFlags = BindFlags.VertexBuffer,
                    OptionFlags = ResourceOptionFlags.DrawIndirect,
                    
                }
                );
            stream.Dispose();

            // 定数バッファ
            Matrix[] mat = new Matrix[100];
            for (int i = 0; i < 100; i++)
            {
                mat[i] = Matrix.Translation(new Vector3((float)(0.2 * i), 0, (float)(0.2 * i)));
                
            }
            stream = new DataStream(mat,true,true);
            inputBuffer = new SlimDX.Direct3D11.Buffer(
                device,
                stream,
                new BufferDescription
                {
                    SizeInBytes = (int)stream.Length,
                    BindFlags = BindFlags.ShaderResource,
                    OptionFlags = ResourceOptionFlags.DrawIndirect,
                    //StructureByteStride = Marshal.SizeOf(typeof(Matrix)),
                }
                );

            // 深度バッファの設定
            Texture2DDescription depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = SlimDX.DXGI.Format.D32_Float,
                Width = clientSize.Width,
                Height = clientSize.Height,
                MipLevels = 1,
                SampleDescription = new SlimDX.DXGI.SampleDescription(1,0)
            };

            using(Texture2D depthBuffer = new Texture2D(device,depthBufferDesc))
            {
                depthStencil = new DepthStencilView(device, depthBuffer);
            }
            device.ImmediateContext.OutputMerger.SetTargets(depthStencil, renderTarget);

            EffectPass pass = effect.GetTechniqueByIndex(2).GetPassByIndex(0);
            foreach(IRenderObject itr in renderObjectArray)
            {
                itr.OnInitialize(device, pass);
            }

            // Textureの読み込み
            try
            {
                int testMode = 2;
                switch (testMode)
                {
                    case 1:

                        Texture2D[] textureArray = new Texture2D[]
                           {
                                Texture2D.FromFile(device, "cobblestone_mossy.png"),
                                Texture2D.FromFile(device, "brick.png"),
                            };
                        stream = new DataStream(textureArray, true, true);
                        
                        srv = new ShaderResourceView(device, textureArray[0]);
                        
                        effect.GetVariableByName("g_DecalMap").AsResource().SetResource(srv);
                        break;
                    case 2:

                        srv
                            = ShaderResourceView.FromFile(device, "cobblestone_mossy.png");
                        effect.GetVariableByName("g_DecalMap").AsResource().SetResource(srv);
                        //device.ImmediateContext.PixelShader.SetShaderResource(shaderResourceView, 19);

                        break;
                }
            
                SamplerDescription description = new SamplerDescription
                {
                    Filter = SlimDX.Direct3D11.Filter.Anisotropic,
                    AddressU = SlimDX.Direct3D11.TextureAddressMode.Wrap,
                    AddressV = SlimDX.Direct3D11.TextureAddressMode.Wrap,
                    AddressW = TextureAddressMode.Wrap,

                };
                samplerState = SamplerState.FromDescription(device, description);
                
                
            }
            catch(Exception e)
            {
                return;
            }
            return;
        }

        private void updateCamera()
        {
            Matrix projection = Matrix.PerspectiveFovRH(
                (float)System.Math.PI / 2,
                clientSize.Width / clientSize.Height,
                0.1f, 1000
                );
            effect.GetVariableByName("ViewProjection")
                .AsMatrix().SetMatrix(camera.ViewMatrix * projection);

        }

        long oldTime = 0;
        protected virtual void MainLoop()
        {
            long nowTime = sw.ElapsedMilliseconds;
            float diffTime = 0;

            form.Text = camera.TEXT;
            if( !sw.IsRunning)
            {
                sw.Start();
                oldTime = sw.ElapsedMilliseconds;
            }
            if(fpsQueue.Count < 60)
            {
                fpsQueue.Enqueue(nowTime);
            }
            else
            {
                diffTime = nowTime - fpsQueue.Dequeue();
                float fps = 1000 * 60 / diffTime;
                form.Text += "fps = " + fps;
            }

            diffTime = nowTime - oldTime;
            diffTime /= 1000;
            oldTime = nowTime;

            OnPreRender(diffTime * camera.Speed);
            OnRender();
            
            
        }

        protected void OnPreRender(float diffTime)
        {
            foreach(IRenderObject itr in renderObjectArray)
            {
                itr.OnPreRender(device,diffTime);
            }

        }

        protected void OnRender()
        {
            

            device.ImmediateContext.ClearRenderTargetView(
                renderTarget,
                new Color4(Color.CornflowerBlue));

            device.ImmediateContext.ClearDepthStencilView(
                depthStencil,
                DepthStencilClearFlags.Depth,
                1,
                0
                );

            updateCamera();
                                    
            device.ImmediateContext.PixelShader.SetSampler(samplerState,0);
            foreach(IRenderObject itr in renderObjectArray)
            {
                itr.OnRender(device.ImmediateContext, effect.GetTechniqueByName("Textured_HW_Instancing").GetPassByIndex(0));
            }
            
            swapChain.Present(0, SlimDX.DXGI.PresentFlags.None);

        }
    }
}
