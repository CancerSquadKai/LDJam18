Shader "Unlit/WingCooldown"
{
	Properties
	{
        _Progress ("_Progress", Range(0,1)) = 1
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
				float4 position : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _Progress;

            uniform float4 _PlayerColor;
			
			v2f vert (appdata v)
			{
				v2f o;
                o.position = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                float a = i.position.z * 50.0 + 0.75;
                //a = 1 - a;
                a = 1 - step((_Progress * 2 - 1) * 0.4 + 0.4, a);
                static const float OPACITY = 0.25;
                a *= step(1, _Progress) * (1 - OPACITY) + OPACITY;
				return a * _PlayerColor * 1.5;
			}
			ENDCG
		}
	}
}
