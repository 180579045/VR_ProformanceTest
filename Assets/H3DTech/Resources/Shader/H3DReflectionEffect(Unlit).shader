Shader "H3D/InGame/Others/H3DReflectionEffect(Unlit)" { 
	Properties { 
		_MainColor ("mainc color",Color) = (0.0,0.0,0.0,1.0)
		_ReflColor ("reflect color",Color) = (1.0,1.0,1.0,1.0)
		_MainTex   ("main texture", 2D) = "" {} 
	}
 
	Subshader { 
	    LOD 200
		Tags { "WaterMode"="Refractive" "RenderType"="Opaque" }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest  
			#include "UnityCG.cginc"
 

			struct appdata {
				float4 vertex : POSITION; 
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0; 
				float4 ref : TEXCOORD1;  
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex); 
				o.ref = ComputeScreenPos(o.pos); 
				o.texcoord = v.texcoord;
				return o;
			}
 
			sampler2D _ReflectionTex; 
			sampler2D _MainTex;
			fixed4 _ReflColor;
			fixed4 _MainColor;

			fixed4 frag( v2f i ) : SV_Target
			{    
				float4 uv1 = i.ref;  
				fixed4 refl = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(uv1) ); 
				fixed4 mainColor = tex2D( _MainTex , i.texcoord);
				fixed4 color = refl * _ReflColor + mainColor * _MainColor;  
				return color;
			}
			ENDCG

		}
	}
	
	Fallback "H3D/InGame/SceneStaticObjects/SimpleDiffuse ( no Supports Lightmap)"

}
