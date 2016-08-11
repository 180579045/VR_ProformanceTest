Shader "H3D/InGame/SceneDynamicObjects/ReflectiveSurfReflectivityMultiply" {
	Properties 
	{
		_MainTex ("(RGB)(A反射率)", 2D) = "white" {}
		_SpecColor ("高光颜色", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("高光强度", Range(0.0,1.0)) = 0.7
		_ReflTex ("反射纹理", 2D) = "white" {}
		_ReflColor ("反射颜色", Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("反射强度",Range(0.0,1.0)) = 1.0
		_ReflAnimFactor("反射纹理坐标动画权重",Range(0.0,1.0)) = 0.0
		_ReflTexUOffsetSpeed ("反射偏移水平速度",float) = 0
		_ReflTexVOffsetSpeed ("反射偏移纵向速度",float) = 0
	}
	
	Category {
        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  } 
        
        CGINCLUDE
        #include "H3DFramework.cginc"
        sampler2D _MainTex;
		sampler2D _ReflTex;
		half4     _ReflColor;
	 
		half      _ReflPower;
		half      _ReflAnimFactor;
		half      _ReflTexUOffsetSpeed;
		half      _ReflTexVOffsetSpeed;
		
		struct Input 
		{
			half2 uv_MainTex;
			half3 worldRefl;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float realTime = _Time.y;
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
//			half3 worldRefl = WorldReflectionVector (IN, o.Normal);
			IN.worldRefl = normalize(IN.worldRefl);  	
			
			half2 reflUVOff = half2(
				fmod( realTime * _ReflTexUOffsetSpeed * _ReflAnimFactor , 1.0 )  , 
				fmod( realTime * _ReflTexVOffsetSpeed * _ReflAnimFactor , 1.0 )
			);
			
			o.Albedo = tex2D(_ReflTex,IN.worldRefl.xy * 0.5f + 0.5f + reflUVOff ).xyz * (_ReflPower * c.a) * _ReflColor.xyz;
			o.Albedo.xyz *= c.xyz;
			o.Alpha = c.a;
			o.Gloss = c.a;
			o.Specular = _Shininess;
		}
		
		ENDCG
		SubShader {
			LOD 400
			CGPROGRAM	
			#pragma surface surf H3DBlinnPhongHalf  nolightmap noforwardadd  interpolateview halfasview exclude_path:prepass  nolightmap
			ENDCG
		}  
		SubShader {
			LOD 300
			Pass {
			    
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex vert_surf
				#pragma fragment frag_surf
				half4     _MainTex_ST;
				struct v2f {
				  float4 pos       : SV_POSITION;
				  half2  uv        : TEXCOORD0;
				  half3  worldRefl : TEXCOORD1;
				  H3D_LIGHT_COODS(2)
				};
				
				v2f vert_surf (appdata_full v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
					H3D_BLINNPHONG_LIGHT(o);
					o.worldRefl = reflect(-worldViewDir, worldNormal);
					return o;
				}
				fixed4 frag_surf (v2f i) : SV_Target
				{
					Input surfInput;
					surfInput.uv_MainTex =i.uv;
					surfInput.worldRefl  =i.worldRefl;
					SurfaceOutput o;
					UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
					surf (surfInput, o);
					fixed4 c = 0;
					c.rgb =  H3D_SURFACE_LIGHT(i,o);
					c.a   = o.Alpha;
					UNITY_OPAQUE_ALPHA(c.a);
					return c;
				} 
				ENDCG
			}
		}//end Lod2  
		SubShader {
		   LOD 200
			Pass {
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex vert_surf
				#pragma fragment frag_surf
				half4 _MainTex_ST;
				struct v2f {
				  float4 pos       : SV_POSITION;
				  half2  uv     : TEXCOORD0;
				  H3D_LIGHT_COODS(1)
				};
				
				void Lod3_surf (Input IN, inout SurfaceOutput o) 
		        {
					half4 c  = tex2D (_MainTex, IN.uv_MainTex);
					o.Albedo = c.xyz * _ReflPower * _ReflColor.xyz * c.a;
					o.Alpha  =  c.a;
		        }
				v2f vert_surf (appdata_full v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					H3D_BLINNPHONG_LIGHT(o);
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
					c.rgb =  H3D_SURFACE_LIGHT(i,o);
					c.a   =  o.Alpha;
					UNITY_OPAQUE_ALPHA(c.a);
					return c;
				} 
				ENDCG
			}
		}//Lod3 
	  FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
	}//end category
}
