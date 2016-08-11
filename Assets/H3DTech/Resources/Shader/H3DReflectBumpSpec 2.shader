Shader "H3D/OutGame/Reflective Bumped Specular 2" {
	Properties { 
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("颜色贴图 (RGB) 反射强弱 (A)", 2D) = "white" {}
		
		_SpecColor ("高光颜色", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("高光范围", Range (0.01, 1)) = 0.078125
		
		_GlossTex("高光贴图",2D) = "white"{}
		
		_Illum ("自发贴光 (A)", 2D) = "black" {}
		_BumpMap ("法线贴图", 2D) = "bump" {}
		_BumpScale("BumpScale",float) = 1
		
		_ReflectColor ("反射颜色", Color) = (1,1,1,0.5)
		_RefTex ("反射纹理", 2D) = "black" {}
		
		_RimColor ("边缘光颜色", Color) = (1.0,1.0,1.0,0.0)
		_RimPower ("边缘光强度", Range(-2,10)) = 0.5 
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
       
		CGPROGRAM 
		#pragma surface surf H3DBlinnPhongHalf exclude_path:prepass noforwardadd   interpolateview halfasview vertex:vert   finalcolor:finalColorMod
		#pragma target 3.0
		#include     "H3DFramework.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Illum;
		sampler2D _RefTex;
		sampler2D _GlossTex;
		fixed4    _Color;
		fixed4    _ReflectColor;
	    fixed4    _RimColor;
		half      _RimPower; 
		half      _BumpScale;
		
		
		
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldRefl;
			fixed3 rim;
			INTERNAL_DATA
		};
		
		void vert (inout appdata_full v, out Input o)
		{  
			UNITY_INITIALIZE_OUTPUT(Input,o); 
			//边缘光在顶点着色器中计算提高效率
			half rim = 1.0f - saturate( dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal) );
			o.rim = (_RimColor.rgb * pow(rim, _RimPower)) * _RimColor.a;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
		
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 hightLight =  tex2D(_GlossTex, IN.uv_MainTex);
			
			fixed4 c = tex * _Color;
			
			o.Normal =  H3DUnpackScaleNormal(tex2D(_BumpMap, IN.uv_BumpMap),_BumpScale);
			half3 worldRefl = WorldReflectionVector (IN, o.Normal);
			worldRefl = normalize(worldRefl);		
			fixed4 reflcol  = tex2D(_RefTex,worldRefl.xy * 0.5f + 0.5f ) * tex.a;  
			
			_SpecColor.rgb *= hightLight.rgb;
			o.Albedo = c.rgb;
//			o.Emission = c.rgb + IN.rim +reflcol.rgb * _ReflectColor.rgb;
	        o.Emission = IN.rim +reflcol.rgb * _ReflectColor.rgb;
			o.Gloss = hightLight.r;
			o.Alpha = c.a;
			o.Specular = _Shininess;
		}
		
		void finalColorMod(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 emis = tex2D(_Illum, IN.uv_MainTex);
			color.rgb = emis.a * tex.rgb + (1.0f - emis.a) * color.rgb;
			
		}
		
		ENDCG
	}

	FallBack "Diffuse"
}
