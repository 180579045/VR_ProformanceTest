Shader "H3D/InGame/Particles/AlphaBlend" {
Properties {
	_TintColor ("颜色缩放", Color) = (1,1,1,1)
	_MainTex ("粒子纹理", 2D) = "white" {} 
	_Alpha("透明度",Range(0,1)) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "ForceNoShadowCasting" = "True" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	// ---- Fragment program cards
	SubShader {
	    LOD 200
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			half4 _MainTex_ST;
			fixed _Alpha;

			struct appdata_t {
				half4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				half4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};
			  

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 finalColor = i.color * _TintColor * tex2D(_MainTex, i.texcoord) * 2.0f;
				finalColor.a *= _Alpha;
				return finalColor;
			}
			ENDCG 
		}
	} 	
	
	// ---- Dual texture cards
	SubShader {
	    LOD 100
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader {
	    LOD 100
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
