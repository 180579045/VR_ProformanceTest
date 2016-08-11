Shader "H3D/InGame/Character/RimHighLightTransparents" {	
  Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {} 
		_RimColor ("边缘光颜色", Color) = (0.8,0.8,0.8,0.6)
		_RimPower ("边缘光强", Range(-2,10)) = 0.5
		_HighLightColor("高亮颜色", Color) = (0.0,0.0,0.0,1.0)
		_HighLightPower("高亮权重", Range(0.0,1.0)) = 0.0
		_Alpha("透明度",Range(0,1)) = 0.5
	}
	Category {
	    Tags {"Queue"="Transparent"  "IgnoreProjector" = "True"  } 
		Blend SrcAlpha OneMinusSrcAlpha 
		
	    CGINCLUDE
	    #include "H3DFramework.cginc"
	    sampler2D  _MainTex; 
		fixed4     _RimColor;
		half       _RimPower; 
		fixed4     _HighLightColor;
		fixed      _HighLightPower;
		fixed      _Alpha;
		struct Input 
		{
			half2 uv_MainTex ;
			half3 viewDir;
			half3 worldNormal; 
			fixed3 rim;
		};
		ENDCG
		SubShader 
		{
			pass { 
				ZWrite On ZTest LEqual ColorMask 0 
		    }
			LOD 400
			CGPROGRAM
			#pragma surface Lod1_surf Lambert decal:blend    exclude_path:prepass exclude_path:deferred nolightmap noforwardadd 
			void Lod1_surf (Input IN, inout SurfaceOutput o) 
			{
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex); 
				o.Albedo = c.rgb + _HighLightColor.rgb * _HighLightPower; 
				half rim = 1.0 - saturate(dot (normalize(IN.viewDir), IN.worldNormal));
				half3 rimrgb = _RimColor.rgb * pow (rim, _RimPower)*_RimColor.a;
	          	o.Emission = rimrgb * _HighLightPower;
				o.Alpha = c.a * _Alpha;
			}
			ENDCG
		} 
	     SubShader {
			LOD 300
			Pass {
			    name "RimHightLight2"
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex   vert_surf
				#pragma fragment frag_surf
				half4     _MainTex_ST;
				struct v2f {
				  float4 pos       : SV_POSITION;
				  half2  uv        : TEXCOORD0;
				  half3  rim       : TEXCOORD1;
				  H3D_LIGHT_COODS(2)
				};
				
				void vert (inout appdata_full v, out Input o)
				{  
					UNITY_INITIALIZE_OUTPUT(Input,o); 
					half rim = 1.0f - saturate( dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal) );
					o.rim = (_RimColor.rgb * pow(rim, _RimPower)) * _RimColor.a;
				}
				
				void Lod2_surf (Input IN, inout SurfaceOutput o) 
				{
					fixed4 c = tex2D (_MainTex, IN.uv_MainTex); 
					o.Albedo = c.rgb + _HighLightColor.rgb * _HighLightPower; 
					o.Emission = IN.rim * _HighLightPower; 
					o.Alpha = c.a * _Alpha;
				}
		
				v2f vert_surf (appdata_full v)
				{
					v2f o;
					Input customInputData;
					vert (v, customInputData);
  					o.rim = customInputData.rim;
  					
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
					
					H3D_LAMBERT_LIGHT(o);

					return o;
				}
				fixed4 frag_surf (v2f i) : SV_Target
				{
					Input surfInput;
					surfInput.uv_MainTex =i.uv;
					surfInput.rim  = i.rim;
					SurfaceOutput o;
					UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
					Lod2_surf (surfInput, o);
					fixed4 c = 0;
					c.rgb =  H3D_SURFACE_LIGHT(i,o);
					c.rgb += o.Emission;
					c.a   = o.Alpha;
					return c;
				} 
				ENDCG
			}//end Lod2
		} 
		SubShader {
		    LOD 200
			Pass {
			    name "RimHightLight3"
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex   vert_surf
				#pragma fragment frag_surf
				half4     _MainTex_ST;
				struct v2f {
				  float4 pos       : SV_POSITION;
				  half2  uv        : TEXCOORD0;
				  H3D_LIGHT_COODS(2)
				};
				void Lod3_surf (Input IN, inout SurfaceOutput o) 
				{
					fixed4 c = tex2D (_MainTex, IN.uv_MainTex); 
					o.Albedo = c.rgb + _HighLightColor.rgb * _HighLightPower; 
					o.Alpha  = _Alpha * c.a;
				}
				v2f vert_surf (appdata_full v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
					H3D_LAMBERT_LIGHT(o);
					return o;
				}
				fixed4 frag_surf (v2f i) : SV_Target
				{
					Input surfInput;
					surfInput.uv_MainTex =i.uv;
					SurfaceOutput o; 
					UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
					Lod3_surf (surfInput, o);
					fixed4 c = 0;
					c.rgb = H3D_SURFACE_LIGHT(i,o);
					c.a   = o.Alpha;
					return c;
				} 
				ENDCG
			}//end Lod2
		}
	
		 FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
	}//Category
}
