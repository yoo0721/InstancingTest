using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;
using SlimDX.Direct3D11;
using System.Runtime.InteropServices;
using System.Drawing;


namespace InstancingTestOnDX11
{
    interface IRenderObject
    {
        bool OnInitialize(Device device);
        void OnRender(DeviceContext context, EffectPass pass);
    }
    class Boxcell : IRenderObject
    {
        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer inputBuffer;
        InputLayout vertexLayout;

        Effect effect;
        public List<Matrix> matrixArray = null;

        public Boxcell(List<Matrix> matrixArray)
        {
            this.matrixArray = matrixArray;
        }

        public Boxcell()
        {
            matrixArray = new List<Matrix>();
            Random r = new Random();
            for (int z = 0; z < 300; z++)
            {
                for (int x = 0; x < 300; x++)
                {
                    Matrix mat = Matrix.Translation(x, r.Next(20), z);
                    mat = Matrix.Transpose(mat);

                    matrixArray.Add(mat);
                }
            }
        }

        struct VertexDefinition
        {
            public Vector3 Position;
            public Color4 color;

            public VertexDefinition(Vector3 pos)
            {
                Position = pos;
                color = new Color4(0,0,0);
            }
            
            public VertexDefinition(float x, float y, float z)
            {
                Position = new Vector3(x,y,z);
                color = new Color4(Color.BlueViolet);
            }

            public VertexDefinition(float x, float y, float z, Color4 color)
            {
                Position = new Vector3(x, y, z);
                this.color = color;
            }

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
                Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
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

        static VertexDefinition[] faces =
        {
            new VertexDefinition( 0.5f, -0.5f,  0.5f, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f,  0.5f,  0.5f, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f,  0.5f, -0.5f, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f,  0.5f, -0.5f, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f, -0.5f, -0.5f, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f, -0.5f,  0.5f, new Color4(Color.Azure)),

            new VertexDefinition(  0.5f, -0.5f, -0.5f, new Color4(Color.Aqua)),
            new VertexDefinition(  0.5f,  0.5f, -0.5f, new Color4(Color.Aqua)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, new Color4(Color.Aqua)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, new Color4(Color.Aqua)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, new Color4(Color.Aqua)),
            new VertexDefinition(  0.5f, -0.5f, -0.5f, new Color4(Color.Aqua)),


            new VertexDefinition( -0.5f, -0.5f, -0.5f, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f,  0.5f,  0.5f, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f,  0.5f,  0.5f, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f, -0.5f,  0.5f, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, new Color4(Color.Brown)),

            new VertexDefinition( -0.5f, -0.5f, 0.5f, new Color4(Color.CadetBlue)),  
            new VertexDefinition( -0.5f,  0.5f, 0.5f, new Color4(Color.CadetBlue)),
            new VertexDefinition(  0.5f,  0.5f, 0.5f, new Color4(Color.CadetBlue)),
            new VertexDefinition(  0.5f,  0.5f, 0.5f, new Color4(Color.CadetBlue)),
            new VertexDefinition(  0.5f, -0.5f, 0.5f, new Color4(Color.CadetBlue)),
            new VertexDefinition( -0.5f, -0.5f, 0.5f, new Color4(Color.CadetBlue)),

            new VertexDefinition(  0.5f,  0.5f,  0.5f, new Color4(Color.Chocolate)),  
            new VertexDefinition( -0.5f,  0.5f,  0.5f, new Color4(Color.Chocolate)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, new Color4(Color.Chocolate)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, new Color4(Color.Chocolate)),
            new VertexDefinition(  0.5f,  0.5f, -0.5f, new Color4(Color.Chocolate)),
            new VertexDefinition(  0.5f,  0.5f,  0.5f, new Color4(Color.Chocolate)),


            new VertexDefinition(  0.5f, -0.5f,  0.5f, new Color4(Color.IndianRed)),
            new VertexDefinition(  0.5f, -0.5f, -0.5f, new Color4(Color.IndianRed)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, new Color4(Color.IndianRed)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, new Color4(Color.IndianRed)),
            new VertexDefinition( -0.5f, -0.5f,  0.5f, new Color4(Color.IndianRed)),
            new VertexDefinition(  0.5f, -0.5f,  0.5f, new Color4(Color.IndianRed)),  
        };

        public bool OnInitialize(Device device)
        {
            DataStream stream;
            try
            {
                stream = new DataStream(faces, true, true);
                vertexBuffer = new SlimDX.Direct3D11.Buffer(
                    device,
                    stream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)stream.Length,
                        BindFlags = SlimDX.Direct3D11.BindFlags.VertexBuffer,
                        OptionFlags = ResourceOptionFlags.DrawIndirect,
                    });
                stream.Dispose();
            }
            catch (Exception e)
            {
                return false;
            }

            try
            {
                stream = new DataStream(matrixArray.ToArray(), true, true);
                inputBuffer = new SlimDX.Direct3D11.Buffer(
                    device,
                    stream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)stream.Length,
                        BindFlags = BindFlags.ShaderResource,
                        OptionFlags = ResourceOptionFlags.DrawIndirect,
                    }
                    );
            }
            catch(Exception e)
            {
                return false;
            }

            using (SlimDX.D3DCompiler.ShaderBytecode shaderBytecode
                        = SlimDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                "simple.fx", "fx_5_0",
                SlimDX.D3DCompiler.ShaderFlags.None,
                SlimDX.D3DCompiler.EffectFlags.None))
            {
                effect = new Effect(device, shaderBytecode);
            }

            vertexLayout = new InputLayout(
                device,
                effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature,
                VertexDefinition.VertexElements
                );

            return true;
        }

        public void OnRender(DeviceContext context, EffectPass pass)
        {
            context.InputAssembler.InputLayout = vertexLayout;
            context.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.TriangleList;

            VertexBufferBinding[] binds = new VertexBufferBinding[]
            {
                new VertexBufferBinding(vertexBuffer, Marshal.SizeOf(typeof(VertexDefinition)),0),
                new VertexBufferBinding(inputBuffer, Marshal.SizeOf(typeof(Matrix)),0),
            };

            context.InputAssembler.SetVertexBuffers(0, binds);

            pass.Apply(context);
            context.DrawInstanced(faces.Length, matrixArray.Count, 0, 0);
        }
    }
}
