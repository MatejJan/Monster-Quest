Shader "Monster Quest/Miniatures/Hit points"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AngleRatio ("Angle ratio", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags 
        {
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "IgnoreProjector"="True"
        }
        
        LOD 100
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _AngleRatio;
            
            static const float PI = 3.14159265;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvRelative = i.uv * 2.0 - 1.0;
                float angle = (atan2(uvRelative.y, uvRelative.x) / PI + 1.0) / 2.0;
                if (angle > _AngleRatio) discard;
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.rgb *= col.a;
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
