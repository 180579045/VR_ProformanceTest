 

Shader "H3D/InGame/Others/H3DAnimBumpDistort" {
Properties { 
	_MainTex ("慢反射 (RGB)", 2D) = "white" {}
	_BumpMap ("扰动图", 2D) = "black" {}
	_Distort ("扰动缩放",range(0,1)) = 0.1
	_ReflTexUOffsetSpeed ("扰动偏移水平速度",float) = 0
	_ReflTexVOffsetSpeed ("扰动偏移纵向速度",float) = 0
	_Alpha("透明度",Range(0,1)) = 1
}

Category {

	Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}


	SubShader {
	   LOD 200

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		//GrabPass {							
		//	Name "BASE"
		//	Tags { "LightMode" = "Always" } 
		//}
 		
 		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
 		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
			Fog {Mode  Off}
			Blend One Zero ,SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				half2 texcoord: TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f {
				float4 vertex : POSITION;
				half4 uvgrab : TEXCOORD0;
				half2 uvbump : TEXCOORD1;
				half2 uvmain : TEXCOORD2;
				fixed4 color : COLOR;
			};

			half  _BumpAmt;
			half4 _BumpMap_ST;
			half4 _MainTex_ST;
			half  _Alpha;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				#if UNITY_UV_STARTS_AT_TOP
					half scale = -1.0;
				#else
					half scale = 1.0;
				#endif
				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;
				o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
				o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.color = v.color;
				return o;
			}
 
			sampler2D _CaptureRT;
			sampler2D _FullScreenTex;
			fixed _ScreenTexFactor;
			
			half4 _GrabTexture_TexelSize;
			sampler2D _BumpMap;
			sampler2D _MainTex;
			half _Distort;
			half _ReflTexUOffsetSpeed;
			half _ReflTexVOffsetSpeed;

			half4 frag( v2f i ) : COLOR
			{
				float realTime = _Time.y;
				
				half2 reflUVOff = half2( 
				fmod( realTime*_ReflTexUOffsetSpeed , 1.0) ,
				fmod( realTime*_ReflTexVOffsetSpeed , 1.0) 
				); 
				
				half2 offset = (tex2D(_BumpMap, i.uvbump + reflUVOff).rg - 0.5) * 2.0;  
				
				fixed2 projUV = i.uvgrab.xy / i.uvgrab.w;
				
				fixed4 col = tex2D( _CaptureRT, projUV);
				fixed4 screen_col = tex2D(_FullScreenTex, projUV);
				fixed4 col_d = tex2D(_CaptureRT,  projUV + offset * _Distort);
				fixed4 tint = tex2D( _MainTex, i.uvmain ); 
				fixed alpha = tint.a * i.color.w;
				fixed screenTexAlpha = screen_col.a * _ScreenTexFactor;
				fixed4 final = (col_d * alpha  + col * (1.0 - alpha)) * ( 1 - screenTexAlpha ) + screen_col * screenTexAlpha; 
				final.a = tint.a * _Alpha; 
				return final;
			}
			ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro
	
	SubShader 
	{
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
  }
  Fallback off
}
