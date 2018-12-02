Shader "!!!JAM!!!/WindupArc"
{
	Properties
	{
		_MainTex           ("Texture", 2D)                   = "white" {}
        _Angle             ("Angle", Float)                  =    0
        _StartAngle        ("StartAngle", Float)             = 0
        _Width             ("Width", Range(0,1))             = 0.0625
        _Progress          ("Progress", Range(0,1))          = 1
        _ColorDanger       ("ColorDanger", Color)            = (1,0,0,1)
        _BackgroundOpacity ("BackgroundOpacity", Range(0,1)) = 0.5
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

            uniform float _Angle; 
            uniform float _StartAngle;
            float         _Width;
            float         _Progress;
            float         _BackgroundOpacity;
			
			v2f vert (appdata v)
			{
				v2f o;
                float4 vertex = float4(
                    cos(v.vertex.y * _Angle + _StartAngle),
                    sin(v.vertex.y * _Angle + _StartAngle),
                    v.vertex.z,
                    v.vertex.w
                    );
				o.vertex = UnityObjectToClipPos(lerp(v.vertex, vertex, v.vertex.x));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
            float arc(float distance_field, float progress)
            {
                return round_quad((distance_field / _Width) + 1 - (progress / _Width) );
            }

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
                i.uv.y = i.uv.y / i.uv.x;
                i.uv = clamp(i.uv, 0, 1);
                float p = ease_out_quad(_Progress * 0.25 + 0.75);
                float len = arc(i.uv.x, p);

                // progress line
                len = max(len, arc(i.uv.x , _Progress));

                // side lines
                len = max(len, arc(abs(i.uv.y * 2 - 1), 1) * round_quad(clamp(i.uv.x * 2 - 1, 0, 1)));

                // fade on the side
                len *= round_quad(i.uv.y) * 0.75 + 0.25;

                // background
                len = max(len,
                    (1 - step(_Progress - _Width * 0.5, i.uv.x)) * i.uv.x * _BackgroundOpacity
                );

                // contrast
                len = ease_out_quad(len);
                // global fade depending on progress
                len *= _Progress;

				return float4(len, 0.0, 0.0,0.0);
			}
			ENDCG
		}
	}
}
