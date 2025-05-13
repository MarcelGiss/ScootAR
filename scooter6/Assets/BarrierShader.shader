Shader "Custom/BarrierShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _PlayerPos ("Player Position", Vector) = (0,0,0,0)
        _Radius ("Effect Radius", Float) = 5.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 worldPos : TEXCOORD1;
};

sampler2D _MainTex;
float4 _Color;
float4 _PlayerPos;
float _Radius;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float distance = distance(i.worldPos, _PlayerPos.xyz);
    float alpha = 1.0 - saturate(distance / _Radius);
    fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
    texColor.a *= alpha;
    return texColor;
}
            ENDCG
        }
    }
FallBack "Diffuse"
}