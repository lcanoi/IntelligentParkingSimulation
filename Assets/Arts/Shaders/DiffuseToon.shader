Shader "Custom/DiffuseToon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
        _SColor ("Shadow Color", Color) = (0.195,0.195,0.195,1.0)

        _RampTex ("Ramp Texture (RGB)", 2D) = "gray" {}

        _Ambient ("Ambient lighting", Range(0, 1)) = 0.2
        _Wrap ("Wrap lighting", Range(0, 1)) = 0.5

        // Dissolve values
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _Amount ("Dissolve Amount",  Range(0, 1)) = 0
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"}
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #pragma multi_compile __ ENABLE_DISSOLVE

            #pragma multi_compile_fwdbase
            
            #include "AutoLight.cginc"
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)

                half3 normal : TEXCOORD2; //world space

                LIGHTING_COORDS(3, 4)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            half4 _Color;

            half4 _HColor;
            half4 _SColor;

            sampler2D _RampTex;
            
            half _Wrap;
            half _Ambient;
            #if ENABLE_DISSOLVE
                sampler2D _DissolveTex;
                half _Amount;
                half4 _OutlineColor;
            #endif
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));

                UNITY_TRANSFER_FOG(o, o.vertex);
                //TRANSFER_VERTEX_TO_FRAGMENT(o);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half3 normal = normalize(i.normal);
                half3 lightDir = _WorldSpaceLightPos0.xyz;
                
                half diffuse = saturate(dot(normal, lightDir) * _Wrap + (1.0 - _Wrap)) * (1.0 - _Ambient) + _Ambient;

                half atten = LIGHT_ATTENUATION(i);
                
                half3 ramp = tex2D(_RampTex, half2(diffuse, diffuse)) * atten;

                _SColor = lerp(_HColor, _SColor, _SColor.a);
                ramp = lerp(_SColor.rgb,  _HColor.rgb, ramp);

                half4 col = tex2D(_MainTex, i.uv) * _Color;
                col.rgb *= _LightColor0.rgb * ramp;
                #if ENABLE_DISSOLVE
                    // dissolve
                    half dissolve = tex2D(_DissolveTex, i.uv).r - _Amount;
                    clip(dissolve);
                    col.rgb = lerp(col, _OutlineColor, step(dissolve, 0.05));
                #endif
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }

    Fallback "VertexLit"
}
