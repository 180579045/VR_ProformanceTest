Shader "H3D/InGame/Others/H3DReflectionEffect (Unlit)" { 
	Properties { 
		_MainColor ("mainc color",Color) = (0.0,0.0,0.0,1.0)
		_ReflColor ("reflect color",Color) = (1.0,1.0,1.0,1.0)
		_MainTex   ("main texture", 2D) = "" {} 
	}
	
	SubShader 
	{
			Tags { "RenderType"="Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert  noforwardadd

			sampler2D _ReflectionTex; 
			sampler2D _MainTex;
			fixed4 _MainColor;
			fixed4 _ReflColor;

			struct Input {
				float2 uv_MainTex; 
				float4 screenPos;
			};
			
			void surf (Input IN, inout SurfaceOutput o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _MainColor;
				fixed4 refl = tex2Dproj( _ReflectionTex, UNITY_PROJ_COORD(IN.screenPos)); 
				
				o.Albedo = refl*_ReflColor + c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
	}
	
	Fallback "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
}
