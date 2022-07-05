// Declare matrices and light direction
float4x4    World;
float4x4    View;
float4x4    Projection;
float4x4    WorldInverseTranspose;
float4      LightPos = float4(1, 1, 1, 1);
float4      LightColor = float4(1, 1, 1, 1);
float4      LightDirection = float4(1, 1, 1, 1);
float4      CameraPosition = float4(1, 1, 1, 1);
float4      CameraView = float4(1, 0, 0, 1);
float4      CameraDir;


float4      AmbientColor = float4(1, 1, 1, 1);
float       AmbientIntensity = 0.2f;

float4      DiffuseColor = float4(1, 1, 1, 1);
float       DiffuseIntensity = 0.5f;

float       Shininess = 4;
float4      SpecularColor = float4(1, 1, 1, 1);
float       SpecularIntensity = 0.3f;

float       FogDistance = 700;
float       FogDensity = 1.0f;
float4      FogColor = float4(0.8, 0.8, 0.8, 0.1f);

//float3      ViewVector = float3(1, 0, 0);
float       FishFactor = 1;
// texture voodoo 
texture ModelTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// each input vertex contains a position, normal and texture
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

// the values to be interpolated across triangle
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0; 
    float4 Normal : TEXCOORD1;
    float4 wPosition : TEXCOORD2;
    float4 oDiffuse : TEXCOORD3;
    float testColor : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    //Position data
    float4 worldPosition = mul(input.Position, World); // object to world xform
    float4 viewPosition = mul(worldPosition, View);  // world to camera xform
    output.Position = mul(viewPosition, Projection); // perspective xform
    output.wPosition = mul(input.Position, World);
    
    output.testColor = output.Position[2] / output.Position[3];

    // Diffuse Lighting
    float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
    //float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
    float lightInt = dot(normal, LightDirection);
    output.oDiffuse = saturate(DiffuseColor * DiffuseIntensity * lightInt);

    output.Normal = normal;
    output.TextureCoordinate = input.TextureCoordinate;
    return output;

    //VertexShaderOutput output;

    //float4 worldPosition = mul(input.Position, World);
    //float4 viewPosition = mul(worldPosition, View);
    //output.Position = mul(viewPosition, Projection);

    //float4 normal = normalize(mul(input.Normal, WorldInverseTranspose));
    //float lightIntensity = dot(normal, LightDirection);
    //output.oDiffuse = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

    //output.Normal = normal;

    //output.TextureCoordinate = input.TextureCoordinate;
    //output.wPosition = worldPosition; // Watch this

    //return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // Texture Color
	float4 textureColor = tex2D(textureSampler, input.TextureCoordinate);
    textureColor.a = 1.0;

    // Ambient Lighting
    float4 ambient = textureColor * AmbientColor * AmbientIntensity; // AmbientColor * AmbientIntensity;

    // Specular Lighting
    float4 lightDir = normalize(LightDirection);
    float4 normal = normalize(input.Normal);
    float4 reflect = normalize( 2 * dot(lightDir, normal) * normal - lightDir);
    float4 viewXworld = normalize(mul(normalize(CameraPosition - input.wPosition), World));
    float dotRV = dot(reflect, viewXworld);
    float4 specular = SpecularIntensity * SpecularColor * max(pow( abs(dotRV), Shininess), 0) * length(input.oDiffuse);

    // Fog interpolation :::: l <= FogDistance = 0, l >= FogDistance * (1+FogDensity) = 1
    float l = length(input.wPosition - CameraPosition);
    float erp = max(l, FogDistance);
    erp = min(erp, FogDistance * (1.0 / FogDensity + 1.0));
    erp = (erp - FogDistance) / (FogDistance / FogDensity);


    float4 texOut = lerp(saturate(textureColor * input.oDiffuse + ambient + specular),
                         saturate(FogColor), erp);// *0.1f + 0.1f * input.testColor; //watch

    return texOut;
}

technique ComplexLightShader
{
    pass Pass1
    {
		VertexShader = compile vs_4_0_level_9_3  VertexShaderFunction();
		PixelShader = compile ps_4_0_level_9_3  PixelShaderFunction();
	}
}
