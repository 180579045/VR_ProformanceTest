Shader "H3D/InGame/SceneDynamicObjects/H3DBumpSpecular" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
	   	_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
	    _Shininess ("Shininess", Range (0.01, 1.0)) = 0.078125	
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
				o.Normal   = H3DUnpackScaleNormal(tex2D (_BumpMap,  IN.uv_BumpMap), _BumpScale);
                o.Specular = _Shininess;
				o.Gloss    = c.a;
		}

		
	    ENDCG
		SubShader {
		
			LOD 400
			CGPROGRAM
			#pragma surface surf  BlinnPhong exclude_path:prepass  nolightmap  
			#pragma target 3.0
			ENDCG
		} 
		SubShader {
	
			LOD 300
			CGPROGRAM
			#pragma surface surf  BlinnPhong exclude_path:prepass  nolightmap noforwardadd interpolateview 
			#pragma target 3.0
			ENDCG
		} 
		SubShader {
			
			LOD 200
			CGPROGRAM
			#pragma surface Lod3_surf BlinnPhong exclude_path:prepass  nolightmap noforwardadd interpolateview
			void Lod3_surf (Input IN, inout SurfaceOutput o) {
				fixed4 c   = tex2D (_MainTex, IN.uv_MainTex);
				o.Albedo   = c.rgb;
				o.Specular = _Shininess;
				o.Gloss    = c.a;
		    }
			ENDCG
		} 
    FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"

	}//end category
}
