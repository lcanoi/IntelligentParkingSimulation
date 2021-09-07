Shader "Custom/DrawDashedLine"
{
    Properties
    {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _FinishColor("Finish Color", Color) = (1,1,1,1)
        _Percentage("Percentage", float) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            fixed4 _Color;
            fixed4 _FinishColor;
            float _Percentage;

            v2f vert (appdata v)
            {
                v2f o;

                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float finishFlag = step(_Percentage, i.texcoord.x);
                fixed4 col = tex2D(_MainTex, i.texcoord) * lerp(_Color, _FinishColor, finishFlag);
                return col;
            }
            ENDCG
        }
    }
}
