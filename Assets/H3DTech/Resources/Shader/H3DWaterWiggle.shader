Shader "H3D/InGame/Others/H3DWater Wiggle" {

	Properties {
		_Color ("主色调", Color) = (1,1,1,1)
		_MainTex ("慢反射纹理", 2D) = "white" {}
		_WiggleTex ("扭动纹理", 2D) = "white" {}
		_WiggleStrength ("扭动强度", Range (0.01, 0.1)) = 0.01
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert exclude_path:prepass exclude_path:deferred nolightmap noforwardadd 

		sampler2D _MainTex;
		sampler2D _WiggleTex;
		fixed4 _Color;
		half _WiggleStrength;

		struct Input
		{
			half2 uv_MainTex;
			half2 uv_WiggleTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{  
			half2 tc2 = IN.uv_WiggleTex;
			tc2.x -= _SinTime;
			tc2.y += _CosTime;
			fixed4 wiggle = tex2D(_WiggleTex, tc2);
			
			IN.uv_MainTex.x -= wiggle.r * _WiggleStrength;
			IN.uv_MainTex.y += wiggle.b * _WiggleStrength*1.5f;
			
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "VertexLit"
}
