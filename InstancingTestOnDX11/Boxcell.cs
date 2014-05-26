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
        bool OnInitialize(Device device, EffectPass pass);
        void OnRender(DeviceContext context, EffectPass pass);

        //秒単位 通常小数点以下 ex. 0.016
        void OnPreRender(Device device, float diffTime);
    }
    class Boxcell : IRenderObject
    {
        public class Status
        {
            public Matrix matrix;
            public Vector3 axis;
            
            public Status(Vector3 position)
            {
                matrix = Matrix.Translation(position);
                Vector3 v2 = new Vector3(position.Z, 0, -position.X);
                axis = Vector3.Cross(position, v2);
                axis.Normalize();
            }
            public float Length
            {
                get
                {
                    Vector4 v = Vector4.Transform(new Vector4(0,0,0,1), matrix);
                    return new Vector3(v.X, v.Y, v.Z).Length();
                }
            }
        }

        SlimDX.Direct3D11.Buffer vertexBuffer;
        SlimDX.Direct3D11.Buffer inputBuffer;
        InputLayout vertexLayout;

        public List<Status> statusArray = null;

        public Boxcell(List<Status> statusArray)
        {
            this.statusArray = statusArray;
        }

        public Boxcell()
        {
            statusArray = new List<Status>();
            Random r = new Random();
            for (int z = 0; z < 100; z++)
            {
                for (int x = 0; x < 100; x++)
                {
                    Status s = new Status(new Vector3(x * 2, r.Next(50), z));
                    statusArray.Add(s);

                }
            }
        }
    [StructLayout(LayoutKind.Sequential)]
        struct VertexDefinition
        {
            public Vector3 Position;
            public Color4 color;
            public Vector2 texel;

            public VertexDefinition(float x, float y, float z, float u, float v, Color4 color)
            {
                Position = new Vector3(x, y, z);
                this.color = color;
                texel = new Vector2(u, v);
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
                SemanticName = "TEXCOORD",
                Format = SlimDX.DXGI.Format.R32G32_Float,
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
            new VertexDefinition( 0.5f, -0.5f,  0.5f, 0, 1, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f,  0.5f,  0.5f, 1, 1, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f,  0.5f, -0.5f, 1, 0, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f,  0.5f, -0.5f, 1, 0, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f, -0.5f, -0.5f, 0, 0, new Color4(Color.Azure)),
            new VertexDefinition( 0.5f, -0.5f,  0.5f, 0, 1, new Color4(Color.Azure)),

            new VertexDefinition(  0.5f, -0.5f, -0.5f, 1, 0, new Color4(Color.Aqua)),
            new VertexDefinition(  0.5f,  0.5f, -0.5f, 1, 1, new Color4(Color.Aqua)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, 0, 1, new Color4(Color.Aqua)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, 0, 1, new Color4(Color.Aqua)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, 0, 0, new Color4(Color.Aqua)),
            new VertexDefinition(  0.5f, -0.5f, -0.5f, 1, 0, new Color4(Color.Aqua)),


            new VertexDefinition( -0.5f, -0.5f, -0.5f, 0, 0, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, 1, 0, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f,  0.5f,  0.5f, 1, 1, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f,  0.5f,  0.5f, 1, 1, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f, -0.5f,  0.5f, 0, 1, new Color4(Color.Brown)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, 0, 0, new Color4(Color.Brown)),

            new VertexDefinition( -0.5f, -0.5f, 0.5f, 0, 0, new Color4(Color.CadetBlue)),  
            new VertexDefinition( -0.5f,  0.5f, 0.5f, 0, 1, new Color4(Color.CadetBlue)),
            new VertexDefinition(  0.5f,  0.5f, 0.5f, 1, 1, new Color4(Color.CadetBlue)),
            new VertexDefinition(  0.5f,  0.5f, 0.5f, 1, 1, new Color4(Color.CadetBlue)),
            new VertexDefinition(  0.5f, -0.5f, 0.5f, 1, 0, new Color4(Color.CadetBlue)),
            new VertexDefinition( -0.5f, -0.5f, 0.5f, 0, 0, new Color4(Color.CadetBlue)),

            new VertexDefinition(  0.5f,  0.5f,  0.5f, 1, 1, new Color4(Color.Chocolate)),  
            new VertexDefinition( -0.5f,  0.5f,  0.5f, 0, 1, new Color4(Color.Chocolate)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, 0, 0, new Color4(Color.Chocolate)),
            new VertexDefinition( -0.5f,  0.5f, -0.5f, 0, 0, new Color4(Color.Chocolate)),
            new VertexDefinition(  0.5f,  0.5f, -0.5f, 1, 0, new Color4(Color.Chocolate)),
            new VertexDefinition(  0.5f,  0.5f,  0.5f, 1, 1, new Color4(Color.Chocolate)),


            new VertexDefinition(  0.5f, -0.5f,  0.5f, 1, 1, new Color4(Color.IndianRed)),
            new VertexDefinition(  0.5f, -0.5f, -0.5f, 1, 0, new Color4(Color.IndianRed)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, 0, 0, new Color4(Color.IndianRed)),
            new VertexDefinition( -0.5f, -0.5f, -0.5f, 0, 0, new Color4(Color.IndianRed)),
            new VertexDefinition( -0.5f, -0.5f,  0.5f, 0, 1, new Color4(Color.IndianRed)),
            new VertexDefinition(  0.5f, -0.5f,  0.5f, 1, 1, new Color4(Color.IndianRed)),  
        };

        public bool OnInitialize(Device device, EffectPass pass)
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

            CreateInputBuffer(device, 0);

            vertexLayout = new InputLayout(
                device,
                pass.Description.Signature,
                VertexDefinition.VertexElements
                );

            return true;
        }

        bool CreateInputBuffer(Device device, float diffTime)
        {
            List<Matrix> matrixArray = new List<Matrix>();
            for(int i = 0; i < statusArray.Count; i++)
            {
                float speed = diffTime / (float)Math.Sqrt(statusArray[i].Length);
                Matrix lot = Matrix.RotationAxis(statusArray[i].axis,speed);
                lot = Matrix.Multiply(statusArray[i].matrix, lot);
                
                statusArray[i].matrix = lot;
                matrixArray.Add(statusArray[i].matrix);
            }

            DataStream stream;
            try
            {
                if( inputBuffer != null)
                {
                    inputBuffer.Dispose();
                }
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
                stream.Dispose();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public void OnPreRender(Device device, float diffTime)
        {
            CreateInputBuffer(device, diffTime);
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
            context.DrawInstanced(faces.Length, statusArray.Count, 0, 0);
        }
    }
}
