Shader "!!!JAM!!!/WindupCircular"
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
			// make fog work
			#pragma multi_compile_fog

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


				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

            float circ(float len, float progress)
            {
                float output = len;
                output = output * (1 / _Width) - ((progress - (_Width)) / _Width);
                output = output * 2 - 1;
                output = -output * output + 1.0;
                output = clamp(output, 0., 1.);
                return output;
            }

            float circ_filled(float len, float progress)
            {
                float output = len;
                output = output * (1 / _Width) - ((progress - (_Width)) / _Width);
                output = output * 2 - 1;
                output = 1 - output;
                output = clamp(output, 0., 1.);
                return output;
            }
			
            float4 frag (v2f i) : SV_Target
			{
                float4 color = (float4)0;

                float2 uv = i.uv * 2 - 1;

                float temp = sin(_Time.y) * 0.5 + 0.5;

                float len = length(uv);

                color = circ(len, _Progress) * lerp(_ColorSafe, _ColorDanger, _Progress);
                color = max(color, circ_filled(len, _Progress) * _ColorDanger * _BackgroundOpacity);
                color = max(color, circ(len, 1) * _ColorDanger);

				return color;
			}
			ENDCG
		}
	}
}
