Shader "H3D/InGame/Character/RimLight" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {} 
		_RimColor ("边缘光颜色", Color) = (0.8,0.8,0.8,0.6)
		_RimPower ("边缘光强", Range(-2,10)) = 0.5 
	}
	Category {
	
	    Tags { "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType"="Opaque" }
	    
	    CGINCLUDE
        #include "H3DFramework.cginc"
		sampler2D _MainTex; 
		fixed4    _RimColor;
		half      _RimPower; 

		struct Input 
		{
			half2  uv_MainTex; 
			half3  viewDir;
			half3  worldNormal; 
			fixed3 rim;
		};

		
		
		ENDCG
		
		SubShader 
		{
			Tags { "RenderType"="Opaque" }
			LOD 400
			CGPROGRAM
			#pragma surface Lod1_surf Lambert exclude_path:prepass exclude_path:deferred nolightmap noforwardadd 
			void Lod1_surf (Input IN, inout SurfaceOutput o) 
			{
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex); 
				o.Albedo = c.rgb; 
				half rim = 1.0 - saturate(dot (normalize(IN.viewDir), IN.worldNormal));
				half3 rimrgb = _RimColor.rgb * pow (rim, _RimPower)*_RimColor.a;
				o.Emission = rimrgb; 
				o.Alpha = c.a;
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
					o.Albedo = c.rgb; 
					o.Emission = IN.rim; 
					
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
					return c;
				} 
				ENDCG
			}
		} //end Lod2
		
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
					o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb; 
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
					return c;
				} 
				ENDCG
			}//end Lod2
		}
		FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
	}
}
