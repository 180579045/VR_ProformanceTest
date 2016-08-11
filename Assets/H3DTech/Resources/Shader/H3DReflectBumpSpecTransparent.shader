Shader "H3D/OutGame/Transparent/Reflective Bumped Specular" {
	Properties { 
		_Color ("主色调(RGB)透明度(A)", Color) = (1,1,1,1) 
		_MainTex ("慢反射纹理(RGB) 透明度(A)", 2D) = "white"{}
		_SpecColor ("高光颜色", Color) = (0.5,0.5,0.5,1)
		_GlossTex("高光色(RGB)光泽度(A)",2D) = "white"{}
		_Shininess ("高光强度", Range (0.01, 1)) = 0.078125
		_ReflectColor ("反射色", Color) = (1,1,1,0.5)
		_RefTex ("反射纹理", 2D) = "black" {}
		_BumpMap ("法线纹理", 2D) = "black" {}
		_RimColor ("边缘光颜色", Color) = (1.0,1.0,1.0,0.0)
		_RimPower ("边缘光强", Range(-2,10)) = 0.5 
		_EmissionTex("自发光纹理",2D) = "black"{}
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" } 
		LOD 400
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM 
		#pragma surface surf BlinnPhong exclude_path:prepass noforwardadd halfasview finalcolor:finalColorMod vertex:vert  
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _RefTex;
		sampler2D _GlossTex;
		sampler2D _EmissionTex;
		
		fixed4 _Color;
		fixed4 _ReflectColor;
		fixed4 _RimColor;
		half _RimPower; 
		half _Shininess;

		struct Input {
			float2 uv_MainTex; 
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

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 gloss = tex2D(_GlossTex, IN.uv_MainTex);
			
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb; 
			o.Gloss = gloss.a;
			_SpecColor.rgb *= gloss.rgb;
			o.Specular = _Shininess; 
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			
			half3 worldRefl = WorldReflectionVector (IN, o.Normal);
			worldRefl = normalize(worldRefl);  		
			fixed4 reflcol  = tex2D(_RefTex,worldRefl.xy * 0.5f + 0.5f ) * tex.a;  
			o.Emission = reflcol.rgb * _ReflectColor.rgb * gloss.rgb + IN.rim;
			o.Alpha = tex.a * _Color.a;
		}
		
		void finalColorMod(Input IN, SurfaceOutput o, inout fixed4 color)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 emis = tex2D(_EmissionTex, IN.uv_MainTex);
			color.rgb = emis.r * tex.rgb + (1.0f - emis.r) * color.rgb;
			
		}
		
		ENDCG
	}

	FallBack "Diffuse"
}
