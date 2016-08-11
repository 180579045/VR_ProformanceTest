Shader "H3D/InGame/ScreenEffect/FocusCameraEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Factor("变暗系数",Range(0,1)) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True"  "ForceNoShadowCasting" = "True"}
		LOD 200
		ZWrite Off
		Pass 
		{  
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					half4 vertex : POSITION;
					half2 texcoord : TEXCOORD0;
				};
				
				struct v2f 
				{
					half4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};

				sampler2D _MainTex;
				half4 _MainTex_ST;
				fixed _Factor;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
			
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					return col * _Factor;
				}

			ENDCG
		}

	} 


	FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse ( no Supports Lightmap)"
}
