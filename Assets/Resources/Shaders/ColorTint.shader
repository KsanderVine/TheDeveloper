// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Particles/ColorTint" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
	_TintColorRed   ("Tint Color Red",   Color)  = (1,0,0,1)
	_TintColorGreen ("Tint Color Green", Color)  = (0,1,0,1)
	_TintColorBlue  ("Tint Color Blue",  Color)  = (0,0,1,1)	
	
	_ColorRed    ("Color Red",   Color)  = (1,0,0,1)
	_ColorGreen  ("Color Green", Color)  = (0,1,0,1)
	_ColorBlue   ("Color Blue",  Color)  = (0,0,1,1)	
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	Fog { Mode off }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	SubShader {
		Pass {

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColorRed;
			fixed4 _TintColorGreen;
			fixed4 _TintColorBlue;
			fixed4 _ColorRed;
			fixed4 _ColorGreen;
			fixed4 _ColorBlue;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			sampler2D _CameraDepthTexture;
			float _InvFade;

			fixed4 frag (v2f i) : COLOR
			{	
				float4 baseColor = tex2D(_MainTex, i.texcoord);
				float4 temp = float4(0,0,0,0);
				//if(baseColor.r == temp.r && baseColor.g == temp.g && baseColor.b == temp.b) baseColor = _TintColorRed;
				
				
				//if(baseColor.r == temp.r && baseColor.g == temp.g && baseColor.b == temp.b) 
				//{
				//if(baseColor.a > 0.1f) baseColor.a += 0.5f;
				//return baseColor;
				//}
				float alpha = baseColor.a;
				//if(alpha > 0.0f) alpha = 1.0f;
				
          
				temp = alpha * (baseColor.r * _TintColorRed + baseColor.g * _TintColorGreen + baseColor.b * _TintColorBlue);
				temp.a = 1.0f - step(alpha, 0.4);
				baseColor = temp;
			    return baseColor;
			    //return lerp(baseColor, temp, temp.a);
				//return fixed4(1.0,0.0,0.0,1.0);
				
				//temp = _ColorRed;
				//if(baseColor.r >= baseColor.g && baseColor.r >= baseColor.b) return _TintColorRed;
				//if(baseColor.b >= baseColor.r && baseColor.b >= baseColor.g) {}
				//if(baseColor.g >= baseColor.r && baseColor.g >= baseColor.b) {}
				
				//return baseColor;
			}
			ENDCG 
		}
	}
}
}