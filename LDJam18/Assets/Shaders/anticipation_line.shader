Shader "!!!JAM!!!/anticipation_line"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Range(0,1)) = 0.0625
        _Progress("Progress", Range(0,1)) = 1
        _ColorSafe("ColorSafe", Color) = (0,1,0,1)
        _ColorDanger("ColorDanger", Color) = (1,0,0,1)
        _BackgroundOpacity("BackgroundOpacity", Range(0,1)) = 0.5
	}
	SubShader
	{
        // inside SubShader
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

        // inside Pass
        ZTest Always
        ZWrite Off
        // Blend SrcAlpha OneMinusSrcAlpha
        Blend OneMinusDstColor One // Aditive
		
        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

            #include "Utils.cginc"
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _ColorSafe;
            float4 _ColorDanger;
            float _Width;
            float _Progress;
            float _BackgroundOpacity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

            float circ_filled(float len, float progress)
            {
                return step(len, progress - _Width * 0.5);
            }
			
            float4 frag (v2f i) : SV_Target
			{
                //_Progress = ease_out_quad(_Progress);
                float4 color = (float4)0;

                float  p   = ease_out_quad(_Progress * 0.25 + 0.75);
                float2 uv             = i.uv * 2 - 1;
                       uv = abs(uv);
                color = circ(uv.x, _Progress, _Width) * lerp(_ColorDanger, _ColorDanger, _Progress);
                color = max(color, circ_filled(uv.x, _Progress) * _ColorDanger * _BackgroundOpacity * _Progress);
                color = max(color, circ(uv.x, p, _Width * p) * _ColorDanger * _Progress);
                color *= 1 - uv.y;

				return color;
			}
			ENDCG
		}
	}
}
