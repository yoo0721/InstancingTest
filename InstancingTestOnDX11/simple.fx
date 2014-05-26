//

matrix World;
matrix ViewProjection;

struct VertexPositionColor
{
	float4 Position : SV_Position;
	float4 Color : COLOR;
	column_major float4x4 world : MATRIX;
};

VertexPositionColor MyVertexShader(VertexPositionColor input)
{
	VertexPositionColor output = input;
	output.Position = mul(output.Position, input.world);
	//output.Position = mul(output.Position, World);
	output.Position = mul(output.Position, ViewProjection);
	
	return output;
}

float4 MyPixelShader(VertexPositionColor input) : SV_Target
{
	return input.Color;
}

VertexPositionColor HW_VS(VertexPositionColor input)
{
	VertexPositionColor output = input;
	Matrix mat = transpose(input.world);
	output.Position = mul(input.Position, mat);
	output.Position = mul(output.Position, ViewProjection);
	output.Color = input.Color;

	return output;
}
/*
VertexDefinition Textured_HW_VS(VertexDefinition input)
{
	float s = sin(Angle);
	float c = cos(Angle);
	float x = input.Axis.x;
	float y = input.Axis.y;
	float z = input.Axis.z;

	float4x4 lot =
	{
		x * x * (1 - c) + c,     x * y * (1 - c) - z *s,  z * x * (1 - c) + y * s, 0,
		x * y * (1 - c) + z * s, y * y * (1 - c) + c,     y * z * (1 - c) - x * s, 0,
		z * x * (1 - c) - y * s, y * z * (1 - c) + x * s, z * z * (1 - c) + c    , 0,
		0                      , 0                      , 0                      , 0
	};
}
*/

struct VS_IN
{
	float4 pos : SV_POSITION;
	float4 color : COLOR;
	float2 texel : TEXCOORD;
	column_major float4x4 world : MATRIX;
	//uint instanceID : SV_InstanceID;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 color : COLOR;
	float3 texel : TEXCOORD0;
};

Texture2DArray g_DecalMap : register(t0);
SamplerState g_Sampler : register(s0);

VS_IN Textured_HW_Instancing_VS(VS_IN input)
{
	VS_IN output = input;
	Matrix mat = transpose(input.world);
	output.pos = mul(input.pos, mat);
	output.pos = mul(output.pos, ViewProjection);
	output.color = input.color;
	//output.texel = float3(input.texel, input.instanceID % 3);

	//output.texel = float3(input.texel, 3);
	output.texel = input.texel;
	return output;
}

float4 Textured_HW_Instancing_PS(VS_IN input) : SV_Target
{
	return input.color;
	//return float4(g_DecalMap.Sample(g_Sampler, input.texel).rgb, 1);
}

technique11 MyTechnique
{
	pass MyPass
	{
		SetVertexShader(CompileShader(vs_5_0, MyVertexShader()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader()));
	}
}

technique11 HW_Instancing
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, HW_VS()));
		SetPixelShader(CompileShader(ps_5_0, MyPixelShader()));
	}
}

technique11 Textured_HW_Instancing
{
	pass P0
	{
		SetVertexShader(CompileShader(vs_5_0, Textured_HW_Instancing_VS()));
		SetPixelShader(CompileShader(ps_5_0, Textured_HW_Instancing_PS()));
	}
}