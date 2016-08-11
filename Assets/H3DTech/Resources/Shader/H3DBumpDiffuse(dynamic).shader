Shader "H3D/InGame/SceneDynamicObjects/H3DBumpDiffuse" {
	Properties {
		_MainTex ("Albedo (RGB) Glass (A)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_BumpScale("BumpScale",float) = 1
	}
	Category{
	    Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  }
	    CGINCLUDE
	    #include     "H3DFramework.cginc"
		sampler2D  _MainTex;
		sampler2D  _BumpMap;
		half	   _BumpScale;
		struct Input {
				half2 uv_MainTex;
				half2 uv_BumpMap;
		};
		void surf (Input IN, inout SurfaceOutput o) {
				fixed4 c   = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo   = c.rgb;
				o.Normal   = UnpackScaleNormal(tex2D (_BumpMap,  IN.uv_BumpMap), _BumpScale);
		}
	    ENDCG
		SubShader {
			LOD 400
			CGPROGRAM
			#pragma surface surf H3DLambert exclude_path:prepass  nolightmap  
			#pragma target 3.0
			ENDCG
		} 
		SubShader {
			LOD 300
			CGPROGRAM
			#pragma surface surf H3DLambert exclude_path:prepass  nolightmap noforwardadd 
			#pragma target 3.0
			ENDCG
		} 
		SubShader {
			LOD 200
			CGPROGRAM
			#pragma surface Lod3_surf H3DLambert exclude_path:prepass  nolightmap noforwardadd 
			void Lod3_surf (Input IN, inout SurfaceOutput o) {
				o.Albedo   = tex2D (_MainTex, IN.uv_MainTex).rgb;
		    }
			ENDCG
		} 
       FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
       
	}//end category
}
