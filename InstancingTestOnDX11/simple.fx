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
	VertexPositionColor output;
	output.Position = mul(input.Position, input.world);
	output.Position = mul(output.Position, ViewProjection);
	output.Color = input.Color;

	return output;
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